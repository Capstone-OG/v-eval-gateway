# UPDATE LOG — V-Eval Gateway

## [24/11/2026] Bổ sung Route YARP cho Non-Blocking Roadmap Approval
- Cập nhật `V-Eval-Gateway.API/appsettings.json`: Bổ sung route `practice-roadmaps-v1-route` chuyển tiếp `/api/v1/roadmaps/{**catch-all}` sang `practice-service-cluster` phục vụ API `PUT /api/v1/roadmaps/{roadmapId}/approve`.
