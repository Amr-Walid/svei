/*
 * Applies the saved admin theme before first paint.
 *
 * This runs from an external file rather than an inline <script> so the
 * Content-Security-Policy can stay at script-src 'self'. It must still be
 * loaded synchronously in <head> (no defer) — the whole point is to set the
 * attribute before the browser paints, otherwise a dark-theme user sees a
 * flash of the light theme on every page load.
 */
(function () {
    try {
        var t = localStorage.getItem('svei-admin-theme') || 'light';
        document.documentElement.setAttribute('data-theme', t);
    } catch (e) { /* private mode / storage disabled — keep the default */ }
})();
