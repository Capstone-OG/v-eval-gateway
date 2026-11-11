# NHẬT KÝ CẬP NHẬT (UPDATE LOG) - GATEWAY SERVICE

## [11/11/2026] - Chuyển Toàn Bộ Nhánh Sang Main, Triển Khai Docker Container Trên Domain pickic.io.vn & Hoạch Định Kiến Trúc Dự Phòng Đa Node (High Availability & SPOF Mitigation)

- **Triển Khai Đóng Gói Docker Container & Ánh Xạ Domain Công Khai ([`docker-compose.yml`](../../docker-compose.yml))**:
  - Triển khai container `v_eval_gateway` hoạt động trên mạng bridge `capstone_veval_network` cổng `5212:5212`, làm cổng tiếp nhận duy nhất (Single Entry Point) cho toàn bộ hệ thống.
  - Cấu hình Cloudflare Tunnel định tuyến tên miền `https://api.pickic.io.vn` trỏ trực tiếp về Gateway `:5212`.
  - Cập nhật định tuyến động tới các cluster backend trong mạng nội bộ Docker: `http://v-eval-identity-service:5155`, `http://v-eval-content-service:5249`, `http://v-eval-practice-service:5002`, `http://v-eval-ai-engine:5104`.
  - Kiểm thử trực tiếp `curl -s https://api.pickic.io.vn/api/v1/campuses` phản hồi HTTP 200 OK kèm danh sách 2 cơ sở đào tạo, xác nhận Gateway định tuyến xuyên suốt 100%.

- **Nghiên Cứu & Hoạch Định Kiến Trúc Dự Phòng Đa Node (High Availability & SPOF Mitigation)**:
  - Phân tích rủi ro Điểm Tắc Nghẽn Đơn Lẻ (Single Point of Failure - SPOF) khi Gateway chỉ vận hành 1 node duy nhất.
  - Khai thác bản chất **100% Stateless** của Microsoft YARP trên nền Kestrel .NET 9 (xác thực token không trạng thái, không lưu cache nghiệp vụ, bộ nhớ siêu nhẹ ~30-50MB RAM) cho phép mở rộng theo chiều ngang (Horizontal Scaling) thành 2-3+ node Active-Active mà không gặp rào cản đồng bộ bộ nhớ.
  - Thiết kế giải pháp cân bằng tải đa tầng: Tầng 1 (Docker/Single Host) qua Cloudflare Tunnel Ingress / Nginx Upstream Round-Robin kết hợp Liveness Probe `/healthz` và Docker self-healing `restart: always`; Tầng 2 (Production K8s) qua Deployment replicas 2+, Horizontal Pod Autoscaler (HPA) tự động scale theo tải CPU/RAM.
  - Sẵn sàng luận điểm bảo vệ đồ án trước Hội đồng phản biện Capstone về độ sẵn sàng cao và khả năng chịu lỗi của hệ thống.
