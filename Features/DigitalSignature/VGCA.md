> https://chukyso.ict-backan.gov.vn/uploads/news/2016_11/hd-su-dung-bit4id.pdf 
> https://binhphuoc.gov.vn/uploads/binhphuoc/news/2019_04/tailieuhuongdancaidatvasudungchukyso.pdf 

# 1 số dịch vụ chứng thực kỹ thuật số công cộng tại Việt Nam
* **`VGCA`**  do Công ty Cổ phần Viễn thông Viettel (Viettel) cung cấp
* **`VNPT-CA`** do Tập đoàn Bưu chính Viễn thông Việt Nam (VNPT) cung cấp
* **`CA247`** của Công ty Cổ phần Công nghệ Dịch vụ Trực tuyến 247
* **`NAPAS eSign`** của Công ty Cổ phần Thanh toán Quốc gia Việt Nam
* **`EDOC`** của Công ty Cổ phần Công nghệ EDOC

# Dịch vụ chứng thực chữ ký số "VGCACrypto" trên nền tảng Web
* do **Ban Cơ yếu Chính phủ cung cấp** vào các hệ thống thông tin: _dịch vụ công trực tuyến; hệ thống quản lý văn bản, điều hành; hệ thống thông tin chuyên ngành…_

* `Giải pháp hỗ trợ tích hợp ký số, xác thực trên môi trường Web` cho phép người sử dụng **`thao tác trực tiếp trên nền tảng Web`** để thực hiện ký số và xác thực văn bản, tài liệu điện tử. 
* `Các ứng dụng điều hành tác nghiệp, dịch vụ công trực tuyến, hệ thống thông tin chuyên ngành,…` được phát triển trên nền tảng Web **đều có thể sử dụng giải pháp này**

# Các "tính năng" cung cấp của giải pháp bao gồm: 
* -> Hỗ trợ đa trình duyệt (Internet Explorer, FireFox, Chrome, Opera…); 
* -> Kiểm tra chứng thư số trực tuyến qua danh sách hủy bỏ trực tuyến (CRLs) hoặc máy chủ trạng thái chứng thư số trực tuyến (OCSP); 
* -> Lấy dấu thời gian chuẩn từ Ban Cơ yếu Chính phủ; 
* -> Hỗ trợ ký số và xác thực các định dạng tài liệu điện tử (Portable Document Format (.pdf), Text)

# Setting - để triển khai giải pháp 
* **`> = .Net Framework 4.0`** platform
* hỗ trợ các hệ điều hành Windows 7, 8, 10, Windows Server 2008, 2012
* **Thư viện** được cài đặt và cấu hình trên `Windows` dưới dạng **Windows Services**
* sử dụng các **hàm JavaSript trên các trình duyệt Web** để gọi các `hàm ký số, xác thực và nhận kết quả`
* `thư viện` sẽ `tự động kết nối tới các thành phần trực tuyến` như CRLs, OCSP, TSA,… của Ban Cơ yếu Chính phủ và **thiết bị lưu khóa bí mật của người ký**

# Bộ cài đặt của Thư viện
* **`VGCASignServiceSetup.msi`** - File cài đặt **phần mềm dịch vụ ký số**
* **`Vgcaplugin.js`** - File **thư viện hàm ký số xác thực** `tích hợp trên website`
* **`Base64.js`** - Các **hàm chuyển đổi dữ liệu dạng text, Base64**

# Các hàm Javascript được Thư viện cung cấp:
* **exc_sign_msg()** - để thực hiện **`ký số dữ liệu web-form`**, với tham số:
* -> `sender` là Id của button submit form 
* -> `sender` sẽ được xử lý ở trong hàm **SignCallBack**

* **SignCallBack()** để **`xử lý kết quả ký số`**

* **exc_verify_msg()** để thực hiện **`xác thực nội dung web*form`**

* **VerifyCallBack()** để **`xử lý kết quả xác thực`**

* **exc_verify_pdf()** là hàm **`xác thực tệp pdf`** với hai tham số _`sessionId`_ và _`filename`_ để sử dụng trong `quá trình tải file PDF` về máy tính người dùng, sau đó thực hiện **`xác thực chữ ký`**

* **VerifyPDFCallBack()** là hàm **`xử lý kết quả xác thực tệp PDF`**

