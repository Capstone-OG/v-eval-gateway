# NHẬT KÝ KIỂM TRẢ TIẾN ĐỘ VẬN HÀNH (DAILY CHECK LOG) - V-EVAL GATEWAY

## [16/09/2026 - 17/09/2026] - Đồng Bộ Cấu Trúc JWT Claims V2 ERD & Thêm Header `X-Campus-Id`
- **Đồng Bộ JWT Claims với Sơ Đồ V2 ERD (`VACT_SCHEMA_V2_ERD`)**:
  - Đối chiếu sơ đồ PlantUML V2 ERD và cập nhật DDL trong [`docs/SQL/SQL.sql`](file:///e:/CapStone/docs/SQL/SQL.sql).
  - Nâng cấp [`ClaimsHeaderTransform.cs`](file:///e:/CapStone/All%20Services/V-Eval-Gateway/V-Eval-Gateway.API/Security/ClaimsHeaderTransform.cs): Bổ sung trích xuất claim `campus_id` và đính kèm vào Header **`X-Campus-Id`** cho các dịch vụ downstream (`Students`, `Teachers`, `AcademicManagers`).
  - Kiểm thử biên dịch `dotnet build V-Eval-Gateway.sln` đạt **0 Error(s)**.

---

## [15/09/2026] - Dockerize Gateway & Chuẩn Hóa Docker Compose Multi-Stage Build
- **Dockerfile Multi-Stage .NET 9**:
  - Khởi tạo `Dockerfile` cho Gateway (`V-Eval-Gateway.API`) trên cổng `5212`.
- **Tích Hợp Docker Compose Orchestration (`docker-compose.yml`)**:
  - Định tuyến các container qua mạng nội bộ bridge `veval_network`.
  - Cấu hình script chạy tự động [`run_docker.bat`](file:///e:/CapStone/Scripts/run_docker/run_docker.bat).
  - Kiểm thử cú pháp `docker compose config` đạt 100% thành công (Exit Code 0).

---

## [14/09/2026] - Chuyển Đổi sang Kiến Trúc Modular Feature Folders, Thêm Response Token Warning & Bảo Mật Production
- **Tái Cấu Trúc Kiến Trúc Solution**:
  - Gỡ bỏ hoàn toàn 3 dự án Clean Architecture rỗng (`Domain`, `Application`, `Infrastructure`).
  - Chuyển `V-Eval-Gateway.API` sang mô hình **Feature Folders / Modular Architecture**.
