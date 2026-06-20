/**
 * SYSTEM PORTAL - LANDING PAGE JS
 * Logic giao diện Cổng thông tin điện tử hành chính
 */

document.addEventListener("DOMContentLoaded", function () {
    // 1. Tự động hiển thị lời chào theo thời gian trong ngày
    showTimeGreeting();

    // 2. Ghim thanh menu điều hướng khi scroll trang
    initStickyHeader();

    // 3. Tính toán chiều cao header động để cân bằng chiều cao hero section
    initHeroHeight();
});

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

/**
 * Khởi tạo hiệu ứng ghim thanh menu điều hướng khi scroll trang
 */
function initStickyHeader() {
    const navbar = document.querySelector(".public-navbar");
    const header = document.querySelector(".public-header");
    if (!navbar || !header) return;

    // Lấy vị trí offset ban đầu của thanh navbar
    const stickyThreshold = navbar.offsetTop;

    function handleScroll() {
        const currentScroll = window.scrollY !== undefined ? window.scrollY : window.pageYOffset;
        if (currentScroll > stickyThreshold) {
            navbar.classList.add("fixed-top");
            // Thêm padding-bottom bằng đúng chiều cao navbar để tránh giật trang
            header.style.paddingBottom = navbar.offsetHeight + "px";
        } else {
            navbar.classList.remove("fixed-top");
            header.style.paddingBottom = "0";
        }
    }

    window.addEventListener("scroll", handleScroll);
    // Chạy thử lúc load trang phòng trường hợp trang được tải lại khi đang ở giữa trang
    handleScroll();
}

/**
 * Tính toán chiều cao header động và truyền vào CSS variable
 */
function initHeroHeight() {
    const header = document.querySelector(".public-header");
    if (!header) return;

    function updateHeaderHeight() {
        const height = header.offsetHeight;
        document.documentElement.style.setProperty("--header-height", `${height}px`);
    }

    updateHeaderHeight();
    window.addEventListener("resize", updateHeaderHeight);
}
