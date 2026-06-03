# SKILL.md

## Skill: Core Foreign Resident Management

Skill này hướng dẫn AI Agent triển khai các chức năng cốt lõi của hệ thống quản lý người nước ngoài.

Chỉ sử dụng skill này cho các chức năng đã xác định:

- Khách vãng lai xem thông tin công khai và đăng ký tài khoản.
- Người nước ngoài đăng nhập, khai báo tạm trú, cập nhật nơi cư trú, tra cứu và cập nhật thông tin cá nhân.
- Chủ cơ sở lưu trú đăng nhập, khai báo lưu trú, cập nhật trạng thái lưu trú, xem danh sách/lịch sử lưu trú và cập nhật thông tin cơ sở lưu trú.
- Cán bộ quản lý xuất nhập cảnh đăng nhập, tra cứu người nước ngoài, thống kê/lập báo cáo và gửi cảnh báo vi phạm.
- Công an Phường/Xã đăng nhập, phê duyệt khai báo tạm trú, cảnh báo vi phạm, báo cáo vi phạm, tra cứu theo địa bàn và thống kê báo cáo.

## 1. Khi nào dùng skill này

Dùng khi task liên quan đến:

- Đăng ký tài khoản.
- Đăng nhập.
- Thông tin công khai.
- Khai báo tạm trú.
- Cập nhật nơi cư trú.
- Tra cứu/cập nhật thông tin cá nhân người nước ngoài.
- Quản lý cơ sở lưu trú.
- Khai báo lưu trú.
- Cập nhật trạng thái lưu trú.
- Phê duyệt khai báo tạm trú.
- Cảnh báo vi phạm.
- Báo cáo vi phạm.
- Thống kê và lập báo cáo.

Không dùng skill này để tự ý triển khai:

- Bảo lãnh.
- Visa.
- Thẻ tạm trú.
- Thanh toán.
- Quản trị hệ thống nâng cao.
- Tích hợp liên ngành ngoài phạm vi.

## 2. Actors

### Guest

Chức năng:

- Xem thông tin công khai.
- Đăng ký tài khoản.

### Foreigner

Chức năng:

- Đăng nhập.
- Khai báo tạm trú.
- Cập nhật nơi cư trú.
- Tra cứu thông tin cá nhân.
- Cập nhật thông tin cá nhân.

### AccommodationOwner

Chức năng:

- Đăng nhập.
- Khai báo lưu trú cho người nước ngoài.
- Cập nhật trạng thái lưu trú của người nước ngoài.
- Xem danh sách người nước ngoài đang lưu trú.
- Xem lịch sử khai báo lưu trú.
- Cập nhật thông tin cơ sở lưu trú.

### ImmigrationOfficer

Chức năng:

- Đăng nhập.
- Tra cứu thông tin người nước ngoài.
- Thống kê và lập báo cáo.
- Gửi cảnh báo về vi phạm của người nước ngoài.

### WardPolice

Chức năng:

- Đăng nhập.
- Phê duyệt khai báo tạm trú trong Phường/Xã phụ trách.
- Cảnh báo vi phạm của người nước ngoài.
- Báo cáo vi phạm lên Cán bộ quản lý xuất nhập cảnh.
- Tra cứu người nước ngoài trên địa bàn phụ trách.
- Thống kê, báo cáo.

## 3. Core Data

### Account

Dữ liệu tài khoản đăng nhập.

Trường gợi ý:

- id
- username
- passwordHash
- role
- email
- phoneNumber
- status
- createdAt
- lastLoginAt

### Foreigner

Dữ liệu người nước ngoài.

Trường gợi ý:

- id
- accountId
- fullName
- dateOfBirth
- gender
- nationality
- passportNumber
- currentAddress
- residenceStatus
- createdAt
- updatedAt

### ResidenceDeclaration

Dữ liệu khai báo tạm trú.

Trường gợi ý:

- id
- foreignerId
- declarationDate
- startDate
- endDate
- purpose
- address
- status
- rejectionReason
- approvedBy
- approvedAt
- note

### AccommodationFacility

Dữ liệu cơ sở lưu trú.

Trường gợi ý:

- id
- ownerAccountId
- name
- address
- phoneNumber
- email
- status

### StayHistory

Dữ liệu lịch sử lưu trú.

Trường gợi ý:

- id
- foreignerId
- accommodationFacilityId
- startDate
- endDate
- room
- status
- note

### ViolationWarning

Dữ liệu cảnh báo vi phạm.

Trường gợi ý:

- id
- foreignerId
- createdBy
- violationType
- content
- severity
- status
- createdAt
- note

### ViolationReport

Dữ liệu báo cáo vi phạm.

Trường gợi ý:

- id
- foreignerId
- reportedBy
- content
- reportDate
- status
- note

## 4. Business Rules

### Authentication

- Người dùng nội bộ phải đăng nhập trước khi sử dụng chức năng.
- Mật khẩu phải được hash.
- Tài khoản bị khóa không được đăng nhập.

### Authorization

- Guest chỉ được xem thông tin công khai và đăng ký.
- Foreigner chỉ được thao tác với dữ liệu của chính mình.
- AccommodationOwner chỉ được quản lý dữ liệu thuộc cơ sở lưu trú của mình.
- ImmigrationOfficer được tra cứu, cảnh báo và thống kê theo phạm vi được cấp.
- WardPolice chỉ được xử lý dữ liệu thuộc Phường/Xã phụ trách.

### Residence Declaration

