// Toast Portal Notification System
document.addEventListener("DOMContentLoaded", function () {
    const tempAlerts = document.querySelectorAll(".temp-data-alert");
    tempAlerts.forEach(alert => {
        const message = alert.getAttribute("data-message");
        const type = alert.getAttribute("data-type"); // success, danger, warning, info
        if (message) {
            showPortalToast(message, type);
        }
        alert.remove();
    });
});

function showPortalToast(message, type = "info") {
    let container = document.querySelector(".toast-portal-container");
    if (!container) {
        container = document.createElement("div");
        container.className = "toast-portal-container";
        document.body.appendChild(container);
    }

    const toast = document.createElement("div");
    toast.className = `toast-portal toast-portal-${type}`;

    let iconClass = "bi-info-circle-fill";
    if (type === "success") iconClass = "bi-check-circle-fill";
    else if (type === "danger") iconClass = "bi-exclamation-triangle-fill";
    else if (type === "warning") iconClass = "bi-exclamation-circle-fill";

    toast.innerHTML = `
        <i class="bi ${iconClass} toast-portal-icon"></i>
        <div class="toast-portal-content">${escapeHtml(message)}</div>
    `;

    container.appendChild(toast);

    // Auto dismiss after 4 seconds
    setTimeout(() => {
        toast.classList.add("fade-out");
        toast.addEventListener("animationend", () => {
            toast.remove();
            if (container.children.length === 0) {
                container.remove();
            }
        });
    }, 4000);
}

function escapeHtml(text) {
    if (!text) return "";
    return text
        .replace(/&/g, "&amp;")
        .replace(/</g, "&lt;")
        .replace(/>/g, "&gt;")
        .replace(/"/g, "&quot;")
        .replace(/'/g, "&#039;");
}
