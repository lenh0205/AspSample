# Example: Web Application 

## Setup DB
* -> tạo table "User" trong database; email sẽ là unique 
```js 
// Tạo "User" collection trong Database; field "_id" is unique Id like primary key on table
```
```js
// Định nghĩa "User" Model cho "User" Collection bằng Mongoose ORM
const mongoose = require("mongoose");
let User = mongoose.model("User", new mongoose.Schema({
    firstName: { type: String, required: true },
    lastName: { type: String, required: true },
    email: { type: String, required: true, unique: true },
    password: { type: String, required: true },
}));
```

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

## Setup Session to store user info

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

## Basic Logic

### Register:
* -> tạo 1 Form đăng ký HTML gồm 4 fields required từ user: firstName, lastName, email, password
```js
// Register API:
app.post("/register", (req, res) => {
    // use "bcrypt" function from "bcryptjs" library to hash password
    // "14" called "work factor" - a param for decrypt alg to determine how strong a hash is
    let hash = bcrypt.hashSync(req.body.password, 14); 

    req.body.password = hash; // override "plain text password"
    let user = new User(req.body); // intial model and pass FormData

    // call save() method to execute mongoDB query
    user.save((err) => { 
        if (err) {
            // ... handle errors - execute fail, email existed, ...
            return res.render("register", { error: error });
        }
        
        res.redirect("/dasboard"); // mọi thứ OK thì cho vào trang Dashboard
    })
})
```

### Login:
```js
// Tìm email và kiểm tra password; 
app.post("/login", (req, res) => {
    User.findOne({ email: req.body.email }, (err, user) => { // tìm theo unique "email"
        if (
            err 
            || !user 
            || !bcrypt.compareSync(req.body.password, user.password)
            // so sánh "plain text password" từ request với "hashed password" trong Database
        ) 
        { 
            return res.render("login", { error: "Incorrect email / password" })
        }

        req.session.userId = user._id; // gán userId trong session với _id trong MongoDB

        res.redirect("/dasboard"); // mọi thứ OK thì cho vào trang Dashboard
    })
})
```

### Access '/Dashboard' as protected route 
* -> trang dữ liệu cần xác minh danh tính để vào được
```js
app.get("/dashboard", (req, res, next) => {
    if (!(req.session && req.session.userId)) { // check if session variable and userId available
        return res.redirect("/login");
    }
    
    User.findById(req.session.userId, (err, user) => {
        if (err) {
            return next(err);
        }
        if (!user) {
            return res.redirect("/login");
        }
        
        res.render("dashboard");
    })
})
```

## Middleware 
* -> thay vì check user exist và lấy user info với mỗi endpoint thì ta **tạo 1 Middleware để lấy user info ra 1 lần thôi** và lưu vào 1 biến local **req.user** để các Middleware sau sử dụng
```js 
app.use((req, res, next) => {
    // check if session exist
    if (!(req.session && req.session.userId)) {
        return next()
    }

    // if session exist, load "user object" from MongoDB
    User.findById(req.session.userId, (err, user) => {
        if (err) {
            return next(err);
        }
        if (!user) {
            return next();
        }
        user.password = undefined;
        req.user = user; 
        res.locals.user = user;

        next();
    })
})
```

* -> ta cũng tách phần logic handle khi user xác thực không thành công thành 1 function riêng và pass vào middleware của protected route, tùy trường hợp sẽ take những actions khác nhau
```js
function loginRequired(req, res, next) {
    if (!req.user) { // s/d "req.user" có từ Middleware trên
        return res.redirect("/login");
    }
    next();
}
app.get("/dashboard", loginRequired, (req, req, next) => {
    // dashboard business handling
})
``` 

## Cross Site Request Forgery - CSRF 
* -> **`tấn công giả mạo chính chủ thể của nó`**, hacker lạm dụng sự tin tưởng của một ứng dụng web trên trình duyệt của nạn nhân để tấn công vào chứng thực request trên web thông qua việc sử dụng Cookies
* -> **Mechanism**: attacker point user to a site they're logged into to perform actions they didn't intend to like _submitting a payment, chaging password,..._
* -> **Rish**: very low espectially using modern web framework to implement code 

```html - 1 withdraw page bình thường có Form chuyển tiền ngân hàng
<form>
    <input type="text" name="account"/> <!-- my account name -->
    <input type="text" name="amount"/> <!-- the amount of money -->
    <input type="text" name="for"/> <!-- account we want to tranfer money -->
</form>

<!-- The hacker send us an email try to trick us clicking on a link -->
<!-- Đường link này gọi API giống như việc ta vào "withdraw page" điền đẩy đủ form chuyển tiền nhưng số tiền lúc này là cả tỷ cho hacker  -->

<!-- Hey John ! check out this picture of my dog -->
<img src="http://bank.com/withdraw?account=John&amp;amount=100000000&amp;for=BadGuy">
```

### Solution
* _gồm 2 phần:_
* -> generating a random token every time a new page request is made
* -> insert that token in a cookie
* -> put that as an input field on a form so that we're able to send back to our Web Server
* -> when someone go to our Web Server to view FormData; we check value in that cookie is the one was actually submitted by the form
* -> nếu 2 cái khác nhau, thì bỏ qua

### Implement
```js
const csurf = require("csurf");
app.use(csurf());

// In any route that render a template that has a Form on it ("/register", "/login"); 
// -> we're going to pass in, extra information "{ csrfToken: req.csrfToken }", into our template 
// -> that way in our HTML code, we're able to use the variable "csrfToken"
// -> "req.csrfToken" will generate random Token for us
app.get("/register", (req, res) => {
    res.render("register", { csrfToken: req.csrfToken })
})
app.get("/login", (req, res) => {
    res.render("login", { csrfToken: req.csrfToken })
})

// In our HTML, we need to modify so that each Form includes "a hidden input field" with name "_csrf" and value is random token (this token change everytime the user views a new page with a Form in it)
form (method="post")
    input(type="hidden", name="_csrf", value=csrfToken)
```

## Additional security in Best Practices

### SSL
* always use SSL because 
* -> if not, any information a user sends from their browser to our Web Server that can be view by anyone in between like Internet service Provide, NSA, Canadian police, ...
* -> SSL encrypted information from browser to server; it really sercure
* -> not matter type of Authentication we using, if we're not use SSL it's not secure 

### Config Cookie for better secure:
* add additional flag: 
```js
app.use(session({
    cookieName: 'session',
    secret: 'some_random_string',
    duration: 30 * 60 * 1000,
    activeDuration: 5 * 60 * 1000,
    httpOnly: true, // don't let Javascript code in client access Cookies
    secure: true, // server only set cookies if website using "https"
    ephemeral: true // destroy cookies when the browser closes (no matter the "duration")
}))
```

### Helmet library:
* set a  bunch of HTTP Headers on our sites and secure them
* prevent clickjacking, ...

### Don't Roll Your Own
* nên s/d nhưng thư viện như: Passport, Node-Login, Aqua, Okta



