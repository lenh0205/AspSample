# Các hàm Javascript được Thư viện (Vgcaplugin.js) cung cấp:
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

===================================================

# Các tiêu chuẩn được sử dụng cho "Chữ ký điện tử"
* **OpenPGP** được hỗ trợ bởi PGP và GnuPG, và các tiêu chuẩn **S/MIME** (_có trong Microsoft Outlook_)

# Mô hình chữ ký điện tử:
* `người nhận` có khả năng **có được khóa công khai của chính người gửi** và có khả năng kiểm tra tính toàn vẹn của văn bản nhận được.
* không yêu cầu giữa 2 bên phải có một kênh thông tin an toàn. 
* Một văn bản được ký có thể được mã hóa khi gửi nhưng điều này không bắt buộc. 
* Việc đảm bảo tính bí mật và tính toàn vẹn của dữ liệu có thể được tiến hành độc lập

# Hệ thống mật mã không đối xứng
* là `hệ thống mật mã` có khả năng **tạo được cặp khóa bao gồm khoá bí mật và khoá công khai**

# Khoá
* là một chuỗi các số nhị phân (0 và 1) dùng trong `hệ thống mật mã`
* -> **Khóa bí mật** là một khóa trong cặp khóa thuộc `hệ thống mật mã không đối xứng`, được dùng để **`tạo chữ ký số`**
* -> **Khóa công khai** là một khóa trong cặp khóa thuộc `hệ thống mật mã không đối xứng`, được sử dụng để **`kiểm tra chữ ký số được tạo bởi khoá bí mật`** tương ứng trong cặp khoá 

# Ký số
* là việc **đưa khóa bí mật vào một chương trình phần mềm** để tự động `tạo và gắn chữ ký số vào thông điệp dữ liệu`

# Sử dụng Chữ ký số
* **Người ký** là thuê bao `dùng đúng khoá bí mật của mình` để `ký số vào một thông điệp dữ liệu dưới tên của mình`

* Mỗi tài khoản sử dụng/ người dùng **bắt buộc phải có một cặp khóa**: **`Khóa Công khai`** và **`Khóa Bảo mật`**
* **Public key:** 
* -> dùng để **`thẩm định Chữ ký số`**, xác thực người dùng của Chữ ký số
* -> là các thông tin công cộng của khách hàng 
* **Private key:** 
* -> dùng để **`tạo Chữ ký số`**
* -> là thông tin bí mật của khách hàng


 
