# NHẬT KÝ KIỂM TRẢ TIẾN ĐỘ VẬN HÀNH (DAILY CHECK LOG) - V-EVAL GATEWAY

## [13/11/2026] - Bổ Sung Định Tuyến YARP Cho Gia Sư Socratic & Academic Graph Về Cụm AI RAG (`:8000`)

- **Bổ Sung Định Tuyến Socratic & Academic Graph ([`appsettings.json`](../V-Eval-Gateway.API/appsettings.json), [`appsettings.example.json`](../V-Eval-Gateway.API/appsettings.example.json))**:
  - Thêm `ai-rag-socratic-route` (`/api/socratic/{**catch-all}`), `ai-rag-socratic-v1-route` (`/api/v1/socratic/{**catch-all}`) và `ai-rag-academic-graph-v1-route` (`/api/v1/academic-graph/{**catch-all}`) trỏ trực tiếp sang `ai-rag-cluster` (`http://v-eval-ai-rag:8000` trong Docker / `http://localhost:8000` ở local).
  - Đồng bộ hoàn toàn với `Configs/V-Eval-Gateway/appsettings.json`.

---

## [11/11/2026] - Chuyển Toàn Bộ Nhánh Làm Việc Sang Main, Triển Khai Docker Container Trên Domain pickic.io.vn & Hoạch Định Kiến Trúc Dự Phòng API Gateway (High Availability & SPOF Mitigation)

- **Triển Khai Đóng Gói Docker Container & Ánh Xạ Domain pickic.io.vn ([`docker-compose.yml`](../../docker-compose.yml))**:
  - Triển khai container `v_eval_gateway` hoạt động trên mạng bridge `capstone_veval_network` cổng `5212:5212`, đóng vai trò là Cổng giao tiếp duy nhất (Single Entry Point) của toàn bộ kiến trúc V-Eval.
  - Cấu hình Cloudflare Tunnel `/home/thai-thinh/.cloudflared/config.yml` định tuyến `https://api.pickic.io.vn` trực tiếp về Gateway `http://127.0.0.1:5212`.
  - Cập nhật định tuyến động tới các cluster backend trong mạng nội bộ Docker: `http://v-eval-identity-service:5155`, `http://v-eval-content-service:5249`, `http://v-eval-practice-service:5002`, `http://v-eval-ai-engine:5104`.
  - Kiểm thử trực tiếp: `curl -s https://api.pickic.io.vn/api/v1/campuses` trả về HTTP 200 kèm danh sách 2 cơ sở đào tạo, xác nhận Gateway định tuyến xuyên suốt 100%.

- **Định Tuyến PayOS Webhook & Cụm Dịch Vụ AI Python RAG Nội Bộ ([`appsettings.example.json`](../V-Eval-Gateway.API/appsettings.example.json), [`docker-compose.yml`](../../docker-compose.yml))**:
  - **Bổ sung định tuyến PayOS Webhook (`practice-payments-route`)**:
    - Khắc phục lỗi `HTTP 404 Not Found` khi cổng thanh toán PayOS gửi thông báo IPN thanh toán khóa học/lượt thi vào endpoint `/api/payments/payos-webhook`.
    - Bổ sung cấu hình route YARP ánh xạ pattern `/api/payments/{**catch-all}` trực tiếp sang cụm `practice-service-cluster`.
  - **Ánh xạ cụm Python FastAPI RAG trong mạng Docker (`ai-rag-cluster`)**:
    - Thiết lập biến môi trường cấu hình động YARP `ReverseProxy__Clusters__ai-rag-cluster__Destinations__destination1__Address=http://v-eval-ai-rag:8000` trên container `v-eval-gateway`.
    - Xóa sạch lỗi `HTTP 502 Bad Gateway (Connection refused localhost:8000)` khi các yêu cầu `/api/v1/diagnostic/{**catch-all}` được gửi qua Gateway.
  - **Kiểm thử trực tiếp**:
    - `curl -s -X POST http://localhost:5212/api/payments/payos-webhook -d '{"test":true}'` trả về `{"success":false,"message":"Invalid signature"}` (xác nhận request đã đi xuyên suốt qua Gateway tới Practice Service thành công).

