document.addEventListener("DOMContentLoaded", function () {
    const sidebar = document.querySelector(".coso-sidebar");
    const toggleBtn = document.querySelector(".coso-sidebar-toggle");
    const menuLinks = document.querySelectorAll(".coso-sidebar-menu a");

    if (toggleBtn && sidebar) {
        toggleBtn.addEventListener("click", function (e) {
            e.stopPropagation();
            sidebar.classList.toggle("show");
        });

        // Click outside sidebar to close on mobile
        document.addEventListener("click", function (e) {
            if (sidebar.classList.contains("show") && !sidebar.contains(e.target) && e.target !== toggleBtn) {
                sidebar.classList.remove("show");
            }
        });
    }

    // Auto-close when menu item is clicked on mobile
    menuLinks.forEach(link => {
        link.addEventListener("click", function () {
            if (window.innerWidth < 768 && sidebar) {
                sidebar.classList.remove("show");
            }
        });
    });
});
