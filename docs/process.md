# KIẾN TRÚC & BẢNG THEO DÕI TIẾN ĐỘ CHỦ THỂ (PROCESS & PLANNING) - V-EVAL GATEWAY

---

## PHẦN 1: KIẾN TRÚC DỊCH VỤ & CÁC THÀNH PHẦN CẦN TRIỂN KHAI

### 1. Kiến Trúc Tổng Thể (YARP Reverse Proxy)
- **Cổng Dịch Vụ**: `5212` (HTTP).
- **Mô Hình**: Modular Feature Folders Architecture.
- **Nhiệm Vụ Bảo Mật**:
  - Giao điểm duy nhất tiếp nhận HTTP Request từ Web/Mobile Clients.
  - Giải mã và xác thực JWT Bearer Token tập trung (`JwtBearerDefaults`).
  - Xóa sạch mạo danh Header (`Anti-Header Spoofing`).
  - Trích xuất Claims thành Trusted Headers chuyển xuống Microservices (`X-User-Id`, `X-User-Role`, `X-User-Email`, `X-Campus-Id`, `X-Token-Expires-At`, `X-Token-Remaining-Seconds`).
  - Đính kèm Response Header `X-Token-Refresh-Required: true` khi JWT còn < 5 phút.

### 2. Danh Sách Mục & Tuyến Đường Định Tuyến (YARP Routes)
- `/api/v1/auth/{**catch-all}` -> Trỏ về Identity Service (`:5155` / `:5001`).
- `/api/v1/users/{**catch-all}` -> Trỏ về Identity Service (`:5155` / `:5001`).
- `/api/content/{**catch-all}` & `/api/v1/content/{**catch-all}` -> Trỏ về Content Service (`:5249`).
- `/api/practice/{**catch-all}`, `/api/v1/practice/{**catch-all}` & `/api/v1/ai-tutor/{**catch-all}` -> Trỏ về Practice Service (`:5261` / `:5002`, `ActivityTimeout = 00:05:00` cho SSE Streaming).
- `/swagger` & `/swagger-docs/{practice|identity|content|ai-engine}/{**catch-all}` -> Cổng kiểm thử Unified Swagger UI đa dịch vụ qua Gateway (`:5212`).
- `/api/ai-engine/{**catch-all}` -> Trỏ về .NET AI Engine (`:5104`) (Exam OCR, Textbook Ingestion, Checkpoint).
- `/api/v1/diagnostic/{**catch-all}` -> Trỏ về Python FastAPI RAG Service (`:8000`) (Sinh đề AI, Phân tích IRT 2PL/BKT).
- `/api/v1/chat/{**catch-all}` -> Trỏ về Python FastAPI RAG Service (`:8000`) (Chat Socratic RAG, SSE Streaming).
- `/api/v1/documents/{**catch-all}` -> Trỏ về Python FastAPI RAG Service (`:8000`) (RAG Knowledge Ingestion).

---

## PHẦN 2: BẢNG THEO DÕI TIẾN ĐỘ CHI TIẾT THEO TỪNG MỤC (PROGRESS MATRIX)

