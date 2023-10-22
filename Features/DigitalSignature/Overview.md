=================================================
# Chương trình ký điện tử - Electronic Signature
* là **`chương trình máy tính`**
* -> được thiết lập để hoạt động độc lập hoặc thông qua thiết bị, hệ thống thông tin, chương trình máy tính khác 
* -> nhằm **tạo ra một chữ ký điện tử đặc trưng** cho `người ký thông điệp dữ liệu`

# Chữ ký điện tử
* là **thông tin đi kèm** theo **dữ liệu**: `văn bản, hình ảnh, video...` 
* -> nhằm mục đích **xác định người chủ của dữ liệu** đó
* -> **dữ liệu đó có bị thay đổi hay không** 

# Chứng thực
* **Chứng thực chữ ký điện tử** là việc xác nhận cơ quan, tổ chức, cá nhân được chứng thực **`là người ký chữ ký điện tử`**
* **Dịch vụ chứng thực chữ ký số** là một loại hình dịch vụ **`chứng thực chữ ký điện tử`** do tổ chức cung cấp dịch vụ chứng thực chữ ký số cấp, bao gồm:
* -> _Tạo cặp khóa bao gồm khóa công khai và khóa bí mật cho thuê bao;_
* -> _Cấp, gia hạn, tạm dừng, phục hồi và thu hồi chứng thư số của thuê bao;_
* -> _Duy trì trực tuyến cơ sở dữ liệu về chứng thư số;_

# Chứng thư 
* **Chứng thư điện tử** là **thông điệp dữ liệu** do `tổ chức cung cấp dịch vụ chứng thực chữ ký điện tử` phát hành nhằm **`xác nhận cơ quan, tổ chức, cá nhân được chứng thực là người ký chữ ký điện tử`**
* **Chứng thư số** là một dạng **`chứng thư điện tử`** do `tổ chức cung cấp dịch vụ chứng thực chữ ký số cấp`

# Điều kiện đảm bảo an toàn cho chữ ký số
* Chữ ký số được tạo ra trong **`thời gian chứng thư số có hiệu lực`** và **`kiểm tra được bằng khoá công khai ghi trên chứng thư số`** có hiệu lực đó
*  Chữ ký số được tạo ra bằng việc **`sử dụng khoá bí mật tương ứng với khoá công khai`** ghi trên chứng thư số do tổ chức cung cấp dịch vụ được chính phủ cấp phép

================================================
# chữ ký số - Digital Signature
* **chữ ký số** là 1 dạng **chữ ký điện tử** (_nhưng đảm bảo bảo mật, toàn vẹn_)
* -> được tạo ra bằng sự `biến đổi` **một thông điệp dữ liệu** sử dụng **hệ thống mật mã không đối xứng**
* -> theo đó, người có được **thông điệp dữ liệu ban đầu** và **khóa công khai của người ký** có thể xác định được chính xác:
* - **`Việc biến đổi được tạo ra bằng đúng khóa bí mật tương ứng với khóa công khai trong cùng một cặp khóa`** 
* - Sự **`toàn vẹn nội dung của thông điệp dữ liệu`** kể từ khi thực hiện việc biến đổi nêu trên

=================================================
# Lợi ích
* trao đổi dữ liệu thuận lợi , tối ưu thời gian, chi phí, tránh phải thực hiện các giấy tờ, thủ tục rườm rà
* **thừa nhận về mặt pháp lý** `khi giao dịch trên môi trường điện tử`
* -> Chữ ký số cho **`doanh nghiệp, tổ chức`**: _kê khai, nộp thuế, thuế hải quan, cổng thông tin quốc gia, trực tuyến,... và tổ chức còn sử dụng chữ ký số trong việc ký văn bản nội bộ, giao dịch ngân hàng điện tử, ký hợp đồng với đối tác,..._
* ->  Chữ ký số cho **`cá nhân/cá nhân thuộc tổ chức/doanh nghiệp`**: _nộp thuế thu nhập cá nhân, khai báo trên trang đăng ký kinh doanh hay ký hợp đồng lao động với đơn vị sử dụng lao động,..._s

* đặc điểm:
* ->  **Tính xác thực**: Thông qua chứng thư số của cá nhân, tổ chức, doanh nghiệp, chữ ký số có thể giúp **`xác thực danh tính chủ nhân của chữ ký số`**
* -> **Tính bảo mật**: Chữ ký số có tính **`bảo mật gần như tuyệt đối`** và thông tin không dễ bị đánh cắp bởi các hacker. Vì chữ ký số **`có tới 2 lớp mã khóa bảo mật`** đó là khóa bí mật và khóa công khai.
* -> **Tính toàn vẹn**: Văn bản/tài liệu `có chữ ký số` **`chỉ có thể được mở bởi duy nhất một người đó là người nhận văn bản/tài liệu đó`**. Vì vậy, trong môi trường giao dịch điện tử, mọi thông tin của tài liệu/văn bản đều được đảm bảo toàn vẹn một cách tuyệt đối. 
* -> **Tính chống chối bỏ**: Khi các văn bản/tài liệu/hợp đồng đã có chữ ký số thì chữ ký số này không thể thay thế cũng không thể xóa bỏ. 

