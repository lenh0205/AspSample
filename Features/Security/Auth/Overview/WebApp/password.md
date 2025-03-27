
## Password

### Lưu mật khẩu:
* **password** - user provide khi đăng ký tài khoản
* **salt** - a random string do hệ thống tự sinh ra (tạo độ nhiễu cho mã băm)
* dùng **Hash function** để tạo mã băm độc nhất từ chuỗi kết hợp `password` + `salt`
* **`Lưu mã băm + salt + 1 số thông tin khác vào database`** 

### Kiểm tra mật khẩu:
* **password** - user enter password
* lấy **salt** lưu trong database theo **`id`** hoặc **`username`**
* lấy **password** vừa nhập + **salt** vừa lấy từ database cộng lại; rồi dùng **hash function** để tạo ra 1 mã băm
* **`so sánh mã băm này với mã băm trong database`** -> khớp thì mật khẩu chỉnh xác

### Password Hashing Algorithms - bcrypt
* **scrypt**, **argon2** cũng rất tốt nhưng vẫn cần xem xét; hiện tại tố nhất vẫn là **`bcrypt`**
* -> ensure **one-way function** and **same input same output**

```js - "bcrypt" function
var password = 'hi';
var hash = bcrypt(password); // $2a$10.....
```

=================================================
# Có 3 kiểu Authentication
* Knowledge-based: Something a user knows (VD: password)
* Possession-based: Something a user has (VD: điện thoại, thẻ ATM, tài khoản Google)
* Inheritance-based: Something a user is (VD: vân tay, võng mạc)

# Trusted path - đường dẫn an toàn
* Ví dụ như s/d Http + SSL để mã hoá thông tin giao tiếp, vì internet là 1 không gian public không an toàn
* hoặc là dùng VPN

# Password Authentication:
* user nhập 1 **unique ID + key** -> are then checked against `stored credentials`

* -> Cost (_passwords là free mà_); convenient (_không cần thiệt bị đặc biệt; easier_)
* -> Esier for system administrator to reset password so với việc cấp lại dấu vân tay mới
* -> nhưng thường dễ bị lộ

# Password vs Key
* **Crypto keys** 
```cs
// Giả sử có 1 khoá 64 bits <=> 1 tổ hợp 2^64 keys 
// => sẽ phải thử tối đa 2^64 lần để tìm được đúng keys
```

* **Password**
```cs
// 1 mật khẩu có 8 ký tự; mà ta có 256 ký tự khác nhau tất thảy 
// <=> có thể có đến 256^8 mật khẩu <=> 2^64 mật khẩu 
// nhưng mà password không có tính random như key (tức là user đưa ra mật khẩu theo 1 rule nào đó dễ nhớ) 
```