- **Nghiên Cứu & Hoạch Định Kiến Trúc Dự Phòng Đa Node (High Availability & SPOF Mitigation)**:
  - *Nhận diện Rủi ro*: Trong kiến trúc Microservices, Gateway là Single Entry Point. Nếu chỉ triển khai duy nhất 1 node thì Gateway trở thành Điểm tắc nghẽn nguy hiểm nhất (Single Point of Failure - SPOF); khi node này gặp sự cố (quá tải, OOM, crash), toàn bộ hệ thống tê liệt dù các service nghiệp vụ phía sau vẫn khỏe mạnh.
  - *Bản chất Stateless của YARP*: YARP trên nền Kestrel .NET 9 hoàn toàn không lưu trữ session người dùng (Token-based Authentication), không lưu cache nghiệp vụ và tiêu tốn cực ít tài nguyên (~30-50MB RAM). Do đó, Gateway có thể mở rộng theo chiều ngang (Horizontal Scaling) thành 2-3+ node Active-Active mà không gặp trở ngại về đồng bộ trạng thái bộ nhớ.
  - *Thiết kế Cân bằng tải Đa tầng*:
    - **Tầng 1 (Host / Docker Compose)**: Phía trước cụm `v_eval_gateway_1` và `v_eval_gateway_2` là Cloudflare Tunnel Ingress hoặc Nginx Reverse Proxy siêu nhẹ thực hiện Round-Robin, kết hợp Liveness Probe `/healthz` và chính sách phục hồi tự động của Docker (`restart: always`).
    - **Tầng 2 (Production / Kubernetes)**: Đóng gói Gateway thành K8s Deployment với `replicas >= 2`, tích hợp HPA (Horizontal Pod Autoscaler) tự động nâng số pod theo ngưỡng CPU/RAM 70%, đứng sau Kubernetes Ingress Controller / Cloud Load Balancer.
  - *Chuẩn bị Luận điểm Bảo vệ Đồ án*: Cung cấp cơ sở lý luận vững chắc, minh chứng rõ năng lực thiết kế hệ thống chịu lỗi cao (Fault Tolerance & Scalability) sẵn sàng bảo vệ trước Hội đồng phản biện Capstone.

---

## [09/11/2026] - Khắc Phục Lỗi HTTP 413 Payload Too Large, Nâng Giới Hạn Tải Tệp Kestrel & FormOptions Lên 250 MB Cho AI Document & Exam Ingestion

- **Nâng Giới Hạn Kestrel Request Body & FormOptions (`Program.cs`)**:
  - Phát hiện và xử lý triệt để nguyên nhân gốc rễ gây lỗi `HTTP 413 Payload Too Large` (`BadHttpRequestException: Request body too large. The max request body size is 30000000 bytes`) khi người dùng nạp tài liệu SGK hoặc đề thi PDF dung lượng lớn qua API Gateway (`http://localhost:5212/api/ai-engine/curriculum/extract`).
  - Cấu hình Kestrel Server: `builder.WebHost.ConfigureKestrel(options => options.Limits.MaxRequestBodySize = 262_144_000)` (250 MB).
  - Cấu hình FormOptions: `builder.Services.Configure<FormOptions>(options => { options.ValueLengthLimit = int.MaxValue; options.MultipartBodyLengthLimit = 262_144_000; options.MultipartHeadersLengthLimit = int.MaxValue; })`.
  - Đồng bộ giới hạn 250 MB xuyên suốt từ Gateway xuống AI Engine và Content Service, loại bỏ hoàn toàn hiện tượng ngắt kết nối gateway khi bóc tách tài liệu lớn.
- **Kiểm Thử & Biên Dịch**:
  - `dotnet build` đạt kết quả biên dịch sạch `0 Warning, 0 Error`.

---