| STT | Hạng Mục / Chức Năng | Vị Trí Triển Khai trong Code | Trạng Thái | Tiến Độ (%) | Ghi Chú Chi Tiết |
| :---: | :--- | :--- | :---: | :---: | :--- |
| 1 | **YARP Reverse Proxy Engine** | `Program.cs`, `appsettings.json` | 🟢 Hoàn thành | 100% | Đã định tuyến đầy đủ sang các microservices downstream |
| 2 | **Tập Trung JWT Authentication** | `Security/GatewayAuthExtensions.cs` | 🟢 Hoàn thành | 100% | Validate SecretKey, Issuer, Audience, Lifetime |
| 3 | **Anti-Header Spoofing** | `Security/ClaimsHeaderTransform.cs` | 🟢 Hoàn thành | 100% | Strip bỏ toàn bộ header `X-` từ client gửi lên |
| 4 | **Claims Transformation Engine** | `Security/ClaimsHeaderTransform.cs` | 🟢 Hoàn thành | 100% | Trích xuất `X-User-Id`, `X-User-Role`, `X-User-Email` |
| 5 | **Đa Cơ Sở (`X-Campus-Id`)** | `Security/ClaimsHeaderTransform.cs` | 🟢 Hoàn thành | 100% | Đồng bộ theo Sơ đồ V2 ERD PlantUML (`campus_id`) |
| 6 | **Token Expiration Warning** | `Security/ClaimsHeaderTransform.cs` | 🟢 Hoàn thành | 100% | Trả response header `X-Token-Refresh-Required: true` |
| 7 | **Script Push Độc Lập** | `Scripts/push.bat` | 🟢 Hoàn thành | 100% | Đóng gói script push 3 chế độ kèm Red Warning |
| 8 | **Định Tuyến Kép AI Engine (.NET & Python RAG)** | `appsettings.json` | 🟢 Hoàn thành | 100% | Định tuyến đồng thời `.NET AI Engine` (`:5104`) và `Python RAG` (`:8000`) |
| 9 | **Rate Limiting Tập Trung** | `RateLimiting/` | 🟢 Hoàn thành | 100% | Đã kích hoạt Fixed Window Rate Limiting |
| 10 | **Health Check Dashboard** | `Health/` | 🟢 Hoàn thành | 100% | Endpoint `/healthz` và DownstreamHealthCheck probe |
| 11 | **Chuẩn Hóa Cấu Hình & Centralized Config** | `V-Eval-Gateway.API/appsettings.example.json` | 🟢 Hoàn thành | 100% | Cập nhật đầy đủ routes và cluster ports, hỗ trợ sync_config.bat |
| 12 | **Unified Swagger Portal, Định Tuyến `/api/v1/ai-tutor` & Chuẩn Hóa `MapInboundClaims = false`** | `Program.cs`, `Security/GatewayAuthExtensions.cs`, `appsettings.example.json` | 🟢 Hoàn thành | 100% | Gom tài liệu OpenAPI 4 dịch vụ tại `/swagger`, giữ nguyên JWT claims chuẩn (`sub`, `email`, `exp`), bổ sung route `/api/v1/ai-tutor/{**catch-all}` & Docker cluster overrides |
| 13 | **Nâng Giới Hạn Tải Tệp Kestrel & FormOptions (250 MB)** | `Program.cs` | 🟢 Hoàn thành | 100% | Khắc phục triệt để lỗi HTTP 413 Payload Too Large khi nạp tài liệu SGK và đề thi PDF dung lượng lớn |
| 14 | **Kiến Trúc Dự Phòng & Mở Rộng Đa Node API Gateway (High Availability & SPOF Mitigation)** | `docs/architecture_acceptance.md`, `docker-compose.yml` | 🟢 Hoàn thành | 100% | Phân tích rủi ro SPOF, khai thác đặc tính 100% Stateless của YARP Kestrel .NET 9, thiết kế kiến trúc mở rộng Active-Active đa node kèm tầng Load Balancer phía trước (Cloudflare Ingress / Nginx / K8s Ingress Controller) triệt tiêu điểm tắc nghẽn |
| 15 | **Định Tuyến PayOS Webhook & Đồng Bộ Cụm AI RAG Mạng Nội Bộ Docker** | `appsettings.example.json`, `docker-compose.yml` | 🟢 Hoàn thành | 100% | Bổ sung YARP route `/api/payments/{**catch-all}` định tuyến thẳng tới Practice Service nhận IPN PayOS (200/400 thay vì 404), cấu hình cluster destination `ai-rag-cluster` ánh xạ tới `http://v-eval-ai-rag:8000` xóa sạch lỗi 502 Connection Refused |
| 16 | **Định Tuyến Gia Sư Socratic & Academic Graph Về Cụm AI RAG (`:8000`)** | `appsettings.json`, `appsettings.example.json` | 🟢 Hoàn thành | 100% | Thêm `ai-rag-socratic-route`, `ai-rag-socratic-v1-route` (`/api/v1/socratic/{**catch-all}`) và `ai-rag-academic-graph-v1-route` (`/api/v1/academic-graph/{**catch-all}`) trỏ về `ai-rag-cluster` |
