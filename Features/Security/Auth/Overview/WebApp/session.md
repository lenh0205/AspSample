
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

=================================================
# Implementation "checking session for fetching data" :

## Server xử lý 1 GET request required Authentication and Authorization:
```js - get "Admin" data từ VD bên trên
app.get("/adminData", (req, res) => {
    const user = SESSIONS.get(req.cookies.sessionId); // checking for the cookie is our sessionId
    if (user == null) {
        res.sendStatus(401) // Not Autheticated
        return 
    }

    if (user.role !== "Admin") {
        res.sendStatus(403); // Not Authorized
    }

    res.send("this is admin stuff")
})
```

## Client gửi Request
* **Client must include Credentials** by **`credentials: "include"`**
* -> make sure Cookie get passed along with all of different requests
* -> **`Cookie don't get passed along by default`**

```js - Client "login"
function login(username) {
    fetch("http://localhost:3000/login", {
        method: "POST",
        credentials: "include",
        headers: { "Content-Type": "application/json" }
    })
}
```

```js - Client get "admin" data
adminBtn.addEventListener("click",  () => {
    fetch("http://localhost:3000/adminData", {
        credentials: "include", // include cookie to request
        header: { "Content-Type": "application/json" }
    })
})
```
