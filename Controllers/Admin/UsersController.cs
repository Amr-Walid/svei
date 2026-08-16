using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SVEI.Web.Data;
using SVEI.Web.Services;
using SVEI.Web.ViewModels.Admin;

namespace SVEI.Web.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("Admin/Users")]
    public class UsersController : AdminBaseController
    {
        private readonly UserManager<IdentityUser> _users;
        private readonly RoleManager<IdentityRole> _roles;

        public UsersController(AppDbContext db, ILang lang,
            UserManager<IdentityUser> users, RoleManager<IdentityRole> roles) : base(db, lang)
        { _users = users; _roles = roles; }

        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var list = new List<AdminUserVm>();
            foreach (var u in await _users.Users.OrderBy(u => u.Email).ToListAsync())
            {
                list.Add(new AdminUserVm
                {
                    Id = u.Id,
                    Email = u.Email ?? u.UserName ?? "",
                    Roles = (await _users.GetRolesAsync(u)).ToList(),
                    LockoutEnd = u.LockoutEnd,
                    LockedOut = u.LockoutEnd.HasValue && u.LockoutEnd > DateTimeOffset.UtcNow
                });
            }

            ViewData["AllRoles"] = await _roles.Roles.Select(r => r.Name!).ToListAsync();
            return View("~/Views/Admin/Users/Index.cshtml", list);
        }

        [HttpPost("create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] string email, [FromForm] string password, [FromForm] string role)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                Err("البريد وكلمة المرور مطلوبان.", "Email and password are required.");
                return RedirectToAction(nameof(Index));
            }

            var user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
            var res = await _users.CreateAsync(user, password);

            if (!res.Succeeded)
            {
                Err(string.Join(" · ", res.Errors.Select(e => e.Description)),
                    string.Join(" · ", res.Errors.Select(e => e.Description)));
                return RedirectToAction(nameof(Index));
            }

            if (!string.IsNullOrWhiteSpace(role) && await _roles.RoleExistsAsync(role))
                await _users.AddToRoleAsync(user, role);

            await AuditAsync("create", "User", user.Id, email);
            Ok("تم إنشاء المستخدم ✓", "User created ✓");
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("password/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Password([FromRoute] string id, [FromForm] string password)
        {
            var user = await _users.FindByIdAsync(id);
            if (user is null) return NotFound();

            var token = await _users.GeneratePasswordResetTokenAsync(user);
            var res = await _users.ResetPasswordAsync(user, token, password);

            if (res.Succeeded)
            {
                await AuditAsync("password", "User", id, user.Email);
                Ok("تم تغيير كلمة المرور ✓", "Password changed ✓");
            }
            else
            {
                Err(string.Join(" · ", res.Errors.Select(e => e.Description)),
                    string.Join(" · ", res.Errors.Select(e => e.Description)));
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("role/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Role([FromRoute] string id, [FromForm] string role)
        {
            var user = await _users.FindByIdAsync(id);
            if (user is null) return NotFound();

            var current = await _users.GetRolesAsync(user);
            await _users.RemoveFromRolesAsync(user, current);
            if (!string.IsNullOrWhiteSpace(role) && await _roles.RoleExistsAsync(role))
                await _users.AddToRoleAsync(user, role);

            await AuditAsync("role", "User", id, $"{user.Email} -> {role}");
            Ok("تم تحديث الصلاحية ✓", "Role updated ✓");
            return RedirectToAction(nameof(Index));
        }

        [HttpPost("delete/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromRoute] string id)
        {
            var user = await _users.FindByIdAsync(id);
            if (user is null) return NotFound();

            // never delete yourself, and never remove the last admin
            if (string.Equals(user.Email, User.Identity?.Name, StringComparison.OrdinalIgnoreCase))
            {
                Err("لا يمكنك حذف حسابك الحالي.", "You cannot delete your own account.");
                return RedirectToAction(nameof(Index));
            }

            var admins = await _users.GetUsersInRoleAsync("Admin");
            if (admins.Count <= 1 && admins.Any(a => a.Id == id))
            {
                Err("لا يمكن حذف آخر مدير للنظام.", "Cannot delete the last administrator.");
                return RedirectToAction(nameof(Index));
            }

            await _users.DeleteAsync(user);
            await AuditAsync("delete", "User", id, user.Email);
            Ok("تم حذف المستخدم ✓", "User deleted ✓");
            return RedirectToAction(nameof(Index));
        }
    }
}
