/**
 * SYSTEM PORTAL - LANDING PAGE JS
 * Logic giao diện Cổng thông tin điện tử hành chính
 */

document.addEventListener("DOMContentLoaded", function () {
    // 1. Cập nhật ngày giờ thời gian thực trên Topbar
    updateDateTime();
    setInterval(updateDateTime, 1000);

    // 2. Chức năng đổi ngôn ngữ giao diện (Giả lập chuyển đổi nhanh trên UI)
    const btnLangVi = document.getElementById("lang-vi");
    const btnLangEn = document.getElementById("lang-en");
    if (btnLangVi && btnLangEn) {
        btnLangVi.addEventListener("click", function (e) {
            e.preventDefault();
            setLanguage("vi");
        });
        btnLangEn.addEventListener("click", function (e) {
            e.preventDefault();
            setLanguage("en");
        });
    }

    // 3. Tự động hiển thị lời chào theo thời gian trong ngày
    showTimeGreeting();
});

/**
 * Cập nhật thời gian thực tế dạng hành chính Việt Nam
 */
function updateDateTime() {
    const timeDisplay = document.getElementById("portal-realtime");
    if (!timeDisplay) return;

    const now = new Date();
    const days = ["Chủ nhật", "Thứ hai", "Thứ ba", "Thứ tư", "Thứ năm", "Thứ sáu", "Thứ bảy"];
    const dayName = days[now.getDay()];
    
    const dateStr = padZero(now.getDate()) + "/" + padZero(now.getMonth() + 1) + "/" + now.getFullYear();
    const timeStr = padZero(now.getHours()) + ":" + padZero(now.getMinutes()) + ":" + padZero(now.getSeconds());

    timeDisplay.innerHTML = `<i class="bi bi-clock me-1"></i> ${dayName}, ngày ${dateStr} - ${timeStr}`;
}

function padZero(num) {
    return num < 10 ? "0" + num : num;
}

/**
 * Lời chào thông báo động
 */
function showTimeGreeting() {
    const greetingText = document.getElementById("portal-greeting");
    if (!greetingText) return;

    const now = new Date();
    const hours = now.getHours();
    let greeting = "";

    if (hours < 12) {
        greeting = "Chúc Quý khách một buổi sáng làm việc hiệu quả!";
    } else if (hours < 18) {
        greeting = "Hệ thống Dịch vụ công trực tuyến sẵn sàng phục vụ!";
    } else {
        greeting = "Cổng khai báo tạm trú và lưu trú trực tuyến phục vụ 24/7.";
    }

    greetingText.innerText = greeting;
}

/**
 * Giả lập đổi ngôn ngữ giao diện nhanh
 */
function setLanguage(lang) {
    const activeClass = "fw-bold text-white";
    const btnVi = document.getElementById("lang-vi");
    const btnEn = document.getElementById("lang-en");

    if (lang === "vi") {
        btnVi.className = activeClass;
        btnEn.className = "text-white-50 text-decoration-none";
        alert("Đã chuyển đổi ngôn ngữ hiển thị sang Tiếng Việt");
        // Trong hệ thống thực tế sẽ reload với văn hoá vi-VN hoặc redirect sang route tiếng Việt.
    } else {
        btnEn.className = activeClass;
        btnVi.className = "text-white-50 text-decoration-none";
        alert("System interface language changed to English");
        // Trong hệ thống thực tế sẽ reload với văn hoá en-US hoặc redirect sang route tiếng Anh.
    }
}
