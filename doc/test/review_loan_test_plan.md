# Kế hoạch Kiểm thử: Duyệt khoản vay (Review Loan Test Plan)

Tài liệu này xác định kế hoạch kiểm thử, kịch bản kiểm thử (Test Cases), và các lệnh mẫu để xác minh tính năng phê duyệt hoặc từ chối một khoản vay mới (Review Loan) bởi Admin trong hệ thống HCRM.

---

## 1. Môi trường & Điều kiện tiên quyết (Environment & Prerequisites)
- **API Endpoint**: `POST http://localhost:5000/api/v1/loan/review`
- **Authentication**: Yêu cầu xác thực JWT của Quản trị viên (Admin/Staff Role).
- **Điều kiện cần**:
  - Đã có tài khoản Admin trong hệ thống (mặc định trong seed data hoặc tự gán `Role = 0`).
  - Có ít nhất một khoản vay ở trạng thái `Pending` (chờ duyệt) với ID cụ thể trong hệ thống.
- **Công cụ kiểm thử khuyên dùng**: `curl`, Postman hoặc Swagger UI (`http://localhost:5000/swagger`)

---

## 2. Kịch bản Kiểm thử Chi tiết (Test Cases)

### TC1: Phê duyệt khoản vay thành công (Admin phê duyệt)
- **Mục tiêu**: Đảm bảo Admin có thể chuyển đổi trạng thái khoản vay `Pending` sang `Approved` kèm theo Feedback.
- **Dữ liệu đầu vào**:
  ```json
  {
    "id": 1,
    "feedBack": "Đủ điều kiện duyệt, lịch sử tín dụng tốt.",
    "status": 1
  }
  ```
- **Các bước thực hiện**:
  1. Đăng nhập bằng tài khoản Admin để lấy JWT Token của Admin.
  2. Gửi request `POST` đến `/api/v1/loan/review` với Token và body ở trên.
  3. Xác minh HTTP Status Code trả về là `200 OK`.
  4. Xác minh Response Body:
     - `result` bằng `1` (SUCCESS).
     - `data` bằng `true`.
     - `message` bằng `"Loan reviewed successfully"`.
  5. Kiểm tra Database:
     - Truy vấn bảng `Loans` với `Id = 1`, kiểm tra cột `Status` đã chuyển thành `1` (`Approved`), và cột `FeedBack` lưu đúng chuỗi phản hồi trên.

### TC2: Từ chối khoản vay thành công (Admin từ chối)
- **Mục tiêu**: Đảm bảo Admin có thể từ chối khoản vay và lưu lý do từ chối.
- **Dữ liệu đầu vào**:
  ```json
  {
    "id": 2,
    "feedBack": "Thu nhập không đủ chi trả nợ.",
    "status": 2
  }
  ```
- **Các bước thực hiện**:
  1. Gửi request `POST` đến `/api/v1/loan/review` với Token Admin và body ở trên.
  2. Xác minh HTTP Status Code trả về là `200 OK`.
  3. Xác minh Response Body:
     - `result` bằng `1` (SUCCESS).
     - `data` bằng `true`.
     - `message` bằng `"Loan reviewed successfully"`.
  4. Kiểm tra Database:
     - Truy vấn bảng `Loans` với `Id = 2`, kiểm tra cột `Status` đã chuyển thành `2` (`Rejected`), và cột `FeedBack` lưu lý do từ chối.

### TC3: Duyệt khoản vay thất bại do Sai phân quyền (User thông thường gửi yêu cầu)
- **Mục tiêu**: Đảm bảo người dùng thông thường không có quyền phê duyệt khoản vay.
- **Điều kiện**: Đăng nhập bằng tài khoản người dùng bình thường (`Role = 2`) để lấy Token.
- **Các bước thực hiện**:
  1. Gửi request `POST` đến `/api/v1/loan/review` sử dụng token của User thường.
  2. Xác minh HTTP Status Code trả về là `403 Forbidden` (hoặc `401 Unauthorized` nếu không gửi token). Hệ thống phải từ chối xử lý yêu cầu.

### TC4: Duyệt khoản vay thất bại do ID khoản vay không tồn tại
- **Mục tiêu**: Đảm bảo hệ thống báo lỗi khi duyệt một ID khoản vay không tồn tại.
- **Dữ liệu đầu vào**: ID khoản vay giả định không tồn tại (ví dụ: `999999`).
- **Các bước thực hiện**:
  1. Gửi request `POST` đến `/api/v1/loan/review` với Token Admin và ID không tồn tại.
  2. Xác minh HTTP Status Code trả về là `200 OK` nhưng trả về kết quả lỗi trong wrapper (`result` = 0) hoặc trả về `400/404` tùy thuộc vào thiết lập xử lý ngoại lệ.

---

## 3. Lệnh kiểm thử nhanh bằng cURL (Quick Testing with cURL)

### Thực thi TC1 (Admin phê duyệt thành công):
*Bước 1: Lấy Token của Admin (Ví dụ tài khoản admin mặc định):*
```bash
ADMIN_TOKEN=$(curl -s -X POST http://localhost:5000/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@example.com",
    "password": "AdminPassword123!"
  }' | jq -r '.data.accessToken')
```
*Bước 2: Admin gọi API duyệt khoản vay:*
```bash
curl -X POST http://localhost:5000/api/v1/loan/review \
  -H "Authorization: Bearer $ADMIN_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "id": 1,
    "feedBack": "Phê duyệt duyệt hồ sơ hợp lệ.",
    "status": 1
  }'
```

### Thực thi TC3 (Sai phân quyền - Dùng token của User thường):
```bash
USER_TOKEN=$(curl -s -X POST http://localhost:5000/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "testuser@example.com",
    "password": "StrongPassword123!"
  }' | jq -r '.data.accessToken')

curl -X POST http://localhost:5000/api/v1/loan/review \
  -H "Authorization: Bearer $USER_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "id": 1,
    "feedBack": "Hack status",
    "status": 1
  }'
```
*(Kết quả mong đợi: Hệ thống trả về `403 Forbidden` hoặc `401 Unauthorized` tùy theo cấu hình).*
