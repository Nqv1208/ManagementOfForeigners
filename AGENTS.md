# AGENTS.md

## 1. Vai trò của AI Coding Agent

AI Coding Agent hỗ trợ đọc hiểu, thiết kế, triển khai và kiểm thử dự án quản lý người nước ngoài dựa đúng trên các chức năng đã xác định.

Agent chỉ được triển khai các chức năng thuộc phạm vi sau:

- Khách vãng lai xem thông tin công khai và đăng ký tài khoản.
- Người nước ngoài đăng nhập, khai báo tạm trú, cập nhật nơi cư trú, tra cứu và cập nhật thông tin cá nhân.
- Chủ cơ sở lưu trú đăng nhập, khai báo lưu trú, cập nhật trạng thái lưu trú, xem danh sách/lịch sử lưu trú và cập nhật thông tin cơ sở.
- Cán bộ quản lý xuất nhập cảnh đăng nhập, tra cứu thông tin người nước ngoài, thống kê/lập báo cáo và gửi cảnh báo vi phạm.
- Công an Phường/Xã đăng nhập, phê duyệt khai báo tạm trú, cảnh báo vi phạm, báo cáo vi phạm, tra cứu theo địa bàn và thống kê báo cáo.

Không tự ý thêm nghiệp vụ ngoài phạm vi như bảo lãnh, visa, thẻ tạm trú, thanh toán, workflow nâng cao hoặc quản trị hệ thống nếu chưa được yêu cầu.

## 2. Nguyên tắc làm việc

Trước khi sửa hoặc sinh code, agent phải:

1. Đọc cấu trúc dự án hiện tại.
2. Xác định framework, ngôn ngữ, kiến trúc và convention đang dùng.
3. Tận dụng component, service, repository, entity và pattern đã có.
4. Không phá vỡ cấu trúc hiện tại.
5. Không xóa file hoặc logic cũ nếu chưa chắc chắn.
6. Không mở rộng phạm vi chức năng ngoài danh sách đã xác định.

## 3. Tác nhân và quyền chính

### 3.1. Guest / Khách vãng lai

Được phép:

- Xem thông tin công khai.
- Đăng ký tài khoản.

Không được phép:

- Khai báo tạm trú.
- Tra cứu dữ liệu người nước ngoài.
- Xem dữ liệu nội bộ.

### 3.2. Foreigner / Người nước ngoài

Được phép:

- Đăng nhập.
- Khai báo tạm trú.
- Cập nhật nơi cư trú.
- Tra cứu thông tin cá nhân.
- Cập nhật thông tin cá nhân.

Giới hạn:

- Chỉ được xem và cập nhật dữ liệu của chính mình.
- Không được xem dữ liệu của người nước ngoài khác.

### 3.3. AccommodationOwner / Chủ cơ sở lưu trú

Được phép:

- Đăng nhập.
- Khai báo lưu trú cho người nước ngoài.
- Cập nhật trạng thái lưu trú.
- Xem danh sách người nước ngoài đang lưu trú.
- Xem lịch sử khai báo lưu trú.
- Cập nhật thông tin cơ sở lưu trú.

Giới hạn:

- Chỉ được quản lý dữ liệu thuộc cơ sở lưu trú của mình.
- Không được phê duyệt khai báo tạm trú.

### 3.4. ImmigrationOfficer / Cán bộ quản lý xuất nhập cảnh

Được phép:

- Đăng nhập.
- Tra cứu thông tin người nước ngoài.
- Thống kê và lập báo cáo.
- Gửi cảnh báo vi phạm của người nước ngoài.

Giới hạn:

- Không tự ý sửa thông tin cá nhân nếu nghiệp vụ không cho phép.
- Khi gửi cảnh báo phải có nội dung, loại vi phạm và người liên quan.

### 3.5. WardPolice / Công an Phường/Xã

Được phép:

- Đăng nhập.
- Phê duyệt khai báo tạm trú trong Phường/Xã phụ trách.
- Cảnh báo vi phạm của người nước ngoài.
- Báo cáo vi phạm lên Cán bộ quản lý xuất nhập cảnh.
- Tra cứu người nước ngoài trên địa bàn phụ trách.
- Thống kê, báo cáo.

Giới hạn:

