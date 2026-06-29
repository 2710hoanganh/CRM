# Kế hoạch Kiểm thử: Danh sách khoản vay cá nhân (Get User Loans Test Plan)

Tài liệu này xác định kế hoạch kiểm thử, kịch bản kiểm thử (Test Cases), và các lệnh mẫu để xác minh tính năng lấy danh sách khoản vay của riêng người dùng hiện tại (Get User Loans) trong hệ thống HCRM.

---

## 1. Môi trường & Điều kiện tiên quyết (Environment & Prerequisites)
- **API Endpoint**: `GET http://localhost:5000/api/v1/loan/all-user`
- **Authentication**: Yêu cầu xác thực JWT (Bearer Token của người dùng thông thường).
- **Request Parameters**:
  - `pageNumber` (mặc định: 1)
  - `pageSize` (mặc định: 10)
- **Công cụ kiểm thử khuyên dùng**: `curl`, Postman hoặc Swagger UI (`http://localhost:5000/swagger`)

---

## 2. Kịch bản Kiểm thử Chi tiết (Test Cases)

### TC1: Lấy danh sách thành công (Có khoản vay)
- **Mục tiêu**: Đảm bảo người dùng đăng nhập có thể lấy được danh sách khoản vay thuộc sở hữu của riêng họ, hiển thị dưới dạng phân trang.
- **Các bước thực hiện**:
  1. Đăng nhập bằng tài khoản người dùng đã có khoản vay trong DB để lấy Access Token.
  2. Gửi request `GET` đến `/api/v1/loan/all-user?pageNumber=1&pageSize=10` kèm Header `Authorization: Bearer <USER_TOKEN>`.
  3. Xác minh HTTP Status Code trả về là `200 OK`.
  4. Xác minh Response Body:
     - `result` bằng `1` (SUCCESS).
     - `data` chứa thông tin phân trang và danh sách `items`. Mỗi item trong `items` phải hiển thị chính xác tên của người dùng (`userName`) và thông tin khoản vay của họ.
     - Đảm bảo danh sách chỉ trả về các khoản vay thuộc sở hữu của người dùng hiện tại (lọc theo `UserId`).

### TC2: Trả về danh sách rỗng thành công (Không có khoản vay nào)
- **Mục tiêu**: Đảm bảo hệ thống trả về kết quả rỗng thành công (không báo lỗi) đối với tài khoản chưa từng đăng ký khoản vay.
- **Điều kiện**: Dùng một tài khoản mới đăng ký và chưa tạo khoản vay nào.
- **Các bước thực hiện**:
  1. Đăng nhập bằng tài khoản mới để lấy Token.
  2. Gửi request `GET` đến `/api/v1/loan/all-user`.
  3. Xác minh HTTP Status Code trả về là `200 OK`.
  4. Xác minh Response Body:
     - `result` bằng `1` (SUCCESS).
     - `data.items` là danh sách rỗng (`[]`).
     - `data.totalCount` bằng `0`.

### TC3: Lọc phân trang hoạt động đúng
- **Mục tiêu**: Xác minh tham số `pageSize` và `pageNumber` hoạt động đúng cho danh sách khoản vay của người dùng.
- **Các bước thực hiện**:
  1. Gửi yêu cầu với `pageSize=1`.
  2. Xác minh số lượng khoản vay trong dữ liệu trả về tối đa là 1.

---

## 3. Lệnh kiểm thử nhanh bằng cURL (Quick Testing with cURL)

### Thực thi TC1 (Lấy danh sách thành công):
```bash
USER_TOKEN=$(curl -s -X POST http://localhost:5000/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "testuser@example.com",
    "password": "StrongPassword123!"
  }' | jq -r '.data.accessToken')

curl -X GET "http://localhost:5000/api/v1/loan/all-user?pageNumber=1&pageSize=5" \
  -H "Authorization: Bearer $USER_TOKEN"
```

### Thực thi TC2 (Tài khoản chưa có khoản vay):
```bash
# Đăng ký tài khoản mới và đăng nhập
EMAIL="empty_loans_"'"$(date +%s)"'@example.com"
curl -s -X POST http://localhost:5000/api/v1/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "'"$EMAIL"'",
    "password": "StrongPassword123!",
    "firstName": "Nguyễn",
    "lastName": "Văn D"
  }'

EMPTY_USER_TOKEN=$(curl -s -X POST http://localhost:5000/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "'"$EMAIL"'",
    "password": "StrongPassword123!"
  }' | jq -r '.data.accessToken')

curl -X GET http://localhost:5000/api/v1/loan/all-user \
  -H "Authorization: Bearer $EMPTY_USER_TOKEN"
```
*(Kết quả mong đợi: Trả về thành công với `items: []` và `totalCount: 0`).*
