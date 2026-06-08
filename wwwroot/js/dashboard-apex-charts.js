(function () {
    const chartKey = "__dashboardApexChart";
    const rendererKey = "__dashboardApexRenderer";
    const renderedElements = new Set();
    const palette = {
        navy: "#111844",
        indigo: "#4B5694",
        steel: "#7288AE",
        beige: "#EAE0CF",
        lateDark: "#8EA2FF",
        undertimeDark: "#F3C96B",
        onsiteDark: "#8EA2FF",
        wfhDark: "#F3C96B",
        darkText: "#B9C9E7",
        darkStrongText: "#F8FAFC",
        darkGrid: "#2F4058",
        darkAxis: "#3A506F",
        darkTooltipBg: "#111C2D",
        darkTooltipBorder: "#2F4058"
    };

    function hasApex() {
        return typeof window.ApexCharts !== "undefined";
    }

    function toNumberArray(values) {
        return Array.isArray(values) ? values.map(value => Number(value) || 0) : [];
    }

    function renderOrUpdate(element, options) {
        if (!element || !hasApex()) {
            return;
        }

        if (element[chartKey]) {
            element[chartKey].updateOptions(options, true, true);
            return;
        }

        element[chartKey] = new ApexCharts(element, options);
        element[chartKey].render();
    }

    function isDarkTheme() {
        return document.body.classList.contains("etrs-dark-theme")
            || document.documentElement.classList.contains("etrs-dark-theme");
    }

    function rememberRenderer(element, renderer) {
        if (!element) {
            return;
        }

        element[rendererKey] = renderer;
        renderedElements.add(element);
    }

    window.addEventListener("etrs:theme-changed", () => {
        renderedElements.forEach(element => {
            if (element && element.isConnected && typeof element[rendererKey] === "function") {
                element[rendererKey]();
            }
        });
    });

    window.dashboardApexCharts = {
        renderLateUndertime(element, payload) {
            const categories = payload?.categories || [];
            const tooltipLabels = payload?.tooltipLabels || categories;
            const lateData = toNumberArray(payload?.lateData);
            const undertimeData = toNumberArray(payload?.undertimeData);
            const dark = isDarkTheme();
            const lateColor = dark ? palette.lateDark : palette.navy;
            const undertimeColor = dark ? palette.undertimeDark : palette.indigo;
            const labelColor = dark ? palette.darkText : "#64748b";
            const gridColor = dark ? palette.darkGrid : "#edf2f7";
            const axisColor = dark ? palette.darkAxis : "#e2e8f0";

            const render = () => window.dashboardApexCharts.renderLateUndertime(element, payload);
            rememberRenderer(element, render);

            renderOrUpdate(element, {
                chart: {
                    type: "bar",
                    height: 340,
                    toolbar: { show: false },
                    animations: { enabled: true, speed: 250 },
                    fontFamily: "Montserrat, Segoe UI, sans-serif"
                },
                series: [
                    { name: "Late", data: lateData },
                    { name: "Undertime", data: undertimeData }
                ],
                colors: [lateColor, undertimeColor],
                plotOptions: {
                    bar: {
                        columnWidth: "58%",
                        borderRadius: 5,
                        borderRadiusApplication: "end"
                    }
                },
                dataLabels: { enabled: false },
                grid: {
                    borderColor: gridColor,
                    strokeDashArray: 0,
                    padding: { left: 6, right: 12, top: 8, bottom: 0 }
                },
                legend: {
                    show: false
                },
                xaxis: {
                    categories,
                    axisBorder: { color: axisColor },
                    axisTicks: { color: axisColor },
                    labels: {
                        rotate: -35,
                        trim: true,
                        style: { colors: labelColor, fontSize: "11px" }
                    },
                    tooltip: { enabled: false }
                },
                yaxis: {
                    min: 0,
                    forceNiceScale: true,
                    tickAmount: 4,
                    labels: {
                        formatter: value => Math.round(value).toString(),
                        style: { colors: labelColor, fontSize: "11px" }
                    }
                },
                tooltip: {
                    theme: dark ? "dark" : "light",
                    shared: true,
                    intersect: false,
                    marker: {
                        fillColors: [lateColor, undertimeColor]
                    },
                    x: {
                        formatter: (_value, context) => tooltipLabels[context.dataPointIndex] || categories[context.dataPointIndex] || ""
                    },
                    y: {
                        formatter: value => Number(value || 0).toLocaleString()
                    }
                },
                noData: {
                    text: "No late or undertime records for this period.",
                    align: "center",
                    verticalAlign: "middle",
                    style: { color: labelColor, fontSize: "13px", fontFamily: "Montserrat, Segoe UI, sans-serif" }
                }
            });
        },

        renderWorkArrangement(element, payload) {
            const series = toNumberArray(payload?.series);
            const total = Number(payload?.total) || 0;
            const dark = isDarkTheme();
            const onsiteColor = dark ? palette.onsiteDark : palette.navy;
            const wfhColor = dark ? palette.wfhDark : palette.beige;
            const labelColor = dark ? palette.darkText : "#64748b";
            const valueColor = dark ? palette.darkStrongText : "#1e2939";
            const strokeColor = dark ? "#111C2D" : "#ffffff";

            const render = () => window.dashboardApexCharts.renderWorkArrangement(element, payload);
            rememberRenderer(element, render);

            renderOrUpdate(element, {
                chart: {
                    type: "donut",
                    height: 300,
                    toolbar: { show: false },
                    animations: { enabled: true, speed: 250 },
                    fontFamily: "Montserrat, Segoe UI, sans-serif"
                },
                series,
                labels: payload?.labels || ["Onsite", "WFH"],
                colors: [onsiteColor, wfhColor],
                states: {
                    hover: {
                        filter: {
                            type: "none"
                        }
                    },
                    active: {
                        allowMultipleDataPointsSelection: false,
                        filter: {
                            type: "none"
                        }
                    }
                },
                stroke: {
                    width: 3,
                    colors: [strokeColor]
                },
                legend: {
                    show: false
                },
                plotOptions: {
                    pie: {
                        expandOnClick: false,
                        donut: {
                            size: "62%",
                            labels: {
                                show: true,
                                name: {
                                    show: true,
                                    color: labelColor,
                                    fontSize: "12px",
                                    offsetY: 16
                                },
                                value: {
                                    show: true,
                                    color: valueColor,
                                    fontSize: "26px",
                                    fontWeight: 700,
                                    offsetY: -12,
                                    formatter: () => total.toLocaleString()
                                },
                                total: {
                                    show: true,
                                    showAlways: true,
                                    label: "Total",
                                    color: labelColor,
                                    formatter: () => total.toLocaleString()
                                }
                            }
                        }
                    }
                },
                dataLabels: {
                    enabled: total > 0,
                    formatter: value => `${Math.round(value)}%`,
                    style: {
                        fontSize: "11px",
                        fontWeight: 700,
                        colors: dark ? ["#F8FAFC"] : ["#ffffff"]
                    },
                    dropShadow: {
                        enabled: dark,
                        top: 1,
                        left: 0,
                        blur: 2,
                        color: "#000000",
                        opacity: 0.35
                    }
                },
                tooltip: {
                    theme: dark ? "dark" : "light",
                    custom({ series, seriesIndex, w }) {
                        const label = w.globals.labels[seriesIndex] || "";
                        const value = Number(series[seriesIndex] || 0).toLocaleString();
                        const color = seriesIndex === 0 ? onsiteColor : wfhColor;
                        return `<div class="dashboard-apex-tooltip"><span class="dashboard-apex-tooltip-dot" style="background:${color}"></span>${label}:&nbsp; <strong>${value} logs</strong></div>`;
                    },
                    y: {
                        formatter: value => `${Number(value || 0).toLocaleString()} logs`
                    }
                },
                noData: {
                    text: "No onsite or WFH logs for this selection.",
                    align: "center",
                    verticalAlign: "middle",
                    style: { color: labelColor, fontSize: "13px", fontFamily: "Montserrat, Segoe UI, sans-serif" }
                }
            });
        },

        destroy(element) {
            if (element && element[chartKey]) {
                element[chartKey].destroy();
                element[chartKey] = null;
                element[rendererKey] = null;
                renderedElements.delete(element);
            }
        }
    };
})();
