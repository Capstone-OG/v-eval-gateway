# Nhật Ký Cập Nhật (Update Log) - V-Eval Gateway Service

## [30/09/2026] - Mở Rộng Định Tuyến Trực Tiếp Cho Phân Hệ Xác Thực & Quản Lý Người Dùng (Identity Service)

- **Mở Rộng YARP Route Reverse Proxy (`appsettings.json`, `appsettings.example.json`)**:
  - Bổ sung các Route trực tiếp trỏ về `identity-service-cluster` (`http://localhost:5155`):
    - `identity-auth-route`: `/api/auth/{**catch-all}`
    - `identity-auth-v1-route`: `/api/v1/auth/{**catch-all}`
    - `identity-users-v1-route`: `/api/v1/users/{**catch-all}`
    - `identity-campuses-v1-route`: `/api/v1/campuses/{**catch-all}`
    - `identity-students-v1-route`: `/api/v1/students/{**catch-all}`
  - Cho phép Web Client và các client bên ngoài gọi tự nhiên các endpoint Auth/User mà không bắt buộc phải có tiền tố `/api/v1/identity`.
- **Kiểm Thử Biên Dịch**:
  - Solution `V-Eval-Gateway.sln` biên dịch sạch 100% (**0 Error, 0 Warning**).
