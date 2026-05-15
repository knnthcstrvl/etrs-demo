window.etrWorkFromHome = (function () {
    const handlers = new WeakMap();

    function registerOutsideClick(element, dotNetRef) {
        if (!element) return;
        const el = element instanceof HTMLElement ? element : element;
        const handler = function (ev) {
            try {
                if (!el.contains(ev.target)) {
                    dotNetRef.invokeMethodAsync('HideEmployeeResults').catch(() => { });
                }
            }
            catch { }
        };
        document.addEventListener('click', handler, { capture: true });
        handlers.set(element, handler);
    }

    function unregisterOutsideClick(element) {
        if (!element) return;
        const h = handlers.get(element);
        if (h) {
            document.removeEventListener('click', h, { capture: true });
            handlers.delete(element);
        }
    }

    return {
        registerOutsideClick: registerOutsideClick,
        unregisterOutsideClick: unregisterOutsideClick
    };
})();

// helper to scroll an element by id into view if present
window.etrWorkFromHome.scrollElementIntoView = function (id) {
    try {
        if (!id) return;
        const el = document.getElementById(id);
        if (el) {
            // scroll the parent dropdown if necessary
            const parent = el.closest('.dropdown-menu');
            if (parent) {
                const rect = el.getBoundingClientRect();
                const parentRect = parent.getBoundingClientRect();
                if (rect.top < parentRect.top || rect.bottom > parentRect.bottom) {
                    // align to top
                    parent.scrollTop = el.offsetTop - parent.offsetTop - 8;
                }
            }
        }
    }
    catch { }
};
