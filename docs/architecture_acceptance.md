# BÁO CÁO NGHIỆM THU & THẤU HIỂU KIẾN TRÚC API GATEWAY (V-EVAL GATEWAY)

> **Dự án**: Hệ thống V-ACT 2026  
> **Người nghiệm thu & Đánh giá**: Developer & AI Agent Pair-Programming  
> **Ngày nghiệm thu**: 18/09/2026  
> **Đánh giá tổng thể**: **Đạt chuẩn Production / Cấu hình bảo mật nâng cao V2 ERD**

---

## 💡 NỘI DUNG TỔNG HỢP KIẾN THỨC & NGUYÊN LÝ ĐÃ THẤU HIỂU

### 1. Tại sao loại bỏ 4 tầng Clean Architecture ở Gateway?
- **Bản chất Gateway**: API Gateway là điểm tiếp nhận (Edge Entry Point) và chuyển tiếp định tuyến (Reverse Proxy). Nó **không sở hữu Database riêng**, không có Entity hay Aggregate Root nghiệp vụ.
- **Tối ưu hóa**: Thay vì chia 4 dự án `.csproj` riêng biệt (`Domain`, `Application`, `Infrastructure`, `API`) tạo ra các dự án rỗng và giảm hiệu năng build, Gateway được gộp về **1 Web API duy nhất sử dụng mô hình Feature Folders (Modular Architecture)**.

---

### 2. Thấu hiểu luồng Bảo mật & Claims Transformer V2 (`ClaimsHeaderTransform.cs`)
- **Chống giả mạo Header (Anti-Header-Spoofing)**: Luôn gọi `Remove()` các header `X-User-Id`, `X-User-Role`, `X-User-Email`, `X-Campus-Id` đầu tiên để ngăn chặn kẻ xấu từ internet tự tạo header giả truyền qua Gateway.
- **Bóc tách JWT Claims Chuẩn V2 ERD**: Trích xuất `UserId` (`sub`), `Role` (`role`), `Email` (`email`), và `CampusId` (`campus_id`) từ Token đã xác thực và tự động đính kèm vào Proxy Request Header gửi xuống Microservice con.
- **Cơ chế Cảnh báo Refresh Token tự động (`X-Token-Refresh-Required`)**:
  - Trích xuất claim `exp` (Unix Timestamp) để tính số giây còn sống của Token (`remainingSeconds`).
  - Nếu `remainingSeconds <= RefreshThresholdMinutes` (Cấu hình linh hoạt qua `appsettings.json`, mặc định 5 phút), Gateway sẽ tự động đính kèm Response Header `X-Token-Refresh-Required: true`.
  - Frontend (React/Mobile) đọc Header này và **âm thầm gia hạn Token (Background Refresh Token)** mà không ngắt đoạn trải nghiệm người dùng.

---

### 3. Thấu hiểu YARP Transform Provider (`GatewayTransformProvider.cs`)
- Đóng vai trò là **"Trái tim Bảo mật & Biến đổi dữ liệu"** tại Cửa khẩu Gateway.
- Đăng ký tập trung các quy tắc biến đổi Request/Response (thêm Header Gateway, xóa Prefix đường dẫn URL, đính dấu chìa khóa nội bộ `X-Internal-Secret`).

---

### 4. Thấu hiểu Rate Limiting (`RateLimiterExtensions.cs`)
- Đóng vai trò **Lá chắn chống Spam API & DDoS / DoS Protection**.
- Giới hạn tần suất gọi API với 2 thuật toán: `Fixed Window` (100 req/min) và `Sliding Window` (60 req/min với 6 segments).
- Trả về mã lỗi **HTTP 429 Too Many Requests** ngay tại Gateway khi phát hiện lưu lượng truy cập bất thường, bảo vệ các Microservice phía sau không bị quá tải hay sập server.

---

