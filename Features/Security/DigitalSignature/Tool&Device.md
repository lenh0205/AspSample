===================================================
## USB Token
* **loại chữ ký số sử dụng chiếc USB ký số để tích hợp phần mềm** (_phổ biến nhất hiện nay_)
* thiết bị `phần cứng (USB)` dùng để `tạo ra cặp khóa công khai và bí mật`; cũng như lưu trữ thông tin của khách hàng
* **`Bản chất`** của USB Token là để lưu trữ và bảo vệ an toàn cho **Private Key**

* Mỗi USB Token có một số **series duy nhất** gồm `8 hoặc 10 ký tự` ở `mặt dưới của Token` và được **gắn duy nhất với một khách hàng**

* Khi nhấn nút trên Token một dãy các mã số ngẫu nhiên sẽ xuất hiện (gồm 6 chữ số hiện ra trên màn hình phía trên Token) và thay đổi liên tục trong một khoảng thời gian nhất định (30 giây hoặc 60 giây)
* -> Mỗi một mã số của USB Token **`chỉ có hiệu lực duy nhất`** đối với một giao dịch tại một thời điểm nhất định và mỗi khách hàng cụ thể.
* -> Chuỗi số được tạo ra **`theo thuật toán rất phức tạp`** mà cho đến nay chưa có trường hợp nào bẽ khóa thành công

* **Cách sử dụng**:
* -> cần cài đặt tiện ích ký số trên máy tính trước
* -> USB sẽ được cắm trực tiếp vào máy tính
* -> người dùng tiến hành đăng nhập vào chữ ký số của mình bằng một mã PIN bí mật
* -> thực hiện các thao tác giao dịch

=======================================================
# 1 số phần mềm Tool 
* **SaveAsPDFandXPS.exe** - phần mềm chuyển đổi MS Office sang PDF
* `UltraViewerQS` - Phần mềm điều khiển máy tính từ xa
* `VGCAUnlockTokenSetup` - Công cụ hỗ trợ khôi phục thiết bị lưu khóa bí mật
* `bit4id_xpki_1.4.10.649-ng-user-vgca-crtmgr` - Trình điều khiển thiết bị USB Token Bit4ID
* `TokenManager` - Trình điều khiển thiết bị
* `mpkicrypto` - Bộ công cụ tích hợp ký số và xác thực văn bản điện tử (PDF) trên thiết bị di động (_quy định tại Nghị đinh số 30/2020/NĐ-CP_)
* `SDK-MobilePKI` - Bộ công cụ hỗ trợ phát triển ứng dụng ký số trên thiết bị di động sử dụng SIM-PKI
* `CACertUtil` - Công cụ cài đặt chứng thư số CA
* `VGCA Renew Tool` - Công cụ hỗ trợ gia hạn chứng thư số
* `vSign.apk` - Phần mềm vSign trên Android sử dụng SD Secure
* `vSignPDF` - Phần mềm vSignPDF hỗ trợ ký số và xác thực tài liệu điện tử định dạng PDF, triển khai cho cơ quan Đảng và Nhà nước