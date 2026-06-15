$(document).ready(function () {
    const txtPassport = $('#txtPassport');
    const btnSearchPassport = $('#btnSearchPassport');
    const passportFeedback = $('#passportFeedback');
    
    const txtHoTen = $('#txtHoTen');
    const txtNgaySinh = $('#txtNgaySinh');
    const ddlGioiTinh = $('#ddlGioiTinh');
    const txtQuocTich = $('#txtQuocTich');
    const txtNgayHetHanHoChieu = $('#txtNgayHetHanHoChieu');
    const txtNgayCapHoChieu = $('#txtNgayCapHoChieu');
    const txtQuocGiaCap = $('#txtQuocGiaCap');
    const txtSoDienThoaiKhach = $('#txtSoDienThoaiKhach');
    const txtEmailKhach = $('#txtEmailKhach');

    const ddlMucDich = $('#ddlMucDich');
    const groupMucDichKhac = $('#groupMucDichKhac');
    const txtMucDichKhac = $('#txtMucDichKhac');

    const chkCommitment = $('#chkCommitment');
    const btnSubmit = $('#btnSubmit');
    const btnResetForm = $('#btnResetForm');
    
    const txtNgayBatDau = $('#txtNgayBatDau');
    const txtNgayKetThuc = $('#txtNgayKetThuc');

    // Check submit button state on load and on checkbox change
    function updateSubmitButtonState() {
        const isApproved = btnSubmit.data('facility-approved') === true;
        const isChecked = chkCommitment.is(':checked');
        
        if (isApproved && isChecked) {
            btnSubmit.removeAttr('disabled');
        } else {
            btnSubmit.attr('disabled', 'disabled');
        }
    }

    chkCommitment.on('change', function () {
        updateSubmitButtonState();
    });

    // Toggle Other Purpose description field
    function toggleMucDichKhac() {
        if (ddlMucDich.val() === 'Khác') {
            groupMucDichKhac.removeClass('d-none');
            txtMucDichKhac.attr('required', 'required');
        } else {
            groupMucDichKhac.addClass('d-none');
            txtMucDichKhac.removeAttr('required');
            txtMucDichKhac.val('');
        }
    }

    ddlMucDich.on('change', function () {
        toggleMucDichKhac();
    });

    // Passport search function
    function searchPassport() {
        const passportNo = txtPassport.val().trim();
        if (passportNo.length < 4) {
            passportFeedback.removeClass('d-none text-success').addClass('text-danger').text('Vui lòng nhập tối thiểu 4 ký tự để tra cứu.');
            return;
        }

        passportFeedback.removeClass('d-none').addClass('text-muted').removeClass('text-success text-danger').text('Đang tra cứu...');
        
        $.ajax({
            url: '/LuuTru/SearchForeignerByPassport',
            type: 'GET',
            data: { passportNo: passportNo },
            success: function (res) {
                if (res && res.found) {
                    txtHoTen.val(res.hoTen);
                    txtNgaySinh.val(res.ngaySinh);
                    ddlGioiTinh.val(res.gioiTinh);
                    txtQuocTich.val(res.quocTich);
                    txtNgayHetHanHoChieu.val(res.ngayHetHanHoChieu);
                    if (res.ngayCapHoChieu) {
                        txtNgayCapHoChieu.val(res.ngayCapHoChieu);
                    }
                    
                    passportFeedback.removeClass('text-muted text-danger').addClass('text-success')
                        .html('<i class="bi bi-check-circle-fill me-1"></i>Đã tìm thấy thông tin khách từng khai báo. Vui lòng kiểm tra trước khi gửi.');
                } else {
                    passportFeedback.removeClass('text-muted text-success').addClass('text-danger')
                        .html('<i class="bi bi-info-circle-fill me-1"></i>Chưa có thông tin khách. Vui lòng nhập mới.');
                }
            },
            error: function () {
                passportFeedback.removeClass('text-muted text-success').addClass('text-danger').text('Có lỗi xảy ra khi tra cứu. Vui lòng thử lại.');
            }
        });
    }

    btnSearchPassport.on('click', function () {
        searchPassport();
    });

    txtPassport.on('keypress', function (e) {
        if (e.which === 13) {
            e.preventDefault();
            searchPassport();
        }
    });

    // Date validations
    function validateDates() {
        const checkinDate = new Date(txtNgayBatDau.val());
        const checkoutVal = txtNgayKetThuc.val();
        
        if (checkoutVal) {
            const checkoutDate = new Date(checkoutVal);
            if (checkoutDate < checkinDate) {
                txtNgayKetThuc.addClass('input-validation-error');
                return false;
            }
        }
        txtNgayKetThuc.removeClass('input-validation-error');
        return true;
    }

    txtNgayBatDau.on('change', function () {
        validateDates();
    });

    txtNgayKetThuc.on('change', function () {
        validateDates();
    });

    // Reset Form
    btnResetForm.on('click', function () {
        // Reset inputs
        $('#declarationForm')[0].reset();
        txtPassport.val('');
        txtHoTen.val('');
        txtNgaySinh.val('');
        ddlGioiTinh.val('');
        txtQuocTich.val('');
        txtNgayHetHanHoChieu.val('');
        txtNgayCapHoChieu.val('');
        txtQuocGiaCap.val('');
        txtSoDienThoaiKhach.val('');
        txtEmailKhach.val('');
        txtMucDichKhac.val('');
        
        // Hide feedback
        passportFeedback.addClass('d-none').text('');
        
        // Reset dates
        const today = new Date().toISOString().split('T')[0];
        txtNgayBatDau.val(today);
        txtNgayKetThuc.val('');

        toggleMucDichKhac();
        updateSubmitButtonState();

        // Clear bootstrap validation classes
        $('.input-validation-error').removeClass('input-validation-error');
        $('.field-validation-error').text('');
    });

    // Form submit validation
    $('#declarationForm').on('submit', function (e) {
        let isValid = true;

        if (!validateDates()) {
            isValid = false;
        }

        // Force name to uppercase
        txtHoTen.val(txtHoTen.val().toUpperCase());

        if (!isValid) {
            e.preventDefault();
            alert('Vui lòng kiểm tra lại thông tin và sửa các lỗi trước khi gửi.');
        }
    });

    // Initialization
    toggleMucDichKhac();
    updateSubmitButtonState();
});
