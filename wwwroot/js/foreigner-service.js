/**
 * foreigner-service.js
 * Client-side script handling interactions in the Foreigner Service Portal.
 */
(function () {
    "use strict";

    document.addEventListener("DOMContentLoaded", function () {
        // Real-time clock update (if present in header)
        var timeEl = document.getElementById("portal-realtime");
        if (timeEl) {
            function updateRealtime() {
                var now = new Date();
                var timeStr = now.toLocaleDateString("vi-VN", {
                    weekday: "long",
                    year: "numeric",
                    month: "long",
                    day: "numeric"
                }) + " - " + now.toLocaleTimeString("vi-VN");
                timeEl.innerHTML = '<i class="bi bi-clock me-1"></i>' + timeStr;
            }
            updateRealtime();
            setInterval(updateRealtime, 1000);
        }

        // Auto-dismiss alerts after 5 seconds
        var toastAlerts = document.querySelectorAll(".temp-data-alert");
        if (toastAlerts.length > 0 && typeof bootstrap !== "undefined") {
            toastAlerts.forEach(function (alertEl) {
                var msg = alertEl.getAttribute("data-message");
                var type = alertEl.getAttribute("data-type") || "success";
                
                // If there's a custom toast notification library, trigger it here.
                // Otherwise, use Bootstrap alerts.
            });
        }
    });
})();
