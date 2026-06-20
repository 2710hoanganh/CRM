# Kế hoạch Kiểm thử: Chi tiết khoản vay (Get Loan Info Test Plan)

Tài liệu này xác định kế hoạch kiểm thử, kịch bản kiểm thử (Test Cases), và các lệnh mẫu để xác minh tính năng lấy thông tin chi tiết một khoản vay cụ thể (Get Loan Info) trong hệ thống HCRM.

---

## 1. Môi trường & Điều kiện tiên quyết (Environment & Prerequisites)
- **API Endpoint**: `GET http://localhost:5000/api/v1/loan/info?id=<loan_id>`
- **Authentication**: Yêu cầu xác thực JWT (Bearer Token).
- **Điều kiện cần**:
  - Đã có ít nhất một khoản vay trong cơ sở dữ liệu để test (ví dụ với `Id = 1`).
- **Công cụ kiểm thử khuyên dùng**: `curl`, Postman hoặc Swagger UI (`http://localhost:5000/swagger`)

---

## 2. Kịch bản Kiểm thử Chi tiết (Test Cases)

### TC1: Lấy chi tiết khoản vay thành công (ID hợp lệ)
- **Mục tiêu**: Đảm bảo người dùng đã đăng nhập có thể xem chi tiết của một khoản vay đang tồn tại qua ID.
- **Dữ liệu đầu vào**: ID khoản vay hợp lệ (Ví dụ: `id = 1`) và Token xác thực.
- **Các bước thực hiện**:
  1. Gửi request `GET` đến `/api/v1/loan/info?id=1` kèm theo header `Authorization: Bearer <TOKEN>`.
  2. Xác minh HTTP Status Code trả về là `200 OK`.
  3. Xác minh Response Body:
     - `result` bằng `1` (SUCCESS).
     - `data` không null và chứa các trường thông tin: `id`, `amount`, `term`, `rate`, `interestRate`, `status`, `total`, `paybackAmount`, `feedBack`, `createdDate`.
     - `message` bằng `"Loan info found"`.

### TC2: Lấy chi tiết khoản vay thất bại do ID không tồn tại
- **Mục tiêu**: Đảm bảo hệ thống trả về thông báo lỗi thích hợp khi ID khoản vay không tồn tại trong hệ thống.
- **Dữ liệu đầu vào**: ID khoản vay giả định không tồn tại (Ví dụ: `id = 999999`).
- **Các bước thực hiện**:
  1. Gửi request `GET` đến `/api/v1/loan/info?id=999999` kèm theo Token.
  2. Xác minh HTTP Status Code trả về là `200 OK`.
  3. Xác minh Response Body:
     - `result` bằng `0` (ERROR).
     - `data` bằng `null`.
     - `message` bằng `"Loan not found"`.

### TC3: Lấy chi tiết khoản vay thất bại do thiếu hoặc sai định dạng Token
- **Mục tiêu**: Đảm bảo hệ thống bảo mật không cho phép người dùng chưa xác thực truy cập API.
- **Các bước thực hiện**:
  1. Gửi request `GET` đến `/api/v1/loan/info?id=1` mà không đính kèm Header `Authorization` hoặc token sai định dạng.
  2. Xác minh HTTP Status Code trả về là `401 Unauthorized`.

---

## 3. Lệnh kiểm thử nhanh bằng cURL (Quick Testing with cURL)

### Thực thi TC1 (Lấy chi tiết thành công):
*Bước 1: Lấy token người dùng:*
```bash
TOKEN=$(curl -s -X POST http://localhost:5000/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "testuser@example.com",
    "password": "StrongPassword123!"
  }' | jq -r '.data.accessToken')
```
*Bước 2: Gửi request lấy chi tiết:*
```bash
curl -X GET http://localhost:5000/api/v1/loan/info?id=1 \
  -H "Authorization: Bearer $TOKEN"
```

### Thực thi TC2 (ID không tồn tại):
```bash
curl -X GET http://localhost:5000/api/v1/loan/info?id=999999 \
  -H "Authorization: Bearer $TOKEN"
```

### Thực thi TC3 (Không gửi Token):
```bash
curl -X GET http://localhost:5000/api/v1/loan/info?id=1
```
