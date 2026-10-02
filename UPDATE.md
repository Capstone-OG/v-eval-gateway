# Nhật Ký Cập Nhật (Update Log) - V-Eval Gateway Service

## [02/10/2026] - Mở Rộng YARP Reverse Proxy Định Tuyến AI Engine & Kích Hoạt JWT Authentication Scheme

- **Khắc phục Lỗi Startup `IAuthenticationSchemeProvider` (`GatewayAuthExtensions.cs`, `csproj`)**:
  - Bổ sung package `Microsoft.AspNetCore.Authentication.JwtBearer` (`9.0.2`).
  - Kích hoạt `services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(...)` đảm bảo middleware `app.UseAuthentication()` khởi động thành công và trích xuất Claims chuẩn xác.
- **Cấu hình Reverse Proxy YARP (`appsettings.json`, `appsettings.example.json`)**:
  - Đăng ký cụm điểm đến mới `ai-rag-cluster` trỏ về Python FastAPI RAG Service (`http://localhost:8000`).
  - Mở rộng định tuyến 5 Routes phân hệ AI:
    - `/api/diagnostic/{**catch-all}` & `/api/v1/diagnostic/{**catch-all}`: Sinh đề thi AI (`generate-exam`), phân tích chẩn đoán năng lực IRT 2PL / BKT (`analyze`), cấu hình động cơ (`config`).
    - `/api/chat/{**catch-all}` & `/api/v1/chat/{**catch-all}`: Gia sư AI Socratic RAG, Token Streaming SSE (`chat/stream`), quản lý phiên (`sessions`).
    - `/api/v1/documents/{**catch-all}`: Quản trị tài liệu tri thức RAG và kiểm tra băm SHA-256.
  - Bổ sung định tuyến giao diện Web: `/view-diagnostic`, `/view-textbook`.
- **Kiểm Thử & Đảm Bảo Vận Hành**:
  - Biên dịch toàn bộ Solution `V-Eval-Gateway.sln` đạt 100% (**0 Error, 0 Warning**).
  - Khởi chạy Gateway thành công 100%, lắng nghe cổng `http://localhost:5212`.
  - Kiểm tra `docker compose config` hợp lệ 100%.
