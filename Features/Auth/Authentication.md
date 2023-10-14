# Quản lý mật khẩu
## Lưu mật khẩu:
* **password** - user provide khi đăng ký tài khoản
* **salt** - a random string do hệ thống tự sinh ra (tạo độ nhiễu cho mã băm)
* dùng **Hash function** để tạo mã băm độc nhất từ chuỗi kết hợp `password` + `salt`
* **`Lưu mã băm + salt + 1 số thông tin khác vào database`** 

## Kiểm tra mật khẩu:
* **password** - user enter password
* lấy **salt** lưu trong database theo **`id`** hoặc **`username`**
* lấy **password** vừa nhập + **salt** vừa lấy từ database cộng lại; rồi dùng **hash function** để tạo ra 1 mã băm
* **`so sánh mã băm này với mã băm trong database`** -> khớp thì mật khẩu chỉnh xác

# Hashing (bcrypt)