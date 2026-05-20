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

    function shouldOpenMenuUpward(element, menuHeight, viewportPadding) {
        if (!element) {
            return false;
        }

        const rect = element.getBoundingClientRect();
        const padding = Number.isFinite(viewportPadding) ? viewportPadding : 12;
        const preferredHeight = Number.isFinite(menuHeight) ? menuHeight : 320;
        const spaceBelow = window.innerHeight - rect.bottom - padding;
        const spaceAbove = rect.top - padding;

        if (spaceBelow >= preferredHeight) {
            return false;
        }

        return spaceAbove > spaceBelow;
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

    function downloadFileFromBase64(fileName, contentBase64, contentType) {
        if (!contentBase64) {
            return;
        }

        const binary = atob(contentBase64);
        const bytes = new Uint8Array(binary.length);

        for (let i = 0; i < binary.length; i++) {
            bytes[i] = binary.charCodeAt(i);
        }

        const blob = new Blob([bytes], { type: contentType || "application/octet-stream" });
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
        shouldOpenMenuUpward: shouldOpenMenuUpward,
        downloadCsv: downloadCsv,
        downloadFileFromBase64: downloadFileFromBase64
    };
})();
