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
