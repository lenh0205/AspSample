# Password

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

## Password Hashing Algorithms - bcrypt
* **scrypt**, **argon2** cũng rất tốt nhưng vẫn cần xem xét; hiện tại tố nhất vẫn là **`bcrypt`**
* -> ensure **one-way function** and **same input same output**

```js - "bcrypt" function
var password = 'hi';
var hash = bcrypt(password); // $2a$10.....
```

# the idea of "Session":
* -> **HTTP is a stateless protocol** (_if we login the first time; send email and password to Server to Authenticate; if we click a link to go to page need Authen, we have to login again_)
* -> **`Web Server has no way to remember who you are`**

* => vậy nên ta cần Web Server tell the Browser to remember who we are when we authenticate 
* => that way, the browser every time request a new page can talk back to Server "who I am"

## Cấu hình Session để lưu user info

* -> Cách 1: sử dụng **`client-sessions`** để lưu trực tiếp **encrypted session data (VD: user info) trong Cookie luôn**, bị cái Cookie chỉ lưu được 4KB
```js - secret, duration, name of "session"
const sessions = require("client-sessions"); // use strong cryptograhy and signing Augorithms
app.use(sessions({
    cookieName: "session",
    secret: "ldfgjl", // random
    duration: 30 * 60 * 1000 // 30 mins - How long will allow user to stay login 
}))
```

* -> Cách 2: sử dụng **`express-session`** (thường dùng cho production) để lưu **session data ở Server** và và lưu **SessionID** trong Cookie
```js
const session = require("express-session");

// Middleware to set up express-session
app.use(session({
    secret: "your-secret-key",  // Secret for signing the session ID cookie
    resave: false,  // Prevents saving unchanged sessions
    saveUninitialized: true,  // Save uninitialized sessions
    cookie: { maxAge: 30 * 60 * 1000 }  // 30 min session expiration
}));
```

## Usage
* -> cấu hình khác nhau thì bên dưới áp flow khác nhau thôi, nhưng lúc dùng thì giống như nhau
```js
// store session data when login
app.post("/login", (req, res) => {
    const { username, password } = req.body;

    if (username === "admin" && password === "password123") {
        req.session.user = { id: 1, username: "admin", role: "admin" }; // Store session data
        res.send("Login successful");
    } else {
        res.status(401).send("Invalid credentials");
    }
});

// access session data in protected routes
app.get("/dashboard", (req, res) => {
    if (!req.session.user) {
        return res.status(401).send("Unauthorized");
    }
    res.send(`Welcome ${req.session.user.username}, your role is ${req.session.user.role}`);
});

// clear session on logout
app.get("/logout", (req, res) => {
    req.session.destroy();  // Destroys session on the server
    res.send("Logged out successfully");
});
```




