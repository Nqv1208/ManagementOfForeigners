(function () {
    function updateCurrentTime() {
        var target = document.getElementById("auth-current-time");
        if (!target) {
            return;
        }

        var now = new Date();
        target.textContent = now.toLocaleString("vi-VN", {
            weekday: "short",
            day: "2-digit",
            month: "2-digit",
            year: "numeric",
            hour: "2-digit",
            minute: "2-digit"
        });
    }

    function setupPasswordToggles() {
        document.querySelectorAll("[data-auth-password-toggle]").forEach(function (button) {
            button.addEventListener("click", function () {
                var inputId = button.getAttribute("data-auth-password-toggle");
                var input = inputId ? document.getElementById(inputId) : button.closest(".auth-password-field")?.querySelector("input");
                if (!input) {
                    return;
                }

                var shouldShow = input.type === "password";
                input.type = shouldShow ? "text" : "password";
                button.setAttribute("aria-label", shouldShow ? "Ẩn mật khẩu" : "Hiện mật khẩu");

                var icon = button.querySelector("i");
                if (icon) {
                    icon.className = shouldShow ? "bi bi-eye-slash" : "bi bi-eye";
                }
            });
        });
    }

    function setSectionEnabled(section, enabled) {
        section.querySelectorAll("input, select, textarea").forEach(function (field) {
            if (field.type === "hidden") {
                return;
            }

            field.disabled = !enabled;
        });
    }

    function clearValidation(form) {
        form.querySelectorAll(".field-validation-error, .field-validation-valid").forEach(function (field) {
            field.textContent = "";
        });

        form.querySelectorAll(".validation-summary-errors ul").forEach(function (list) {
            list.innerHTML = "";
        });

        if (window.jQuery && window.jQuery.fn && window.jQuery.fn.validate) {
            var validator = window.jQuery(form).data("validator");
            if (validator) {
                validator.resetForm();
            }
        }
    }

    function setupRegisterSwitch() {
        var form = document.querySelector("[data-auth-register-form]");
        if (!form) {
            return;
        }

        var accountTypeInput = form.querySelector("#AccountType");
        var buttons = Array.from(form.querySelectorAll("[data-auth-account-type]"));
        var sections = Array.from(form.querySelectorAll("[data-auth-section]"));

        function activate(type, resetValidation) {
            var selectedType = type || "Foreigner";
            if (accountTypeInput) {
                accountTypeInput.value = selectedType;
            }

            buttons.forEach(function (button) {
                var isActive = button.getAttribute("data-auth-account-type") === selectedType;
                button.classList.toggle("is-active", isActive);
                button.setAttribute("aria-selected", isActive ? "true" : "false");
            });

            sections.forEach(function (section) {
                var isActive = section.getAttribute("data-auth-section") === selectedType;
                section.classList.toggle("auth-hidden", !isActive);
                setSectionEnabled(section, isActive);
            });

            if (resetValidation) {
                clearValidation(form);
            }
        }

        buttons.forEach(function (button) {
            button.addEventListener("click", function () {
                activate(button.getAttribute("data-auth-account-type"), true);
            });
        });

        form.querySelectorAll("[data-auth-reset]").forEach(function (button) {
            button.addEventListener("click", function () {
                var currentType = accountTypeInput?.value || "Foreigner";
                form.reset();
                if (accountTypeInput) {
                    accountTypeInput.value = currentType;
                }

                activate(currentType, true);
                setupCommitmentGate();
            });
        });

        activate(accountTypeInput?.value || "Foreigner", false);
    }

    function setupCommitmentGate() {
        var checkbox = document.querySelector("[data-auth-commitment]");
        var submit = document.querySelector("[data-auth-requires-commitment]");
        if (!checkbox || !submit) {
            return;
        }

        function sync() {
            submit.disabled = !checkbox.checked;
        }

        checkbox.addEventListener("change", sync);
        sync();
    }

    document.addEventListener("DOMContentLoaded", function () {
        updateCurrentTime();
        window.setInterval(updateCurrentTime, 60000);
        setupPasswordToggles();
        setupRegisterSwitch();
        setupCommitmentGate();
    });
})();
