# Kế hoạch Kiểm thử: Danh sách tất cả các khoản vay (Admin - Get All Loans Test Plan)

Tài liệu này xác định kế hoạch kiểm thử, kịch bản kiểm thử (Test Cases), và các lệnh mẫu để xác minh tính năng lấy danh sách toàn bộ khoản vay (Get All Loans - dành cho Admin) trong hệ thống HCRM.

---

## 1. Môi trường & Điều kiện tiên quyết (Environment & Prerequisites)
- **API Endpoint**: `GET http://localhost:5000/api/v1/loan/all-admin`
- **Authentication**: Yêu cầu Token của tài khoản Quản trị viên (Admin/Staff Role).
- **Request Parameters**:
  - `pageNumber` (mặc định: 1)
  - `pageSize` (mặc định: 10)
- **Công cụ kiểm thử khuyên dùng**: `curl`, Postman hoặc Swagger UI (`http://localhost:5000/swagger`)

---

## 2. Kịch bản Kiểm thử Chi tiết (Test Cases)

### TC1: Lấy danh sách thành công khi tài khoản gửi request có quyền Admin/Staff
- **Mục tiêu**: Đảm bảo Admin có thể truy cập thành công API và nhận danh sách tất cả các khoản vay của toàn bộ khách hàng được phân trang.
- **Các bước thực hiện**:
  1. Đăng nhập bằng tài khoản Admin để lấy Access Token.
  2. Gửi request `GET` đến `/api/v1/loan/all-admin?pageNumber=1&pageSize=10` kèm Header `Authorization: Bearer <ADMIN_TOKEN>`.
  3. Xác minh HTTP Status Code trả về là `200 OK`.
  4. Xác minh Response Body:
     - `result` bằng `1` (SUCCESS).
     - `data` chứa các trường metadata phân trang: `pageNumber`, `pageSize`, `totalCount`, `totalPages`, và danh sách `items`.
     - Mỗi item trong `items` chứa thông tin: `id`, `amount`, `term`, `interestRate`, `status`, `total`, `userName`.

### TC2: Lọc phân trang chính xác
- **Mục tiêu**: Đảm bảo tham số phân trang hoạt động chính xác (ví dụ: kích thước trang và số lượng bản ghi tương ứng).
- **Các bước thực hiện**:
  1. Gửi request với `pageSize=2` và `pageNumber=1`.
  2. Xác minh trường `pageSize` trong kết quả là `2`, và số lượng phần tử trong mảng `items` tối đa là 2.

### TC3: Lấy danh sách thất bại do sai quyền truy cập (User thường)
- **Mục tiêu**: Đảm bảo bảo mật, chặn các tài khoản không có quyền Admin/Staff truy cập danh sách khoản vay của người khác.
- **Các bước thực hiện**:
  1. Đăng nhập bằng tài khoản người dùng bình thường để lấy Token.
  2. Gửi request `GET` đến `/api/v1/loan/all-admin`.
  3. Xác minh HTTP Status Code trả về là `403 Forbidden` hoặc `401 Unauthorized` tùy thuộc vào thiết lập Middleware.

---

## 3. Lệnh kiểm thử nhanh bằng cURL (Quick Testing with cURL)

### Thực thi TC1 (Lấy danh sách thành công bằng Token Admin):
```bash
ADMIN_TOKEN=$(curl -s -X POST http://localhost:5000/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "admin@example.com",
    "password": "AdminPassword123!"
  }' | jq -r '.data.accessToken')

curl -X GET "http://localhost:5000/api/v1/loan/all-admin?pageNumber=1&pageSize=5" \
  -H "Authorization: Bearer $ADMIN_TOKEN"
```

### Thực thi TC3 (Người dùng thường truy cập bị từ chối):
```bash
USER_TOKEN=$(curl -s -X POST http://localhost:5000/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "testuser@example.com",
    "password": "StrongPassword123!"
  }' | jq -r '.data.accessToken')

curl -X GET "http://localhost:5000/api/v1/loan/all-admin" \
  -H "Authorization: Bearer $USER_TOKEN"
```
*(Kết quả mong đợi: `403 Forbidden` / `401 Unauthorized`).*
