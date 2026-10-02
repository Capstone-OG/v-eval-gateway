# NHẬT KÝ KIỂM TRẢ TIẾN ĐỘ VẬN HÀNH (DAILY CHECK LOG) - V-EVAL GATEWAY

## [02/10/2026] - Mở Rộng YARP Reverse Proxy Định Tuyến AI Engine (.NET & Python FastAPI RAG)
- **Tích Hợp Cụm Routing AI Engine Đa Nền Tảng (`appsettings.json`, `appsettings.example.json`)**:
  - Bổ sung Cluster `ai-rag-cluster` trỏ về Python FastAPI RAG Service (`http://localhost:8000`).
  - Cấu hình 5 Route mới cho phân hệ AI RAG & Chẩn đoán năng lực Core Flow 1:
    - `ai-rag-diagnostic-route`: `/api/diagnostic/{**catch-all}`
    - `ai-rag-diagnostic-v1-route`: `/api/v1/diagnostic/{**catch-all}` (sinh đề thi AI `generate-exam`, phân tích IRT 2PL / BKT `analyze`, config)
    - `ai-rag-chat-route`: `/api/chat/{**catch-all}`
    - `ai-rag-chat-v1-route`: `/api/v1/chat/{**catch-all}` (chat gia sư Socratic RAG, SSE Token Streaming)
    - `ai-rag-documents-v1-route`: `/api/v1/documents/{**catch-all}` (quản trị tài liệu tri thức RAG phiên bản hóa)
  - Bổ sung Route trực quan cho Web Client:
    - `ai-engine-view-diagnostic-route`: `/view-diagnostic`
    - `ai-engine-view-textbook-route`: `/view-textbook`
- **Khắc Phục Lỗi Startup `IAuthenticationSchemeProvider` (`GatewayAuthExtensions.cs`, `csproj`)**:
  - Phát hiện lỗi runtime `System.InvalidOperationException: Unable to resolve service for type 'Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider'` do pipeline middleware gọi `app.UseAuthentication()` nhưng service collection chưa đăng ký JWT Bearer scheme.
  - Bổ sung package `Microsoft.AspNetCore.Authentication.JwtBearer` (Version `9.0.2`).
  - Kích hoạt `services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(...)` đọc cấu hình từ `JwtSettings` (`SecretKey`, `Issuer`, `Audience`), kích hoạt tính năng trích xuất Claims (`ClaimsHeaderTransform`) an toàn.
- **Kiểm Thử Vận Hành & Khả Năng Tương Thích**:
  - Biên dịch toàn bộ Solution `V-Eval-Gateway.sln` đạt 100% (**0 Error, 0 Warning**).
  - Khởi chạy Gateway thành công 100%, lắng nghe cổng `http://localhost:5212` trơn tru.
  - Kiểm tra `docker compose config` cấu hình hợp lệ 100%.

---

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

---

## [18/09/2026] - Phát Hành Công Cụ Push Độc Lập `Scripts/push.bat` Cho Gateway Service
- **Tích Hợp `Scripts/push.bat` Độc Lập**:
  - Khởi tạo script [`Scripts/push.bat`](file:///e:/CapStone/All%20Services/V-Eval-Gateway/Scripts/push.bat) độc lập cho Gateway.
  - Hỗ trợ 3 chế độ Push (nhánh hiện tại, chọn nhánh có sẵn qua menu số, tạo nhánh mới).
  - Tự động kiểm tra đồng bộ lịch sử Git, tự động pull code khi bi cham (behind) và đưa ra **Cảnh báo Đỏ (Red Warning)** ngắt quy trình nếu phát hiện xung đột lịch sử (Conflict/Diverged).

---

## [16/09/2026 - 17/09/2026] - Đồng Bộ Cấu Trúc JWT Claims V2 ERD & Thêm Header `X-Campus-Id`
- **Đồng Bộ JWT Claims với Sơ Đồ V2 ERD (`VACT_SCHEMA_V2_ERD`)**:
  - Đối chiếu sơ đồ PlantUML V2 ERD và cập nhật DDL trong [`docs/SQL/SQL.sql`](file:///e:/CapStone/docs/SQL/SQL.sql).
  - Nâng cấp [`ClaimsHeaderTransform.cs`](file:///e:/CapStone/All%20Services/V-Eval-Gateway/V-Eval-Gateway.API/Security/ClaimsHeaderTransform.cs): Bổ sung trích xuất claim `campus_id` và đính kèm vào Header **`X-Campus-Id`** cho các dịch vụ downstream (`Students`, `Teachers`, `AcademicManagers`).
  - Kiểm thử biên dịch `dotnet build V-Eval-Gateway.sln` đạt **0 Error(s)**.

---

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
