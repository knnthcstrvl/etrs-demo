window.attendanceLogs = (function () {
    function openFileDialog(element) {
        if (!element) {
            return;
        }

        element.click();
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
        downloadCsv: downloadCsv
    };
})();
