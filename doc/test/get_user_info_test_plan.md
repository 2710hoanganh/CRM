# Kế hoạch Kiểm thử: Lấy thông tin người dùng (Get User Info Test Plan)

Tài liệu này xác định kế hoạch kiểm thử, kịch bản kiểm thử (Test Cases), và các lệnh mẫu để xác minh tính năng lấy thông tin tài khoản người dùng hiện tại (Get User Info) trong hệ thống HCRM.

---

## 1. Môi trường & Điều kiện tiên quyết (Environment & Prerequisites)
- **API Endpoint**: `GET http://localhost:5000/api/v1/account/info`
- **Authentication**: Yêu cầu JWT Access Token hợp lệ (Bearer Token) trong Header `Authorization: Bearer <token>`.
- **Dữ liệu kiểm thử tiên quyết**: Cần đăng nhập thành công trước để có JWT Access Token.
- **Công cụ kiểm thử khuyên dùng**: `curl`, Postman hoặc Swagger UI (`http://localhost:5000/swagger`)

---

## 2. Kịch bản Kiểm thử Chi tiết (Test Cases)

### TC1: Lấy thông tin tài khoản thành công (Token hợp lệ)
- **Mục tiêu**: Đảm bảo người dùng đã xác thực lấy được thông tin chi tiết tài khoản của chính mình.
- **Dữ liệu đầu vào**: JWT Access Token hợp lệ lấy được từ API Login.
- **Các bước thực hiện**:
  1. Gửi request `GET` đến `/api/v1/account/info` kèm theo header `Authorization: Bearer <JWT_ACCESS_TOKEN>`.
  2. Xác minh HTTP Status Code trả về là `200 OK`.
  3. Xác minh Response Body:
     - `result` bằng `1` (SUCCESS).
     - `data` chứa thông tin khớp với token của người dùng đăng nhập gồm: `id`, `email`, `fullName`, và `role`.
     - `message` bằng `"Get user info successful"`.

### TC2: Lấy thông tin thất bại do thiếu Token (Chưa đăng nhập)
- **Mục tiêu**: Đảm bảo hệ thống chặn truy cập trái phép khi không cung cấp JWT Token.
- **Các bước thực hiện**:
  1. Gửi request `GET` đến `/api/v1/account/info` mà không gửi kèm Header `Authorization`.
  2. Xác minh HTTP Status Code trả về là `401 Unauthorized` (do ASP.NET Core Middleware chặn tự động).

### TC3: Lấy thông tin thất bại do Token không hợp lệ hoặc hết hạn
- **Mục tiêu**: Đảm bảo hệ thống chặn các token giả mạo, sai định dạng hoặc đã hết hạn.
- **Dữ liệu đầu vào**: Gửi kèm Header `Authorization: Bearer invalid_token_value`.
- **Các bước thực hiện**:
  1. Gửi request `GET` đến `/api/v1/account/info` với token sai.
  2. Xác minh HTTP Status Code trả về là `401 Unauthorized`.

---

## 3. Lệnh kiểm thử nhanh bằng cURL (Quick Testing with cURL)

### Thực thi TC1 (Lấy thông tin thành công):
*Bước 1: Đăng nhập để lấy Access Token:*
```bash
TOKEN=$(curl -s -X POST http://localhost:5000/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "testuser@example.com",
    "password": "StrongPassword123!"
  }' | jq -r '.data.accessToken')
```
*Bước 2: Lấy thông tin tài khoản:*
```bash
curl -X GET http://localhost:5000/api/v1/account/info \
  -H "Authorization: Bearer $TOKEN"
```

### Thực thi TC2 (Thiếu Token):
```bash
curl -X GET http://localhost:5000/api/v1/account/info
```

### Thực thi TC3 (Token không hợp lệ):
```bash
curl -X GET http://localhost:5000/api/v1/account/info \
  -H "Authorization: Bearer invalidtoken123"
```
