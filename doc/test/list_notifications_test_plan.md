# Kế hoạch Kiểm thử: Danh sách thông báo (List Notifications Test Plan)

Tài liệu này xác định kế hoạch kiểm thử, kịch bản kiểm thử (Test Cases), và các lệnh mẫu để xác minh tính năng lấy danh sách thông báo cá nhân (List Notifications) trong hệ thống HCRM.

---

## 1. Môi trường & Điều kiện tiên quyết (Environment & Prerequisites)
- **API Endpoint**: `GET http://localhost:5000/api/v1/notification/list`
- **Authentication**: Yêu cầu xác thực JWT (Bearer Token của người dùng).
- **Request Parameters**:
  - `pageNumber` (mặc định: 1)
  - `pageSize` (mặc định: 10)
- **Công cụ kiểm thử khuyên dùng**: `curl`, Postman hoặc Swagger UI (`http://localhost:5000/swagger`)

---

## 2. Kịch bản Kiểm thử Chi tiết (Test Cases)

### TC1: Lấy danh sách thông báo thành công (Có thông báo)
- **Mục tiêu**: Đảm bảo người dùng có thông báo nhận được đúng danh sách thông báo của riêng họ dưới dạng phân trang.
- **Các bước thực hiện**:
  1. Đăng nhập bằng tài khoản người dùng đã có thông báo trong hệ thống để lấy Access Token.
  2. Gửi request `GET` đến `/api/v1/notification/list?pageNumber=1&pageSize=10` kèm Header `Authorization: Bearer <TOKEN>`.
  3. Xác minh HTTP Status Code trả về là `200 OK`.
  4. Xác minh Response Body:
     - `result` bằng `1` (SUCCESS).
     - `data` chứa danh sách các thông báo của riêng người dùng đó (mỗi phần tử có: `id`, `title`, `content`, `type`, `createdDate`).
     - `message` bằng `"Notifications fetched successfully"`.

### TC2: Trả về danh sách rỗng thành công (Người dùng không có thông báo nào)
- **Mục tiêu**: Đảm bảo hệ thống trả về mảng rỗng thành công đối với tài khoản không có bất kỳ thông báo nào.
- **Các bước thực hiện**:
  1. Đăng nhập bằng một tài khoản mới đăng ký để lấy Token.
  2. Gửi request `GET` đến `/api/v1/notification/list`.
  3. Xác minh HTTP Status Code trả về là `200 OK`.
  4. Xác minh Response Body:
     - `result` bằng `1` (SUCCESS).
     - `data` bằng danh sách rỗng (`[]`).

### TC3: Lấy danh sách thất bại do thiếu xác thực
- **Mục tiêu**: Đảm bảo thông tin thông báo cá nhân không thể bị rò rỉ nếu không có Token hợp lệ.
- **Các bước thực hiện**:
  1. Gửi request `GET` đến `/api/v1/notification/list` không có Header `Authorization`.
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

curl -X GET "http://localhost:5000/api/v1/notification/list?pageNumber=1&pageSize=5" \
  -H "Authorization: Bearer $TOKEN"
```

### Thực thi TC2 (Người dùng không có thông báo):
```bash
# Đăng ký và đăng nhập tài khoản mới
EMAIL="new_notif_user_"'"$(date +%s)"'@example.com"
curl -s -X POST http://localhost:5000/api/v1/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "'"$EMAIL"'",
    "password": "StrongPassword123!",
    "firstName": "Nguyễn",
    "lastName": "Văn F"
  }'

NEW_USER_TOKEN=$(curl -s -X POST http://localhost:5000/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "'"$EMAIL"'",
    "password": "StrongPassword123!"
  }' | jq -r '.data.accessToken')

curl -X GET http://localhost:5000/api/v1/notification/list \
  -H "Authorization: Bearer $NEW_USER_TOKEN"
```
*(Kết quả mong đợi: `result: 1` và `data: []`).*
