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

        const dismiss = function () {
            dotNetRef.invokeMethodAsync("CloseTableMenus").catch(() => { });
        };

        const outsideHandler = function (ev) {
            try {
                if (!element.contains(ev.target)) {
                    dismiss();
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

                dismiss();
            }
            catch {
            }
        };

        const scrollTargets = getScrollTargets(element);

        document.addEventListener("pointerdown", outsideHandler, { capture: true });
        document.addEventListener("click", outsideHandler, { capture: true });
        scrollTargets.forEach(function (target) {
            target.addEventListener("scroll", scrollHandler, { capture: true, passive: true });
        });

        tableMenuDismissHandlers.set(element, {
            outsideHandler: outsideHandler,
            scrollHandler: scrollHandler,
            scrollTargets: scrollTargets
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

        document.removeEventListener("pointerdown", handlers.outsideHandler, { capture: true });
        document.removeEventListener("click", handlers.outsideHandler, { capture: true });
        (handlers.scrollTargets || [window]).forEach(function (target) {
            target.removeEventListener("scroll", handlers.scrollHandler, { capture: true });
        });
        tableMenuDismissHandlers.delete(element);
    }

    function getScrollTargets(element) {
        const targets = [window, document, document.documentElement, document.body];
        const appContent = document.querySelector(".app-content");

        if (appContent) {
            targets.push(appContent);
        }

        let parent = element ? element.parentElement : null;
        while (parent) {
            const style = window.getComputedStyle(parent);
            const overflow = `${style.overflow}${style.overflowX}${style.overflowY}`;

            if (/(auto|scroll|overlay)/.test(overflow)) {
                targets.push(parent);
            }

            parent = parent.parentElement;
        }

        return Array.from(new Set(targets.filter(Boolean)));
    }

    function shouldOpenMenuUpward(element, menuHeight, viewportPadding) {
        if (!element) {
            return false;
        }

        return getMenuPlacement(element, menuHeight, viewportPadding).openUpward;
    }

    function getMenuPlacement(element, menuHeight, viewportPadding) {
        if (!element) {
            return { openUpward: false, maxHeight: Number.isFinite(menuHeight) ? menuHeight : 320 };
        }

        const rect = element.getBoundingClientRect();
        const padding = Number.isFinite(viewportPadding) ? viewportPadding : 12;
        const preferredHeight = Number.isFinite(menuHeight) ? menuHeight : 320;
        const spaceBelow = window.innerHeight - rect.bottom - padding;
        const spaceAbove = rect.top - padding;
        const openUpward = spaceBelow < preferredHeight && spaceAbove > spaceBelow;
        const availableSpace = Math.max(openUpward ? spaceAbove : spaceBelow, 96);

        return {
            openUpward: openUpward,
            maxHeight: Math.min(preferredHeight, availableSpace)
        };
    }

    function getAccessChildMenuPlacement(parentKey, menuWidth, menuHeight, viewportPadding) {
        const parent = document.querySelector(`[data-access-child-key="${CSS.escape(parentKey || "")}"]`);
        if (!parent) {
            return { openLeft: false, openUpward: false };
        }

        const rect = parent.getBoundingClientRect();
        const padding = Number.isFinite(viewportPadding) ? viewportPadding : 12;
        const width = Number.isFinite(menuWidth) ? menuWidth : 256;
        const height = Number.isFinite(menuHeight) ? menuHeight : 180;
        const rem = parseFloat(window.getComputedStyle(document.documentElement).fontSize) || 16;
        const horizontalOverlap = 1.1 * rem;
        const verticalOverlap = 0.75 * rem;
        const defaultLeft = rect.right - horizontalOverlap;
        const defaultTop = rect.bottom - verticalOverlap;
        const openLeft = defaultLeft + width > window.innerWidth - padding
            && rect.left - width + horizontalOverlap >= padding;
        const spaceBelow = window.innerHeight - defaultTop - padding;
        const spaceAbove = rect.top - padding;
        const openUpward = spaceBelow < height && spaceAbove > spaceBelow;
        return {
            openLeft: openLeft,
            openUpward: openUpward
        };
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
        getMenuPlacement: getMenuPlacement,
        getAccessChildMenuPlacement: getAccessChildMenuPlacement,
        getRowMenuPosition: getRowMenuPosition,
        downloadCsv: downloadCsv,
        downloadFileFromBase64: downloadFileFromBase64
    };
})();
