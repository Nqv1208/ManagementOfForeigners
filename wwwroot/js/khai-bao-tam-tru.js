document.addEventListener("DOMContentLoaded", function () {
    const phuongXaSelect = document.getElementById("PhuongXaId");
    const searchInput = document.getElementById("CoSoSearchInput");
    const coSoIdInput = document.getElementById("CoSoLuuTruId");
    const resultsContainer = document.getElementById("CoSoSearchResults");
    const mucDichSelect = document.getElementById("MucDichLuuTru");
    const mucDichKhacContainer = document.getElementById("MucDichKhacContainer");
    const mucDichKhacInput = document.getElementById("MucDichKhac");
    const commitCheckbox = document.getElementById("CommitCheckbox");
    const submitBtn = document.getElementById("SubmitDeclarationBtn");

    const ngayBatDauInput = document.getElementById("NgayBatDau");
    const ngayKetThucInput = document.getElementById("NgayKetThuc");

    // Summary Elements
    const summaryWard = document.getElementById("SumWard");
    const summaryFacility = document.getElementById("SumFacility");
    const summaryDates = document.getElementById("SumDates");
    const summaryPurpose = document.getElementById("SumPurpose");

    // Checklist Elements
    const chkWard = document.getElementById("ChkWard");
    const chkFacility = document.getElementById("ChkFacility");
    const chkDates = document.getElementById("ChkDates");
    const chkCommit = document.getElementById("ChkCommit");

    let debounceTimeout = null;

    // 1. Clear selected facility when ward changes
    if (phuongXaSelect) {
        phuongXaSelect.addEventListener("change", function () {
            clearSelectedFacility();
            hideResults();
            updateSummaryAndChecklist();
        });
    }

    // 2. Autocomplete for Lodging Facility search
    if (searchInput) {
        // Trigger fetch when user focuses or clicks on the input
        searchInput.addEventListener("focus", function () {
            const query = this.value.trim();
            fetchFacilities(query);
        });

        searchInput.addEventListener("click", function () {
            const query = this.value.trim();
            fetchFacilities(query);
        });

        searchInput.addEventListener("input", function () {
            const query = this.value.trim();
            clearTimeout(debounceTimeout);

            // Clear hidden ID to ensure user picks from autocomplete list
            coSoIdInput.value = "";

            debounceTimeout = setTimeout(() => {
                fetchFacilities(query);
            }, 300); // Debounce 300ms
        });

        // Close dropdown when clicking outside
        document.addEventListener("click", function (e) {
            if (!searchInput.contains(e.target) && !resultsContainer.contains(e.target)) {
                hideResults();
                validateFacilitySelection();
                updateSummaryAndChecklist();
            }
        });
    }

    // Fetch facilities from server
    function fetchFacilities(keyword) {
        const phuongXaId = phuongXaSelect ? phuongXaSelect.value : "";
        let url = `/Foreigner/SearchCoSoLuuTru?keyword=${encodeURIComponent(keyword)}`;
        if (phuongXaId) {
            url += `&phuongXaId=${phuongXaId}`;
        }

        // Show loading indicator
        resultsContainer.innerHTML = '<div class="p-3 text-muted text-center"><span class="spinner-border spinner-border-sm me-2 text-primary" role="status"></span>Đang tìm kiếm...</div>';
        resultsContainer.style.display = "block";

        fetch(url)
            .then(response => {
                if (!response.ok) {
                    throw new Error("Mạng lỗi");
                }
                return response.json();
            })
            .then(data => {
                renderResults(data);
            })
            .catch(error => {
                console.error("Lỗi tìm kiếm cơ sở:", error);
                resultsContainer.innerHTML = '<div class="p-3 text-danger text-center"><i class="bi bi-exclamation-circle me-1"></i>Không thể tải dữ liệu. Vui lòng thử lại.</div>';
            });
    }

    // Render autocomplete dropdown list
    function renderResults(facilities) {
        if (!facilities || facilities.length === 0) {
            resultsContainer.innerHTML = '<div class="p-3 text-muted text-center"><i class="bi bi-info-circle me-1"></i>Không tìm thấy cơ sở lưu trú phù hợp</div>';
            return;
        }

        resultsContainer.innerHTML = "";
        facilities.forEach(facility => {
            const item = document.createElement("div");
            item.className = "search-result-item";
            item.innerHTML = `
                <div class="search-result-name">
                    ${escapeHtml(facility.tenCoSo)} 
                </div>
                <div class="search-result-address text-truncate">
                    <i class="bi bi-geo-alt me-1"></i>${escapeHtml(facility.diaChi)} | Phường: ${escapeHtml(facility.phuongXa)}
                </div>
                ${facility.soDienThoai ? `<div class="search-result-address"><i class="bi bi-telephone me-1"></i>SĐT: ${escapeHtml(facility.soDienThoai)}</div>` : ''}
            `;

            item.addEventListener("click", function () {
                selectFacility(facility);
            });

            resultsContainer.appendChild(item);
        });
    }

    function selectFacility(facility) {
        searchInput.value = facility.tenCoSo;
        coSoIdInput.value = facility.id;
        searchInput.classList.remove("is-invalid");
        
        const errorSpan = document.getElementById("CoSoLuuTruId-error-custom");
        if (errorSpan) {
            errorSpan.style.display = "none";
        }
        hideResults();
        
        // Auto-fill address detail if empty
        const addressDetailInput = document.getElementById("DiaChiLuuTruCuThe");
        if (addressDetailInput && !addressDetailInput.value.trim()) {
            addressDetailInput.value = facility.diaChi;
        }
        
        updateSummaryAndChecklist();
    }

    function clearSelectedFacility() {
        if (searchInput) {
            searchInput.value = "";
            searchInput.classList.remove("is-invalid");
        }
        if (coSoIdInput) {
            coSoIdInput.value = "";
        }
    }

    function hideResults() {
        if (resultsContainer) {
            resultsContainer.style.display = "none";
        }
    }

    function validateFacilitySelection() {
        const errorSpan = document.getElementById("CoSoLuuTruId-error-custom");
        if (!errorSpan) return;

        if (searchInput.value.trim() !== "" && coSoIdInput.value === "") {
            searchInput.classList.add("is-invalid");
            errorSpan.textContent = "Vui lòng chọn cơ sở lưu trú hợp lệ từ danh sách gợi ý.";
            errorSpan.style.display = "block";
        } else if (searchInput.value.trim() === "") {
            errorSpan.style.display = "none";
            searchInput.classList.remove("is-invalid");
        }
    }

    // Helper to escape HTML tags
    function escapeHtml(text) {
        if (!text) return "";
        return text
            .replace(/&/g, "&amp;")
            .replace(/</g, "&lt;")
            .replace(/>/g, "&gt;")
            .replace(/"/g, "&quot;")
            .replace(/'/g, "&#039;");
    }

    // 3. Dynamic input for "Other" purpose
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
        toggleMucDichKhac(); // Initial state
    }

    // 4. Enable submit button upon checking the agreement checkbox
    if (commitCheckbox && submitBtn) {
        commitCheckbox.addEventListener("change", function () {
            submitBtn.disabled = !this.checked;
            updateSummaryAndChecklist();
        });
        submitBtn.disabled = !commitCheckbox.checked; // Initial state
    }

    // 5. Update summary panel and checklist
    function updateSummaryAndChecklist() {
        // A. Update Ward
        if (phuongXaSelect && summaryWard && chkWard) {
            const selectedText = phuongXaSelect.options[phuongXaSelect.selectedIndex]?.text || "";
            if (phuongXaSelect.value) {
                // Remove district suffix for cleaner look in summary
                summaryWard.textContent = selectedText.split('(')[0].trim();
                chkWard.classList.add("checked");
                chkWard.querySelector("i").className = "bi bi-check-circle-fill text-success";
            } else {
                summaryWard.textContent = "Chưa chọn";
                chkWard.classList.remove("checked");
                chkWard.querySelector("i").className = "bi bi-circle";
            }
        }

        // B. Update Facility
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

        // C. Update Dates
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

        // D. Update Purpose
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

        // E. Update Commitment
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

    // Register event listeners
    if (phuongXaSelect) phuongXaSelect.addEventListener("change", updateSummaryAndChecklist);
    if (searchInput) searchInput.addEventListener("input", updateSummaryAndChecklist);
    if (ngayBatDauInput) ngayBatDauInput.addEventListener("change", updateSummaryAndChecklist);
    if (ngayKetThucInput) ngayKetThucInput.addEventListener("change", updateSummaryAndChecklist);
    if (mucDichSelect) mucDichSelect.addEventListener("change", updateSummaryAndChecklist);
    if (mucDichKhacInput) mucDichKhacInput.addEventListener("input", updateSummaryAndChecklist);
    if (commitCheckbox) commitCheckbox.addEventListener("change", updateSummaryAndChecklist);

    // Initial load
    updateSummaryAndChecklist();
});