### 5. Thấu hiểu CORS & Giao tiếp Docker (`ServiceCollectionExtensions.cs`)
- **CORS tại Gateway**: Mở chính sách `AllowAll` cho phép Frontend (Web SPA/Mobile Client) gọi API qua Gateway không bị trình duyệt chặn (Same-Origin Policy).
- **Giao tiếp Server-to-Server trên Docker**: Giữa Gateway và các Service Containers là giao tiếp nội bộ Server-to-Server, không chịu ảnh hưởng bởi CORS. Khi đưa lên Docker chỉ cần tham gia chung `veval_network` là kết nối mượt mà.

---

### 6. Thấu hiểu Cấu hình Bảo mật Git (`appsettings.example.json` & `.gitignore`)
- Tất cả các file chứa Mật khẩu / Connection String / API Key thật (`appsettings.json`, `appsettings.Development.json`) đều được untrack và đưa vào `.gitignore`.
- Chỉ lưu giữ file [`appsettings.example.json`](file:///e:/CapStone/All%20Services/V-Eval-Gateway/V-Eval-Gateway.API/appsettings.example.json) chứa các Label tham số mẫu trên Git repository, đảm bảo chuẩn mã nguồn mở & an toàn Production 100%.

---

## 📑 BẢNG NGHIỆM THU DANH MỤC FILE CODE GATEWAY

| Tên Module | Tệp Code / Tài Liệu | Trạng Thái Nghiệm Thu |
| :--- | :--- | :---: |
| **Middlewares** | [`CorrelationIdMiddleware.cs`](file:///e:/CapStone/All%20Services/V-Eval-Gateway/V-Eval-Gateway.API/Middlewares/CorrelationIdMiddleware.cs), [`GlobalExceptionMiddleware.cs`](file:///e:/CapStone/All%20Services/V-Eval-Gateway/V-Eval-Gateway.API/Middlewares/GlobalExceptionMiddleware.cs), [`RequestLoggingMiddleware.cs`](file:///e:/CapStone/All%20Services/V-Eval-Gateway/V-Eval-Gateway.API/Middlewares/RequestLoggingMiddleware.cs) | **ĐẠT (100%)** |
| **Security** | [`ClaimsHeaderTransform.cs`](file:///e:/CapStone/All%20Services/V-Eval-Gateway/V-Eval-Gateway.API/Security/ClaimsHeaderTransform.cs), [`GatewayAuthExtensions.cs`](file:///e:/CapStone/All%20Services/V-Eval-Gateway/V-Eval-Gateway.API/Security/GatewayAuthExtensions.cs) | **ĐẠT (100%)** |
| **Transforms** | [`GatewayTransformProvider.cs`](file:///e:/CapStone/All%20Services/V-Eval-Gateway/V-Eval-Gateway.API/Transforms/GatewayTransformProvider.cs) | **ĐẠT (100%)** |
| **RateLimiting** | [`RateLimiterExtensions.cs`](file:///e:/CapStone/All%20Services/V-Eval-Gateway/V-Eval-Gateway.API/RateLimiting/RateLimiterExtensions.cs) | **ĐẠT (100%)** |
| **Health** | [`DownstreamHealthCheck.cs`](file:///e:/CapStone/All%20Services/V-Eval-Gateway/V-Eval-Gateway.API/Health/DownstreamHealthCheck.cs) | **ĐẠT (100%)** |
| **Extensions** | [`ServiceCollectionExtensions.cs`](file:///e:/CapStone/All%20Services/V-Eval-Gateway/V-Eval-Gateway.API/Extensions/ServiceCollectionExtensions.cs), [`MiddlewarePipelineExtensions.cs`](file:///e:/CapStone/All%20Services/V-Eval-Gateway/V-Eval-Gateway.API/Extensions/MiddlewarePipelineExtensions.cs) | **ĐẠT (100%)** |
| **Config & Git** | [`appsettings.example.json`](file:///e:/CapStone/All%20Services/V-Eval-Gateway/V-Eval-Gateway.API/appsettings.example.json), [` .gitignore`](file:///e:/CapStone/All%20Services/V-Eval-Gateway/.gitignore) | **ĐẠT (100%)** |
