window.etrsLayoutSync = (() => {
    let resizeObserver;
    let mutationObserver;
    let cleanupScheduled = false;

    function getElements() {
        return {
            sidebarTop: document.querySelector('.etrs-sidebar-top'),
            header: document.querySelector('.finance-header')
        };
    }

    function syncHeights() {
        const { sidebarTop, header } = getElements();

        if (!sidebarTop || !header) {
            return;
        }

        sidebarTop.style.height = '';
        header.style.height = '';

        const targetHeight = Math.max(sidebarTop.offsetHeight, header.offsetHeight);

        if (targetHeight > 0) {
            const value = `${targetHeight}px`;
            sidebarTop.style.height = value;
            header.style.height = value;
        }
    }

    function queueSync() {
        if (cleanupScheduled) {
            return;
        }

        cleanupScheduled = true;
        requestAnimationFrame(() => {
            cleanupScheduled = false;
            syncHeights();
        });
    }

    function observeElement(element) {
        if (element) {
            resizeObserver.observe(element);
        }
    }

    function init() {
        if (!resizeObserver) {
            resizeObserver = new ResizeObserver(() => queueSync());
        }

        if (!mutationObserver) {
            mutationObserver = new MutationObserver(() => {
                const { sidebarTop, header } = getElements();

                if (sidebarTop && header) {
                    observeElement(sidebarTop);
                    observeElement(header);
                    queueSync();
                }
            });

            mutationObserver.observe(document.body, {
                childList: true,
                subtree: true
            });
        }

        const { sidebarTop, header } = getElements();
        observeElement(sidebarTop);
        observeElement(header);

        window.addEventListener('resize', queueSync, { passive: true });
        queueSync();
    }

    function getSidebarOpen() {
        try {
            const value = window.localStorage.getItem('etrs.sidebar.isOpen');
            if (value === null) {
                return null;
            }

            return value === 'true';
        } catch (e) {
            return null;
        }
    }

    function setSidebarOpen(isOpen) {
        try {
            window.localStorage.setItem('etrs.sidebar.isOpen', isOpen ? 'true' : 'false');
        } catch (e) {
            /* ignore storage failures */
        }
    }

    function getStoredTheme() {
        try {
            return window.localStorage.getItem('etrs.theme');
        } catch (e) {
            return null;
        }
    }

    function setStoredTheme(theme) {
        try {
            window.localStorage.setItem('etrs.theme', theme);
        } catch (e) {
            /* ignore storage failures */
        }
    }

    function applyTheme(theme) {
        const isDark = theme === 'dark';
        document.documentElement.classList.toggle('etrs-dark-theme', isDark);
        document.body.classList.toggle('etrs-dark-theme', isDark);
        document.documentElement.setAttribute('data-bs-theme', isDark ? 'dark' : 'light');
        document.documentElement.style.colorScheme = isDark ? 'dark' : 'light';
        window.dispatchEvent(new CustomEvent('etrs:theme-changed', { detail: { theme } }));
    }

    function initTheme() {
        const savedTheme = getStoredTheme();
        applyTheme(savedTheme === 'dark' ? 'dark' : 'light');
    }

    function toggleTheme() {
        const nextTheme = document.body.classList.contains('etrs-dark-theme') ? 'light' : 'dark';
        setStoredTheme(nextTheme);
        applyTheme(nextTheme);
    }

    initTheme();

    return { init, getSidebarOpen, setSidebarOpen, initTheme, toggleTheme };
})();
