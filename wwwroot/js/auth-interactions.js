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
        var currentStep = 1;

        function updateStepperView(selectedType) {
            var isLodging = selectedType === "LodgingOwner";
            var stepperHeader = document.getElementById("authStepper");
            
            // Stepper progress indicator header
            if (stepperHeader) {
                stepperHeader.classList.toggle("d-none", !isLodging);
                if (isLodging) {
                    stepperHeader.querySelectorAll(".auth-step").forEach(function (stepEl) {
                        var stepNum = parseInt(stepEl.getAttribute("data-step"), 10);
                        stepEl.classList.toggle("is-active", stepNum === currentStep);
                        stepEl.classList.toggle("is-complete", stepNum < currentStep);
                    });
                }
            }
            
            // Toggle form sections and disable/enable fields
            sections.forEach(function (section) {
                var sectionType = section.getAttribute("data-auth-section");
                var stepSectionVal = section.getAttribute("data-auth-step-section");
                
                var isVisible = false;
                if (!isLodging) {
                    // Foreigner mode: show account info (step section 1) and Foreigner section
                    isVisible = (stepSectionVal === "1" || sectionType === "Foreigner");
                } else {
                    // LodgingOwner mode: show only the active step section
                    if (stepSectionVal) {
                        isVisible = (parseInt(stepSectionVal, 10) === currentStep);
                    } else {
                        isVisible = (sectionType === "LodgingOwner");
                    }
                }
                
                section.classList.toggle("auth-hidden", !isVisible);
                setSectionEnabled(section, isVisible);
            });
            
            // Account info section (step section 1) visibility check for LodgingOwner
            var accountSection = form.querySelector('[data-auth-step-section="1"]');
            if (accountSection) {
                var isAccountVisible = !isLodging || (currentStep === 1);
                accountSection.classList.toggle("auth-hidden", !isAccountVisible);
                setSectionEnabled(accountSection, isAccountVisible);
            }
            
            // Commitment checkbox visibility
            var commitmentSection = document.querySelector("[data-auth-commitment-section]");
            if (commitmentSection) {
                var showCommitment = !isLodging || (currentStep === 3);
                commitmentSection.classList.toggle("auth-hidden", !showCommitment);
                setSectionEnabled(commitmentSection, showCommitment);
            }
            
            // Button visibility management
            var btnCancel = document.querySelector("[data-auth-btn-cancel]");
            var btnReset = document.querySelector("[data-auth-btn-reset]");
            var btnPrev = document.querySelector("[data-auth-btn-prev]");
            var btnNext = document.querySelector("[data-auth-btn-next]");
            var btnSubmit = document.querySelector("[data-auth-btn-submit]");
            
            if (isLodging) {
                if (currentStep === 1) {
                    if (btnCancel) btnCancel.classList.remove("d-none");
                    if (btnReset) btnReset.classList.remove("d-none");
                    if (btnPrev) btnPrev.classList.add("d-none");
                    if (btnNext) btnNext.classList.remove("d-none");
                    if (btnSubmit) btnSubmit.classList.add("d-none");
                } else if (currentStep === 2) {
                    if (btnCancel) btnCancel.classList.add("d-none");
                    if (btnReset) btnReset.classList.add("d-none");
                    if (btnPrev) btnPrev.classList.remove("d-none");
                    if (btnNext) btnNext.classList.remove("d-none");
                    if (btnSubmit) btnSubmit.classList.add("d-none");
                } else if (currentStep === 3) {
                    if (btnCancel) btnCancel.classList.add("d-none");
                    if (btnReset) btnReset.classList.add("d-none");
                    if (btnPrev) btnPrev.classList.remove("d-none");
                    if (btnNext) btnNext.classList.add("d-none");
                    if (btnSubmit) btnSubmit.classList.remove("d-none");
                }
            } else {
                // Foreigner
                if (btnCancel) btnCancel.classList.remove("d-none");
                if (btnReset) btnReset.classList.remove("d-none");
                if (btnPrev) btnPrev.classList.add("d-none");
                if (btnNext) btnNext.classList.add("d-none");
                if (btnSubmit) btnSubmit.classList.remove("d-none");
            }
            
            setupCommitmentGate();
        }

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

            currentStep = 1;
            updateStepperView(selectedType);

            if (resetValidation) {
                clearValidation(form);
            }
        }

        buttons.forEach(function (button) {
            button.addEventListener("click", function () {
                activate(button.getAttribute("data-auth-account-type"), true);
            });
        });

        // Set up Next & Prev buttons click listeners
        var btnNext = document.querySelector("[data-auth-btn-next]");
        if (btnNext) {
            btnNext.addEventListener("click", function () {
                var selectedType = accountTypeInput?.value || "Foreigner";
                if (selectedType !== "LodgingOwner") return;
                
                // Validate fields in the active step section
                var isValid = true;
                
                // Step 1: Validate Account info fields
                if (currentStep === 1) {
                    var accountSection = form.querySelector('[data-auth-step-section="1"]');
                    if (accountSection) {
                        accountSection.querySelectorAll("input, select, textarea").forEach(function(el) {
                            if (window.jQuery && window.jQuery(el).valid) {
                                if (!window.jQuery(el).valid()) {
                                    isValid = false;
                                }
                            }
                        });
                    }
                } 
                // Step 2: Validate Representative fields
                else if (currentStep === 2) {
                    var repSection = form.querySelector('[data-auth-step-section="2"]');
                    if (repSection) {
                        repSection.querySelectorAll("input, select, textarea").forEach(function(el) {
                            if (window.jQuery && window.jQuery(el).valid) {
                                if (!window.jQuery(el).valid()) {
                                    isValid = false;
                                }
                            }
                        });
                    }
                }

                if (isValid) {
                    if (currentStep < 3) {
                        currentStep++;
                        updateStepperView(selectedType);
                    }
                }
            });
        }

        var btnPrev = document.querySelector("[data-auth-btn-prev]");
        if (btnPrev) {
            btnPrev.addEventListener("click", function () {
                var selectedType = accountTypeInput?.value || "Foreigner";
                if (selectedType === "LodgingOwner" && currentStep > 1) {
                    currentStep--;
                    updateStepperView(selectedType);
                }
            });
        }

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
