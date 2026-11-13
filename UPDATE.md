# NHẬT KÝ CẬP NHẬT (UPDATE LOG) - GATEWAY SERVICE

## [13/11/2026] - Bổ Sung Định Tuyến YARP Cho Gia Sư Socratic & Academic Graph Về Cụm AI RAG (`:8000`)

- **Bổ Sung Định Tuyến Socratic & Academic Graph ([`appsettings.json`](./V-Eval-Gateway.API/appsettings.json), [`appsettings.example.json`](./V-Eval-Gateway.API/appsettings.example.json))**:
  - Thêm `ai-rag-socratic-route` (`/api/socratic/{**catch-all}`) -> `ai-rag-cluster`.
  - Thêm `ai-rag-socratic-v1-route` (`/api/v1/socratic/{**catch-all}`) -> `ai-rag-cluster`.
  - Thêm `ai-rag-academic-graph-v1-route` (`/api/v1/academic-graph/{**catch-all}`) -> `ai-rag-cluster`.
  - Đồng bộ với cấu hình `Configs/V-Eval-Gateway/appsettings.json` của `System-Repo`.
