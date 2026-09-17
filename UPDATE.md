# Nhật Ký Cập Nhật - V-Eval Gateway Service

## [18/09/2026] - Phát Hành Công Cụ Push Độc Lập `Scripts/push.bat` Cho Gateway Service
- **Tích Hợp `Scripts/push.bat` Độc Lập**:
  - Khởi tạo công cụ [`Scripts/push.bat`](file:///e:/CapStone/All%20Services/V-Eval-Gateway/Scripts/push.bat) độc lập cho riêng Gateway Service.
  - Hỗ trợ Push nhanh trên nhánh hiện tại, chọn nhánh đã có qua Menu đánh số, hoặc tạo nhánh mới tự động.
  - Tích hợp kiểm tra đồng bộ lịch sử Git, tự động pull code khi chậm (behind) và đưa ra **Cảnh báo Đỏ (Red Warning)** ngắt quy trình nếu bị xung đột lịch sử (Conflict/Diverged).

## [16/09/2026 - 17/09/2026] - Đồng Bộ Cấu Trúc JWT Claims V2 ERD & Thêm Header `X-Campus-Id`
- **Nâng Cấp Security Claims Transformer (`ClaimsHeaderTransform.cs`)**:
  - Bổ sung trích xuất claim `campus_id` từ JWT Token và đính kèm vào Header **`X-Campus-Id`** gửi xuống downstream microservices (`Students`, `Teachers`, `AcademicManagers`).
  - Đồng bộ trọn bộ X-Headers: `X-User-Id`, `X-User-Role`, `X-User-Email`, `X-Campus-Id`, `X-Token-Expires-At`, `X-Token-Remaining-Seconds`, và `X-Token-Refresh-Required`.
- **Đồng Bộ Tài Liệu Kiến Trúc & Nhật Ký Tiến Độ**:
  - Cập nhật [`docs/daily_check_log.md`](file:///e:/CapStone/All%20Services/V-Eval-Gateway/docs/daily_check_log.md) và [`docs/nghiem_thu_va_thau_hieu_kien_truc.md`](file:///e:/CapStone/All%20Services/V-Eval-Gateway/docs/nghiem_thu_va_thau_hieu_kien_truc.md).
- **Kiểm Thử Biên Dịch**: `dotnet build V-Eval-Gateway.sln` đạt **100% Succeeded (0 Error, 0 Warning)**.

## [15/09/2026] - Dockerize Gateway & Chuẩn Hóa Docker Compose Multi-Stage Build
- **Dockerfile Multi-Stage .NET 9**:
  - Khởi tạo `Dockerfile` chuẩn cho Gateway (`V-Eval-Gateway.API`) với cổng `5212`.
- **Tích Hợp Docker Compose Orchestration (`docker-compose.yml`)**:
  - Định tuyến các container qua mạng nội bộ bridge `veval_network`.

## [14/09/2026] - Chuyển Đổi sang Kiến Trúc Modular Feature Folders, Security Claims Transformer & Production Config
- **Tái Cấu Trúc Kiến Trúc**:
  - Gỡ bỏ 3 tầng dự án Clean Architecture dư thừa (`Domain`, `Application`, `Infrastructure`).
  - Đưa `V-Eval-Gateway.API` về mô hình **Feature Folders / Modular Architecture**.
