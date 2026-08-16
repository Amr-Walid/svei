/*!
 * async-css.js — promote non-critical stylesheets once the page can paint.
 *
 * The icon font, carousel and lightbox stylesheets are marked media="print" in
 * the markup so the browser downloads them at low priority without blocking
 * first paint. This flips them back to media="all" as soon as they have
 * arrived, which is the standard "print onload" trick — implemented here as an
 * external file because the site runs a strict CSP (script-src 'self'), so the
 * usual inline onload="this.media='all'" attribute is not allowed.
 *
 * Loaded synchronously in <head>: it is ~1 KB, it must run before the sheets
 * finish downloading, and doing it here avoids a flash of unstyled icons.
 */
(function () {
    'use strict';

    function promote(link) {
        if (!link || link.media === 'all') return;
        link.media = 'all';
    }

    function init() {
        var sheets = document.querySelectorAll('link[data-async-style]');
        for (var i = 0; i < sheets.length; i++) {
            var link = sheets[i];

            // Already cached and parsed before this script ran.
            if (link.sheet) {
                promote(link);
                continue;
            }

            // Bind per-link so the closure captures the right element.
            (function (el) {
                el.addEventListener('load', function () { promote(el); }, { once: true });
                // If the request fails we still want the site usable; leaving the
                // sheet on media="print" would silently drop every icon.
                el.addEventListener('error', function () { promote(el); }, { once: true });
            })(link);
        }

        // Safety net: if a sheet neither loads nor errors (proxy stalls, blocked
        // request), promote everything once the page is interactive so icons are
        // never permanently invisible.
        window.addEventListener('load', function () {
            var all = document.querySelectorAll('link[data-async-style]');
            for (var j = 0; j < all.length; j++) promote(all[j]);
        }, { once: true });
    }

    if (document.readyState === 'loading') {
        document.addEventListener('DOMContentLoaded', init, { once: true });
        // DOMContentLoaded can be late; also try immediately for links already
        // parsed above this script.
        init();
    } else {
        init();
    }
})();
