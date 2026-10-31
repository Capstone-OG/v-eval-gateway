# NHẬT KÝ CẬP NHẬT (UPDATE LOG) - V-EVAL GATEWAY SERVICE

## [27/10/2026] - Khởi Tạo Bộ Unit Test Cho CorrelationId Middleware Và Tích Hợp CI Test

- **Khởi Tạo Dự Án Kiểm Thử `V-Eval-Gateway.Tests` (3 Tests)**:
  - Thiết lập dự án kiểm thử theo chuẩn xUnit và .NET 9, tham chiếu đến `V-Eval-Gateway.API`.
  - Triển khai bộ 3 unit test cho middleware truy vết [`CorrelationIdMiddlewareTests.cs`](./V-Eval-Gateway.Tests/Middlewares/CorrelationIdMiddlewareTests.cs):
    - Kiểm thử khi request không mang `X-Correlation-ID`: Middleware tự động sinh GUID mới và gán vào Headers cũng như `TraceIdentifier`.
    - Kiểm thử khi request đã có sẵn `X-Correlation-ID`: Middleware bảo tồn và tái sử dụng chính xác mã định danh đã truyền.
    - Kiểm thử middleware luôn gọi chuyển tiếp `_next(context)` đến middleware tiếp theo trong pipeline.
  - Tích hợp project vào `V-Eval-Gateway.sln`.
- **Tích Hợp Tự Động Hóa CI/CD Workflow**:
  - Bổ sung bước thực thi `dotnet test --no-build --configuration Release --verbosity normal` vào `.github/workflows/ci.yml`.
