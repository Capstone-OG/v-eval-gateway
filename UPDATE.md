# NHẬT KÝ CẬP NHẬT (UPDATE LOG) - V-EVAL GATEWAY SERVICE

## [20/10/2026] - Nâng Cấp YARP Reverse Proxy: Khắc Phục Triệt Để Lỗi 504 Gateway Timeout Với ActivityTimeout 10 Phút Cho Cụm AI

- **Khắc Phục Lỗi 504 Gateway Timeout Cho Các Tác Vụ Bóc Tách Tri Thức AI**:
  - *Nguyên nhân sự cố*: Mặc định YARP Reverse Proxy v2.3.0 áp dụng thời gian chờ `ActivityTimeout` là 100 giây. Khi AI Engine thực hiện các tác vụ bóc tách tài liệu khổ lớn hoặc sinh nội dung 3D/2D chuyên sâu, thời gian sinh văn bản của Google Gemini API vượt ngưỡng 100s dẫn đến Gateway ngắt kết nối và trả về mã lỗi `HTTP 504 Gateway Timeout` cho Web Client.
  - *Cấu hình ActivityTimeout 10 Phút Cho Cụm AI*:
    + Nâng cấp cấu hình `HttpRequest: { ActivityTimeout: "00:10:00" }` trong `appsettings.json` cho cả 2 cụm: `ai-engine-cluster` (Port 5104) và `ai-rag-cluster` (Port 8000).
    + Đảm bảo các luồng upload tài liệu PDF/DOCX, bóc tách tri thức, OCR và RAG Psychometrics có đủ thời gian hoàn thành trọn vẹn mà không bao giờ bị Gateway ngắt ngang.
  - *Đồng bộ cấu hình tập trung*: Cập nhật đồng thời tại `All Services/V-Eval-Gateway/V-Eval-Gateway.API/appsettings.json` và `Configs/V-Eval-Gateway/appsettings.json`.