* **exc_sign_file()** là **`hàm ký số tệp PDF`** với các tham số _`sessionId, fileName`_ để sử dụng trong quá trình **`tải về file cần ký số trên server`** và **`tải lên file đã ký số lên server`** 
* -> trong trường hợp fileName rỗng, phần mềm sẽ yêu cầu người dùng chọn đường dẫn file trên máy tính để ký số trên một cửa sổ Browse File. 
* -> Tham số `metadata` là các thuộc tính đi kèm với tệp có kiểm List trong đó `KeyValue` là class KeyValue{ string Key; string Value;}

* **SignFileCallBack()** **`xử lý kết quả ký số`** với tham số rv là một `json object` có cấu trúc:
```js
{ 
    "Status": 0, // 0: ký số thành công, khác 0 ký số lỗi
    "Message": "" //miêu tả lỗi
    "FileName": "" //Tên file ký số
    "FileServer": ""// đường dẫn tệp đã ký trên máy chủ do FileUPloadHandler trả về 
}
```

# Tích hợp:
* **Thêm các file JavaScript sau vào trang web**:
* -> **`base64.js`**, để **chuyển đổi dữ liệu web-form cần ký số sang dạng chuỗi**
* -> **`vgcaplugin.js`**, để **thực thi các hàm ký số** và **xác thực với phần mềm plugin ký số**

* **thêm các Script lên trang**
* -> `nhúng script base64.js`
* -> `nhúng script vgcaplugin.js`

* **s/d các hàm ký số và xác thực** 

