# Example: Web Application 

## Register:
* -> tạo table "User" trong database; email sẽ là unique 
```js 
// Tạo "User" collection trong Database; field "_id" is unique Id like primary key on table
```
```
// Định nghĩa "User" Model cho "User" Collection bằng Mongoose ORM
const mongoose = require("mongoose");
let User = mongoose.model("User", new mongoose.Schema({
    firstName: { type: String, required: true },
    lastName: { type: String, required: true },
    email: { type: String, required: true, unique: true },
    password: { type: String, required: true },
}));
```

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

## Login:
```js
// Tìm email và kiểm tra password; 
app.post("/login", (req, res) => {
    User.findOne({ email: req.body.email }, (err, user) => { // tìm theo unique "email"
        if (
            err 
            || !user 
            || !bcrypt.compareSync(req.body.password, user.password)
            // so sánh "plain text passwor" từ request với "hashed password" trong Database
        ) 
        { 
            return res.render("login", { error: "Incorrect email / password" })
        }

        req.session.userId = user._id; // gán userId trong session với _id trong MongoDB

        res.redirect("/dasboard"); // mọi thứ OK thì cho vào trang Dashboard
    })
})
```

## '/Dashboard' as protected route 
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

==================================================
# Refactoring
* Không cần care về session khi xử lý business nữa; ta chỉ cần có 1 `"user object" in all of our routes` if a user exists 
* -> bằng cách **`tạo 1 Middleware để lấy user info ra 1 lần thôi`** (_s/d session_) và lưu vào 1 biến local "req.user" để Middleware sau sử dụng
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

* Tách phần code `check if user logged in` ra thành 1 Middleware; và pass nó cho route cần Auth 
* -> if user logged in, ta để họ làm bất cứ gì họ muốn
* -> if not, force them to login
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

# Cross Site Request Forgery - CSRF 
* **`tấn công giả mạo chính chủ thể của nó`**
* -> hacker lạm dụng sự tin tưởng của một ứng dụng web trên trình duyệt của nạn nhân
* -> tấn công vào chứng thực request trên web thông qua việc sử dụng Cookies

* **Mechanism**: attacker point user to a site they're logged into to perform actions they didn't intend to like _submitting a payment, chaging password,..._

* **Rish**: very low espectially using modern web framework to implement code 

```html - 1 withdraw page có Form chuyển tiền ngân hàng
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

* **Solution** gồm 2 phần
* -> generating a random token every time a new page request is made
* -> insert that token in a cookie
* -> put that as an input field on a form so that we're able to send back to our Web Server
* -> when someone go to our Web Server to view FormData; we check value in that cookie is the one was actually submitted by the form
* -> nếu 2 cái khác nhau, thì bỏ qua

* **Implement**
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

# Additional security
## SSL
* always use SSL because 
* -> if not, any information a user sends from their browser to our Web Server that can be view by anyone in between like Internet service Provide, NSA, Canadian police, ...
* -> SSL encrypted information from browser to server; it really sercure
* -> not matter type of Authentication we using, if we're not use SSL it's not secure 

## Config Cookie for better secure:
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

## Helmet library:
* set a  bunch of HTTP Headers on our sites and secure them
* prevent clickjacking, ...

## Don't Roll Your Own
* nên s/d nhưng thư viện như: Passport, Node-Login, Aqua, Okta

# Disavandtage of Session
* **`Session Stored on server`** - we need to store the sessionId in a database or keep it in memory on server
* Most of `today's cloud applications` are **`scaled horizontally`** 
* -> this can be a huge bottleneck in production
* -> that bring us to **Token-based**

