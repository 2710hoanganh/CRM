# Kế hoạch Triển khai: Lấy danh sách thông báo (List Notifications)

## 1. Tổng quan
Tính năng cho phép người dùng xem danh sách các thông báo cá nhân được hệ thống gửi (ví dụ: thông báo nhắc nợ, phê duyệt khoản vay, v.v.).

## 2. API Endpoint Specification
- **Method**: `GET`
- **URL**: `/api/v1/notification/list`
- **Authentication**: Yêu cầu xác thực JWT (User Role)
- **Request Parameters**:
  - `pageNumber` (ulong, default: 0)
  - `pageSize` (ulong, default: 20)
- **Response Body**: `Response<List<NotficationList>>`
  - Thành công:
    ```json
    {
      "result": 1,
      "data": [
        {
          "id": 1,
          "title": "Nhắc nợ khoản vay",
          "content": "Khoản vay của bạn sắp đến hạn thanh toán trong 3 ngày tới.",
          "type": 1,
          "status": 0,
          "isRead": false,
          "createdAt": "2026-06-20T19:12:00"
        }
      ],
      "message": "Notifications fetched successfully"
    }
    ```

## 3. Các thực thể Database liên quan (Database Entities)
- **Notification**: Lưu các thông báo của người dùng.
  - `Id`, `UserId`, `Title`, `Content`, `Type`, `CreatedDate`.

## 4. Quy trình xử lý & Nghiệp vụ (Business Logic & Workflow)
1. Người dùng gửi yêu cầu lấy thông tin thông báo qua `/api/v1/notification/list`.
2. Controller trích xuất `UserId` từ Token, gán vào `Id` của `query` và gửi `ListNoitifiactionQuery` qua MediatR.
3. Trong Handler (`ListNoitifiactionQueryHandler`):
   - Gọi `INotificationRepository.GetPagination` với điều kiện lọc `x => x.UserId == request.Id`.
   - Sử dụng selector `x => _autoMapper.Map<NotficationList>(x)` để map sang `NotficationList` DTO.
   - Trả về danh sách thông báo của người dùng kèm thông điệp kết quả.

## 5. Cấu trúc mã nguồn chi tiết
- **Presentation Layer**:
  - [NotifcationController.cs](file:///d:/CRM/Presentation/Controllers/NotifcationController.cs)
- **Application Layer**:
  - [ListNoitifiaction.cs](file:///d:/CRM/Application/Features/Notification/Query/ListNoitifiaction.cs)
  - `NotficationList` DTO trong [Domain/Models/DTO/Notification](file:///d:/CRM/Domain/Models/DTO/Notification)
  - Interface: `INotificationRepository`
- **Persistence Layer**:
  - [NotificationRepository.cs](file:///d:/CRM/Persistence/Repositories/NotificationRepository.cs)

## 6. Kịch bản Kiểm thử (Test Cases)
- **TC1: Lấy danh sách thông báo thành công** trả về các thông báo của chính người dùng hiện tại đang đăng nhập.
- **TC2: Trả về danh sách trống** (thành công) nếu người dùng không có thông báo nào.
- **TC3: Kiểm tra phân quyền** (yêu cầu Bearer Token phải hợp lệ).
