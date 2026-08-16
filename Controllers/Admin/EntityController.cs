using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SVEI.Web.Admin;
using SVEI.Web.Data;
using SVEI.Web.Models;
using SVEI.Web.Services;
using SVEI.Web.ViewModels.Admin;

namespace SVEI.Web.Controllers.Admin
{
    /// <summary>
    /// One controller to rule all 31 admin tables. Everything it needs is
    /// declared in <see cref="AdminSchema"/>; the views are fully generic.
    /// Routes:  /Admin/e/{key}  ·  /Admin/e/{key}/create  ·  /Admin/e/{key}/edit/{id}
    /// </summary>
    [Route("Admin/e")]
    public class EntityController : AdminBaseController
    {
        private readonly IMediaService _media;
        private readonly ISettings _cfg;
        private readonly IHtmlSanitizer _html;

        public EntityController(AppDbContext db, ILang lang, IMediaService media, ISettings cfg,
                                IHtmlSanitizer html)
            : base(db, lang)
        { _media = media; _cfg = cfg; _html = html; }

        // ══════════════════════════════════════════════════════════════════════
        //  LIST
        // ══════════════════════════════════════════════════════════════════════
        [HttpGet("{key}")]
        public async Task<IActionResult> Index([FromRoute] string key, [FromQuery] string? q, [FromQuery] string? g, [FromQuery] int page = 1, [FromQuery] int size = 25)
        {
            var def = AdminSchema.Find(key);
            if (def is null) return NotFound();

            var query = Set(def.ClrType);

            // free-text search across all Searchable fields
            if (!string.IsNullOrWhiteSpace(q))
                query = ApplySearch(query, def, q.Trim());

            // group filter (e.g. settings by Group, sections by PageKey)
            if (!string.IsNullOrWhiteSpace(g) && def.GroupField is not null)
                query = ApplyEquals(query, def.ClrType, def.GroupField, g);

            var total = await CountAsync(query, def.ClrType);
            query = ApplyOrder(query, def);

            size = Math.Clamp(size, 10, 200);
            page = Math.Max(1, page);
            var paged = ApplyTake(ApplySkip(query, def.ClrType, (page - 1) * size), def.ClrType, size);
            var items = await ToListAsync(paged, def.ClrType);

            var vm = new EntityListVm
            {
                Def = def,
                Items = items,
                Page = page,
                PageSize = size,
                Total = total,
                Query = q,
                Group = g,
                Groups = def.GroupField is null ? new() : await DistinctGroupsAsync(def),
                Lookups = await LoadLookupsAsync(def)
            };
            return View("~/Views/Admin/Entity/Index.cshtml", vm);
        }

        // ══════════════════════════════════════════════════════════════════════
        //  CREATE
        // ══════════════════════════════════════════════════════════════════════
        [HttpGet("{key}/create")]
        public async Task<IActionResult> Create([FromRoute] string key, [FromQuery] int? parentId)
        {
            var def = AdminSchema.Find(key);
            if (def is null) return NotFound();
            if (!def.CanCreate) return Forbid();

            var entity = Activator.CreateInstance(def.ClrType)!;

            // sensible defaults
            SetIfExists(entity, "IsActive", true);
            SetIfExists(entity, "IsVisible", true);
            SetIfExists(entity, "IsPublished", true);
            SetIfExists(entity, "InStock", true);
            SetIfExists(entity, "IsLocalized", true);
            SetIfExists(entity, "Vacancies", 1);
            SetIfExists(entity, "MapZoom", 14);
            SetIfExists(entity, "Rating", 5);
            SetIfExists(entity, "OverlayOpacity", 55);
            SetIfExists(entity, "SortOrder", await NextSortAsync(def));
            SetIfExists(entity, "PublishedAt", DateTime.UtcNow);
            SetIfExists(entity, "PostedAt", DateTime.UtcNow);
            if (parentId is > 0 && def.Fields.Any(f => f.Kind == FieldKind.Lookup))
            {
                var fk = def.Fields.First(f => f.Kind == FieldKind.Lookup);
                SetIfExists(entity, fk.Name, parentId.Value);
            }

            return View("~/Views/Admin/Entity/Form.cshtml", await FormVmAsync(def, entity, true));
        }

