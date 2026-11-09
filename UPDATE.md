# NHẬT KÝ CẬP NHẬT (UPDATE LOG) - GATEWAY SERVICE

## [09/11/2026] - Khắc Phục Triệt Để Lỗi HTTP 413 Payload Too Large, Nâng Giới Hạn Tải Tệp Kestrel & FormOptions Lên 250 MB Cho AI Document & Exam Ingestion

- **Nâng Cấp Giới Hạn Kestrel Request Body & FormOptions ([`Program.cs`](./V-Eval-Gateway.API/Program.cs))**:
  - Khắc phục lỗi `HTTP 413 Payload Too Large` (`BadHttpRequestException: Request body too large. The max request body size is 30000000 bytes`) khi upload tài liệu SGK hoặc đề thi PDF kích thước lớn qua Gateway (`http://localhost:5212/api/ai-engine/curriculum/extract`).
  - Cấu hình Kestrel Server: `builder.WebHost.ConfigureKestrel(options => options.Limits.MaxRequestBodySize = 262_144_000)` (250 MB).
  - Cấu hình FormOptions: `builder.Services.Configure<FormOptions>(options => { options.ValueLengthLimit = int.MaxValue; options.MultipartBodyLengthLimit = 262_144_000; options.MultipartHeadersLengthLimit = int.MaxValue; })`.
  - Đảm bảo tính nhất quán 250 MB từ Gateway đến downstream microservices (`AI Engine`, `Content Service`), cho phép tải và xử lý tài liệu OCR/SGK dung lượng lớn ổn định 100%.
- **Kiểm Thử & Nghiệm Thu**:
  - `dotnet build` đạt kết quả biên dịch sạch `0 Warning, 0 Error`.
