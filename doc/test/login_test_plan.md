# Kế hoạch Kiểm thử: Đăng nhập (User Login Test Plan)

Tài liệu này xác định kế hoạch kiểm thử, kịch bản kiểm thử (Test Cases), và các lệnh mẫu để xác minh tính năng đăng nhập (User Login) trong hệ thống HCRM.

---

## 1. Môi trường & Điều kiện tiên quyết (Environment & Prerequisites)
- **API Endpoint**: `POST http://localhost:5000/api/v1/auth/login` (hoặc HTTPS tương ứng)
- **Dữ liệu kiểm thử tiên quyết**: Cần có một tài khoản đã được đăng ký trước (ví dụ từ kế hoạch đăng ký `testuser@example.com` với mật khẩu `StrongPassword123!`).
- **Công cụ kiểm thử khuyên dùng**: `curl`, Postman hoặc Swagger UI (`http://localhost:5000/swagger`)

---

## 2. Kịch bản Kiểm thử Chi tiết (Test Cases)

### TC1: Đăng nhập thành công (Thông tin chính xác)
- **Mục tiêu**: Đảm bảo người dùng đăng nhập thành công khi nhập đúng email và mật khẩu. Hệ thống phải trả về Access Token và Refresh Token hợp lệ cùng thông tin người dùng chính xác.
- **Dữ liệu đầu vào**:
  ```json
  {
    "email": "testuser@example.com",
    "password": "StrongPassword123!"
  }
  ```
- **Các bước thực hiện**:
  1. Gửi request `POST` đến `/api/v1/auth/login` với body ở trên.
  2. Xác minh HTTP Status Code trả về là `200 OK`.
  3. Xác minh Response Body:
     - `result` bằng `1` (SUCCESS).
     - `data` chứa `accessToken` (không rỗng), `refreshToken` (không rỗng), `tokenType` bằng `"Bearer"`, `expiresIn` khớp cấu hình JWT, và đối tượng `userInfo` chứa đúng thông tin người dùng (`id`, `email`, `fullName`, `role` = 2).
     - `message` bằng `"Login successful"`.
  4. Xác minh tính hợp lệ của Token: Sử dụng công cụ giải mã JWT (như jwt.io) để kiểm tra:
     - Access Token chứa claim `sub` / `NameIdentifier` bằng Id của user.
     - Claim `email` bằng `testuser@example.com`.
     - Claim `role` bằng `User` (hoặc tương đương).
  5. Kiểm tra Database: Đảm bảo trường `RefreshTokenHash` của User trong bảng `Users` đã được cập nhật giá trị mới.

### TC2: Đăng nhập thất bại do nhập sai Email
- **Mục tiêu**: Đảm bảo hệ thống chặn không cho đăng nhập và trả về thông báo lỗi chung khi email không tồn tại trong hệ thống.
- **Dữ liệu đầu vào**:
  ```json
  {
    "email": "wrongemail@example.com",
    "password": "StrongPassword123!"
  }
  ```
- **Các bước thực hiện**:
  1. Gửi request `POST` đến `/api/v1/auth/login` với body ở trên.
  2. Xác minh HTTP Status Code trả về là `200 OK` (hoặc status tương ứng trong response wrapper).
  3. Xác minh Response Body:
     - `result` bằng `0` (ERROR).
     - `data` bằng `null`.
     - `message` bằng `"Email or password is incorrect"` (không được tiết lộ là email không tồn tại vì lý do bảo mật).

### TC3: Đăng nhập thất bại do nhập sai Mật khẩu
- **Mục tiêu**: Đảm bảo hệ thống từ chối đăng nhập khi email đúng nhưng nhập sai mật khẩu.
- **Dữ liệu đầu vào**:
  ```json
  {
    "email": "testuser@example.com",
    "password": "WrongPassword123!"
  }
  ```
- **Các bước thực hiện**:
  1. Gửi request `POST` đến `/api/v1/auth/login` với body ở trên.
  2. Xác minh HTTP Status Code trả về là `200 OK`.
  3. Xác minh Response Body:
     - `result` bằng `0` (ERROR).
     - `data` bằng `null`.
     - `message` bằng `"Email or password is incorrect"`.

### TC4: Đăng nhập thất bại do thiếu thông tin (Validation Error)
- **Mục tiêu**: Đảm bảo hệ thống bắt lỗi định dạng email hoặc thiếu email/mật khẩu trước khi xử lý nghiệp vụ.
- **Kịch bản nhỏ (Sub-cases)**:
  - **TC4.1: Thiếu Email** (`{"password": "StrongPassword123!"}`)
  - **TC4.2: Thiếu Mật khẩu** (`{"email": "testuser@example.com"}`)
  - **TC4.3: Định dạng Email sai** (`{"email": "bademail", "password": "StrongPassword123!"}`)
- **Các bước thực hiện**:
  1. Gửi request `POST` đến `/api/v1/auth/login` với dữ liệu thiếu hoặc sai định dạng.
  2. Xác minh HTTP Status Code trả về là `400 Bad Request`.
  3. Xác minh Response Body chứa chi tiết lỗi xác thực của các trường tương ứng.

---

## 3. Lệnh kiểm thử nhanh bằng cURL (Quick Testing with cURL)

### Thực thi TC1 (Đăng nhập thành công):
*(Lưu ý: Thay đổi email nếu bạn đã tạo email khác khi test)*
```bash
curl -X POST http://localhost:5000/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "testuser@example.com",
    "password": "StrongPassword123!"
  }'
```

### Thực thi TC2 (Sai Email):
```bash
curl -X POST http://localhost:5000/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "nonexistent@example.com",
    "password": "StrongPassword123!"
  }'
```

### Thực thi TC3 (Sai Mật khẩu):
```bash
curl -X POST http://localhost:5000/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "testuser@example.com",
    "password": "WrongPassword123!"
  }'
```

### Thực thi TC4 (Định dạng sai & thiếu thông tin):
```bash
curl -X POST http://localhost:5000/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "invalidemail",
    "password": ""
  }'
```

---

## 4. Xác minh trong Cơ sở Dữ liệu (Database Verification Query)
Sau khi thực hiện đăng nhập thành công (TC1), chạy truy vấn SQL sau để kiểm chứng việc lưu trữ Refresh Token:
```sql
SELECT Id, Email, RefreshTokenHash FROM Users WHERE Email = 'testuser@example.com';
```
*Kết quả mong đợi:*
- Cột `RefreshTokenHash` không được null và phải chứa chuỗi hash bảo mật của refresh token vừa tạo.
