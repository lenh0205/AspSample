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

# the idea of "Session":
* -> **HTTP is a stateless protocol** (_if we login the first time; send email and password to Server to Authenticate; if we click a link to go to page need Authen, we have to login again_)
* -> **`Web Server has no way to remember who you are`**

* => vậy nên ta cần Web Server tell the Browser to remember who we are when we authenticate 
* => that way, the browser every time request a new page can talk back to Server "who I am"

## Cookies
* this works behind the scene is via **Cookies** - **`just a string`** (_not files,..._), get pass in a request

## HTTP Request:
* have 2 component: 
* -> Request Headers - contain some metadata (key/value pair) about a request
* -> Request Body - where information is sent and received 
```js - VD về "Request body":
// Khi ta make GET request for a web page, the HTML code is in the body 
// khi ta send data to a web server; the data is in the Request Body
```

```json - VD về "Cookie" header
{
    "Cookie": "session=12345"
}
// name of cookie is "session", value is "12345"
// ta có thể thêm multiple cookies bằng ";"
```

```json - Khi ta muốn set 1 Cookie (VD: khi ai đó login và cần remember value)
{
    "Set-Cookie": "session=12345"
}
```

## client-sessions - Cấu hình Middleware cho session

```js - secret, duration, name of "session"
const sessions = require("client-sessions"); // use strong cryptograhy and signing Augorithms
app.use(sessions({
    cookieName: "session",
    secret: "ldfgjl", // random
    duration: 30 * 60 * 1000 // 30 mins - How long will allow user to stay login 
}))
```
* the **secret** use for handling **`encryption of "Cookies"`** behind the scenes
* => need to be the same on all of our Web Servers; but should never be checked into version control or public on the internet

* **encrypted cookie** - ta login -> view the HTTP header `Set-Cookie` that server send back to us  

# Storing password
* vấn đề: **`password đang được lưu ở dạng plain text trong bảng User`**

* **Password Hashing** - take a password and hash it, it'll generate a long random string
* -> **`if we hash the same password, we're always get the same long random string`**
* -> **one-way function** - **`we can't never can turn the hashed string back to original password`** 
* => useful for storing sensitive information

* **`Password Hashing Algorithms`**: md5, sha256, **bcrypt**, scrypt, argon2, ...
* **`scrypt, argon2`** cũng rất tốt nhưng vẫn cần xem xét
* -> for now, safe bet is **bcrypt**

```js - "bcrypt" function
var password = 'hi';
var hash = bcrypt(password); // $2a$10.....
```
