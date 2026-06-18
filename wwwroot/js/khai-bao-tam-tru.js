document.addEventListener("DOMContentLoaded", function () {
    const facilitySearchEndpoint = "/KhaiBaoTamTru/SearchCoSoLuuTru";

    const phuongXaSelect = document.getElementById("PhuongXaId");
    const searchInput = document.getElementById("CoSoSearchInput");
    const coSoIdInput = document.getElementById("CoSoLuuTruId");
    const mucDichSelect = document.getElementById("MucDichLuuTru");
    const mucDichKhacContainer = document.getElementById("MucDichKhacContainer");
    const mucDichKhacInput = document.getElementById("MucDichKhac");
    const commitCheckbox = document.getElementById("CommitCheckbox");
    const submitBtn = document.getElementById("SubmitDeclarationBtn");

    const ngayBatDauInput = document.getElementById("NgayBatDau");
    const ngayKetThucInput = document.getElementById("NgayKetThuc");

    const summaryWard = document.getElementById("SumWard");
    const summaryFacility = document.getElementById("SumFacility");
    const summaryDates = document.getElementById("SumDates");
    const summaryPurpose = document.getElementById("SumPurpose");

    const chkWard = document.getElementById("ChkWard");
    const chkFacility = document.getElementById("ChkFacility");
    const chkDates = document.getElementById("ChkDates");
    const chkCommit = document.getElementById("ChkCommit");

    initializeFacilityAutocomplete({
        searchInputId: "CoSoSearchInput",
        hiddenInputId: "CoSoLuuTruId",
        resultsId: "CoSoSearchResults",
        wardSelectId: "PhuongXaId",
        errorId: "CoSoLuuTruId-error-custom",
        addressInputId: "DiaChiLuuTruCuThe",
        onChange: updateSummaryAndChecklist
    });

    initializeFacilityAutocomplete({
        searchInputId: "ResidenceCoSoSearchInput",
        hiddenInputId: "ResidenceMaCoSoLuuTru",
        resultsId: "ResidenceCoSoSearchResults",
        errorId: "ResidenceMaCoSoLuuTru-error-custom"
    });

    if (mucDichSelect && mucDichKhacContainer && mucDichKhacInput) {
        function toggleMucDichKhac() {
            if (mucDichSelect.value === "Khác") {
                mucDichKhacContainer.style.display = "block";
                mucDichKhacInput.setAttribute("required", "required");
            } else {
                mucDichKhacContainer.style.display = "none";
                mucDichKhacInput.removeAttribute("required");
                mucDichKhacInput.value = "";
            }
        }

        mucDichSelect.addEventListener("change", function () {
            toggleMucDichKhac();
            updateSummaryAndChecklist();
        });
        toggleMucDichKhac();
    }

    if (commitCheckbox && submitBtn) {
        commitCheckbox.addEventListener("change", function () {
            submitBtn.disabled = !this.checked;
            updateSummaryAndChecklist();
        });
        submitBtn.disabled = !commitCheckbox.checked;
    }

    if (phuongXaSelect) phuongXaSelect.addEventListener("change", updateSummaryAndChecklist);
    if (searchInput) searchInput.addEventListener("input", updateSummaryAndChecklist);
    if (ngayBatDauInput) ngayBatDauInput.addEventListener("change", updateSummaryAndChecklist);
    if (ngayKetThucInput) ngayKetThucInput.addEventListener("change", updateSummaryAndChecklist);
    if (mucDichSelect) mucDichSelect.addEventListener("change", updateSummaryAndChecklist);
    if (mucDichKhacInput) mucDichKhacInput.addEventListener("input", updateSummaryAndChecklist);
    if (commitCheckbox) commitCheckbox.addEventListener("change", updateSummaryAndChecklist);

    updateSummaryAndChecklist();

    function initializeFacilityAutocomplete(options) {
        const facilityInput = document.getElementById(options.searchInputId);
        const facilityIdInput = document.getElementById(options.hiddenInputId);
        const resultsContainer = document.getElementById(options.resultsId);
        const wardSelect = options.wardSelectId ? document.getElementById(options.wardSelectId) : null;
        const errorSpan = options.errorId ? document.getElementById(options.errorId) : null;
        const addressInput = options.addressInputId ? document.getElementById(options.addressInputId) : null;
        let debounceTimeout = null;

        if (!facilityInput || !facilityIdInput || !resultsContainer) return;

        if (wardSelect) {
            wardSelect.addEventListener("change", function () {
                clearSelectedFacility();
                hideResults();
                options.onChange?.();
            });
        }

        facilityInput.addEventListener("focus", function () {
            const query = facilityInput.value.trim();
            if (query.length >= 2) {
                fetchFacilities(query);
            }
        });

        facilityInput.addEventListener("click", function () {
            const query = facilityInput.value.trim();
            if (query.length >= 2) {
                fetchFacilities(query);
            }
        });

        facilityInput.addEventListener("input", function () {
            const query = facilityInput.value.trim();
            clearTimeout(debounceTimeout);
            facilityIdInput.value = "";
            facilityInput.classList.remove("is-invalid");
            hideCustomError();
            options.onChange?.();

            if (query.length === 0) {
                hideResults();
                return;
            }

            if (query.length < 2) {
                showMessage("Nhập tối thiểu 2 ký tự để tìm cơ sở lưu trú.");
                return;
            }

            debounceTimeout = setTimeout(() => {
                fetchFacilities(query);
            }, 300);
        });

        document.addEventListener("click", function (event) {
            if (!facilityInput.contains(event.target) && !resultsContainer.contains(event.target)) {
                hideResults();
                validateFacilitySelection();
                options.onChange?.();
            }
        });

        function fetchFacilities(keyword) {
            const phuongXaId = wardSelect ? wardSelect.value : "";
            let url = `${facilitySearchEndpoint}?keyword=${encodeURIComponent(keyword)}`;
            if (phuongXaId) {
                url += `&phuongXaId=${encodeURIComponent(phuongXaId)}`;
            }

            resultsContainer.innerHTML = '<div class="p-3 text-muted text-center"><span class="spinner-border spinner-border-sm me-2 text-primary" role="status"></span>Đang tìm kiếm...</div>';
            resultsContainer.style.display = "block";

            fetch(url)
                .then(response => {
                    if (!response.ok) {
                        throw new Error("Không thể tải danh sách cơ sở lưu trú.");
                    }
                    return response.json();
                })
                .then(renderResults)
                .catch(error => {
                    console.error("Lỗi tìm kiếm cơ sở:", error);
                    resultsContainer.innerHTML = '<div class="p-3 text-danger text-center"><i class="bi bi-exclamation-circle me-1"></i>Không thể tải dữ liệu. Vui lòng thử lại.</div>';
                    resultsContainer.style.display = "block";
                });
        }

        function renderResults(facilities) {
            if (!facilities || facilities.length === 0) {
                showMessage("Không tìm thấy cơ sở lưu trú phù hợp.");
                return;
            }

            resultsContainer.innerHTML = "";
            facilities.forEach(facility => {
                const item = document.createElement("div");
                item.className = "search-result-item";
                item.innerHTML = `
                    <div class="search-result-name">${escapeHtml(facility.tenCoSo)}</div>
                    <div class="search-result-address"><i class="bi bi-geo-alt me-1"></i>${escapeHtml(facility.diaChi)}</div>
                    <div class="search-result-address"><i class="bi bi-map me-1"></i>${escapeHtml(facility.phuongXa)}</div>
                `;

                item.addEventListener("click", function () {
                    selectFacility(facility);
                });

                resultsContainer.appendChild(item);
            });
            resultsContainer.style.display = "block";
        }

        function selectFacility(facility) {
            facilityInput.value = facility.tenCoSo;
            facilityIdInput.value = facility.id;
            facilityInput.classList.remove("is-invalid");
            hideCustomError();
            hideResults();

            if (addressInput && !addressInput.value.trim()) {
                addressInput.value = facility.diaChi;
            }

            options.onChange?.();
        }

        function clearSelectedFacility() {
            facilityInput.value = "";
            facilityIdInput.value = "";
            facilityInput.classList.remove("is-invalid");
            hideCustomError();
        }

        function validateFacilitySelection() {
            if (!errorSpan) return;

            if (facilityInput.value.trim() !== "" && facilityIdInput.value === "") {
                facilityInput.classList.add("is-invalid");
                errorSpan.textContent = "Vui lòng chọn cơ sở lưu trú hợp lệ từ danh sách gợi ý.";
                errorSpan.style.display = "block";
            } else {
                hideCustomError();
                facilityInput.classList.remove("is-invalid");
            }
        }

        function hideCustomError() {
            if (errorSpan) {
                errorSpan.style.display = "none";
            }
        }

        function showMessage(message) {
            resultsContainer.innerHTML = `<div class="p-3 text-muted text-center"><i class="bi bi-info-circle me-1"></i>${escapeHtml(message)}</div>`;
            resultsContainer.style.display = "block";
        }

        function hideResults() {
            resultsContainer.style.display = "none";
            resultsContainer.innerHTML = "";
        }
    }

    function updateSummaryAndChecklist() {
        if (phuongXaSelect && summaryWard && chkWard) {
            const selectedText = phuongXaSelect.options[phuongXaSelect.selectedIndex]?.text || "";
            if (phuongXaSelect.value) {
                summaryWard.textContent = selectedText.split("(")[0].trim();
                chkWard.classList.add("checked");
                chkWard.querySelector("i").className = "bi bi-check-circle-fill text-success";
            } else {
                summaryWard.textContent = "Chưa chọn";
                chkWard.classList.remove("checked");
                chkWard.querySelector("i").className = "bi bi-circle";
            }
        }

        if (coSoIdInput && searchInput && summaryFacility && chkFacility) {
            if (coSoIdInput.value) {
                summaryFacility.textContent = searchInput.value;
                chkFacility.classList.add("checked");
                chkFacility.querySelector("i").className = "bi bi-check-circle-fill text-success";
            } else {
                summaryFacility.textContent = "Chưa chọn";
                chkFacility.classList.remove("checked");
                chkFacility.querySelector("i").className = "bi bi-circle";
            }
        }

        if (ngayBatDauInput && ngayKetThucInput && summaryDates && chkDates) {
            if (ngayBatDauInput.value && ngayKetThucInput.value) {
                summaryDates.textContent = `${formatDate(ngayBatDauInput.value)} - ${formatDate(ngayKetThucInput.value)}`;
                chkDates.classList.add("checked");
                chkDates.querySelector("i").className = "bi bi-check-circle-fill text-success";
            } else {
                summaryDates.textContent = "Chưa nhập";
                chkDates.classList.remove("checked");
                chkDates.querySelector("i").className = "bi bi-circle";
            }
        }

        if (mucDichSelect && summaryPurpose) {
            if (mucDichSelect.value) {
                if (mucDichSelect.value === "Khác" && mucDichKhacInput && mucDichKhacInput.value.trim()) {
                    summaryPurpose.textContent = `Khác (${mucDichKhacInput.value.trim()})`;
                } else {
                    summaryPurpose.textContent = mucDichSelect.value;
                }
            } else {
                summaryPurpose.textContent = "Chưa chọn";
            }
        }

        if (commitCheckbox && chkCommit) {
            if (commitCheckbox.checked) {
                chkCommit.classList.add("checked");
                chkCommit.querySelector("i").className = "bi bi-check-circle-fill text-success";
            } else {
                chkCommit.classList.remove("checked");
                chkCommit.querySelector("i").className = "bi bi-circle";
            }
        }
    }

    function formatDate(dateString) {
        if (!dateString) return "";
        const parts = dateString.split("-");
        if (parts.length === 3) {
            return `${parts[2]}/${parts[1]}/${parts[0]}`;
        }
        return dateString;
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
});
