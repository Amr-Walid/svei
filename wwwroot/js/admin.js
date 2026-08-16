/* ═══════════════════════════════════════════════════════════════════════════
   SVEI — Admin panel behaviour
   Theme toggle · sidebar · Quill editors · Sortable rows · inline toggles
   · slug helper · upload previews · bulk select · confirm dialogs · toasts
   ═══════════════════════════════════════════════════════════════════════════ */
(function () {
    'use strict';

    var $  = function (s, r) { return (r || document).querySelector(s); };
    var $$ = function (s, r) { return Array.prototype.slice.call((r || document).querySelectorAll(s)); };

    function token() {
        var el = $('input[name="__RequestVerificationToken"]');
        return el ? el.value : '';
    }

    /* ── Theme ─────────────────────────────────────────────────────────── */
    function initTheme() {
        var btn = $('[data-theme-toggle]');
        if (!btn) return;

        function paint() {
            var t = document.documentElement.getAttribute('data-theme');
            var i = btn.querySelector('i');
            if (i) i.className = t === 'dark' ? 'fa-solid fa-sun' : 'fa-solid fa-moon';
        }
        paint();

        btn.addEventListener('click', function () {
            var cur = document.documentElement.getAttribute('data-theme');
            var next = cur === 'dark' ? 'light' : 'dark';
            document.documentElement.setAttribute('data-theme', next);
            try { localStorage.setItem('svei-admin-theme', next); } catch (e) { }
            paint();
        });
    }

    /* ── Sidebar ───────────────────────────────────────────────────────── */
    function initSidebar() {
        var body = document.body;
        var mobile = function () { return window.matchMedia('(max-width: 820px)').matches; };

        try {
            if (localStorage.getItem('svei-admin-collapsed') === '1' && !mobile())
                body.classList.add('ad--collapsed');
        } catch (e) { }

        var toggle = $('[data-toggle-sb]');
        if (toggle) {
            toggle.addEventListener('click', function () {
                if (mobile()) {
                    body.classList.toggle('ad--open');
                } else {
                    body.classList.toggle('ad--collapsed');
                    try {
                        localStorage.setItem('svei-admin-collapsed',
                            body.classList.contains('ad--collapsed') ? '1' : '0');
                    } catch (e) { }
                }
            });
        }

        var scrim = $('[data-close-sb]');
        if (scrim) scrim.addEventListener('click', function () { body.classList.remove('ad--open'); });

        // keep the active link in view
        var on = $('.ad-sb__link.is-on');
        if (on && on.scrollIntoView) {
            try { on.scrollIntoView({ block: 'center' }); } catch (e) { }
        }
    }

    /* ── Rich text (Quill) ─────────────────────────────────────────────── */
    function initEditors() {
        if (typeof Quill === 'undefined') return;

        $$('[data-rte]').forEach(function (host) {
            var name = host.getAttribute('data-rte');
            var hidden = document.querySelector('input[type="hidden"][name="' + name + '"]');
            if (!hidden) return;

            var q = new Quill(host, {
                theme: 'snow',
                modules: {
                    toolbar: [
                        [{ header: [2, 3, 4, false] }],
                        ['bold', 'italic', 'underline', 'strike'],
                        [{ list: 'ordered' }, { list: 'bullet' }],
                        [{ align: [] }, { direction: 'rtl' }],
                        ['blockquote', 'link'],
                        [{ color: [] }, { background: [] }],
                        ['clean']
                    ]
                }
            });

            // seed existing HTML
            if (hidden.value) {
                try { q.clipboard.dangerouslyPasteHTML(hidden.value); }
                catch (e) { q.root.innerHTML = hidden.value; }
            }

            q.on('text-change', function () {
                var html = q.root.innerHTML;
                hidden.value = (html === '<p><br></p>') ? '' : html;
            });
        });
    }

    /* ── Sortable rows ─────────────────────────────────────────────────── */
    function initSortable() {
        if (typeof Sortable === 'undefined') return;

        $$('[data-sortable]').forEach(function (tbody) {
            var url = tbody.getAttribute('data-sortable');
            Sortable.create(tbody, {
                handle: '.ad-drag',
                animation: 150,
                ghostClass: 'sortable-ghost',
                onEnd: function () {
                    var ids = $$('tr[data-id]', tbody).map(function (tr) { return tr.getAttribute('data-id'); });
                    var fd = new FormData();
                    fd.append('__RequestVerificationToken', token());
                    ids.forEach(function (id) { fd.append('ids', id); });

                    fetch(url, { method: 'POST', body: fd, credentials: 'same-origin' })
                        .then(function (r) { return r.json(); })
                        .then(function () { toast('تم حفظ الترتيب ✓ / Order saved ✓'); })
                        .catch(function () { toast('تعذّر حفظ الترتيب / Could not save order', 'err'); });
                }
            });
        });
    }

    /* ── Inline boolean toggles ────────────────────────────────────────── */
    function initToggles() {
        $$('[data-toggle-url]').forEach(function (input) {
            input.addEventListener('change', function () {
                var url = input.getAttribute('data-toggle-url');
                var field = input.getAttribute('data-toggle-field');
                var fd = new FormData();
                fd.append('__RequestVerificationToken', token());
                fd.append('field', field);

                input.disabled = true;
                fetch(url, { method: 'POST', body: fd, credentials: 'same-origin' })
                    .then(function (r) { return r.json(); })
                    .then(function (d) { input.checked = !!d.value; })
                    .catch(function () {
                        input.checked = !input.checked;
                        toast('تعذّر التحديث / Update failed', 'err');
                    })
                    .then(function () { input.disabled = false; });
            });
        });
    }

    /* ── Slug helper ───────────────────────────────────────────────────── */
    function slugify(s) {
        return (s || '')
            .toString().trim().toLowerCase()
            .replace(/[\u0621-\u064A]/g, function (c) { return c; })   // keep Arabic letters
            .replace(/[^\w\u0621-\u064A\s-]/g, '')
            .replace(/[\s_]+/g, '-')
            .replace(/-+/g, '-')
            .replace(/^-|-$/g, '');
    }

    function initSlug() {
        $$('[data-slug-from]').forEach(function (slugInput) {
            var srcName = slugInput.getAttribute('data-slug-from');
            var src = document.querySelector('[name="' + srcName + '"]');
            if (!src) return;

            var touched = slugInput.value.trim().length > 0;
            slugInput.addEventListener('input', function () { touched = true; });

            src.addEventListener('input', function () {
                if (!touched) slugInput.value = slugify(src.value);
            });

            var btn = slugInput.parentElement
                ? slugInput.parentElement.querySelector('[data-slug-gen]') : null;
            if (btn) {
                btn.addEventListener('click', function () {
                    slugInput.value = slugify(src.value);
                    touched = true;
                });
            }
        });
    }

    /* ── Upload previews ───────────────────────────────────────────────── */
    function initUploads() {
        $$('.ad-up input[type="file"]').forEach(function (input) {
            input.addEventListener('change', function () {
                var box = input.closest('.ad-up');
                if (!box) return;
                var prev = box.querySelector('.ad-up__prev');
                var name = box.querySelector('.ad-up__name');
                var f = input.files && input.files[0];
                if (!f) return;

                if (name) name.textContent = f.name;
                if (prev && /^image\//.test(f.type)) {
                    var reader = new FileReader();
                    reader.onload = function (e) {
                        prev.innerHTML = '<img src="' + e.target.result + '" alt="" />';
                    };
                    reader.readAsDataURL(f);
                }
            });
        });

        // "remove current file" checkbox
        $$('[data-clear-file]').forEach(function (cb) {
            cb.addEventListener('change', function () {
                var box = cb.closest('.ad-up');
                if (!box) return;
                var prev = box.querySelector('.ad-up__prev');
                if (prev) prev.style.opacity = cb.checked ? '.3' : '1';
            });
        });
    }

    /* ── Bulk selection ────────────────────────────────────────────────── */
    function initBulk() {
        var all = $('[data-check-all]');
        if (all) {
            all.addEventListener('change', function () {
                $$('[data-check]').forEach(function (cb) { cb.checked = all.checked; });
                syncBulk();
            });
        }
        $$('[data-check]').forEach(function (cb) { cb.addEventListener('change', syncBulk); });
        syncBulk();

        function syncBulk() {
            var n = $$('[data-check]:checked').length;
            var bar = $('[data-bulk-bar]');
            if (bar) bar.style.display = n > 0 ? 'flex' : 'none';
            var lbl = $('[data-bulk-count]');
            if (lbl) lbl.textContent = n;
        }

        $$('[data-bulk-action]').forEach(function (btn) {
            btn.addEventListener('click', function () {
                var action = btn.getAttribute('data-bulk-action');
                var form = $('#bulkForm');
                if (!form) return;
                if (action === 'delete' && !confirm(btn.getAttribute('data-confirm') || 'Delete selected items?')) return;
                var hidden = form.querySelector('input[name="action"]');
                if (hidden) hidden.value = action;
                form.submit();
            });
        });
    }

    /* ── Confirm dialogs ───────────────────────────────────────────────── */
    function initConfirm() {
        document.addEventListener('submit', function (e) {
            var f = e.target;
            if (f && f.hasAttribute && f.hasAttribute('data-confirm')) {
                if (!confirm(f.getAttribute('data-confirm'))) e.preventDefault();
            }
        });
    }

    /* ── Toasts ────────────────────────────────────────────────────────── */
    function toast(msg, kind) {
        var wrap = $('.ad-toasts');
        if (!wrap) return;
        var el = document.createElement('div');
        el.className = 'ad-toast' + (kind === 'err' ? ' ad-toast--err' : kind === 'warn' ? ' ad-toast--warn' : '');
        el.innerHTML = '<i class="fa-solid ' +
            (kind === 'err' ? 'fa-circle-xmark' : kind === 'warn' ? 'fa-triangle-exclamation' : 'fa-circle-check') +
            '"></i><span></span>';
        el.querySelector('span').textContent = msg;
        wrap.appendChild(el);
        setTimeout(function () { fade(el); }, 3200);
    }

    function fade(el) {
        el.style.transition = 'opacity .3s, transform .3s';
        el.style.opacity = '0';
        el.style.transform = 'translateY(-8px)';
        setTimeout(function () { if (el.parentNode) el.parentNode.removeChild(el); }, 320);
    }

    function autoDismiss() {
        $$('.ad-toasts .ad-toast').forEach(function (el) {
            setTimeout(function () { fade(el); }, 4200);
        });
    }

    /* ── Unsaved-changes guard ─────────────────────────────────────────── */
    function initDirtyGuard() {
        var form = $('form[data-guard]');
        if (!form) return;
        var dirty = false;
        form.addEventListener('input', function () { dirty = true; });
        form.addEventListener('change', function () { dirty = true; });
        form.addEventListener('submit', function () { dirty = false; });
        window.addEventListener('beforeunload', function (e) {
            if (!dirty) return;
            e.preventDefault();
            e.returnValue = '';
        });
    }

    /* ── Keyboard shortcuts ────────────────────────────────────────────── */
    function initKeys() {
        document.addEventListener('keydown', function (e) {
            // Ctrl/Cmd + S saves the open form
            if ((e.ctrlKey || e.metaKey) && e.key.toLowerCase() === 's') {
                var form = $('form[data-guard]');
                if (form) { e.preventDefault(); form.requestSubmit ? form.requestSubmit() : form.submit(); }
            }
            // "/" focuses search
            if (e.key === '/' && !/input|textarea|select/i.test(document.activeElement.tagName)) {
                var s = $('[data-search]');
                if (s) { e.preventDefault(); s.focus(); }
            }
        });
    }

    /* ── Inline-handler replacements ────────────────────────────────────
       These used to be onclick="" / oninput="" / onerror="" attributes in the
       Razor views. Inline handlers are blocked by the Content-Security-Policy
       (script-src 'self'), and allowing them back with 'unsafe-inline' would
       defeat most of the XSS protection the CSP exists to provide — so the
       behaviour lives here instead, bound by delegation.                    */
    function initInlineReplacements() {
        // "Save & stay" — flag the form so the server returns to the editor.
        document.addEventListener('click', function (e) {
            var stay = e.target.closest('[data-stay]');
            if (stay) {
                var flag = document.getElementById('stayFlag');
                if (flag) flag.value = '1';
            }
        });

        // Row delete — confirm, then submit the matching hidden form.
        document.addEventListener('click', function (e) {
            var btn = e.target.closest('[data-del-form]');
            if (!btn) return;
            var msg = btn.getAttribute('data-del-confirm') || 'Delete?';
            if (!confirm(msg)) return;
            var form = document.getElementById(btn.getAttribute('data-del-form'));
            if (form) form.submit();
        });

        // Media library — copy the asset path to the clipboard.
        document.addEventListener('click', function (e) {
            var btn = e.target.closest('[data-copy]');
            if (!btn) return;
            var path = btn.getAttribute('data-p') || '';
            var done = btn.getAttribute('data-copy');
            var ok = function () { if (window.adToast) window.adToast(done); };

            if (navigator.clipboard && navigator.clipboard.writeText) {
                navigator.clipboard.writeText(path).then(ok, function () { });
            } else {
                var t = document.createElement('textarea');
                t.value = path;
                document.body.appendChild(t);
                t.select();
                try { document.execCommand('copy'); ok(); } catch (err) { }
                document.body.removeChild(t);
            }
        });

        // Colour picker mirrors its value into the adjacent hex text input.
        document.addEventListener('input', function (e) {
            var pick = e.target.closest('[data-color-sync]');
            if (pick && pick.nextElementSibling)
                pick.nextElementSibling.value = pick.value;
        });

        // Hide a broken logo rather than showing the browser's placeholder.
        // Capture phase: "error" does not bubble.
        document.addEventListener('error', function (e) {
            var img = e.target;
            if (img && img.tagName === 'IMG' && img.hasAttribute('data-hide-on-error'))
                img.style.display = 'none';
        }, true);
    }

    /* ── Boot ──────────────────────────────────────────────────────────── */
    document.addEventListener('DOMContentLoaded', function () {
        initTheme();
        initSidebar();
        initEditors();
        initSortable();
        initToggles();
        initSlug();
        initUploads();
        initBulk();
        initConfirm();
        initDirtyGuard();
        initKeys();
        initInlineReplacements();
        autoDismiss();
    });

    window.adToast = toast;
})();
