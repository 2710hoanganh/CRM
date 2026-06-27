# Kế hoạch Kiểm thử: Đăng ký tài khoản (User Registration Test Plan)

Tài liệu này xác định kế hoạch kiểm thử, kịch bản kiểm thử (Test Cases), và các lệnh mẫu để xác minh tính năng đăng ký tài khoản (User Registration) trong hệ thống HCRM.

---

## 1. Môi trường & Điều kiện tiên quyết (Environment & Prerequisites)
- **API Endpoint**: `POST http://localhost:5000/api/v1/auth/register` (hoặc HTTPS tương ứng)
- **Database**: SQL Server (Database `HCRM`)
- **Công cụ kiểm thử khuyên dùng**: `curl`, Postman hoặc Swagger UI (`http://localhost:5000/swagger`)

---

## 2. Kịch bản Kiểm thử Chi tiết (Test Cases)

### TC1: Đăng ký tài khoản thành công (Dữ liệu hợp lệ)
- **Mục tiêu**: Đảm bảo người dùng có thể đăng ký tài khoản mới khi nhập thông tin hợp lệ và email chưa tồn tại trong hệ thống.
- **Dữ liệu đầu vào**:
  ```json
  {
    "email": "newuser@example.com",
    "password": "StrongPassword123!",
    "firstName": "Nguyễn",
    "lastName": "Văn A"
  }
  ```
- **Các bước thực hiện**:
  1. Gửi request `POST` đến `/api/v1/auth/register` với body ở trên.
  2. Xác minh HTTP Status Code trả về là `200 OK`.
  3. Xác minh Response Body khớp với định dạng chuẩn:
     - `result` bằng `1` (SUCCESS).
     - `data` chứa `id` tự tăng, `email` khớp với dữ liệu gửi lên, và `fullName` khớp với `"Nguyễn Văn A"`.
     - `message` bằng `"User registered successfully"`.
  4. Kiểm tra Database: Truy vấn bảng `Users` kiểm tra xem bản ghi mới đã được chèn vào chưa, password được hash bằng BCrypt, và `Role` được thiết lập mặc định là `2` (User).

### TC2: Đăng ký thất bại do Email đã tồn tại
- **Mục tiêu**: Đảm bảo hệ thống chặn không cho đăng ký tài khoản mới với email đã có trong cơ sở dữ liệu.
- **Điều kiện**: Email `newuser@example.com` đã được đăng ký thành công trước đó (từ TC1).
- **Dữ liệu đầu vào**:
  ```json
  {
    "email": "newuser@example.com",
    "password": "AnotherStrongPassword123!",
    "firstName": "Trần",
    "lastName": "Văn B"
  }
  ```
- **Các bước thực hiện**:
  1. Gửi request `POST` đến `/api/v1/auth/register` với body ở trên.
  2. Xác minh HTTP Status Code trả về là `200 OK`.
  3. Xác minh Response Body:
     - `result` bằng `0` (ERROR).
     - `data` bằng `null`.
     - `message` bằng `"Email already exists"`.
  4. Kiểm tra Database: Đảm bảo không có bản ghi mới nào cho email `newuser@example.com` được tạo thêm.

### TC3: Đăng ký thất bại do Lỗi Xác thực Dữ liệu (Validation Error)
- **Mục tiêu**: Đảm bảo hệ thống kiểm tra và bác bỏ các thông tin đầu vào không hợp lệ hoặc thiếu trường bắt buộc.
- **Kịch bản nhỏ (Sub-cases)**:
  - **TC3.1: Định dạng Email không hợp lệ** (`"email": "invalid-email"`)
  - **TC3.2: Thiếu trường bắt buộc** (ví dụ: không gửi `lastName` hoặc `email`)
  - **TC3.3: Mật khẩu không đáp ứng độ mạnh tối thiểu** (không có ký tự hoa, ký tự đặc biệt hoặc số, hoặc dưới 8 ký tự, ví dụ: `"password": "123"`)

- **Các bước thực hiện**:
  1. Gửi request `POST` đến `/api/v1/auth/register` với thông tin không hợp lệ.
  2. Xác minh HTTP Status Code trả về là `400 Bad Request` (do cơ chế Model Validation của ASP.NET Core tự động kích hoạt nhờ `[ApiController]`).
  3. Xác minh Response Body trả về danh sách các lỗi tương ứng với trường dữ liệu lỗi.

---

## 3. Lệnh kiểm thử nhanh bằng cURL (Quick Testing with cURL)

### Thực thi TC1 (Đăng ký thành công):
```bash
curl -X POST http://localhost:5000/api/v1/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "testuser_"'"$(date +%s)"'@example.com",
    "password": "StrongPassword123!",
    "firstName": "Nguyễn",
    "lastName": "Văn A"
  }'
```

### Thực thi TC2 (Đăng ký trùng Email):
*Chạy lệnh này 2 lần liên tiếp với cùng một email để kiểm chứng.*
```bash
curl -X POST http://localhost:5000/api/v1/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "duplicate@example.com",
    "password": "StrongPassword123!",
    "firstName": "Nguyễn",
    "lastName": "Văn A"
  }'
```

### Thực thi TC3 (Độ mạnh mật khẩu và định dạng email không hợp lệ):
```bash
curl -X POST http://localhost:5000/api/v1/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "bademail",
    "password": "123",
    "firstName": "Nguyễn",
    "lastName": "Văn A"
  }'
```

---

## 4. Xác minh trong Cơ sở Dữ liệu (Database Verification Query)
Có thể kết nối vào SQL Server và chạy câu lệnh sau để kiểm tra kết quả:
```sql
SELECT Id, Email, FirstName, LastName, FullName, Role, CreatedAt 
FROM Users 
WHERE Email = 'newuser@example.com';
```
*Kết quả mong đợi:*
- `FullName` sẽ là `Nguyễn Văn A`.
- `Role` sẽ là `2` (tương ứng với `Role.User`).
- `CreatedAt` ghi nhận thời gian đăng ký (UTC).
