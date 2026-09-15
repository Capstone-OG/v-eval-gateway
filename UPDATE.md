# Nhật Ký Cập Nhật - V-Eval Gateway Service

## [15/09/2026] - Dockerize Gateway & Chuẩn Hóa Docker Compose Multi-Stage Build
- **Tạo Dockerfile Multi-Stage .NET 9**:
  - Khởi tạo `Dockerfile` chuẩn cho Gateway (`V-Eval-Gateway.API`) với cổng `5212`.
  - Sử dụng mcr `dotnet/sdk:9.0` cho build/publish và `dotnet/aspnet:9.0` cho nhẹ runtime.
- **Tích Hợp Docker Compose Orchestration (`docker-compose.yml`)**:
  - Định tuyến các container qua mạng nội bộ bridge `veval_network`.
  - Kiểm thử cú pháp `docker compose config` đạt 100% thành công.

## [14/09/2026] - Chuyển Đổi sang Kiến Trúc Modular Feature Folders, Security Claims Transformer & Production Config
- **Tái Cấu Trúc Kiến Trúc**:
  - Gỡ bỏ 3 tầng dự án Clean Architecture dư thừa (`Domain`, `Application`, `Infrastructure`).
  - Đưa `V-Eval-Gateway.API` về mô hình **Feature Folders / Modular Architecture**.
- **Xây Dựng Mô-Đun Xử Lý Cắt Ngang**:
  - `Middlewares/`: Triển khai `CorrelationIdMiddleware.cs`, `GlobalExceptionMiddleware.cs`, `RequestLoggingMiddleware.cs`.
  - `Security/`: Triển khai `ClaimsHeaderTransform.cs` (Anti-Header-Spoofing, trích xuất JWT Claims, tự động gửi `X-Token-Refresh-Required` khi Token còn `< 5 phút`) và `GatewayAuthExtensions.cs`.
  - `Transforms/`: Triển khai `GatewayTransformProvider.cs` đăng ký quy tắc YARP Transforms tập trung.
  - `RateLimiting/`: Triển khai `RateLimiterExtensions.cs` chống Spam API & DDoS (Fixed Window 100 req/min, Sliding Window 60 req/min).
  - `Health/`: Triển khai `DownstreamHealthCheck.cs` kiểm tra sức khỏe các microservice nội bộ.
- **Cấu Hình Môi Trường & Git Security**:
  - Khởi tạo `appsettings.example.json` chứa các label cấu hình mẫu cho Production.
  - Cập nhật `.gitignore` ẩn các file `appsettings.json` bí mật trên máy local.
  - Hoàn thiện tài liệu kiến trúc & nghiệm thu trong thư mục [`docs/`](file:///e:/CapStone/All%20Services/V-Eval-Gateway/docs).
