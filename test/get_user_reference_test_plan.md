# Kế hoạch Kiểm thử: Danh sách người tham chiếu (Get User References Test Plan)

Tài liệu này xác định kế hoạch kiểm thử, kịch bản kiểm thử (Test Cases), và các lệnh mẫu để xác minh tính năng lấy danh sách người tham chiếu (Get User References) trong hệ thống HCRM.

---

## 1. Môi trường & Điều kiện tiên quyết (Environment & Prerequisites)
- **API Endpoint**: `GET http://localhost:5000/api/v1/user-reference/get-all`
- **Authentication**: Yêu cầu xác thực JWT (Bearer Token của người dùng).
- **Request Parameters**:
  - `pageNumber` (mặc định: 1)
  - `pageSize` (mặc định: 10)
- **Công cụ kiểm thử khuyên dùng**: `curl`, Postman hoặc Swagger UI (`http://localhost:5000/swagger`)

---

## 2. Kịch bản Kiểm thử Chi tiết (Test Cases)

### TC1: Lấy danh sách thành công (Người dùng đã có người tham chiếu)
- **Mục tiêu**: Đảm bảo người dùng xem được danh sách những người tham chiếu của riêng họ dưới dạng phân trang.
- **Các bước thực hiện**:
  1. Đăng nhập bằng tài khoản người dùng đã tạo thông tin tham chiếu để lấy Access Token.
  2. Gửi request `GET` đến `/api/v1/user-reference/get-all?pageNumber=1&pageSize=10` kèm Header `Authorization: Bearer <TOKEN>`.
  3. Xác minh HTTP Status Code trả về là `200 OK`.
  4. Xác minh Response Body:
     - `result` bằng `1` (SUCCESS).
     - `data` chứa thông tin phân trang và danh sách `items`. Mỗi phần tử trong `items` chứa: `id`, `fullName`, `phoneNumber`, `relationship`.
     - `message` bằng `"Success"`.

### TC2: Trả về danh sách rỗng thành công (Chưa khai báo người tham chiếu nào)
- **Mục tiêu**: Đảm bảo hệ thống trả về danh sách trống thành công khi tài khoản chưa từng khai báo người tham chiếu.
- **Các bước thực hiện**:
  1. Đăng nhập bằng tài khoản mới đăng ký (hoặc chưa từng tạo người tham chiếu) để lấy Token.
  2. Gửi request `GET` đến `/api/v1/user-reference/get-all`.
  3. Xác minh HTTP Status Code trả về là `200 OK`.
  4. Xác minh Response Body:
     - `result` bằng `1` (SUCCESS).
     - `data.items` là danh sách rỗng (`[]`).
     - `data.totalCount` bằng `0`.

### TC3: Lấy danh sách thất bại do không có Token xác thực
- **Mục tiêu**: Đảm bảo hệ thống bảo vệ thông tin cá nhân của người tham chiếu khỏi các yêu cầu không xác thực.
- **Các bước thực hiện**:
  1. Gửi request `GET` đến `/api/v1/user-reference/get-all` mà không truyền kèm Header `Authorization`.
  2. Xác minh HTTP Status Code trả về là `401 Unauthorized`.

---

## 3. Lệnh kiểm thử nhanh bằng cURL (Quick Testing with cURL)

### Thực thi TC1 (Lấy danh sách thành công):
```bash
TOKEN=$(curl -s -X POST http://localhost:5000/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "testuser@example.com",
    "password": "StrongPassword123!"
  }' | jq -r '.data.accessToken')

curl -X GET "http://localhost:5000/api/v1/user-reference/get-all?pageNumber=1&pageSize=5" \
  -H "Authorization: Bearer $TOKEN"
```

### Thực thi TC2 (Chưa khai báo người tham chiếu):
```bash
# Đăng ký tài khoản mới và đăng nhập
EMAIL="empty_ref_"'"$(date +%s)"'@example.com"
curl -s -X POST http://localhost:5000/api/v1/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "'"$EMAIL"'",
    "password": "StrongPassword123!",
    "firstName": "Nguyễn",
    "lastName": "Văn E"
  }'

EMPTY_USER_TOKEN=$(curl -s -X POST http://localhost:5000/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "'"$EMAIL"'",
    "password": "StrongPassword123!"
  }' | jq -r '.data.accessToken')

curl -X GET http://localhost:5000/api/v1/user-reference/get-all \
  -H "Authorization: Bearer $EMPTY_USER_TOKEN"
```
*(Kết quả mong đợi: `result: 1` và `totalCount: 0`).*
