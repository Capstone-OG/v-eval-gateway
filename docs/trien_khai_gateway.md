# TÀI LIỆU TRIỂN KHAI KIẾN TRÚC & PIPELINE API GATEWAY (V-EVAL GATEWAY)

> **Dự án**: Hệ thống cá nhân hóa lộ trình học và luyện thi Đánh giá năng lực tích hợp AI (V-ACT 2026)  
> **Phân hệ phụ trách**: `V-Eval-Gateway` (.NET 9 Web API & YARP Reverse Proxy v2.3.0)  
> **Thời điểm cập nhật**: Tháng 09/2026  
> **Trạng thái**: **100% Hoàn thành & Đã sẵn sàng vận hành (Production Ready)**

---

## 1. TỔNG QUAN KIẾN TRÚC MODULAR GATEWAY

Phân hệ `V-Eval-Gateway` áp dụng mô hình **Feature Folders / Modular Architecture** tinh gọn trên nền .NET 9 Web API đơn thay vì áp dụng Clean Architecture 4 tầng rập khuôn (`Domain`, `Application`, `Infrastructure`, `API`). 

Gateway tập trung vào các chức năng cắt ngang (Cross-Cutting Concerns) và quản lý luồng định tuyến proxy thông qua YARP v2.3.0.

### Cấu trúc thư mục dự án (`V-Eval-Gateway.API/`):

```text
V-Eval-Gateway.API/
├── Middlewares/                     # Pipeline xử lý request cắt ngang
│   ├── CorrelationIdMiddleware.cs   # Gán & truyền header X-Correlation-ID cho Distributed Tracing
│   ├── GlobalExceptionMiddleware.cs # Bắt lỗi hệ thống tập trung (ProblemDetails)
│   └── RequestLoggingMiddleware.cs  # Log HTTP Request, Response Code & Latency (ms)
│
├── Security/                        # Mô-đun bảo mật & Trích xuất thông tin User
│   ├── ClaimsHeaderTransform.cs     # Bóc tách JWT Claims (UserId, Role, Email, ExpiredTime), tiêm X-User-* và X-Token-Refresh-Required
│   └── GatewayAuthExtensions.cs     # Cấu hình JWT Bearer Validation & Authorization Policies
│
├── Transforms/                      # Mô-đun YARP Request/Response Transformers
│   └── GatewayTransformProvider.cs  # Trạm đăng ký quy tắc biến đổi Header, URL & Đóng dấu Gateway
│
├── RateLimiting/                    # Mô-đun chống Spam API & DDoS Protection
│   └── RateLimiterExtensions.cs     # Cấu hình Fixed Window (100 req/min) & Sliding Window (60 req/min)
│
├── Health/                          # Mô-đun giám sát sức khỏe cụm Microservices
│   └── DownstreamHealthCheck.cs     # Health Check probe kiểm tra kết nối tới các service nội bộ
│
├── Extensions/                      # Extension methods đóng gói DI & Pipeline
│   ├── ServiceCollectionExtensions.cs # Nạp CORS, YARP, RateLimiter, Auth, HealthChecks vào DI
│   └── MiddlewarePipelineExtensions.cs # Sắp xếp luồng Middleware theo thứ tự chuẩn
│
├── appsettings.example.json         # File cấu hình mẫu chuẩn Production (Không chứa secret thật)
└── Program.cs                       # File khởi chạy ứng dụng tinh gọn (< 30 dòng code)
```

---

## 2. MA TRẬN ĐỊNH TUYẾN YARP (ROUTES & CLUSTERS)

| Tên Route ID | Match Path | Cluster ID | Target Destination | Chức Năng Chính |
| :--- | :--- | :--- | :--- | :--- |
| `ai-engine-api-route` | `/api/ai-engine/{**catch-all}` | `ai-engine-cluster` | `http://localhost:5104` | Phân tích PDF đề thi, Chatbot AI Tutor |
| `ai-engine-view-route` | `/view-exam` | `ai-engine-cluster` | `http://localhost:5104` | Preview giao diện đề thi HTML |
| `ai-engine-static-route` | `/extracted_images/{**catch-all}` | `ai-engine-cluster` | `http://localhost:5104` | Phục vụ ảnh minh họa cắt từ PDF |
| `content-service-route` | `/api/content/{**catch-all}` | `content-service-cluster` | `http://localhost:5249` | Quản lý Ngân hàng câu hỏi, Đề thi |
| `identity-service-route` | `/api/identity/{**catch-all}` | `identity-service-cluster` | `http://localhost:5001` | Đăng ký, Đăng nhập JWT, User Profile |
| `practice-service-route` | `/api/practice/{**catch-all}` | `practice-service-cluster` | `http://localhost:5002` | Làm bài thi, Chấm điểm, Thống kê kỹ năng |

---

## 3. CƠ CHẾ BẢO MẬT & XỬ LÝ TOKEN SẮP HẾT HẠN

1. **Anti-Header-Spoofing**: Luôn xóa toàn bộ các header `X-User-Id`, `X-User-Role`, `X-User-Email` do Client tự gửi trước khi forward xuống backend.
2. **Cảnh báo Refresh Token**: Tự động kiểm tra thời gian sống còn lại của JWT Token (`remainingSeconds`). Nếu `remainingSeconds <= RefreshThresholdMinutes * 60` (Mặc định 5 phút), Gateway sẽ tự đính kèm Response Header `X-Token-Refresh-Required: true` để Frontend âm thầm gia hạn Token mà không gián đoạn người dùng.
