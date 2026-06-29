# Kế hoạch Kiểm thử: Tạo khoản vay (Create Loan Test Plan)

Tài liệu này xác định kế hoạch kiểm thử, kịch bản kiểm thử (Test Cases), và các lệnh mẫu để xác minh tính năng đăng ký một khoản vay mới (Create Loan) trong hệ thống HCRM.

---

## 1. Môi trường & Điều kiện tiên quyết (Environment & Prerequisites)
- **API Endpoint**: `POST http://localhost:5000/api/v1/loan/create`
- **Authentication**: Yêu cầu xác thực JWT (Bearer Token của tài khoản người dùng bình thường).
- **Điều kiện cần**: Tài khoản người dùng thực hiện yêu cầu.
- **Công cụ kiểm thử khuyên dùng**: `curl`, Postman hoặc Swagger UI (`http://localhost:5000/swagger`)

---

## 2. Kịch bản Kiểm thử Chi tiết (Test Cases)

### TC1: Tạo khoản vay thành công (Có đủ người tham chiếu)
- **Mục tiêu**: Đảm bảo người dùng có đầy đủ thông tin người tham chiếu có thể đăng ký khoản vay mới thành công. Lịch trả nợ (`UserRepayment`) tương ứng phải được tạo tự động.
- **Điều kiện**: User đã thêm người tham chiếu trước đó.
- **Dữ liệu đầu vào**:
  ```json
  {
    "request": {
      "loanAmount": 10000000,
      "loanTerm": 12
    }
  }
  ```
- **Các bước thực hiện**:
  1. Đăng nhập để lấy JWT Token của người dùng hợp lệ.
  2. Gửi request `POST` đến `/api/v1/loan/create` với Token và body ở trên.
  3. Xác minh HTTP Status Code trả về là `200 OK`.
  4. Xác minh Response Body:
     - `result` bằng `1` (SUCCESS).
     - `data` bằng `true`.
     - `message` bằng `"Loan created successfully"`.
  5. Kiểm tra Database:
     - Bảng `Loans`: Có dòng mới chứa số tiền gốc `10000000`, kỳ hạn `12` tháng, trạng thái `0` (`Pending`), lãi suất và tổng tiền được tính toán đúng.
     - Bảng `UserRepayments`: Có chính xác `12` bản ghi tương ứng với kỳ hạn 12 tháng, cột `RepaymentDate` cách nhau lần lượt 1 tháng, trạng thái là `0` (`Pending`).

### TC2: Tạo khoản vay thất bại do chưa khai báo người tham chiếu
- **Mục tiêu**: Đảm bảo hệ thống chặn không cho tạo khoản vay nếu người dùng chưa cung cấp thông tin người tham chiếu.
- **Điều kiện**: Đăng ký một user mới hoàn toàn và chưa thêm bất kỳ người tham chiếu nào.
- **Dữ liệu đầu vào**:
  ```json
  {
    "request": {
      "loanAmount": 10000000,
      "loanTerm": 12
    }
  }
  ```
- **Các bước thực hiện**:
  1. Đăng nhập bằng tài khoản chưa có thông tin tham chiếu để lấy Token.
  2. Gửi request `POST` đến `/api/v1/loan/create` với Token và body ở trên.
  3. Xác minh HTTP Status Code trả về là `200 OK`.
  4. Xác minh Response Body:
     - `result` bằng `0` (ERROR).
     - `data` bằng `false`.
     - `message` bằng `"User have to add at least two references"`.
  5. Kiểm tra Database: Xác minh không có khoản vay mới nào được tạo trong bảng `Loans`.

### TC3: Tạo khoản vay thất bại do Lỗi Xác thực Dữ liệu (Validation Error)
- **Mục tiêu**: Đảm bảo hệ thống từ chối các giá trị tiền hoặc thời gian không hợp lệ.
- **Kịch bản nhỏ (Sub-cases)**:
  - **TC3.1: Số tiền vay <= 0** (`"loanAmount": -100`)
  - **TC3.2: Kỳ hạn vay <= 0 hoặc không có trong danh mục kỳ hạn** (`"loanTerm": 0`)
- **Các bước thực hiện**:
  1. Gửi request `POST` đến `/api/v1/loan/create` kèm Token và body chứa thông tin không hợp lệ.
  2. Xác minh HTTP Status Code trả về là `400 Bad Request`.
  3. Xác minh Response Body chứa thông báo lỗi Model Validation.

---

## 3. Lệnh kiểm thử nhanh bằng cURL (Quick Testing with cURL)

### Thực thi TC1 (Tạo khoản vay thành công):
*Bước 1: Lấy Token của người dùng đã có người tham chiếu:*
```bash
TOKEN=$(curl -s -X POST http://localhost:5000/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "testuser@example.com",
    "password": "StrongPassword123!"
  }' | jq -r '.data.accessToken')
```
*Bước 2: Gọi API tạo khoản vay:*
```bash
curl -X POST http://localhost:5000/api/v1/loan/create \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "request": {
      "loanAmount": 15000000,
      "loanTerm": 6
    }
  }'
```

### Thực thi TC2 (Chưa có người tham chiếu):
*Đăng ký và đăng nhập user mới:*
```bash
EMAIL="newuser_"'"$(date +%s)"'@example.com"
curl -s -X POST http://localhost:5000/api/v1/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "'"$EMAIL"'",
    "password": "StrongPassword123!",
    "firstName": "Nguyễn",
    "lastName": "Văn C"
  }'

NEW_TOKEN=$(curl -s -X POST http://localhost:5000/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "'"$EMAIL"'",
    "password": "StrongPassword123!"
  }' | jq -r '.data.accessToken')

curl -X POST http://localhost:5000/api/v1/loan/create \
  -H "Authorization: Bearer $NEW_TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "request": {
      "loanAmount": 5000000,
      "loanTerm": 12
    }
  }'
```
*(Kết quả mong đợi: Trả về lỗi `"User have to add at least two references"`).*