==============================================
# Tool ký số VGCASignService
* là **giải pháp hỗ trợ tích hợp chữ ký số** và các **dịch vụ chứng thực chữ ký số** **trên nền tảng WEB**(_theo quy định tại Nghị đinh số 30/2020/NĐ-CP_)
* được xây dựng dưới dạng **`phần mềm plugin cho các trình duyệt`**
* sử dụng các `dịch vụ chứng thực chữ ký số` của **`hệ thống cung cấp dịch vụ chứng thực chữ ký số chuyên dùng phục vụ các cơ quan thuộc hệ thống chính trị`**
* do **Ban Cơ yếu Chính phủ** cung cấp được publish trên trang chủ của Cục Chứng thực số và Bảo mật thông tin. (**https://ca.gov.vn/tai-phan-mem**)

* Bộ công cụ:
* -> `VGCASignServiceSetup.msi` - **Tool ký số với USB Token** cài đặt trên máy tính người dùng
* -> `demo/vgcaplugin.js` - **Thư viện Javascript** nhúng vào trong site sẽ tích hợp ký số
* -> `demo/test-bc.htm` - `file demo gọi hàm ký số`, get version tool ký số
* -> `demo/Demo.html` - `file demo gọi các hàm ký số`

=====================================================
# Setup trình điều khiển thiết bị TokenMe Evo – Bit4id 
## Infastructure
* cần có **`Thiết bị TokenMe EVO – Bit4id`** là **thiết bị PKI USB Token** 
* Tải và chạy setup wizard tệp Driver **Trình điều kiển thiết bị USB Token Bit4ID** (từ trang chủ https://ca.gov.vn/tai-phan-mem)
* vào menu Start → Bit4id để kiểm tra có chưa (_icon hình cái sim_)

## Đổi "password" cho thiết bị USB Token:
* -> Cắm thiết bị USB Token vào cổng USB của máy tính, thấy `đèn xanh` nhấp nháy
* -> right-click on "Bit4id" icon và chọn `Bit4id – Quản trị Token` để hiển thị thông tin của USB Token
* -> chọn "Đăng nhập" và nhập Mã PIN
* -> Để xem chi tiết thông tin chứng thư số, nhấn chọn "Xem chứng thư số"
* -> Nhấn chọn `Thay đổi mã Pin` để thay đổi mật khẩu thiết bị (mật khẩu đăng nhập)

* _mật khẩu mới cần có độ dài ít nhất 4 ký tự và tối đa 16 ký tự_
* _`mặc định` của thiết bị USB Token, người dùng nhập sai mật khẩu liên tiếp `quá 10 lần`, thì USB Token sẽ tự động khóa và người dùng sẽ không tiếp tục sử dụng được USB Token_
* _Để mở khóa thiết bị người sử dụng phải liên hệ và chuyển thiết bị về cho các cơ quan đăng ký để thực hiện mở khóa_

# Setup Trình điều khiển thiết bị eToken Safenet – TokenManager
## Infastructure
* cần có **`Thiết bị eToken Safenet – TokenManager`** là 1 **USB Token**
* tải và chạy setup wizard Driver **Trình điều khiển thiết bị eToken Safenet – TokenManager**
* vào menu Start → SafeNet → SafeNet Authentication Client xem có chưa (_icon chữ S màu đỏ_)

## Đổi mật khẩu cho thiết bị eToken
* -> Cắm thiết bị USB Token vào cổng USB của máy tính, thấy `đèn đỏ` nhấp nháy
* -> right click on "Safenet" icon và chọn `Đổi mật khẩu của token` và đổi mật khẩu

* _Mật khẩu mới phải có độ dài ít nhất 8 ký tự, phải chứa chữ hoa, chữ thường và số_
* _`mặc định` của thiết bị USB Token, người dùng nhập sai mật khẩu liên tiếp `quá 15 lần`, thì USB Token sẽ tự động khóa và người dùng sẽ không tiếp tục sử dụng được USB Token!_
* _Để mở khóa thiết bị người sử dụng phải liên hệ và chuyển thiết bị về cho các cơ quan đăng ký để thực hiện mở khóa_

=================================================
# cài đặt và cấu hình tool ký số VGCASignService
## Yêu cầu:
* `OS`: Windows phiên bản XP SP3 trở lên.
- `RAM`: 512Mb trở lên.
- `disk`: 10Gb trở lên.
- `.Net Framework 4.0`

## Cài đặt
* chạy setup wizard file `VGCASignServiceSetup.msi` trong bộ công cụ đã tải về
* vào Start -> VGCASignService xem có chưa (_icon chữ V màu đỏ_)

## Đăng ký sử dụng phần mềm: 
### Đăng ký thủ công
* right click on "VGCASignServiceSetup" tool -> Cấu hình hệ thống -> Đăng ký sử dụng -> Đăng ký thủ công
* Trên "Đăng ký sử dụng phần mềm thủ công" window, chọn nút "Khởi tạo yêu cầu"

* Trên "Khởi tạo yêu cầu đăng ký sử dụng phần mềm"
* -> nhập `"Mã đăng ký phần mềm"` (_Lưu ý: Liên hệ với Cục Chứng thức số và Bảo mật thông tin để nhận mã đăng ký phầm mềm_) 
* -> chọn "Chứng thư số người ký" để đăng ký sử dụng 

* nhấn nút "Khởi tạo"
* -> phần mềm thực hiện sinh mã yêu cầu kích hoạt
* -> Copy và dán mã yêu cầu kích hoạt nhận được lên website **https://cms.ca.gov.vn/Request.aspx** (_mở bằng tap ẩn danh_)
* -> chọn nút "Gửi yêu cầu"

* Sau khi gửi yêu cầu đi, hệ thống sẽ tạo License Key để sử dụng phần mềm. 
* -> Trên thanh tìm kiếm, nhập thông tin đăng ký (Họ tên, Cơ quan, Đơn vị,…)
* -> nhấn chọn "Tìm kiếm"
* -> Chọn vào kết quả phù hợp rồi copy mã kích hoạt phần mềm (License Key) trên thông tin hiển thị 

* Quay lại giao diện "Đăng ký sử dụng phầm mềm thủ công" 
* -> chọn mục "Bước 2: Kích hoạt phần mềm"
* -> dán mã kích hoạt rồi nhấn nút "Kích hoạt phần mềm"
* -> Hệ thống thông báo "kích hoạt thành công" là OK

### Đăng ký nhanh
* right-click on logo của tool VGCASignService, chọn "Đăng ký sử dụng"

* Trên cửa sổ "Đăng ký sử dụng phần mềm"
* -> nhập "Mã đăng ký phần mềm" (_Lưu ý: Liên hệ với Cục Chứng thức số và Bảo mật thông tin để nhận mã đăng ký phầm mềm_)
* -> chọn Chứng thư số người ký để đăn ký sử dụng 

* Nhấn chọn "Đăng ký" và đợi thông báo đăng ký thành công

## Cấu hình dịch vụ chứng thực
* Cấu hình sử dụng dịch vụ chứng thực chữ ký số của tổ chức cung cấp dịch vụ chứng thực **`đã được thiết lập mặc định trong phần mềm`** 

* Nếu muốn thay đổi cấu hình:
* -> Chọn chuột phải vào logo của tool VGCASignService, chọn "Cấu hình hệ thống"
* -> Trên giao diện của chức năng "Cấu hình hệ thống" chọn mục "Dịch vụ chứng thực"

* -> Cấu hình sử dụng dịch vụ cấp dấu thời gian, nhằm mục đích gắn dấu thời gian cho chữ ký. Tích chọn "Sử dụng dịch vụ cấp dấu thời gian (TSA)", nhập địa chỉ máy chủ cấp dấu thời gian vào khung Địa chỉ: http://ca.gov.vn.
* -> Cấu hình sử dụng dịch vụ kiểm tra trạng thái thu hồi của chứng thư số. Tích chọn Sử dụng dịch vụ kiểm tra trạng thái thu hồi của chứng thư số"
* -> Tích chọn "Cho phép kiểm tra chứng thư số người ký qua OCSP" để sử dụng dịch vụ Trạng thái chứng thư trực tuyến (OCSP), mục đích là chỉ định sử dụng dịch vụ OCSP thay vì kiểm tra trong danh sách thu hồi (CRLs)
* -> Thêm hoặc xóa danh sách thu hồi (CRLs)

* chọn "Lưu"

## Cấu hình mẫu chữ ký

### Cấu hình form cho "Lãnh đạo ký số phê duyệt công văn"
* Mở tab "Mở cấu hình Hiển thị chữ ký trên PDF"
* Chọn "Tạo mẫu mới…"
* Nhập "tên mẫu" lãnh đạo ký phê duyệt (Ví dụ: Phó Cục trưởng – Lê Quang Tùng)
* Chọn hiện thị chữ ký: "Hình ảnh"
* bấm chuột phải vảo hình ảnh chữ ký và chọn menu "Thay ảnh khác" -> Chọn **`ảnh chữ ký của lãnh đạo`**, định dạng .png 
* `Nhập độ rộng và độ cao của ảnh chữ ký` theo đơn vị Points (1 point = 1 pixel x 96 / 72)
* Nhập vào "thông tin người ký" tên `lãnh đạo ký số công văn` để tự động tìm kiếm vị trí chữ ký.
* Các thông tin khác không cần thay đổi
* Bấm Lưu để tạo và lưu mẫu 

### Cấu hình cho "Văn thư ký số phát hành công văn"
* Tạo mẫu **`số công văn đi`**
* -> Nhập tên mẫu (Ví dụ: Số công văn đi)
* -> Loại chữ ký: Mẫu số công văn đi
* -> Hiển thị chữ ký: "Thông tin"
* -> Cỡ chữ: 13

* Tạo mẫu **`ngày công văn đi`**
* -> Nhập tên mẫu (Ví dụ: Ngày công văn đi)
* -> Loại chữ ký: Mẫu ngày công văn đi
* -> Hiển thị chữ ký: Thông tin
* -> Cỡ chữ : 13

* Tạo mẫu **`Mẫu chũ ký tổ chức tương ứng với lãnh đạo ký số công văn`**
* Tên mẫu (Ví du: Dấu tổ chức)
* Loại chữ ký: Mẫu chữ ký tổ chức
* Hiển thị: Hình ảnh
* Thay ảnh dấu của đơn vị
* Độ cao bằng độ cao của ảnh theo đơn vị Points ( 1px = 0.75point)
* Độ rộng bằng độ rộng của ảnh con dấu của tổ chức theo đơn vị point
* Nhập họ tên lãnh đạo ký số công văn.

===============================================

# Chức năng ký số
* https://chukyso.ict-backan.gov.vn/uploads/news/2016_11/hd-su-dung-bit4id.pdf 

## Ký số lãnh đạo (ký phê duyệt)
* Về cơ bản thì ta mở `công văn` trên giao diện của **`VGCASignService`** ta chọn chức năng `ký số`; 
* rồi hoặc ta "chọn vị trí ký" và di chuyển đến vị trí đặt chữ ký số; hoặc Chọn nút "Ký số", **`phần mềm tự động xác định vị trí đặt chữ ký số`** theo thông tin đã **`cấu hình trong mẫu chữ ký số`**
*  chọn **mẫu chữ ký số** _VD: Phó cục trưởng - Lê Quang Tùng_ và **chứng thư số** để ký số
* **nhập mật khẩu** và Ok

* => và h ta đã có 1 công văn dạng pdf với phần chữ ký có "chữ ký tay" của lãnh đạo như bình thường
