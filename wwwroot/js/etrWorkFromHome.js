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

    async function startCamera(videoElement) {
        if (!videoElement) return;

        stopCamera(videoElement);

        if (!navigator.mediaDevices || !navigator.mediaDevices.getUserMedia) {
            throw new Error('Camera API is not available.');
        }

        const stream = await navigator.mediaDevices.getUserMedia({
            video: {
                facingMode: 'user'
            },
            audio: false
        });

        videoElement.srcObject = stream;
        await videoElement.play();
    }

    function stopCamera(videoElement) {
        if (!videoElement || !videoElement.srcObject) return;

        const stream = videoElement.srcObject;
        if (stream && typeof stream.getTracks === 'function') {
            stream.getTracks().forEach(track => track.stop());
        }

        videoElement.srcObject = null;
    }

    function capturePhoto(videoElement) {
        if (!videoElement || !videoElement.videoWidth || !videoElement.videoHeight) {
            throw new Error('Camera preview is not ready.');
        }

        const maxWidth = 480;
        const maxHeight = 360;
        const scale = Math.min(1, maxWidth / videoElement.videoWidth, maxHeight / videoElement.videoHeight);
        const canvas = document.createElement('canvas');
        canvas.width = Math.max(1, Math.round(videoElement.videoWidth * scale));
        canvas.height = Math.max(1, Math.round(videoElement.videoHeight * scale));

        const context = canvas.getContext('2d');
        context.imageSmoothingEnabled = true;
        context.imageSmoothingQuality = 'high';
        context.drawImage(videoElement, 0, 0, canvas.width, canvas.height);

        return canvas.toDataURL('image/jpeg', 0.7);
    }

    return {
        registerOutsideClick: registerOutsideClick,
        unregisterOutsideClick: unregisterOutsideClick,
        startCamera: startCamera,
        stopCamera: stopCamera,
        capturePhoto: capturePhoto
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
