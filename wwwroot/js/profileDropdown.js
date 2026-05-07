document.addEventListener('DOMContentLoaded', function () {
    document.querySelectorAll('.dropdown').forEach(function (drop) {
        var toggle = drop.querySelector('[data-bs-toggle="dropdown"]');
        var menu = drop.querySelector('.profile-dropdown');
        if (!toggle || !menu) return;

        // Store original parent for restoration
        var originalParent = menu.parentElement;

        drop.addEventListener('show.bs.dropdown', function () {
            // Move menu to body so it's not clipped by overflow on ancestors
            document.body.appendChild(menu);
            menu.classList.add('fixed-fallback');
            menu.style.position = 'fixed';
            var rect = toggle.getBoundingClientRect();
            // Prefer aligning right edges
            var menuWidth = menu.offsetWidth || 300;
            var left = Math.round(rect.right - menuWidth);
            if (left < 8) left = 8;
            menu.style.top = (Math.round(rect.bottom) + 6) + 'px';
            menu.style.left = left + 'px';
            menu.style.zIndex = 99999;
        });

        drop.addEventListener('hidden.bs.dropdown', function () {
            // Remove fixed positioning and restore to original parent
            menu.classList.remove('fixed-fallback');
            menu.style.position = '';
            menu.style.top = '';
            menu.style.left = '';
            menu.style.zIndex = '';
            // Put back into original container (anchor) so Bootstrap can reuse it
            if (originalParent) originalParent.appendChild(menu);
        });

        // Reposition on window resize while open
        window.addEventListener('resize', function () {
            if (menu.classList.contains('fixed-fallback')) {
                var rect = toggle.getBoundingClientRect();
                var menuWidth = menu.offsetWidth || 300;
                var left = Math.round(rect.right - menuWidth);
                if (left < 8) left = 8;
                menu.style.top = (Math.round(rect.bottom) + 6) + 'px';
                menu.style.left = left + 'px';
            }
        });
    });
});