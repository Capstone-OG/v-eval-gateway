# V-Eval API Gateway (`v-eval-gateway`)

> Cổng giao tiếp API Gateway đảo ngược proxy (YARP Reverse Proxy) cho hệ thống V-Eval (V-ACT 2026).

## 🚀 Tổng Quan
`V-Eval-Gateway` đóng vai trò làm điểm tiếp nhận duy nhất (Single Entry Point) cho toàn bộ các yêu cầu từ phía Client (Web Viewer SPA, Mobile App, Admin Portal) và chuyển tiếp định tuyến chính xác đến các microservices nội bộ:

- **`V-Eval-Ai_Engine`** (`:5104`): Bóc tách PDF đề thi bằng AI & Phục vụ HTML Web Viewer.
- **`V-Eval-Content_Service`** (`:5249`): Ngân hàng câu hỏi & Lưu trữ Supabase DB.
- **`V-Eval-Identity_Service`** (`:5001`): Xác thực người dùng, JWT & Phân quyền.
- **`V-Eval-Practice_Service`** (`:5002`): Quản lý lượt làm bài thi & Đánh giá năng lực.

## 🛠️ Công Nghệ Sử Dụng
- **.NET 9 Web API** (Clean Architecture)
- **YARP (Yet Another Reverse Proxy v2.3.0)**
- **Distributed Tracing**: Standard `X-Correlation-ID` header forwarding.
- **CORS Policy**: Global `AllowAll` configuration for SPA/Web Clients.

## 📖 Tài Liệu Chi Tiết
Xem đặc tả chi tiết kiến trúc và luồng định tuyến tại: [docs/api_gateway_yarp_dinh_tuyen.md](file:///e:/CapStone/All%20Services/V-Eval-Gateway/docs/api_gateway_yarp_dinh_tuyen.md)

## 🏃‍♂️ Hướng Dẫn Khởi Chạy
```powershell
cd "e:\CapStone\All Services\V-Eval-Gateway"
dotnet build
dotnet run --project V-Eval-Gateway.API
```
Endpoint Health Check: `GET http://localhost:5212/healthz`
