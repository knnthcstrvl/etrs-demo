window.attendanceLogs = (function () {
    const periodDismissHandlers = new WeakMap();
    const tableMenuDismissHandlers = new WeakMap();
    let lastPointerTarget = null;

    function handlePointerMove(ev) {
        lastPointerTarget = ev.target || null;
    }

    document.addEventListener("pointermove", handlePointerMove, { capture: true, passive: true });

    function openFileDialog(element) {
        if (!element) {
            return;
        }

        element.click();
    }

    function registerPeriodDismiss(element, dotNetRef) {
        if (!element || !dotNetRef) {
            return;
        }

        unregisterPeriodDismiss(element);

        const clickHandler = function (ev) {
            try {
                if (!element.contains(ev.target)) {
                    dotNetRef.invokeMethodAsync("ClosePeriodMenus").catch(() => { });
                }
            }
            catch {
            }
        };

        const scrollHandler = function () {
            try {
                dotNetRef.invokeMethodAsync("ClosePeriodMenus").catch(() => { });
            }
            catch {
            }
        };

        document.addEventListener("click", clickHandler, { capture: true });
        window.addEventListener("scroll", scrollHandler, { capture: true, passive: true });

        periodDismissHandlers.set(element, {
            clickHandler: clickHandler,
            scrollHandler: scrollHandler
        });
    }

    function unregisterPeriodDismiss(element) {
        if (!element) {
            return;
        }

        const handlers = periodDismissHandlers.get(element);
        if (!handlers) {
            return;
        }

        document.removeEventListener("click", handlers.clickHandler, { capture: true });
        window.removeEventListener("scroll", handlers.scrollHandler, { capture: true });
        periodDismissHandlers.delete(element);
    }

    function registerTableMenuDismiss(element, dotNetRef) {
        if (!element || !dotNetRef) {
            return;
        }

        unregisterTableMenuDismiss(element);

        const clickHandler = function (ev) {
            try {
                if (!element.contains(ev.target)) {
                    dotNetRef.invokeMethodAsync("CloseTableMenus").catch(() => { });
                }
            }
            catch {
            }
        };

        const scrollHandler = function () {
            try {
                if (lastPointerTarget && element.contains(lastPointerTarget)) {
                    return;
                }

                dotNetRef.invokeMethodAsync("CloseTableMenus").catch(() => { });
            }
            catch {
            }
        };

        document.addEventListener("click", clickHandler, { capture: true });
        window.addEventListener("scroll", scrollHandler, { capture: true, passive: true });

        tableMenuDismissHandlers.set(element, {
            clickHandler: clickHandler,
            scrollHandler: scrollHandler
        });
    }

    function unregisterTableMenuDismiss(element) {
        if (!element) {
            return;
        }

        const handlers = tableMenuDismissHandlers.get(element);
        if (!handlers) {
            return;
        }

        document.removeEventListener("click", handlers.clickHandler, { capture: true });
        window.removeEventListener("scroll", handlers.scrollHandler, { capture: true });
        tableMenuDismissHandlers.delete(element);
    }

    function downloadCsv(fileName, content) {
        const csvWithBom = "\uFEFF" + (content || "");
        const blob = new Blob([csvWithBom], { type: "text/csv;charset=utf-8;" });
        const url = URL.createObjectURL(blob);
        const link = document.createElement("a");

        link.href = url;
        link.download = fileName;
        link.style.display = "none";

        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);

        setTimeout(function () {
            URL.revokeObjectURL(url);
        }, 0);
    }

    return {
        openFileDialog: openFileDialog,
        registerPeriodDismiss: registerPeriodDismiss,
        unregisterPeriodDismiss: unregisterPeriodDismiss,
        registerTableMenuDismiss: registerTableMenuDismiss,
        unregisterTableMenuDismiss: unregisterTableMenuDismiss,
        downloadCsv: downloadCsv
    };
})();