        [HttpPost("{key}/create")]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(60 * 1024 * 1024)]
        public async Task<IActionResult> Create([FromRoute] string key, IFormCollection form)
        {
            var def = AdminSchema.Find(key);
            if (def is null) return NotFound();
            if (!def.CanCreate) return Forbid();

            var entity = Activator.CreateInstance(def.ClrType)!;
            var errors = await BindAsync(entity, def, form, isNew: true);

            if (errors.Count > 0)
            {
                foreach (var e in errors) ModelState.AddModelError(e.Key, e.Value);
                return View("~/Views/Admin/Entity/Form.cshtml", await FormVmAsync(def, entity, true));
            }

            SetIfExists(entity, "CreatedAt", DateTime.UtcNow);
            SetIfExists(entity, "UpdatedAt", DateTime.UtcNow);

            Db.Add(entity);
            await Db.SaveChangesAsync();
            _cfg.Invalidate();

            var id = AdminSchema.GetValue(entity, "Id");
            await AuditAsync("create", def.ClrType.Name, id, AdminSchema.Label(entity, true));
            Ok("تم الحفظ بنجاح ✓", "Saved successfully ✓");

            return form["__stay"] == "1"
                ? RedirectToAction(nameof(Edit), new { key, id })
                : RedirectToAction(nameof(Index), new { key });
        }

        // ══════════════════════════════════════════════════════════════════════
        //  EDIT
        // ══════════════════════════════════════════════════════════════════════
        [HttpGet("{key}/edit/{id:int}")]
        public async Task<IActionResult> Edit([FromRoute] string key, [FromRoute] int id)
        {
            var def = AdminSchema.Find(key);
            if (def is null) return NotFound();

            var entity = await FindAsync(def.ClrType, id);
            if (entity is null) return NotFound();

            // opening an inbox item marks it read
            if (def.HasUnread && AdminSchema.GetValue(entity, "IsRead") is false)
            {
                SetIfExists(entity, "IsRead", true);
                await Db.SaveChangesAsync();
                await LoadBadgesAsync();
            }

            return View("~/Views/Admin/Entity/Form.cshtml", await FormVmAsync(def, entity, false));
        }

