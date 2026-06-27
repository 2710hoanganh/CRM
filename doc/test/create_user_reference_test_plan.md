# Kế hoạch Kiểm thử: Tạo người tham chiếu (Create User Reference Test Plan)

Tài liệu này xác định kế hoạch kiểm thử, kịch bản kiểm thử (Test Cases), và các lệnh mẫu để xác minh tính năng tạo danh sách người tham chiếu (Create User Reference) trong hệ thống HCRM.

---

## 1. Môi trường & Điều kiện tiên quyết (Environment & Prerequisites)
- **API Endpoint**: `POST http://localhost:5000/api/v1/user-reference/create`
- **Authentication**: Yêu cầu xác thực JWT (Bearer Token của người dùng).
- **Công cụ kiểm thử khuyên dùng**: `curl`, Postman hoặc Swagger UI (`http://localhost:5000/swagger`)

---

## 2. Kịch bản Kiểm thử Chi tiết (Test Cases)

### TC1: Tạo thông tin tham chiếu thành công (Thông tin hợp lệ)
- **Mục tiêu**: Đảm bảo người dùng có thể gửi danh sách thông tin người tham chiếu hợp lệ và hệ thống lưu vào DB thành công.
- **Dữ liệu đầu vào**:
  ```json
  {
    "requests": [
      {
        "fullName": "Nguyễn Văn B",
        "phoneNumber": "0987654321",
        "relationship": 1
      },
      {
        "fullName": "Trần Thị C",
        "phoneNumber": "0912345678",
        "relationship": 2
      }
    ]
  }
  ```
- **Các bước thực hiện**:
  1. Đăng nhập để lấy Access Token của người dùng.
  2. Gửi request `POST` đến `/api/v1/user-reference/create` kèm Token và body ở trên.
  3. Xác minh HTTP Status Code trả về là `200 OK`.
  4. Xác minh Response Body:
     - `result` bằng `1` (SUCCESS).
     - `data` bằng `true`.
     - `message` bằng `"User references created successfully"`.
  5. Kiểm tra Database:
     - Truy vấn bảng `UserReferences` lọc theo `UserId` của tài khoản thực hiện yêu cầu, đảm bảo cả 2 bản ghi trên được lưu chính xác (họ tên, số điện thoại, mối quan hệ).

### TC2: Tạo thông tin tham chiếu thất bại do lỗi xác thực dữ liệu (Thiếu trường bắt buộc)
- **Mục tiêu**: Đảm bảo hệ thống bắt lỗi và chặn việc nhập thiếu thông tin bắt buộc của người tham chiếu.
- **Kịch bản nhỏ (Sub-cases)**:
  - **TC2.1: Thiếu Họ tên** (`"fullName": ""`)
  - **TC2.2: Thiếu Số điện thoại** (`"phoneNumber": ""`)
- **Các bước thực hiện**:
  1. Gửi request `POST` đến `/api/v1/user-reference/create` với dữ liệu không hợp lệ.
  2. Xác minh HTTP Status Code trả về là `400 Bad Request`.
  3. Xác minh Response Body chứa thông báo lỗi Model Validation.

### TC3: Tạo thông tin tham chiếu thất bại do không có Token xác thực
- **Mục tiêu**: Bảo vệ API tránh khỏi việc chèn dữ liệu không xác thực.
- **Các bước thực hiện**:
  1. Gửi request `POST` đến `/api/v1/user-reference/create` mà không truyền kèm Header `Authorization`.
  2. Xác minh HTTP Status Code trả về là `401 Unauthorized`.

---

## 3. Lệnh kiểm thử nhanh bằng cURL (Quick Testing with cURL)

### Thực thi TC1 (Tạo thành công):
```bash
TOKEN=$(curl -s -X POST http://localhost:5000/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "testuser@example.com",
    "password": "StrongPassword123!"
  }' | jq -r '.data.accessToken')

curl -X POST http://localhost:5000/api/v1/user-reference/create \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "requests": [
      {
        "fullName": "Nguyễn Văn B",
        "phoneNumber": "0987654321",
        "relationship": 1
      },
      {
        "fullName": "Trần Thị C",
        "phoneNumber": "0912345678",
        "relationship": 2
      }
    ]
  }'
```

### Thực thi TC2 (Thiếu số điện thoại):
```bash
curl -X POST http://localhost:5000/api/v1/user-reference/create \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "requests": [
      {
        "fullName": "Nguyễn Văn B",
        "phoneNumber": "",
        "relationship": 1
      }
    ]
  }'
```

### Thực thi TC3 (Không gửi Token):
```bash
curl -X POST http://localhost:5000/api/v1/user-reference/create \
  -H "Content-Type: application/json" \
  -d '{
    "requests": [
      {
        "fullName": "Nguyễn Văn B",
        "phoneNumber": "0987654321",
        "relationship": 1
      }
    ]
  }'
```
*(Kết quả mong đợi: `401 Unauthorized`).*
