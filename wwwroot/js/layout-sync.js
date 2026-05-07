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

    return { init };
})();
