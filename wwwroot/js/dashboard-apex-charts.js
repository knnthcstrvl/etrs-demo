(function () {
    const chartKey = "__dashboardApexChart";
    const palette = {
        navy: "#111844",
        indigo: "#4B5694",
        steel: "#7288AE",
        beige: "#EAE0CF"
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

    window.dashboardApexCharts = {
        renderLateUndertime(element, payload) {
            const categories = payload?.categories || [];
            const tooltipLabels = payload?.tooltipLabels || categories;
            const lateData = toNumberArray(payload?.lateData);
            const undertimeData = toNumberArray(payload?.undertimeData);

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
                colors: [palette.navy, palette.indigo],
                plotOptions: {
                    bar: {
                        columnWidth: "58%",
                        borderRadius: 5,
                        borderRadiusApplication: "end"
                    }
                },
                dataLabels: { enabled: false },
                grid: {
                    borderColor: "#edf2f7",
                    strokeDashArray: 0,
                    padding: { left: 6, right: 12, top: 8, bottom: 0 }
                },
                legend: {
                    show: false
                },
                xaxis: {
                    categories,
                    axisBorder: { color: "#e2e8f0" },
                    axisTicks: { color: "#e2e8f0" },
                    labels: {
                        rotate: -35,
                        trim: true,
                        style: { colors: "#64748b", fontSize: "11px" }
                    },
                    tooltip: { enabled: false }
                },
                yaxis: {
                    min: 0,
                    forceNiceScale: true,
                    tickAmount: 4,
                    labels: {
                        formatter: value => Math.round(value).toString(),
                        style: { colors: "#64748b", fontSize: "11px" }
                    }
                },
                tooltip: {
                    shared: true,
                    intersect: false,
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
                    style: { color: "#64748b", fontSize: "13px", fontFamily: "Montserrat, Segoe UI, sans-serif" }
                }
            });
        },

        renderWorkArrangement(element, payload) {
            const series = toNumberArray(payload?.series);
            const total = Number(payload?.total) || 0;

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
                colors: [palette.navy, palette.beige],
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
                    colors: ["#ffffff"]
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
                                    color: "#64748b",
                                    fontSize: "12px",
                                    offsetY: 16
                                },
                                value: {
                                    show: true,
                                    color: "#1e2939",
                                    fontSize: "26px",
                                    fontWeight: 700,
                                    offsetY: -12,
                                    formatter: () => total.toLocaleString()
                                },
                                total: {
                                    show: true,
                                    showAlways: true,
                                    label: "Total",
                                    color: "#64748b",
                                    formatter: () => total.toLocaleString()
                                }
                            }
                        }
                    }
                },
                dataLabels: {
                    enabled: total > 0,
                    formatter: value => `${Math.round(value)}%`,
                    style: { fontSize: "11px", fontWeight: 700 }
                },
                tooltip: {
                    custom({ series, seriesIndex, w }) {
                        const label = w.globals.labels[seriesIndex] || "";
                        const value = Number(series[seriesIndex] || 0).toLocaleString();
                        return `<div class="dashboard-apex-tooltip">${label}:&nbsp; <strong>${value} logs</strong></div>`;
                    },
                    y: {
                        formatter: value => `${Number(value || 0).toLocaleString()} logs`
                    }
                },
                noData: {
                    text: "No onsite or WFH logs for this selection.",
                    align: "center",
                    verticalAlign: "middle",
                    style: { color: "#64748b", fontSize: "13px", fontFamily: "Montserrat, Segoe UI, sans-serif" }
                }
            });
        },

        destroy(element) {
            if (element && element[chartKey]) {
                element[chartKey].destroy();
                element[chartKey] = null;
            }
        }
    };
})();
