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

## 4 loại chữ ký số được sử dụng hiện nay:
* **Chữ ký số USB Token**
* **Chữ ký số HSM**: được cài đặt cho các ứng dụng chữ ký số với **`yêu cầu tốc độ cao`**, đáp ứng việc xác thực và mã hóa ngay lập tức (_cho phép người dùng có thể cùng lúc thực hiện hàng nghìn chữ ký, thay vì 4 - 5 chữ ký như khi sử dụng USB Token_) 
* **Chữ ký số SmartCard**: **`tích hợp trên sim điện thoại`** - nhanh chóng và dễ dàng thực hiện ký số ngay trên điện thoại di động của mình mọi lúc mọi nơi
* **Chữ ký số từ xa - Remote Signature**: được sử dụng trên nền tảng công nghệ **`điện toán đám mây`**; thực hiện ký số mọi lúc mọi nơi, như trên điện thoại, laptop, máy tính bảng,.... nhưng còn một số vấn đề liên quan đến bảo mật dữ liệu

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

