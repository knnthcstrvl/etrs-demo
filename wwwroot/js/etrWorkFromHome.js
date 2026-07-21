window.etrWorkFromHome = (function () {
    const handlers = new WeakMap();
    const cameraStates = new WeakMap();
    let faceDetectorPromise = null;

    function getAssetUrl(relativePath) {
        return new URL(relativePath, document.baseURI).href;
    }

    function getFaceDetector() {
        if (!faceDetectorPromise) {
            faceDetectorPromise = import(getAssetUrl('vendor/mediapipe/vision_bundle.mjs'))
                .then(async visionTasks => {
                    const vision = await visionTasks.FilesetResolver.forVisionTasks(
                        getAssetUrl('vendor/mediapipe/wasm/')
                    );

                    return visionTasks.FaceDetector.createFromOptions(vision, {
                        baseOptions: {
                            modelAssetPath: getAssetUrl('models/blaze_face_short_range.tflite'),
                            delegate: 'CPU'
                        },
                        runningMode: 'VIDEO',
                        minDetectionConfidence: 0.6,
                        minSuppressionThreshold: 0.3
                    });
                })
                .catch(error => {
                    faceDetectorPromise = null;
                    throw error;
                });
        }

        return faceDetectorPromise;
    }

    function setFaceGuideState(videoElement, status) {
        const guide = videoElement
            ?.closest('.etr-camera-frame')
            ?.querySelector('.etr-camera-guide');
        if (!guide) return;

        guide.classList.remove('face-aligned', 'face-misaligned', 'no-face', 'multiple-faces');
        const message = guide.querySelector('.etr-face-guidance');

        switch (status) {
            case 'aligned':
                guide.classList.add('face-aligned');
                if (message) message.textContent = 'Face aligned';
                break;
            case 'misaligned':
                guide.classList.add('face-misaligned');
                if (message) message.textContent = 'Center your face inside the oval';
                break;
            case 'multiple':
                guide.classList.add('multiple-faces');
                if (message) message.textContent = 'Only one person should be visible';
                break;
            case 'none':
                guide.classList.add('no-face');
                if (message) message.textContent = 'Position your face inside the oval';
                break;
            default:
                if (message) message.textContent = 'Checking face position...';
                break;
        }
    }

    function mapFaceBoxToFrame(videoElement, boundingBox) {
        const frame = videoElement.closest('.etr-camera-frame');
        const frameWidth = frame?.clientWidth || videoElement.clientWidth;
        const frameHeight = frame?.clientHeight || videoElement.clientHeight;
        const scale = Math.max(frameWidth / videoElement.videoWidth, frameHeight / videoElement.videoHeight);
        const renderedWidth = videoElement.videoWidth * scale;
        const renderedHeight = videoElement.videoHeight * scale;
        const offsetX = (frameWidth - renderedWidth) / 2;
        const offsetY = (frameHeight - renderedHeight) / 2;

        return {
            left: (boundingBox.originX * scale + offsetX) / frameWidth,
            top: (boundingBox.originY * scale + offsetY) / frameHeight,
            width: boundingBox.width * scale / frameWidth,
            height: boundingBox.height * scale / frameHeight
        };
    }

    function assessFaceDetections(videoElement, detections) {
        if (!detections || detections.length === 0) {
            return { status: 'none', detection: null };
        }

        if (detections.length > 1) {
            return { status: 'multiple', detection: null };
        }

        const detection = detections[0];
        if (!detection.boundingBox) {
            return { status: 'none', detection: null };
        }

        const box = mapFaceBoxToFrame(videoElement, detection.boundingBox);
        const centerX = box.left + box.width / 2;
        const centerY = box.top + box.height / 2;
        const isCentered = Math.abs(centerX - 0.5) <= 0.1 && Math.abs(centerY - 0.39) <= 0.12;
        const isUsefulSize = box.width >= 0.2 && box.width <= 0.46
            && box.height >= 0.28 && box.height <= 0.64;
        const fitsGuide = box.left >= 0.23 && box.left + box.width <= 0.77
            && box.top >= 0.03 && box.top + box.height <= 0.75;

        return {
            status: isCentered && isUsefulSize && fitsGuide ? 'aligned' : 'misaligned',
            detection: detection
        };
    }

    function detectFace(videoElement, detector) {
        const result = detector.detectForVideo(videoElement, performance.now());
        return assessFaceDetections(videoElement, result?.detections || []);
    }

    function scheduleFaceTracking(videoElement, state) {
        if (!state.active) return;

        state.timer = window.setTimeout(() => {
            if (!state.active || videoElement.readyState < HTMLMediaElement.HAVE_CURRENT_DATA) {
                scheduleFaceTracking(videoElement, state);
                return;
            }

            try {
                state.lastAssessment = detectFace(videoElement, state.detector);
                setFaceGuideState(videoElement, state.lastAssessment.status);
            }
            catch {
                state.lastAssessment = null;
                setFaceGuideState(videoElement, 'checking');
            }

            scheduleFaceTracking(videoElement, state);
        }, 250);
    }

    function registerOutsideClick(element, dotNetRef, methodName) {
        if (!element) return;
        const el = element instanceof HTMLElement ? element : element;
        const handler = function (ev) {
            try {
                if (!el.contains(ev.target)) {
                    dotNetRef.invokeMethodAsync(methodName || 'HideEmployeeResults').catch(() => { });
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

        setFaceGuideState(videoElement, 'checking');
        let detector;
        try {
            detector = await getFaceDetector();
        }
        catch (error) {
            stopCamera(videoElement);
            throw error;
        }
        const state = { active: true, timer: null, detector: detector, lastAssessment: null };
        cameraStates.set(videoElement, state);
        scheduleFaceTracking(videoElement, state);
    }

    function stopCamera(videoElement) {
        if (!videoElement) return;

        const state = cameraStates.get(videoElement);
        if (state) {
            state.active = false;
            if (state.timer) window.clearTimeout(state.timer);
            cameraStates.delete(videoElement);
        }

        setFaceGuideState(videoElement, 'none');
        if (!videoElement.srcObject) return;

        const stream = videoElement.srcObject;
        if (stream && typeof stream.getTracks === 'function') {
            stream.getTracks().forEach(track => track.stop());
        }

        videoElement.srcObject = null;
    }

    function getFaceRegion(detection, canvas, videoElement) {
        const box = detection.boundingBox;
        const scaleX = canvas.width / videoElement.videoWidth;
        const scaleY = canvas.height / videoElement.videoHeight;
        const paddingX = box.width * 0.08;
        const paddingY = box.height * 0.08;

        const x = Math.max(0, Math.floor((box.originX - paddingX) * scaleX));
        const y = Math.max(0, Math.floor((box.originY - paddingY) * scaleY));
        const right = Math.min(canvas.width, Math.ceil((box.originX + box.width + paddingX) * scaleX));
        const bottom = Math.min(canvas.height, Math.ceil((box.originY + box.height + paddingY) * scaleY));

        return { x: x, y: y, width: Math.max(1, right - x), height: Math.max(1, bottom - y) };
    }

    function analyzeBrightness(context, region) {
        const pixels = context.getImageData(region.x, region.y, region.width, region.height).data;
        let luminanceTotal = 0;
        let samples = 0;

        for (let index = 0; index < pixels.length; index += 16) {
            luminanceTotal += 0.2126 * pixels[index]
                + 0.7152 * pixels[index + 1]
                + 0.0722 * pixels[index + 2];
            samples++;
        }

        return samples ? luminanceTotal / samples : 0;
    }

    function calculateLaplacianVariance(context, region) {
        const image = context.getImageData(region.x, region.y, region.width, region.height);
        const width = image.width;
        const height = image.height;
        const grayscale = new Float32Array(width * height);

        for (let pixel = 0, offset = 0; pixel < grayscale.length; pixel++, offset += 4) {
            grayscale[pixel] = 0.299 * image.data[offset]
                + 0.587 * image.data[offset + 1]
                + 0.114 * image.data[offset + 2];
        }

        let total = 0;
        let totalSquared = 0;
        let samples = 0;

        for (let y = 1; y < height - 1; y += 2) {
            for (let x = 1; x < width - 1; x += 2) {
                const index = y * width + x;
                const laplacian = grayscale[index - 1] + grayscale[index + 1]
                    + grayscale[index - width] + grayscale[index + width]
                    - 4 * grayscale[index];
                total += laplacian;
                totalSquared += laplacian * laplacian;
                samples++;
            }
        }

        if (!samples) return 0;
        const mean = total / samples;
        return totalSquared / samples - mean * mean;
    }

    function invalidCapture(title, message) {
        return { isValid: false, dataUrl: '', title: title, message: message };
    }

    async function capturePhoto(videoElement) {
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

        const guideRegion = {
            x: Math.floor(canvas.width * 0.3),
            y: Math.floor(canvas.height * 0.08),
            width: Math.max(1, Math.floor(canvas.width * 0.4)),
            height: Math.max(1, Math.floor(canvas.height * 0.64))
        };
        const guideBrightness = analyzeBrightness(context, guideRegion);
        if (guideBrightness < 45) {
            return invalidCapture('Photo is too dark', 'Move to a brighter area or face a light source so your face is clearly visible.');
        }

        if (guideBrightness > 220) {
            return invalidCapture('Photo is too bright', 'Move away from strong light or reduce glare so your face is clearly visible.');
        }

        const detector = await getFaceDetector();
        const faceAssessment = detectFace(videoElement, detector);

        if (faceAssessment.status === 'none') {
            return invalidCapture('Face not detected', 'Keep one clearly visible face inside the oval, then try again.');
        }

        if (faceAssessment.status === 'multiple') {
            return invalidCapture('Multiple faces detected', 'Only the employee taking attendance should be visible in the photo.');
        }

        if (faceAssessment.status !== 'aligned') {
            return invalidCapture('Face not aligned', 'Center your face inside the oval and move closer or farther away as needed.');
        }

        const faceRegion = getFaceRegion(faceAssessment.detection, canvas, videoElement);
        const brightness = analyzeBrightness(context, faceRegion);
        if (brightness < 55) {
            return invalidCapture('Photo is too dark', 'Move to a brighter area or face a light source so your face is clearly visible.');
        }

        if (brightness > 205) {
            return invalidCapture('Photo is too bright', 'Move away from strong light or reduce glare so your face is clearly visible.');
        }

        const sharpness = calculateLaplacianVariance(context, faceRegion);
        if (sharpness < 40) {
            return invalidCapture('Photo is blurred', 'Hold still, clean the camera lens, and wait for the image to become sharp before trying again.');
        }

        return {
            isValid: true,
            dataUrl: canvas.toDataURL('image/jpeg', 0.82),
            title: '',
            message: ''
        };
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