## [08/11/2026] - Tích Hợp Unified Swagger Portal Đa Dịch Vụ, Bổ Sung Định Tuyến Core Flow 4/6 (`/api/v1/ai-tutor`) & Chuẩn Hóa JWT Claims (`MapInboundClaims = false`)

- **Cổng Kiểm Thử Tập Trung Unified Swagger UI (`http://localhost:5212/swagger`) ([`Program.cs`](../V-Eval-Gateway.API/Program.cs), [`V-Eval-Gateway.API.csproj`](../V-Eval-Gateway.API/V-Eval-Gateway.API.csproj))**:
  - Tích hợp `Swashbuckle.AspNetCore 7.3.1` và cấu hình `UseSwaggerUI` gom toàn bộ tài liệu OpenAPI của 4 dịch vụ downstream (`Practice Service`, `Identity Service`, `Content Service`, `AI Engine`) về một giao diện duy nhất trên Gateway thông qua các route YARP `PathRemovePrefix` `/swagger-docs/{service}/{**catch-all}`.
  - Khi bấm `Authorize` (Bearer JWT) trên `:5212/swagger`, toàn bộ request gửi đi tự động qua `ClaimsHeaderTransform` giải mã JWT và đính kèm `X-User-Id`, `X-User-Role`, `X-Campus-Id` chuẩn xác xuống từng microservice.
- **Chuẩn Hóa Ánh Xạ JWT Claims & Mở Rộng Định Tuyến Core Flow 4 & 6 ([`GatewayAuthExtensions.cs`](../V-Eval-Gateway.API/Security/GatewayAuthExtensions.cs), [`appsettings.example.json`](../V-Eval-Gateway.API/appsettings.example.json))**:
  - Đặt `options.MapInboundClaims = false` trên `AddJwtBearer` để giữ nguyên tên claim chuẩn (`sub`, `email`, `exp`, `campus_id`), đảm bảo [`ClaimsHeaderTransform.cs`](../V-Eval-Gateway.API/Security/ClaimsHeaderTransform.cs) trích xuất định danh nhất quán 100% trong khi giữ nguyên cơ chế chống giả mạo `X-User-Id` (`Anti-Header Spoofing`).
  - Bổ sung route `practice-ai-tutor-v1-route` (`/api/v1/ai-tutor/{**catch-all}`) và tăng `ActivityTimeout = 00:05:00` trên `practice-service-cluster` phục vụ luồng SSE Streaming Socratic Tutor & Core Flow 6.
  - Đồng bộ cấu hình tập trung tại `Configs/V-Eval-Gateway/appsettings.json` và bổ sung biến môi trường ánh xạ cổng nội bộ container (`:5001`, `:5002`, `:5249`, `:5104`) trên `docker-compose.yml`.
- **Kiểm Thử & Nghiệm Thu**:
  - Biên dịch sạch `0 Warning, 0 Error`, `dotnet test` đạt `3/3 Passed (100%)`, `docker compose config` hợp lệ 100%, kiểm thử E2E xuyên suốt Gateway `:5212` -> Practice `:5261` đạt `200 OK` toàn diện.

---

## [02/11/2026] - Thiết Lập Kiến Trúc Test Nền Tảng Cho GATEWAY SERVICE

- **Chuẩn Hóa Hạ Tầng Dự Án Kiểm Thử (V-Eval-Gateway.Tests)**:
  - Nâng cấp bộ thư viện kiểm thử: `xunit` (2.9.2), `Moq` (4.20.72), `FluentAssertions` (7.2.0), `coverlet.collector` (6.0.2).
  - Tích hợp Global Usings giúp code test ngắn gọn, sạch sẽ, không cần lặp lại namespace kiểm thử.
  - Cấu hình công cụ ReportGenerator v5.4.18 qua `.config/dotnet-tools.json` để phục vụ xuất báo cáo Code Coverage dạng HTML.
  - Phân vùng cấu trúc thư mục kiểm thử chuẩn hóa: `Commands/`, `Queries/`, `Validators/`, `Common/`.
