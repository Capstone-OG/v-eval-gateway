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
- `/api/v1/auth/{**catch-all}` -> Trỏ về Identity Service (`:5001`).
- `/api/v1/users/{**catch-all}` -> Trỏ về Identity Service (`:5001`).
- `/api/content/{**catch-all}` -> Trỏ về Content Service (`:5249`).
- `/api/practice/{**catch-all}` -> Trỏ về Practice Service (`:5002`).
- `/api/ai-engine/{**catch-all}` -> Trỏ về AI Engine (`:5104`).

---

## PHẦN 2: BẢNG THEO DÕI TIẾN ĐỘ CHI TIẾT THEO TỪNG MỤC (PROGRESS MATRIX)

| STT | Hạng Mục / Chức Năng | Vị Trí Triển Khai trong Code | Trạng Thái | Tiến Độ (%) | Ghi Chú Chi Tiết |
| :---: | :--- | :--- | :---: | :---: | :--- |
| 1 | **YARP Reverse Proxy Engine** | `Program.cs`, `appsettings.json` | 🟢 Hoàn thành | 100% | Đã định tuyến đầy đủ sang 4 microservices con |
| 2 | **Tập Trung JWT Authentication** | `Security/GatewayAuthExtensions.cs` | 🟢 Hoàn thành | 100% | Validate SecretKey, Issuer, Audience, Lifetime |
| 3 | **Anti-Header Spoofing** | `Security/ClaimsHeaderTransform.cs` | 🟢 Hoàn thành | 100% | Strip bỏ toàn bộ header `X-` từ client gửi lên |
| 4 | **Claims Transformation Engine** | `Security/ClaimsHeaderTransform.cs` | 🟢 Hoàn thành | 100% | Trích xuất `X-User-Id`, `X-User-Role`, `X-User-Email` |
| 5 | **Đa Cơ Sở (`X-Campus-Id`)** | `Security/ClaimsHeaderTransform.cs` | 🟢 Hoàn thành | 100% | Đồng bộ theo Sơ đồ V2 ERD PlantUML (`campus_id`) |
| 6 | **Token Expiration Warning** | `Security/ClaimsHeaderTransform.cs` | 🟢 Hoàn thành | 100% | Trả response header `X-Token-Refresh-Required: true` |
| 7 | **Script Push Độc Lập** | `Scripts/push.bat` | 🟢 Hoàn thành | 100% | Đóng gói script push 3 chế độ kèm Red Warning |
| 8 | **Rate Limiting Tập Trung** | `Transforms/` (dự kiến) | 🟡 Đang chờ | 0% | Sẽ triển khai ở Milestone tiếp theo |
| 9 | **Health Check Dashboard** | `Program.cs` (dự kiến) | 🟡 Đang chờ | 0% | Kiểm tra liveness/readiness của 5 services |
