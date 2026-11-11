# NHẬT KÝ CẬP NHẬT (UPDATE LOG) - GATEWAY SERVICE

## [08/11/2026] - Tích Hợp Unified Swagger Portal Đa Dịch Vụ, Bổ Sung Định Tuyến `/api/v1/ai-tutor` & Chuẩn Hóa JWT Claims (`MapInboundClaims = false`)

- **Cổng Kiểm Thử Tập Trung Unified Swagger UI (`http://localhost:5212/swagger`) ([`Program.cs`](./V-Eval-Gateway.API/Program.cs), [`V-Eval-Gateway.API.csproj`](./V-Eval-Gateway.API/V-Eval-Gateway.API.csproj))**:
  - Tích hợp `Swashbuckle.AspNetCore 7.3.1` và cấu hình `UseSwaggerUI` tổng hợp tài liệu OpenAPI của `Practice Service`, `Identity Service`, `Content Service` và `AI Engine` qua các route YARP `/swagger-docs/{service}/{**catch-all}`.
  - Hỗ trợ đính kèm `Bearer` JWT một lần tại `:5212/swagger` để Gateway tự động xác thực và truyền `X-User-Id`, `X-User-Role`, `X-Campus-Id` xuống toàn bộ dịch vụ phía sau.
- **Chuẩn Hóa JWT Claims & Định Tuyến Core Flow 4 / 6 ([`GatewayAuthExtensions.cs`](./V-Eval-Gateway.API/Security/GatewayAuthExtensions.cs), [`appsettings.example.json`](./V-Eval-Gateway.API/appsettings.example.json))**:
  - Bật `options.MapInboundClaims = false` trong `AddJwtBearer` giữ nguyên các claim chuẩn (`sub`, `email`, `exp`, `campus_id`) cho [`ClaimsHeaderTransform.cs`](./V-Eval-Gateway.API/Security/ClaimsHeaderTransform.cs) đồng thời bảo toàn tuyệt đối lớp chắn `Anti-Header Spoofing`.
  - Bổ sung route `practice-ai-tutor-v1-route` (`/api/v1/ai-tutor/{**catch-all}`), tăng `ActivityTimeout = 00:05:00` cho `practice-service-cluster` và đồng bộ cấu hình sang `Configs/V-Eval-Gateway/appsettings.json` cùng `docker-compose.yml`.
- **Kiểm Thử & Vận Hành**:
  - `dotnet build` đạt `0 Warning, 0 Error`; `dotnet test` đạt `3/3 Passed (100%)`; `docker compose config` hợp lệ 100%; kiểm thử HTTP E2E qua Gateway `:5212` đạt `200 OK` toàn diện.