- Chỉ xử lý dữ liệu thuộc địa bàn phụ trách.
- Khi từ chối hoặc báo cáo vi phạm phải có lý do/nội dung rõ ràng.

## 4. Quy tắc thiết kế chức năng

### 4.1. Xác thực

- Chức năng nội bộ phải yêu cầu đăng nhập.
- Mật khẩu phải được hash.
- Không trả về mật khẩu hoặc password hash trong response.

### 4.2. Phân quyền

- Phải kiểm tra quyền ở backend.
- Frontend chỉ ẩn/hiện nút là chưa đủ.
- Mỗi API cần xác định rõ vai trò được phép truy cập.

### 4.3. Dữ liệu cá nhân

- Thông tin người nước ngoài là dữ liệu nhạy cảm.
- Không log số hộ chiếu, mật khẩu, token hoặc dữ liệu cá nhân nhạy cảm.
- Không cho phép truy cập dữ liệu ngoài phạm vi vai trò.

### 4.4. Khai báo tạm trú

Luồng cơ bản:

```text
Tạo khai báo -> Chờ duyệt -> Công an Phường/Xã phê duyệt/từ chối -> Cập nhật trạng thái
```

Quy tắc:

- Ngày kết thúc phải lớn hơn ngày bắt đầu.
- Địa chỉ lưu trú không được để trống.
- Mục đích lưu trú không được để trống.
- Nếu từ chối, phải có lý do từ chối.

### 4.5. Lưu trú tại cơ sở

Luồng cơ bản:

```text
Chủ cơ sở lưu trú -> Khai báo lưu trú -> Cập nhật đang ở/đã rời/quá hạn -> Xem danh sách và lịch sử
```

Quy tắc:

- Mỗi bản ghi lưu trú phải gắn với người nước ngoài và cơ sở lưu trú.
- Trạng thái lưu trú phải rõ ràng.
- Khi người nước ngoài rời đi, cần cập nhật ngày kết thúc hoặc trạng thái tương ứng.

### 4.6. Cảnh báo và báo cáo vi phạm

Luồng cơ bản:

```text
Tạo cảnh báo/báo cáo -> Gửi đến cán bộ liên quan -> Theo dõi trạng thái xử lý
```

Quy tắc:

- Cảnh báo/báo cáo phải gắn với người nước ngoài.
- Nội dung vi phạm không được để trống.
- Nên có mức độ vi phạm: nhẹ, trung bình, nghiêm trọng.
- Nên có trạng thái xử lý: chưa xử lý, đang xử lý, đã xử lý.

## 5. Quy tắc API

Khi tạo hoặc sửa API:

- API danh sách cần có phân trang.
- API danh sách nên có tìm kiếm và lọc.
- API xử lý trạng thái cần kiểm tra trạng thái hiện tại.
- API phê duyệt/từ chối khai báo cần tách rõ hành động.
- API báo cáo/thống kê cần lọc theo thời gian và phạm vi dữ liệu.
- API không trả về dữ liệu nhạy cảm không cần thiết.

Endpoint gợi ý:

```text
/public-info
/auth
/foreigners
/residence-declarations
/accommodation-facilities
/stay-histories
/violation-warnings
/violation-reports
/reports
```

## 6. Quy tắc UI

Khi triển khai giao diện:

- Giao diện phải rõ ràng theo từng vai trò.
- Form cần validate và hiển thị lỗi dễ hiểu.
- Bảng dữ liệu cần có tìm kiếm, lọc và phân trang nếu dữ liệu nhiều.
- Trạng thái khai báo/lưu trú/vi phạm nên hiển thị bằng badge.
- Hành động quan trọng như phê duyệt, từ chối, cảnh báo cần có xác nhận.
- Không hiển thị chức năng ngoài quyền của người dùng.

## 7. Checklist hoàn thành task

- [ ] Đã bám đúng danh sách chức năng được yêu cầu.
- [ ] Không thêm nghiệp vụ ngoài phạm vi.
- [ ] Có xác thực cho chức năng nội bộ.
- [ ] Có kiểm tra phân quyền ở backend.
- [ ] Có validate dữ liệu đầu vào.
- [ ] Có xử lý lỗi rõ ràng.
- [ ] Không lộ dữ liệu nhạy cảm.
- [ ] Có kiểm thử hoặc hướng dẫn kiểm thử.
- [ ] Không phá vỡ chức năng cũ.