- `endDate` phải lớn hơn `startDate`.
- `address` không được để trống.
- `purpose` không được để trống.
- Hồ sơ mới nên có trạng thái mặc định `ChoDuyet`.
- Khi từ chối phải có `rejectionReason`.

### Stay History

- Trạng thái lưu trú gồm: `DangO`, `DaRoi`, `QuaHan`.
- Khi cập nhật trả phòng, cần cập nhật `endDate` hoặc trạng thái `DaRoi`.
- Chủ cơ sở lưu trú chỉ xem được danh sách và lịch sử thuộc cơ sở của mình.

### Violation

- Cảnh báo/báo cáo phải gắn với một người nước ngoài.
- Nội dung vi phạm không được rỗng.
- Mức độ vi phạm nên dùng enum: `Nhe`, `TrungBinh`, `NghiemTrong`.
- Trạng thái xử lý nên dùng enum: `ChuaXuLy`, `DangXuLy`, `DaXuLy`.

## 5. Status Constants

```text
AccountStatus:
- ChoDuyet
- HoatDong
- BiKhoa

DeclarationStatus:
- ChoDuyet
- DaDuyet
- TuChoi

StayStatus:
- DangO
- DaRoi
- QuaHan

ViolationSeverity:
- Nhe
- TrungBinh
- NghiemTrong

ProcessingStatus:
- ChuaXuLy
- DangXuLy
- DaXuLy
```

## 6. API Guidelines

Endpoint gợi ý:

```text
GET    /public-info
POST   /auth/register
POST   /auth/login
GET    /foreigners/me
PUT    /foreigners/me
POST   /residence-declarations
GET    /residence-declarations/my
PATCH  /residence-declarations/{id}/approve
PATCH  /residence-declarations/{id}/reject
GET    /accommodation-facilities/me
PUT    /accommodation-facilities/me
POST   /stay-histories
PATCH  /stay-histories/{id}/status
GET    /stay-histories/current
GET    /stay-histories/history
GET    /foreigners/search
POST   /violation-warnings
POST   /violation-reports
GET    /reports/statistics
```

Quy tắc API:

- API danh sách cần phân trang.
- API tra cứu cần hỗ trợ tìm kiếm và lọc.
- API phê duyệt/từ chối phải kiểm tra quyền WardPolice.
- API cảnh báo phải kiểm tra quyền ImmigrationOfficer hoặc WardPolice.
- API báo cáo vi phạm phải kiểm tra quyền WardPolice.
- Không trả về password hash.
- Không trả về dữ liệu cá nhân ngoài phạm vi quyền.

## 7. UI Guidelines

Màn hình gợi ý:

- Trang thông tin công khai.
- Trang đăng ký.
- Trang đăng nhập.
- Trang người nước ngoài: thông tin cá nhân, khai báo tạm trú, cập nhật nơi cư trú.
- Trang chủ cơ sở lưu trú: khai báo lưu trú, danh sách đang lưu trú, lịch sử khai báo, thông tin cơ sở.
- Trang cán bộ xuất nhập cảnh: tra cứu người nước ngoài, cảnh báo vi phạm, thống kê báo cáo.
- Trang Công an Phường/Xã: phê duyệt khai báo, tra cứu địa bàn, cảnh báo/báo cáo vi phạm, thống kê.

Quy tắc UI:

- Form phải có validate.
- Bảng dữ liệu nên có tìm kiếm, lọc, phân trang.
- Trạng thái nên hiển thị bằng badge.
- Hành động quan trọng cần modal xác nhận.
- Giao diện phải ẩn chức năng không thuộc vai trò hiện tại.

## 8. Testing Checklist

- Đăng ký tài khoản.
- Đăng nhập thành công/thất bại.
- Người nước ngoài khai báo tạm trú.
- Người nước ngoài cập nhật nơi cư trú.
- Người nước ngoài cập nhật thông tin cá nhân.
- Chủ cơ sở lưu trú khai báo lưu trú.
- Chủ cơ sở lưu trú cập nhật trạng thái lưu trú.
- Công an Phường/Xã phê duyệt/từ chối khai báo.
- Cán bộ xuất nhập cảnh tra cứu người nước ngoài.
- Cảnh báo vi phạm.
- Báo cáo vi phạm.
- Thống kê, báo cáo.
- Kiểm tra truy cập sai vai trò.

## 9. Prompt Pattern

```text
Đọc codebase hiện tại trước, không phá vỡ kiến trúc có sẵn.

Triển khai chức năng [TÊN CHỨC NĂNG] cho hệ thống quản lý người nước ngoài.

Chỉ triển khai theo phạm vi chức năng đã xác định:
- Khách vãng lai
- Người nước ngoài
- Chủ cơ sở lưu trú
- Cán bộ quản lý xuất nhập cảnh
- Công an Phường/Xã

Yêu cầu:
- Đúng vai trò được phép thao tác.
- Có kiểm tra đăng nhập/phân quyền.
- Có validate dữ liệu đầu vào.
- Không trả về dữ liệu nhạy cảm.
- Có xử lý lỗi rõ ràng.
- Có test hoặc hướng dẫn test.

Không tự ý thêm nghiệp vụ ngoài phạm vi.
```

## 10. Done Criteria

- Đúng chức năng được yêu cầu.
- Không thêm nghiệp vụ ngoài phạm vi.
- Đúng phân quyền.
- Có validation.
- Có xử lý lỗi.
- Không lộ dữ liệu nhạy cảm.
- Có kiểm thử hoặc hướng dẫn kiểm thử.
