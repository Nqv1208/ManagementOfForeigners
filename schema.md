# Danh sách trường của từng bảng

## 1. PhanQuyenVaiTro

| STT | Tên trường | Kiểu dữ liệu | Khóa |
|---|---|---|---|
| 1 | MaQuyen | INT | PK |
| 2 | TenQuyen | NVARCHAR(100) |  |
| 3 | MoTaQuyen | NVARCHAR(255) |  |
| 4 | MaVaiTro | INT | FK |
| 5 | NgayCapNhat | DATETIME |  |

---

## 2. VaiTro

| STT | Tên trường | Kiểu dữ liệu | Khóa |
|---|---|---|---|
| 1 | MaVaiTro | INT | PK |
| 2 | TenVaiTro | NVARCHAR(50) |  |
| 3 | MoTaVaiTro | NVARCHAR(255) |  |
| 4 | NgayTao | DATETIME |  |
| 5 | TrangThai | VARCHAR(20) |  |

---

## 3. TaiKhoan

| STT | Tên trường | Kiểu dữ liệu | Khóa |
|---|---|---|---|
| 1 | MaTaiKhoan | CHAR(9) | PK |
| 2 | TenDangNhap | NVARCHAR(50) |  |
| 3 | MatKhauHash | NVARCHAR(255) |  |
| 4 | MaVaiTro | INT | FK |
| 5 | Email | NVARCHAR(100) |  |
| 6 | SoDienThoai | NVARCHAR(15) |  |
| 7 | TrangThai | NVARCHAR(20) |  |
| 8 | NgayTao | DATETIME |  |
| 9 | LanDangNhapCuoi | DATETIME |  |

---

## 4. CanBo

| STT | Tên trường | Kiểu dữ liệu | Khóa |
|---|---|---|---|
| 1 | MaCanBo | CHAR(9) | PK |
| 2 | MaTaiKhoan | CHAR(9) | FK |
| 3 | HoTen | NVARCHAR(100) |  |
| 4 | SoCCCD | CHAR(12) |  |
| 5 | NgayCapCCCD | DATE |  |
| 6 | NoiCapCCCD | NVARCHAR(100) |  |
| 7 | DiaChiThuongTru | NVARCHAR(255) |  |
| 8 | NgaySinh | DATE |  |
| 9 | GioiTinh | NVARCHAR(10) |  |
| 10 | DonViCongTac | NVARCHAR(100) |  |
| 11 | ChucVu | NVARCHAR(50) |  |
| 12 | CapQuanLy | NVARCHAR(50) |  |
| 13 | TrangThai | NVARCHAR(20) |  |

---

## 5. ChuCoSoLuuTru

| STT | Tên trường | Kiểu dữ liệu | Khóa |
|---|---|---|---|
| 1 | MaChuCoSo | CHAR(9) | PK |
| 2 | MaTaiKhoan | CHAR(9) | FK |
| 3 | HoTen | NVARCHAR(100) |  |
| 4 | NgaySinh | DATE |  |
| 5 | GioiTinh | NVARCHAR(10) |  |
| 6 | SoCCCD | CHAR(12) |  |
| 7 | NgayCapCCCD | DATE |  |
| 8 | NoiCapCCCD | NVARCHAR(100) |  |
| 9 | DiaChiThuongTru | NVARCHAR(255) |  |

---

## 6. NguoiNuocNgoai

| STT | Tên trường | Kiểu dữ liệu | Khóa |
|---|---|---|---|
| 1 | MaNguoiNuocNgoai | CHAR(9) | PK |
| 2 | MaTaiKhoan | CHAR(9) | FK |
| 3 | HoTen | NVARCHAR(100) |  |
| 4 | NgaySinh | DATE |  |
| 5 | GioiTinh | NVARCHAR(10) |  |
| 6 | QuocTich | NVARCHAR(50) |  |
| 7 | SoHoChieu | VARCHAR(20) |  |
| 8 | NgayCapHoChieu | DATE |  |
| 9 | NgayHetHanHoChieu | DATE |  |
| 10 | LoaiVisa | NVARCHAR(20) |  |
| 11 | NgayHetHanVisa | DATE |  |

