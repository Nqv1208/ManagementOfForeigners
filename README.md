# Hệ Thống Quản Lý Người Nước Ngoài (TP. Đà Nẵng)

Hệ thống quản lý cư trú, tạm trú và lưu trú của người nước ngoài tại Thành phố Đà Nẵng. Ứng dụng được xây dựng theo kiến trúc **ASP.NET Core MVC** truyền thống, tối ưu hóa hiển thị bằng **Razor Views**, lưu trữ trên **SQL Server (EF Core)** và bảo mật nghiêm ngặt bằng phân quyền dạng Cookie-Session (Role-based).

---

## 1. Danh Sách Tài Khoản Thử Nghiệm (Demo Accounts)

Khi ứng dụng khởi chạy lần đầu, dữ liệu mẫu đầy đủ cho 5 vai trò sẽ tự động được seed vào cơ sở dữ liệu. Dưới đây là thông tin tài khoản demo để chạy thử nghiệm:

| Vai trò | Tên tài khoản (Username) | Mật khẩu (Password) | Phạm vi và mô tả |
| :--- | :--- | :--- | :--- |
| **Khách vãng lai (Guest)** | *Không đăng nhập* | *Không có* | Đăng ký tài khoản mới trực tuyến |
| **Người nước ngoài (Foreigner)** | `john.doe` | `Password123` | John Doe (Mỹ) - Tự khai báo tạm trú, xem lịch sử cư trú và cập nhật thông tin cá nhân |
| **Chủ cơ sở lưu trú (Owner)** | `chu.facility` | `Password123` | Quản lý Khách sạn Da Nang Riverside - Khai báo lưu trú cho khách, cập nhật trạng thái lưu trú |
| **Công an Phường (Ward Police)** | `congan01` | `Password123` | Phụ trách địa bàn **Hải Châu** - Duyệt hồ sơ tạm trú, tra cứu địa bàn, cảnh báo & báo cáo vi phạm, thống kê |
| **Công an Phường (Ward Police)** | `congan02` | `Password123` | Phụ trách địa bàn **Sơn Trà** - Duyệt hồ sơ tạm trú, tra cứu địa bàn, cảnh báo & báo cáo vi phạm, thống kê |
| **Cán bộ QL XNC (Officer)** | `officer01` | `Password123` | Cán bộ quản lý xuất nhập cảnh TP. Đà Nẵng - Tra cứu toàn hệ thống, gửi cảnh báo vi phạm, thống kê báo cáo |

---

## 2. Kiến Trúc Kỹ Thuật (Technology Stack)

Hệ thống tuân thủ nghiêm ngặt các quy định về mặt kỹ thuật và nghiệp vụ:
1. **Framework Core**: ASP.NET Core 8.0 MVC (Razor Views server-side). Không sử dụng SPA framework (React/Vue/Angular) hay cơ chế API-First không cần thiết.
2. **Cơ sở dữ liệu**: Microsoft SQL Server (sử dụng LocalDB cho môi trường phát triển local) kết hợp **Entity Framework Core** Code-First.
3. **Quản lý xác thực**: Sử dụng **Cookie Authentication** kết hợp với **Session** (lưu thông tin phiên hoạt động), tuyệt đối không dùng JWT.
4. **Phân quyền Backend**: Sử dụng Filter tùy biến `AuthorizeRoleAttribute` chặn đứng các truy cập trái phép ở tầng Action/Controller.
5. **Ghi log bảo mật (Audit Log)**: Mọi thao tác cập nhật dữ liệu nhạy cảm của người nước ngoài được lưu vết nghiêm ngặt vào bảng `LichSuCapNhatThongTin`.
6. **Mã thực thể tăng tuần tự**: Các mã định danh như `MaTaiKhoan`, `MaNguoiNuocNgoai`... được tạo tự động dưới định dạng `CHAR(9)` tăng tuần tự dạng `prefix + 6 chữ số` (ví dụ: TK000001, NN000001) thông qua helper `IdGenerator`.

---

## 3. Cấu Trúc Codebase Thực Tế

