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

    function getRowMenuPosition(rowKey, pointerX, pointerY, menuWidth, menuHeight, viewportPadding) {
        const row = document.querySelector(`[data-row-menu-key="${CSS.escape(rowKey || "")}"]`);
        if (!row) {
            return { left: 0, top: 0 };
        }

        const rowRect = row.getBoundingClientRect();
        const tableCard = row.closest(".attendance-table-card");
        const cardRect = tableCard ? tableCard.getBoundingClientRect() : rowRect;
        const padding = Number.isFinite(viewportPadding) ? viewportPadding : 12;
        const width = Number.isFinite(menuWidth) ? menuWidth : 152;
        const height = Number.isFinite(menuHeight) ? menuHeight : 88;
        const x = Number.isFinite(pointerX) ? pointerX : rowRect.left;
        const y = Number.isFinite(pointerY) ? pointerY : rowRect.top;
        const visibleRight = Math.min(cardRect.right, window.innerWidth - padding);
        const visibleLeft = Math.max(cardRect.left, padding);
        const preferredLeft = x + 12;
        const preferredTop = y + 12;
        const left = Math.min(Math.max(visibleLeft, preferredLeft), Math.max(visibleLeft, visibleRight - width));
        const rowBottomLimit = rowRect.bottom - 8;
        const rowAlignedTop = Math.min(Math.max(rowRect.top + 8, preferredTop), Math.max(rowRect.top + 8, rowBottomLimit));
        const top = Math.min(
            Math.max(padding, rowAlignedTop),
            Math.max(padding, window.innerHeight - height - padding)
        );

        return {
            left: Math.round(left),
            top: Math.round(top)
        };
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
        getRowMenuPosition: getRowMenuPosition,
        downloadCsv: downloadCsv,
        downloadFileFromBase64: downloadFileFromBase64
    };
})();
