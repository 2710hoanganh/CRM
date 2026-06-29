# Kế hoạch Kiểm thử: Lịch trả nợ theo khoản vay (Get Loan Repayment Dates Test Plan)

Tài liệu này xác định kế hoạch kiểm thử, kịch bản kiểm thử (Test Cases), và các lệnh mẫu để xác minh tính năng lấy chi tiết lịch trình trả nợ (Get Loan Repayment Dates) trong hệ thống HCRM.

---

## 1. Môi trường & Điều kiện tiên quyết (Environment & Prerequisites)
- **API Endpoint**: `GET http://localhost:5000/api/v1/loan/repayment?id=<loan_id>`
- **Authentication**: Yêu cầu xác thực JWT (Bearer Token).
- **Điều kiện cần**:
  - Đã có ít nhất một khoản vay trong cơ sở dữ liệu đã được sinh lịch trả nợ (bảng `UserRepayments` có chứa các kỳ trả nợ cho khoản vay này).
- **Công cụ kiểm thử khuyên dùng**: `curl`, Postman hoặc Swagger UI (`http://localhost:5000/swagger`)

---

## 2. Kịch bản Kiểm thử Chi tiết (Test Cases)

### TC1: Lấy lịch trả nợ thành công (ID khoản vay hợp lệ)
- **Mục tiêu**: Đảm bảo người dùng xem được đầy đủ danh sách các kỳ trả nợ sắp xếp theo thứ tự giảm dần của ngày trả nợ (như được định nghĩa trong quy trình nghiệp vụ).
- **Dữ liệu đầu vào**: ID khoản vay đang có lịch trả nợ (Ví dụ: `id = 1`) và Token hợp lệ.
- **Các bước thực hiện**:
  1. Gửi request `GET` đến `/api/v1/loan/repayment?id=1` kèm Header `Authorization: Bearer <TOKEN>`.
  2. Xác minh HTTP Status Code trả về là `200 OK`.
  3. Xác minh Response Body:
     - `result` bằng `1` (SUCCESS).
     - `data` chứa số tiền gốc khoản vay (`amount`) và danh sách `repaymentDates`.
     - Số lượng phần tử trong mảng `repaymentDates` bằng đúng kỳ hạn của khoản vay (ví dụ: 12 kỳ đối với kỳ hạn 12 tháng).
     - Các kỳ được sắp xếp giảm dần theo ngày thanh toán (`RepaymentDate`).
     - `message` bằng `"Loan repayment dates retrieved successfully"`.

### TC2: Lấy lịch trả nợ thất bại do ID khoản vay không tồn tại
- **Mục tiêu**: Đảm bảo hệ thống báo lỗi khi xem lịch trả nợ của ID khoản vay không có thật.
- **Dữ liệu đầu vào**: ID không tồn tại (Ví dụ: `id = 999999`).
- **Các bước thực hiện**:
  1. Gửi request `GET` đến `/api/v1/loan/repayment?id=999999` kèm Token.
  2. Xác minh HTTP Status Code trả về là `200 OK`.
  3. Xác minh Response Body:
     - `result` bằng `0` (ERROR hoặc NOT_FOUND).
     - `data` bằng `null`.
     - `message` bằng `"Loan not found"`.

### TC3: Lấy lịch trả nợ thất bại do thiếu xác thực
- **Mục tiêu**: Bảo vệ thông tin tài chính người dùng khỏi việc bị xem lén bởi khách truy cập không đăng nhập.
- **Các bước thực hiện**:
  1. Gửi request `GET` đến `/api/v1/loan/repayment?id=1` không kèm theo Header `Authorization`.
  2. Xác minh HTTP Status Code trả về là `401 Unauthorized`.

---

## 3. Lệnh kiểm thử nhanh bằng cURL (Quick Testing with cURL)

### Thực thi TC1 (Lấy lịch trả nợ thành công):
```bash
TOKEN=$(curl -s -X POST http://localhost:5000/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "email": "testuser@example.com",
    "password": "StrongPassword123!"
  }' | jq -r '.data.accessToken')

curl -X GET "http://localhost:5000/api/v1/loan/repayment?id=1" \
  -H "Authorization: Bearer $TOKEN"
```

### Thực thi TC2 (Khoản vay không tồn tại):
```bash
curl -X GET "http://localhost:5000/api/v1/loan/repayment?id=999999" \
  -H "Authorization: Bearer $TOKEN"
```

### Thực thi TC3 (Không gửi Token):
```bash
curl -X GET http://localhost:5000/api/v1/loan/repayment?id=1
```