```text
ManagementOfForeigners/
├── Controllers/
│   ├── AccountController.cs       # Đăng nhập, Đăng ký, Phân quyền
│   ├── ForeignerController.cs     # Nghiệp vụ Người nước ngoài
│   ├── AccommodationController.cs # Nghiệp vụ Chủ cơ sở lưu trú
│   ├── OfficerController.cs       # Nghiệp vụ Cán bộ QL XNC
│   ├── PoliceController.cs        # Nghiệp vụ Công an Phường/Xã
│   └── HomeController.cs          # Trang chủ công khai (Landing Page)
├── Data/
│   ├── ApplicationDbContext.cs    # Cấu hình EF Core Fluent API
│   └── SeedData.cs                # Tự động nạp dữ liệu mẫu
├── Filters/
│   └── AuthorizeRoleAttribute.cs  # Phân quyền chặn API/Action
├── Helpers/
│   └── IdGenerator.cs             # Helper tạo ID CHAR(9) tuần tự
├── Models/
│   ├── Entities/                  # 9 Bảng dữ liệu nghiệp vụ
│   └── ViewModels/                # Dữ liệu truyền nhận giữa View và Controller
├── Views/
│   ├── Account/                   # Form Đăng nhập, Đăng ký, 403
│   ├── Foreigner/                 # Giao diện của Người nước ngoài
│   ├── Accommodation/             # Giao diện của Chủ cơ sở
│   ├── Officer/                   # Giao diện của Cán bộ XNC
│   ├── Police/                    # Giao diện của Công an Phường
│   └── Shared/                    # Layout chung và các partial views
├── Program.cs                     # Cấu hình Services, Auth, Session & Routing
└── appsettings.json               # Cấu hình Connection String tới SQL Server
```

---

## 4. Các Luồng Nghiệp Vụ Chính Đã Triển Khai

1. **Khách vãng lai đăng ký**: Khách vãng lai truy cập trang chủ, xem thông báo pháp luật và biểu mẫu hướng dẫn. Khách có thể chọn đăng ký tài khoản mới dưới vai trò **Người nước ngoài** (nhập thông tin hộ chiếu, visa) hoặc **Chủ cơ sở lưu trú** (nhập thông tin cơ sở lưu trú).
2. **Khai báo tạm trú**: Người nước ngoài đăng nhập, tạo mới hồ sơ khai báo tạm trú (nhập địa chỉ, ngày bắt đầu, ngày kết thúc và mục đích). Trạng thái hồ sơ ban đầu sẽ là `Chờ duyệt`.
3. **Phê duyệt khai báo**: Công an Phường đăng nhập (ví dụ `congan01`), hệ thống tự nhận diện địa bàn phụ trách để hiển thị danh sách hồ sơ tạm trú thuộc địa bàn đó. Công an có quyền `Phê duyệt` (hồ sơ chuyển sang trạng thái `Đã duyệt` và nơi cư trú hiện tại của người nước ngoài được tự động cập nhật) hoặc `Từ chối` (bắt buộc nhập lý do từ chối).
4. **Lưu trú tại cơ sở**: Chủ cơ sở đăng nhập, khai báo lưu trú cho khách nước ngoài tại cơ sở của mình. Có thể cập nhật trạng thái lưu trú (`Đang ở`, `Đã rời`, `Quá hạn`).
5. **Cảnh báo và Báo cáo vi phạm**:
   - Công an phường và Cán bộ XNC có thể gửi cảnh báo vi phạm trực tiếp đến người nước ngoài cư trú trên địa bàn.
   - Công an phường có quyền tạo báo cáo vi phạm gửi lên Phòng Quản lý Xuất nhập cảnh để phối hợp xử lý.
6. **Thống kê và Tra cứu**: 
   - Công an và Cán bộ XNC có thể tra cứu thông tin chi tiết người nước ngoài (trong địa bàn hoặc toàn thành phố).
   - Biểu đồ thống kê cơ cấu quốc tịch và mật độ khách tại các cơ sở được dựng trực quan bằng Bento-grid và các thanh đo CSS hiện đại, sang trọng.

---

## 5. Hướng Dẫn Vận Hành Hệ Thống

### Bước 1: Chuẩn bị cơ sở dữ liệu
Ứng dụng sử dụng chuỗi kết nối mặc định nhắm vào **SQL Server LocalDB**:
`Server=(localdb)\\mssqllocaldb;Database=ManagementOfForeignersDb;Trusted_Connection=True;MultipleActiveResultSets=true`

*Lưu ý: Nếu bạn dùng phiên bản SQL Server khác, hãy cập nhật lại Connection String trong file `appsettings.json`.*

### Bước 2: Chạy ứng dụng lần đầu
Khi chạy ứng dụng lần đầu tiên, hệ thống sẽ tự động thực hiện quá trình khởi tạo database (`DbContext.Database.EnsureCreated()`) và nạp toàn bộ dữ liệu mẫu (Seed Data) bao gồm các tài khoản demo được liệt kê ở mục 1.

Mở terminal tại thư mục dự án và chạy lệnh:
```bash
dotnet run
```

### Bước 3: Thử nghiệm các vai trò
Truy cập `http://localhost:5000` hoặc cổng HTTPS hiển thị trên màn hình terminal:
1. Đăng nhập tài khoản `john.doe` để khai báo tạm trú.
2. Đăng nhập tài khoản `congan01` để duyệt hồ sơ tạm trú vừa tạo.
3. Đăng nhập tài khoản `chu.facility` để khai báo lưu trú tại cơ sở.
4. Đăng nhập tài khoản `officer01` để xem thống kê toàn thành phố.