===================================================

## Nhà cung cấp dịch vụ Chữ ký số được Nhà nước cấp phép:
* VINA – CA, FPT, Viettel, VNPT, Bkav, Nacencomn,…

## 4 loại chữ ký số được sử dụng hiện nay:
* **Chữ ký số USB Token**
* **Chữ ký số HSM**: được cài đặt cho các ứng dụng chữ ký số với **`yêu cầu tốc độ cao`**, đáp ứng việc xác thực và mã hóa ngay lập tức (_cho phép người dùng có thể cùng lúc thực hiện hàng nghìn chữ ký, thay vì 4 - 5 chữ ký như khi sử dụng USB Token_) 
* **Chữ ký số SmartCard**: **`tích hợp trên sim điện thoại`** - nhanh chóng và dễ dàng thực hiện ký số ngay trên điện thoại di động của mình mọi lúc mọi nơi
* **Chữ ký số từ xa - Remote Signature**: được sử dụng trên nền tảng công nghệ **`điện toán đám mây`**; thực hiện ký số mọi lúc mọi nơi, như trên điện thoại, laptop, máy tính bảng,.... nhưng còn một số vấn đề liên quan đến bảo mật dữ liệu

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

=============================================
# Lỗi "không tìm thấy chữ ký số", "chữ ký số không hợp lệ"

## Vấn đề với "USB Token EFY"
* **USB Token EFY** ta cắm không đúng với công ty mà ta đang thực hiện kê khai 
* cắm USB Token của công ty này nhưng login **tài khoản nộp thuế** của công ty khác

# Driver của chữ ký số bị lỗi
* có thể do `USB Token đã sử dụng lâu`

# Nhập sai mã pin
* Hầu hết `USB Token` chỉ cho phép gõ sai **mã pin** một số lần nhất định (khoảng 5 lần). 
* Nếu sai quá số lần quy định, phải liên hệ **nhà cung cấp chữ ký số** để được mở khóa hoặc có thể tự lấy lại mật khẩu Token tại nhà

# Vấn đề với số serial trên USB Token 
* **số serial trên USB Token** không trùng khớp với số serial khai báo trên Tài khoản trang khai báo

* -> có thể là do USB Token của ta mới được khởi tạo hoặc gia hạn, 
* => khi có sự tác động vào thông tin trong thiết bị USB Token sẽ tự động cập nhật dãy serial mới 
* => nhằm đảm bảo bảo mật đề phòng trường hợp chữ ký số bị người lạ lấy được mật khẩu.

* **Giải pháp**: thay đổi serial của USB Token trên **trang Thuế**

================================================
# Cài đặt "chữ ký số" sử dụng trên "Phần mềm Hệ thống quản lý văn bản và điều hành"

## Download bộ "vgca sign web"
* Truy cập vào trang: `https://ca.gov.vn/tai-phan-mem-`
* Chọn bộ cài đặt  để tải bộ `Driver` tương ứng với Window OS (x32 , x64):
* -> tải **vgca-sign-web** - Bộ công cụ ký số phục vụ liên thông (xác thực chéo) các hệ thống CA
* -> tải driver **gca01-client-v2-x64-8.3** - Trình điều khiển thiết bị (Driver) GCA-01

## Install trình điều khiển Chữ ký số (eToken Driver)
* chạy tệp install wizard `gca01-client-v2-x64-8.3` ta vừa tải về (_khi xong sẽ xuất hiện icon "S" màu đỏ trên Taskbar_)
* giải nén bộ `vgca-sign-web` rồi chạy file `VGCASignServiceSetup_v1.0.0` (_khi xong sẽ xuất hiện icon "V" màu đỏ trên Taskbar_)

* để **Cấu hình và thiết lập chữ ký số**
* -> right-click on "V" icon; chọn `Cấu hình hệ thống` window
* -> trong tab `kết nối mạng`, chọn **Sử dụng máy chủ proxy**, **Sử dụng cấu hình proxy mặc định** rồi "Lưu"
* -> trong tab `Dịch vụ chứng thực`, check **Sử dụng dịch vụ cấp dấu thời gian (TSA)**, địa chỉ nhập: **http://ca.gov.vn/tsa**, check **Sử dụng dịch vụ kiểm tra chứng thư số trực tuyến** rồi "Lưu"
* -> trong tab `Hiển thị chữ ký trên PDF`, quản lý mẫu chữ ký chọn `Chữ ký các nhân`, rồi điền tên, `ảnh chữ ký`
* -> trong tab `Đăng ký sử dụng`: Cắm `USB chữ ký số` vào máy tính và nhất nút "Đăng ký sử dụng"
* -> **ký số trên trình duyệt Chrome**: mở Chrome vào đường dẫn `chrome://flags/#allow-insecure-localhost`
* -> vào mục **Allow invalid certificates for resources loaded from localhost**: chọn Enable -> rồi chọn `RELAUNCH NOW` để khởi động lại trình duyệt