# Nhật Ký Cập Nhật - V-Eval Gateway Service

## [12/09/2026] - Hoàn Thiện Health Check Endpoint & Kiểm Thử Xây Dựng Gateway
- **Cấu Hình Health Check Endpoint (`/healthz`)**:
  - Bổ sung MapGet(`/healthz`) trả về trạng thái JSON (`Healthy`, Timestamp UTC, Service Name).
- **Middleware Correlation ID & Distributed Tracing**:
  - Bổ sung middleware `X-Correlation-ID` tự động gán & chuyển tiếp header trace giữa các request.
- **Biên Dịch & Kiểm Thử**:
  - Biên dịch toàn bộ giải pháp `V-Eval-Gateway.sln` thành công 100% (0 lỗi, 0 cảnh báo).

## [11/09/2026] - Ma Trận Định Tuyến YARP Reverse Proxy & CORS Policy
- **Bảng Định Tuyến Routes & Clusters Matrix (`appsettings.json`)**:
  - `ai-engine-api-route`: `/api/ai-engine/{**catch-all}` -> `http://localhost:5104`
  - `ai-engine-view-route`: `/view-exam` -> `http://localhost:5104`
  - `ai-engine-static-route`: `/extracted_images/{**catch-all}` -> `http://localhost:5104`
  - `content-service-route`: `/api/content/{**catch-all}` -> `http://localhost:5249`
  - `identity-service-route`: `/api/identity/{**catch-all}` -> `http://localhost:5001`
  - `practice-service-route`: `/api/practice/{**catch-all}` -> `http://localhost:5002`
- **CORS Policy & Request Forwarding**:
  - Thiết lập chính sách CORS `AllowAll` hỗ trợ Frontend Web Viewer & Mobile Client kết nối không bị chặn.

## [10/09/2026] - Khởi Tạo YARP Reverse Proxy & gRPC Contracts Sync
- **Tích Hợp Thư Viện YARP Reverse Proxy**:
  - Thêm gói NuGet `Yarp.ReverseProxy` v2.3.0 cho `V-Eval-Gateway.API.csproj`.
- **Đồng Bộ Hóa Protobuf Contracts**:
  - Cấu hình liên kết Protobuf (`ai.proto`, `content.proto`, `auth.proto`, `practice.proto`) từ thư mục chung `/grpc`.
