// Mobile sidebar toggle with document-level event delegation
// Works seamlessly across static SSR, enhanced navigation, and interactive Blazor modes
(function () {
    function toggleSidebar(forceState) {
        var shell = document.getElementById('dashboard-shell');
        var sidebar = document.getElementById('dashboard-sidebar');
        var backdrop = document.getElementById('sidebar-backdrop');
        var toggleBtn = document.getElementById('mobile-sidebar-toggle');
        if (!sidebar) return;

        var shouldOpen = typeof forceState === 'boolean'
            ? forceState
            : !sidebar.classList.contains('sidebar-open');

        if (shell) shell.classList.toggle('sidebar-open', shouldOpen);
        sidebar.classList.toggle('sidebar-open', shouldOpen);
        if (backdrop) backdrop.classList.toggle('sidebar-open', shouldOpen);
        if (toggleBtn) {
            toggleBtn.setAttribute('aria-expanded', shouldOpen ? 'true' : 'false');
            toggleBtn.classList.toggle('active', shouldOpen);
        }
    }

    document.addEventListener('click', function (e) {
        var toggle = e.target.closest('#mobile-sidebar-toggle, .mobile-menu-btn');
        if (toggle) {
            e.preventDefault();
            e.stopPropagation();
            toggleSidebar();
            return;
        }

        var backdrop = e.target.closest('#sidebar-backdrop, .sidebar-backdrop');
        if (backdrop) {
            e.preventDefault();
            e.stopPropagation();
            toggleSidebar(false);
            return;
        }

        var navLink = e.target.closest('.dashboard-sidebar .sidebar-link, .dashboard-sidebar .sidebar-create, .dashboard-sidebar .sidebar-signout');
        if (navLink) {
            toggleSidebar(false);
        }
    });

    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape') {
            toggleSidebar(false);
        }
    });
})();
