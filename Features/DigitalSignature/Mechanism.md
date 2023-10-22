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


 
