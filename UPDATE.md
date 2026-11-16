# NHẬT KÝ CẬP NHẬT (UPDATE LOG) - GATEWAY SERVICE

## [15/11/2026] - Bổ Sung Định Tuyến YARP Cho 4 Thực Thể Học Kỳ (`/api/v1/academic-terms`), Khóa Học (`/api/v1/courses`), Lớp Học (`/api/v1/classes`) & Lịch Học (`/api/v1/schedules`) Về Cụm Practice Service

- **Bổ Sung 4 Routes YARP Tách Biệt Theo Từng Aggregate Root ([`appsettings.json`](./V-Eval-Gateway.API/appsettings.json), [`appsettings.example.json`](./V-Eval-Gateway.API/appsettings.example.json))**:
  - `practice-academic-terms-v1-route`: `/api/v1/academic-terms/{**catch-all}` -> `practice-service-cluster`.
  - `practice-courses-v1-route`: `/api/v1/courses/{**catch-all}` -> `practice-service-cluster`.
  - `practice-classes-v1-route`: `/api/v1/classes/{**catch-all}` -> `practice-service-cluster`.
  - `practice-schedules-v1-route`: `/api/v1/schedules/{**catch-all}` -> `practice-service-cluster`.
  - Đảm bảo từng Aggregate Root (`AcademicTerm`, `Course`, `Class`, `Schedule/Session/Attendance`) có định tuyến RESTful độc lập chuẩn doanh nghiệp qua cổng API Gateway.