- **Bảo Vệ Kho Lưu Trữ & Script Vận Hành**:
  - Bổ sung `TestResults/` và `*.cobertura.xml` vào `.gitignore` của dịch vụ.
  - Tạo script `Scripts/test.bat` hỗ trợ chạy test nhanh và xem báo cáo coverage trực tiếp tại máy phát triển.

## [07/10/2026] - Đồng Bộ & Chuẩn Hóa Khung Cấu Hình YARP Reverse Proxy Cho Toàn Hệ Thống
- **Chuẩn Hóa File Cấu Hình Mẫu (`appsettings.example.json`)**:
  - Cập nhật đầy đủ toàn bộ danh mục routes mới nhất bao gồm AI RAG Diagnostic, Socratic Chat, Content Service, Identity Service IAM và Practice Service.
  - Cập nhật chính xác port đích của các cluster microservices (Identity: `:5155`, Content: `:5249`, Practice: `:5261`, AI Engine: `:5104`, AI RAG: `:8000`).
  - Đặt placeholder bảo mật cho khóa bí mật `JwtSettings:SecretKey`.
- **Đồng Bộ Hệ Thống Cấu Hình Tập Trung**:
  - Tích hợp vào thư mục `Configs/V-Eval-Gateway/` của System-Repo để tự động cấp phát khi chạy `sync_config.bat`.
- **Kiểm Thử Biên Dịch**:
  - `dotnet build` đạt 100% thành công (0 warning, 0 error).

---

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
  - Khởi tạo script [`Scripts/push.bat`](../Scripts/push.bat) độc lập cho Gateway.
  - Hỗ trợ 3 chế độ Push (nhánh hiện tại, chọn nhánh có sẵn qua menu số, tạo nhánh mới).
  - Tự động kiểm tra đồng bộ lịch sử Git, tự động pull code khi bi cham (behind) và đưa ra **Cảnh báo Đỏ (Red Warning)** ngắt quy trình nếu phát hiện xung đột lịch sử (Conflict/Diverged).

---

## [16/09/2026 - 17/09/2026] - Đồng Bộ Cấu Trúc JWT Claims V2 ERD & Thêm Header `X-Campus-Id`
- **Đồng Bộ JWT Claims với Sơ Đồ V2 ERD (`VACT_SCHEMA_V2_ERD`)**:
  - Đối chiếu sơ đồ PlantUML V2 ERD và cập nhật DDL trong `docs/SQL/SQL.sql`.
  - Nâng cấp [`ClaimsHeaderTransform.cs`](../V-Eval-Gateway.API/Security/ClaimsHeaderTransform.cs): Bổ sung trích xuất claim `campus_id` và đính kèm vào Header **`X-Campus-Id`** cho các dịch vụ downstream (`Students`, `Teachers`, `AcademicManagers`).
  - Kiểm thử biên dịch `dotnet build V-Eval-Gateway.sln` đạt **0 Error(s)**.

---

## [15/09/2026] - Dockerize Gateway & Chuẩn Hóa Docker Compose Multi-Stage Build
- **Dockerfile Multi-Stage .NET 9**:
  - Khởi tạo `Dockerfile` cho Gateway (`V-Eval-Gateway.API`) trên cổng `5212`.
- **Tích Hợp Docker Compose Orchestration (`docker-compose.yml`)**:
  - Định tuyến các container qua mạng nội bộ bridge `veval_network`.
  - Cấu hình script chạy tự động `Scripts/run_docker/run_docker.bat`.
  - Kiểm thử cú pháp `docker compose config` đạt 100% thành công (Exit Code 0).

---

## [14/09/2026] - Chuyển Đổi sang Kiến Trúc Modular Feature Folders, Thêm Response Token Warning & Bảo Mật Production
- **Tái Cấu Trúc Kiến Trúc Solution**:
  - Gỡ bỏ hoàn toàn 3 dự án Clean Architecture rỗng (`Domain`, `Application`, `Infrastructure`).
  - Chuyển `V-Eval-Gateway.API` sang mô hình **Feature Folders / Modular Architecture**.
