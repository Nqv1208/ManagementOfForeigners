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

// Portal Topbar Clock & Language Switcher
document.addEventListener("DOMContentLoaded", function () {
    const timeDisplay = document.getElementById("portal-realtime");
    if (timeDisplay) {
        updatePortalTime();
        setInterval(updatePortalTime, 1000);
    }

    const btnLangVi = document.getElementById("lang-vi");
    const btnLangEn = document.getElementById("lang-en");
    if (btnLangVi && btnLangEn) {
        btnLangVi.addEventListener("click", function (e) {
            e.preventDefault();
            setPortalLanguage("vi");
        });
        btnLangEn.addEventListener("click", function (e) {
            e.preventDefault();
            setPortalLanguage("en");
        });
    }
});

function updatePortalTime() {
    const timeDisplay = document.getElementById("portal-realtime");
    if (!timeDisplay) return;

    const now = new Date();
    const days = ["Chủ nhật", "Thứ hai", "Thứ ba", "Thứ tư", "Thứ năm", "Thứ sáu", "Thứ bảy"];
    const dayName = days[now.getDay()];
    
    const pad = (num) => num < 10 ? "0" + num : num;
    const dateStr = pad(now.getDate()) + "/" + pad(now.getMonth() + 1) + "/" + now.getFullYear();
    const timeStr = pad(now.getHours()) + ":" + pad(now.getMinutes()) + ":" + pad(now.getSeconds());

    timeDisplay.innerHTML = `<i class="bi bi-clock me-1"></i> ${dayName}, ngày ${dateStr} - ${timeStr}`;
}

function setPortalLanguage(lang) {
    const activeClass = "fw-bold text-white";
    const btnVi = document.getElementById("lang-vi");
    const btnEn = document.getElementById("lang-en");
    if (!btnVi || !btnEn) return;

    if (lang === "vi") {
        btnVi.className = activeClass;
        btnEn.className = "text-white-50 text-decoration-none";
        alert("Đã chuyển đổi ngôn ngữ hiển thị sang Tiếng Việt");
    } else {
        btnEn.className = activeClass;
        btnVi.className = "text-white-50 text-decoration-none";
        alert("System interface language changed to English");
    }
}
