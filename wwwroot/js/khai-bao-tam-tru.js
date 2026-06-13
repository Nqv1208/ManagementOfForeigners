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

    let debounceTimeout = null;

    // 1. Reset facility when ward changes
    if (phuongXaSelect) {
        phuongXaSelect.addEventListener("change", function () {
            clearSelectedFacility();
            hideResults();
        });
    }

    // 2. Autocomplete for Lodging Facility search
    if (searchInput) {
        searchInput.addEventListener("input", function () {
            const query = this.value.trim();
            clearTimeout(debounceTimeout);

            // If user types, clear previously selected hidden ID to enforce selection from autocomplete list
            coSoIdInput.value = "";
            searchInput.classList.remove("is-valid");

            if (query.length < 2) {
                hideResults();
                return;
            }

            debounceTimeout = setTimeout(() => {
                fetchFacilities(query);
            }, 300); // Debounce 300ms
        });

        // Close dropdown when clicking outside
        document.addEventListener("click", function (e) {
            if (!searchInput.contains(e.target) && !resultsContainer.contains(e.target)) {
                hideResults();
                validateFacilitySelection();
            }
        });
    }

    // Fetch matching facilities via AJAX
    function fetchFacilities(keyword) {
        const phuongXaId = phuongXaSelect ? phuongXaSelect.value : "";
        let url = `/Foreigner/SearchCoSoLuuTru?keyword=${encodeURIComponent(keyword)}`;
        if (phuongXaId) {
            url += `&phuongXaId=${phuongXaId}`;
        }

        // Show loading indicator
        resultsContainer.innerHTML = '<div class="p-3 text-muted text-center"><span class="spinner-border spinner-border-sm me-2" role="status"></span>Đang tìm kiếm cơ sở lưu trú...</div>';
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
                    <span class="badge ${facility.isActive ? 'bg-success' : 'bg-secondary'} search-result-badge">
                        ${facility.isActive ? 'Đang hoạt động' : 'Ngừng hoạt động'}
                    </span>
                </div>
                <div class="search-result-address text-truncate">
                    <i class="bi bi-geo-alt me-1"></i>${escapeHtml(facility.diaChi)} | Phường/Xã: ${escapeHtml(facility.phuongXa)}
                </div>
                ${facility.soDienThoai ? `<div class="search-result-address"><i class="bi bi-telephone me-1"></i>SĐT: ${escapeHtml(facility.soDienThoai)}</div>` : ''}
            `;

            // If facility is inactive, disable selecting
            if (!facility.isActive) {
                item.style.opacity = "0.5";
                item.style.cursor = "not-allowed";
            } else {
                item.addEventListener("click", function () {
                    selectFacility(facility);
                });
            }

            resultsContainer.appendChild(item);
        });
    }

    function selectFacility(facility) {
        searchInput.value = facility.tenCoSo;
        coSoIdInput.value = facility.id;
        searchInput.classList.add("is-valid");
        searchInput.classList.remove("is-invalid");
        document.getElementById("CoSoLuuTruId-error-custom").style.display = "none";
        hideResults();
        
        // Auto-fill address detail if empty
        const addressDetailInput = document.getElementById("DiaChiLuuTruCuThe");
        if (addressDetailInput && !addressDetailInput.value.trim()) {
            addressDetailInput.value = facility.diaChi;
        }
    }

    function clearSelectedFacility() {
        if (searchInput) {
            searchInput.value = "";
            searchInput.classList.remove("is-valid");
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
            errorSpan.textContent = "Vui lòng chọn cơ sở lưu trú từ danh sách gợi ý.";
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

        mucDichSelect.addEventListener("change", toggleMucDichKhac);
        toggleMucDichKhac(); // Initial state
    }

    // 4. Enable submit button upon ticking the agreement checkbox
    if (commitCheckbox && submitBtn) {
        commitCheckbox.addEventListener("change", function () {
            submitBtn.disabled = !this.checked;
        });
        submitBtn.disabled = !commitCheckbox.checked; // Initial state
    }
});
