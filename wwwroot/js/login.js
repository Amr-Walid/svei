/*
 * Login page behaviour.
 *
 * External rather than an inline onerror="" attribute so the page is covered
 * by the same script-src 'self' CSP as the rest of the site. The login screen
 * is the one page an unauthenticated attacker can always reach, so it is the
 * last place that should be granted a CSP exception.
 */
(function () {
    // Hide the brand logo if it fails to load (e.g. the admin set a path that
    // no longer exists) instead of showing the browser's broken-image icon.
    // Bound in the capture phase because "error" does not bubble.
    document.addEventListener('error', function (e) {
        var el = e.target;
        if (el && el.tagName === 'IMG' && el.hasAttribute('data-hide-on-error'))
            el.style.display = 'none';
    }, true);
})();