---

## 7. CoSoLuuTru

| STT | Tên trường | Kiểu dữ liệu | Khóa |
|---|---|---|---|
| 1 | MaCoSoLuuTru | CHAR(9) | PK |
| 2 | MaTaiKhoan | CHAR(9) | FK |
| 3 | TenCoSo | NVARCHAR(255) |  |
| 4 | DiaChi | NVARCHAR(255) |  |
| 5 | SoDienThoai | NVARCHAR(15) |  |
| 6 | Email | NVARCHAR(100) |  |
| 7 | TrangThai | NVARCHAR(20) |  |

---

## 8. HoSoKhaiBaoTamTru

| STT | Tên trường | Kiểu dữ liệu | Khóa |
|---|---|---|---|
| 1 | MaHSKhaiBao | CHAR(9) | PK |
| 2 | MaTaiKhoan | CHAR(9) | FK |
| 3 | MaCoSoLuuTru | CHAR(9) | FK |
| 4 | NgayKhaiBao | DATE |  |
| 5 | NgayBatDau | DATE |  |
| 6 | NgayKetThuc | DATE |  |
| 7 | MucDichLuuTru | NVARCHAR(100) |  |
| 8 | DiaChiLuuTru | NVARCHAR(255) |  |
| 9 | TrangThai | NVARCHAR(20) |  |
| 10 | LyDoTuChoi | NVARCHAR(255) |  |
| 11 | GhiChu | NVARCHAR(255) |  |

---

## 9. LichSuCuTru

| STT | Tên trường | Kiểu dữ liệu | Khóa |
|---|---|---|---|
| 1 | MaLSLuuTru | CHAR(9) | PK |
| 2 | MaNguoiNuocNgoai | CHAR(9) | FK |
| 3 | MaCoSoLuuTru | CHAR(9) | FK |
| 4 | NgayBatDau | DATETIME |  |
| 5 | NgayKetThuc | DATETIME |  |
| 6 | Phong | NVARCHAR(20) |  |
| 7 | TrangThai | NVARCHAR(20) |  |
| 8 | GhiChu | NVARCHAR(255) |  |

---

## 10. CanhBaoViPham

| STT | Tên trường | Kiểu dữ liệu | Khóa |
|---|---|---|---|
| 1 | MaCanhBao | INT | PK |
| 2 | MaNguoiNuocNgoai | CHAR(9) | FK |
| 3 | MaCanBo | CHAR(9) | FK |
| 4 | LoaiViPham | NVARCHAR(100) |  |
| 5 | NoiDungCanhBao | TEXT |  |
| 6 | MucDoViPham | NVARCHAR(20) |  |
| 7 | NgayCanhBao | DATETIME |  |
| 8 | TrangThai | NVARCHAR(20) |  |
| 9 | GhiChu | TEXT |  |

---

## 11. BaoCaoViPham

| STT | Tên trường | Kiểu dữ liệu | Khóa |
|---|---|---|---|
| 1 | MaBaoCao | CHAR(9) | PK |
| 2 | MaNguoiNuocNgoai | CHAR(9) | FK |
| 3 | MaCanBo | CHAR(9) | FK |
| 4 | NoiDungBaoCao | NVARCHAR(255) |  |
| 5 | NgayBaoCao | DATETIME |  |
| 6 | TrangThaiXuLy | NVARCHAR(20) |  |

---

## 12. LichSuCapNhatThongTinCaNhan

| STT | Tên trường | Kiểu dữ liệu | Khóa |
|---|---|---|---|
| 1 | MaLSCapNhat | CHAR(9) | PK |
| 2 | MaTaiKhoan | CHAR(9) | FK |
| 3 | TruongCapNhat | NVARCHAR(100) |  |
| 4 | GiaTriCu | NVARCHAR(255) |  |
| 5 | GiaTriMoi | NVARCHAR(255) |  |
| 6 | NgayCapNhat | DATETIME |  |
| 7 | LyDoCapNhat | NVARCHAR(255) |  |
| 8 | TrangThai | NVARCHAR(25) |  |