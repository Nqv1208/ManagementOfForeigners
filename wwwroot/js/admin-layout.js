document.addEventListener("DOMContentLoaded", function () {
    const sidebar = document.querySelector(".admin-sidebar");
    const toggleBtn = document.querySelector(".admin-sidebar-toggle");
    const menuLinks = document.querySelectorAll(".admin-sidebar-menu a");

    if (toggleBtn && sidebar) {
        toggleBtn.addEventListener("click", function (e) {
            e.stopPropagation();
            sidebar.classList.toggle("show");
        });

        // Click ngoài sidebar để đóng trên mobile
        document.addEventListener("click", function (e) {
            if (sidebar.classList.contains("show") && !sidebar.contains(e.target) && e.target !== toggleBtn) {
                sidebar.classList.remove("show");
            }
        });
    }

    // Tự động đóng khi chọn menu trên mobile
    menuLinks.forEach(link => {
        link.addEventListener("click", function () {
            if (window.innerWidth < 992 && sidebar) {
                sidebar.classList.remove("show");
            }
        });
    });
});
