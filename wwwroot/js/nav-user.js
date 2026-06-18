/**
 * nav-user.js
 * Toggle logic for the avatar dropdown menu (.nav-user-trigger / .nav-user-menu).
 * Works with multiple instances on the same page.
 */
(function () {
    "use strict";

    document.addEventListener("DOMContentLoaded", function () {

        var triggers = document.querySelectorAll(".nav-user-trigger");

        if (!triggers.length) return;

        /**
         * Close every open dropdown menu on the page.
         */
        function closeAllMenus() {
            document.querySelectorAll(".nav-user-menu.show").forEach(function (menu) {
                menu.classList.remove("show");
            });
            triggers.forEach(function (btn) {
                btn.setAttribute("aria-expanded", "false");
            });
        }

        /**
         * Toggle the dropdown associated with a trigger button.
         */
        triggers.forEach(function (trigger) {
            trigger.addEventListener("click", function (e) {
                e.preventDefault();
                e.stopPropagation();

                var menu = trigger.nextElementSibling;
                if (!menu || !menu.classList.contains("nav-user-menu")) {
                    // Try finding the menu as a sibling inside the same .nav-user container
                    var parent = trigger.closest(".nav-user");
                    if (parent) {
                        menu = parent.querySelector(".nav-user-menu");
                    }
                }
                if (!menu) return;

                var isOpen = menu.classList.contains("show");

                // Close all other menus first
                closeAllMenus();

                if (!isOpen) {
                    menu.classList.add("show");
                    trigger.setAttribute("aria-expanded", "true");
                }
            });
        });

        /**
         * Close menus when clicking anywhere outside.
         */
        document.addEventListener("click", function (e) {
            if (!e.target.closest(".nav-user")) {
                closeAllMenus();
            }
        });

        /**
         * Close menus on Escape key.
         */
        document.addEventListener("keydown", function (e) {
            if (e.key === "Escape") {
                closeAllMenus();
            }
        });
    });
})();