        [HttpPost("{key}/edit/{id:int}")]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(60 * 1024 * 1024)]
        public async Task<IActionResult> Edit([FromRoute] string key, [FromRoute] int id, IFormCollection form)
        {
            var def = AdminSchema.Find(key);
            if (def is null) return NotFound();
            if (!def.CanEdit) return Forbid();

            var entity = await FindAsync(def.ClrType, id);
            if (entity is null) return NotFound();

            var errors = await BindAsync(entity, def, form, isNew: false);
            if (errors.Count > 0)
            {
                foreach (var e in errors) ModelState.AddModelError(e.Key, e.Value);
                return View("~/Views/Admin/Entity/Form.cshtml", await FormVmAsync(def, entity, false));
            }

            SetIfExists(entity, "UpdatedAt", DateTime.UtcNow);
            await Db.SaveChangesAsync();
            _cfg.Invalidate();

            await AuditAsync("update", def.ClrType.Name, id, AdminSchema.Label(entity, true));
            Ok("تم تحديث البيانات ✓", "Updated successfully ✓");

            return form["__stay"] == "1"
                ? RedirectToAction(nameof(Edit), new { key, id })
                : RedirectToAction(nameof(Index), new { key });
        }

        // ══════════════════════════════════════════════════════════════════════
        //  DELETE / TOGGLE / REORDER
        // ══════════════════════════════════════════════════════════════════════
        [HttpPost("{key}/delete/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromRoute] string key, [FromRoute] int id)
        {
            var def = AdminSchema.Find(key);
            if (def is null) return NotFound();
            if (!def.CanDelete) return Forbid();

            var entity = await FindAsync(def.ClrType, id);
            if (entity is null) return NotFound();

            // clean up owned files
            foreach (var f in def.Fields.Where(f => f.Kind is FieldKind.Image or FieldKind.File))
                _media.Delete(AdminSchema.GetValue(entity, f.Name) as string);

            try
            {
                Db.Remove(entity);
                await Db.SaveChangesAsync();
                _cfg.Invalidate();
                await AuditAsync("delete", def.ClrType.Name, id);
                Ok("تم الحذف ✓", "Deleted ✓");
            }
            catch (DbUpdateException)
            {
                Err("لا يمكن الحذف: هناك سجلات مرتبطة بهذا العنصر.",
                    "Cannot delete: other records depend on this item.");
            }
            return RedirectToAction(nameof(Index), new { key });
        }

        /// <summary>Inline checkbox toggle from the list view (AJAX).</summary>
        [HttpPost("{key}/toggle/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Toggle([FromRoute] string key, [FromRoute] int id, [FromForm] string field)
        {
            var def = AdminSchema.Find(key);
            if (def is null) return NotFound();

            var f = def.Get(field);
            if (f is null || f.Kind != FieldKind.Bool) return BadRequest();

            var entity = await FindAsync(def.ClrType, id);
            if (entity is null) return NotFound();

            var cur = AdminSchema.GetValue(entity, field) as bool? ?? false;
            SetIfExists(entity, field, !cur);
            SetIfExists(entity, "UpdatedAt", DateTime.UtcNow);
            await Db.SaveChangesAsync();
            _cfg.Invalidate();

            return Json(new { ok = true, value = !cur });
        }

        /// <summary>Drag-and-drop reorder from the list view (AJAX).</summary>
        [HttpPost("{key}/reorder")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reorder([FromRoute] string key, [FromForm] int[] ids)
        {
            var def = AdminSchema.Find(key);
            if (def is null || !def.Sortable) return BadRequest();

            for (var i = 0; i < ids.Length; i++)
            {
                var e = await FindAsync(def.ClrType, ids[i]);
                if (e is not null) SetIfExists(e, "SortOrder", i + 1);
            }
            await Db.SaveChangesAsync();
            _cfg.Invalidate();
            return Json(new { ok = true });
        }

        /// <summary>Bulk actions from the list view.</summary>
        [HttpPost("{key}/bulk")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Bulk([FromRoute] string key, [FromForm] string action, [FromForm] int[] ids)
        {
            var def = AdminSchema.Find(key);
            if (def is null || ids.Length == 0) return RedirectToAction(nameof(Index), new { key });

            foreach (var id in ids)
            {
                var e = await FindAsync(def.ClrType, id);
                if (e is null) continue;

                switch (action)
                {
                    case "delete" when def.CanDelete:
                        foreach (var f in def.Fields.Where(f => f.Kind is FieldKind.Image or FieldKind.File))
                            _media.Delete(AdminSchema.GetValue(e, f.Name) as string);
                        Db.Remove(e);
                        break;
                    case "activate":   SetBoolAny(e, true); break;
                    case "deactivate": SetBoolAny(e, false); break;
                    case "read":       SetIfExists(e, "IsRead", true); break;
                    case "unread":     SetIfExists(e, "IsRead", false); break;
                }
            }

            try { await Db.SaveChangesAsync(); _cfg.Invalidate(); Ok("تم تنفيذ الإجراء ✓", "Bulk action applied ✓"); }
            catch (DbUpdateException) { Err("تعذّر تنفيذ الإجراء على بعض العناصر.", "Some items could not be processed."); }

            return RedirectToAction(nameof(Index), new { key });
        }

        private static void SetBoolAny(object e, bool val)
        {
            foreach (var n in new[] { "IsActive", "IsVisible", "IsPublished" })
                if (AdminSchema.Prop(e.GetType(), n) is not null) { SetIfExists(e, n, val); return; }
        }

        // ══════════════════════════════════════════════════════════════════════
        //  BINDING
        // ══════════════════════════════════════════════════════════════════════
        private async Task<Dictionary<string, string>> BindAsync(
            object entity, EntityDef def, IFormCollection form, bool isNew)
        {
            var errors = new Dictionary<string, string>();
            var type = def.ClrType;

            foreach (var f in def.Fields)
            {
                if (f.Kind == FieldKind.ReadOnly) continue;
                var prop = AdminSchema.Prop(type, f.Name);
                if (prop is null || !prop.CanWrite) continue;

                // ── uploads ────────────────────────────────────────────────
                if (f.Kind is FieldKind.Image or FieldKind.File)
                {
                    if (form[$"__clear_{f.Name}"] == "1")
                    {
                        _media.Delete(prop.GetValue(entity) as string);
                        prop.SetValue(entity, null);
                    }
                    var file = form.Files[f.Name];
                    if (file is { Length: > 0 })
                    {
                        var folder = f.Folder ?? def.Key;
                        var saved = f.Kind == FieldKind.Image
                            ? await _media.SaveImageAsync(file, folder)
                            : await _media.SaveFileAsync(file, folder);
                        if (saved is not null)
                        {
                            _media.Delete(prop.GetValue(entity) as string);
                            prop.SetValue(entity, saved);
                        }
                        else errors[f.Name] = Pick("نوع الملف غير مسموح.", "File type not allowed.");
                    }
                    continue;
                }

                if (!form.ContainsKey(f.Name))
                {
                    // unchecked checkboxes are simply absent
                    if (f.Kind == FieldKind.Bool) prop.SetValue(entity, false);
                    continue;
                }

                var raw = form[f.Name].ToString();

                // ── rich text ─────────────────────────────────────────────
                // Html fields are rendered with @Html.Raw, so the encoder is
                // out of the picture by design. Sanitize on the way in, which
                // makes the stored value the thing that is trusted.
                if (f.Kind == FieldKind.Html)
                {
                    prop.SetValue(entity, _html.Clean(raw));
                    continue;
                }

                // ── slug auto-generation ──────────────────────────────────
                if (f.Kind == FieldKind.Slug)
                {
                    var src = string.IsNullOrWhiteSpace(raw)
                        ? form[f.SlugFrom ?? "TitleEn"].ToString()
                        : raw;
                    if (string.IsNullOrWhiteSpace(src)) src = form["TitleAr"].ToString();
                    if (string.IsNullOrWhiteSpace(src)) src = form["NameAr"].ToString();

                    var slug = SlugHelper.Generate(src);
                    if (string.IsNullOrWhiteSpace(slug)) slug = "item-" + Guid.NewGuid().ToString("N")[..6];

                    var currentId = AdminSchema.GetValue(entity, "Id") as int? ?? 0;
                    slug = await UniqueSlugAsync(type, slug, currentId);
                    prop.SetValue(entity, slug);
                    continue;
                }

                if (!TryConvert(raw, prop.PropertyType, out var value))
                {
                    errors[f.Name] = Pick($"قيمة غير صالحة في «{f.LabelAr}».", $"Invalid value for \"{f.LabelEn}\".");
                    continue;
                }

                if (f.Required && value is null or "")
                {
                    errors[f.Name] = Pick($"حقل «{f.LabelAr}» مطلوب.", $"\"{f.LabelEn}\" is required.");
                    continue;
                }

                prop.SetValue(entity, value);
            }

            // required non-nullable string safety net
            foreach (var f in def.Fields.Where(f => f.Required && f.Kind is not (FieldKind.Image or FieldKind.File)))
            {
                var prop = AdminSchema.Prop(type, f.Name);
                if (prop?.PropertyType == typeof(string) &&
                    string.IsNullOrWhiteSpace(prop.GetValue(entity) as string) &&
                    !errors.ContainsKey(f.Name))
                    errors[f.Name] = Pick($"حقل «{f.LabelAr}» مطلوب.", $"\"{f.LabelEn}\" is required.");
            }

            return errors;
        }

        private static bool TryConvert(string raw, Type target, out object? value)
        {
            value = null;
            var t = Nullable.GetUnderlyingType(target) ?? target;
            var nullable = Nullable.GetUnderlyingType(target) is not null || !t.IsValueType;

            if (t == typeof(string)) { value = raw; return true; }

            if (t == typeof(bool))
            {
                value = raw is "1" or "true" or "True" or "on" or "yes";
                return true;
            }

            if (string.IsNullOrWhiteSpace(raw))
            {
                if (nullable) { value = null; return true; }
                value = Activator.CreateInstance(t);   // 0 / default
                return true;
            }

            try
            {
                if (t == typeof(int))      { value = int.Parse(raw, CultureInfo.InvariantCulture); return true; }
                if (t == typeof(long))     { value = long.Parse(raw, CultureInfo.InvariantCulture); return true; }
                if (t == typeof(double))   { value = double.Parse(raw, CultureInfo.InvariantCulture); return true; }
                if (t == typeof(decimal))  { value = decimal.Parse(raw, CultureInfo.InvariantCulture); return true; }
                if (t == typeof(DateTime)) { value = DateTime.Parse(raw, CultureInfo.InvariantCulture, DateTimeStyles.None); return true; }
                value = Convert.ChangeType(raw, t, CultureInfo.InvariantCulture);
                return true;
            }
            catch { return false; }
        }

        private async Task<string> UniqueSlugAsync(Type type, string slug, int currentId)
        {
            if (AdminSchema.Prop(type, "Slug") is null) return slug;

            var baseSlug = slug; var i = 2;
            while (await SlugExistsAsync(type, slug, currentId))
                slug = $"{baseSlug}-{i++}";
            return slug;
        }

        private async Task<bool> SlugExistsAsync(Type type, string slug, int currentId)
        {
            var items = await ToListAsync(ApplyEquals(Set(type), type, "Slug", slug), type);
            return items.Any(x => (AdminSchema.GetValue(x, "Id") as int? ?? 0) != currentId);
        }

        private async Task<int> NextSortAsync(EntityDef def)
        {
            if (AdminSchema.Prop(def.ClrType, "SortOrder") is null) return 0;
            var all = await ToListAsync(Set(def.ClrType), def.ClrType);
            var max = all.Select(x => AdminSchema.GetValue(x, "SortOrder") as int? ?? 0).DefaultIfEmpty(0).Max();
            return max + 1;
        }

        // ══════════════════════════════════════════════════════════════════════
        //  VIEW-MODEL HELPERS
        // ══════════════════════════════════════════════════════════════════════
        private async Task<EntityFormVm> FormVmAsync(EntityDef def, object entity, bool isNew)
        {
            var vm = new EntityFormVm
            {
                Def = def,
                Entity = entity,
                IsNew = isNew,
                Lookups = await LoadLookupsAsync(def)
            };

            // child rows shown under the parent form (e.g. product specs)
            if (!isNew && def.ChildKey is not null && def.ChildFk is not null)
            {
                var childDef = AdminSchema.Find(def.ChildKey);
                if (childDef is not null)
                {
                    var id = AdminSchema.GetValue(entity, "Id") as int? ?? 0;
                    var q = ApplyEquals(Set(childDef.ClrType), childDef.ClrType, def.ChildFk, id.ToString());
                    vm.ChildDef = childDef;
                    vm.Children = await ToListAsync(ApplyOrder(q, childDef), childDef.ClrType);
                }
            }
            return vm;
        }

        /// <summary>Loads option lists for every Lookup field on the descriptor.</summary>
        private async Task<Dictionary<string, List<(int Id, string Label)>>> LoadLookupsAsync(EntityDef def)
        {
            var map = new Dictionary<string, List<(int, string)>>();
            foreach (var f in def.Fields.Where(f => f.Kind == FieldKind.Lookup && f.LookupType is not null))
            {
                if (map.ContainsKey(f.Name)) continue;
                var rows = await ToListAsync(Set(f.LookupType!), f.LookupType!);
                map[f.Name] = rows
                    .Select(r => (AdminSchema.GetValue(r, "Id") as int? ?? 0, AdminSchema.Label(r, IsAr)))
                    .OrderBy(x => x.Item2)
                    .ToList();
            }
            return map;
        }

        private async Task<List<string>> DistinctGroupsAsync(EntityDef def)
        {
            var rows = await ToListAsync(Set(def.ClrType), def.ClrType);
            return rows.Select(r => AdminSchema.GetValue(r, def.GroupField!) as string)
                       .Where(s => !string.IsNullOrWhiteSpace(s))
                       .Select(s => s!)
                       .Distinct()
                       .OrderBy(s => s)
                       .ToList();
        }

        // ══════════════════════════════════════════════════════════════════════
        //  UNTYPED EF HELPERS (reflection over DbSet<T>)
        // ══════════════════════════════════════════════════════════════════════
        private IQueryable Set(Type t) => (IQueryable)_setMethod.MakeGenericMethod(t).Invoke(Db, null)!;
        private static readonly MethodInfo _setMethod =
            typeof(DbContext).GetMethods().First(m => m.Name == "Set" && m.IsGenericMethod && m.GetParameters().Length == 0);

        private async Task<object?> FindAsync(Type t, int id)
        {
            var q = ApplyEquals(Set(t), t, "Id", id.ToString());
            var list = await ToListAsync(q, t);
            return list.FirstOrDefault();
        }

        private static IQueryable ApplyEquals(IQueryable q, Type t, string prop, string value)
        {
            var p = AdminSchema.Prop(t, prop);
            if (p is null) return q;

            var param = Expression.Parameter(t, "x");
            var member = Expression.Property(param, p);

            object? typed;
            var pt = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
            if (pt == typeof(int)) { if (!int.TryParse(value, out var i)) return q; typed = i; }
            else typed = value;

            var constant = Expression.Constant(typed, p.PropertyType);
            var body = Expression.Equal(member, constant);
            var lambda = Expression.Lambda(body, param);

            return q.Provider.CreateQuery(Expression.Call(
                typeof(Queryable), "Where", new[] { t }, q.Expression, Expression.Quote(lambda)));
        }

        /// <summary>OR-combines a Contains() filter across every searchable string field.</summary>
        private static IQueryable ApplySearch(IQueryable q, EntityDef def, string term)
        {
            var t = def.ClrType;
            var param = Expression.Parameter(t, "x");
            Expression? body = null;

            var contains = typeof(string).GetMethod("Contains", new[] { typeof(string) })!;
            var needle = Expression.Constant(term, typeof(string));

            foreach (var name in def.SearchFields)
            {
                var p = AdminSchema.Prop(t, name);
                if (p is null || p.PropertyType != typeof(string)) continue;

                var member = Expression.Property(param, p);
                var notNull = Expression.NotEqual(member, Expression.Constant(null, typeof(string)));
                var call = Expression.Call(member, contains, needle);
                var clause = Expression.AndAlso(notNull, call);
                body = body is null ? clause : Expression.OrElse(body, clause);
            }

            if (body is null) return q;

            var lambda = Expression.Lambda(body, param);
            return q.Provider.CreateQuery(Expression.Call(
                typeof(Queryable), "Where", new[] { t }, q.Expression, Expression.Quote(lambda)));
        }

        private static IQueryable ApplyOrder(IQueryable q, EntityDef def)
        {
            var t = def.ClrType;
            var first = true;

            foreach (var part in def.OrderBy.Split(',', StringSplitOptions.RemoveEmptyEntries))
            {
                var bits = part.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var p = AdminSchema.Prop(t, bits[0]);
                if (p is null) continue;

                var desc = bits.Length > 1 && bits[1].StartsWith("desc", StringComparison.OrdinalIgnoreCase);
                var method = first ? (desc ? "OrderByDescending" : "OrderBy")
                                   : (desc ? "ThenByDescending" : "ThenBy");

                var param = Expression.Parameter(t, "x");
                var lambda = Expression.Lambda(Expression.Property(param, p), param);

                q = q.Provider.CreateQuery(Expression.Call(
                    typeof(Queryable), method, new[] { t, p.PropertyType },
                    q.Expression, Expression.Quote(lambda)));
                first = false;
            }
            return q;
        }

        private static IQueryable ApplySkip(IQueryable q, Type t, int n) =>
            n <= 0 ? q : q.Provider.CreateQuery(Expression.Call(
                typeof(Queryable), "Skip", new[] { t }, q.Expression, Expression.Constant(n)));

        private static IQueryable ApplyTake(IQueryable q, Type t, int n) =>
            q.Provider.CreateQuery(Expression.Call(
                typeof(Queryable), "Take", new[] { t }, q.Expression, Expression.Constant(n)));

        private static async Task<List<object>> ToListAsync(IQueryable q, Type t)
        {
            var task = (Task)_toListAsync.MakeGenericMethod(t)
                .Invoke(null, new object?[] { q, CancellationToken.None })!;
            await task;
            var result = task.GetType().GetProperty("Result")!.GetValue(task)!;
            return ((IEnumerable)result).Cast<object>().ToList();
        }
        private static readonly MethodInfo _toListAsync = typeof(EntityFrameworkQueryableExtensions)
            .GetMethods().First(m => m.Name == "ToListAsync" && m.GetParameters().Length == 2);

        private static async Task<int> CountAsync(IQueryable q, Type t)
        {
            var task = (Task)_countAsync.MakeGenericMethod(t)
                .Invoke(null, new object?[] { q, CancellationToken.None })!;
            await task;
            return (int)task.GetType().GetProperty("Result")!.GetValue(task)!;
        }
        private static readonly MethodInfo _countAsync = typeof(EntityFrameworkQueryableExtensions)
            .GetMethods().First(m => m.Name == "CountAsync" && m.GetParameters().Length == 2);

        private static void SetIfExists(object entity, string prop, object value)
        {
            var p = AdminSchema.Prop(entity.GetType(), prop);
            if (p is null || !p.CanWrite) return;
            var target = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
            if (!target.IsInstanceOfType(value)) return;
            p.SetValue(entity, value);
        }
    }
}
