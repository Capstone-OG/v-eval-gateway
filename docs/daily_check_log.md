# NHẬT KÝ KIỂM TRẢ TIẾN ĐỘ VẬN HÀNH (DAILY CHECK LOG) - V-EVAL GATEWAY

## [15/09/2026] - Dockerize Gateway & Chuẩn Hóa Docker Compose Multi-Stage Build
- **Dockerfile Multi-Stage .NET 9**:
  - Khởi tạo `Dockerfile` cho Gateway (`V-Eval-Gateway.API`) trên cổng `5212`.
- **Tích Hợp Docker Compose Orchestration (`docker-compose.yml`)**:
  - Định tuyến các container qua mạng nội bộ bridge `veval_network`.
  - Cấu hình script chạy tự động [`run_docker.bat`](file:///e:/CapStone/Scripts/run_docker/run_docker.bat).
  - Kiểm thử cú pháp `docker compose config` đạt 100% thành công (Exit Code 0).

---

## [14/09/2026] - Chuyển Đổi sang Kiến Trúc Modular Feature Folders, Thêm Response Token Warning & Bảo Mật Production
- **Tái Cấu Trúc Kiến Trúc Solution**:
  - Gỡ bỏ hoàn toàn 3 dự án Clean Architecture rỗng (`Domain`, `Application`, `Infrastructure`).
  - Chuyển `V-Eval-Gateway.API` sang mô hình **Feature Folders / Modular Architecture**.
- **Bổ Sung Mô-Đun Bảo Mật & YARP Transforms (`Security/` & `Transforms/`)**:
  - Triển khai [`ClaimsHeaderTransform.cs`](file:///e:/CapStone/All%20Services/V-Eval-Gateway/V-Eval-Gateway.API/Security/ClaimsHeaderTransform.cs): Bóc tách JWT Claims, chống giả mạo Header (`Anti-Header-Spoofing`), tự động đính kèm `X-Token-Refresh-Required: true` khi Token sắp hết hạn trong ngưỡng cấu hình (`RefreshThresholdMinutes`).
  - Triển khai [`GatewayTransformProvider.cs`](file:///e:/CapStone/All%20Services/V-Eval-Gateway/V-Eval-Gateway.API/Transforms/GatewayTransformProvider.cs): Đăng ký tập trung các quy tắc biến đổi Request/Response.
- **Bổ Sung Mô-Đun Chống Spam API (`RateLimiting/`)**:
  - Cấu hình Fixed Window (100 req/min) & Sliding Window (60 req/min) trả về HTTP 429 khi bị tấn công quá tải.
- **Bổ Sung Mô-Đun Giám Sát Sức Khỏe (`Health/`)**:
  - Khởi tạo `DownstreamHealthCheck.cs` đo nhịp tim kết nối của các microservice nội bộ.
- **Chuẩn Hóa Cấu Hình Production & Git Security**:
  - Khởi tạo `appsettings.example.json` chứa các label tham số mẫu.
  - Cập nhật `.gitignore` và untrack các file `appsettings.json` cá nhân chứa thông tin bí mật.
  - Biên dịch giải pháp `dotnet build` đạt **0 Error(s), 0 Warning(s)**.

---

## [12/09/2026] - Khởi Tạo Health Check Endpoint & Correlation ID Middleware
- **Health Check Endpoint**: Cấu hình MapGet(`/healthz`) trả về trạng thái sống của Gateway.
- **Distributed Tracing**: Tích hợp middleware tự động tiêm `X-Correlation-ID` cho mọi HTTP Request.

---

## [11/09/2026] - Ma Trận Định Tuyến YARP Reverse Proxy & CORS Policy
- **Cấu hình Routes & Clusters (`appsettings.json`)**: Định tuyến 4 microservice (`AI Engine`, `Content`, `Identity`, `Practice`).
- **CORS Policy**: Bật chính sách `AllowAll` cho phép Frontend Web/Mobile truy cập xuyên suốt.
