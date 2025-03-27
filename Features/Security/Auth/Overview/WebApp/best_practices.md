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

## HttpOnly - store "Session" in a "Cookie" 
* _HttpOnly_ is **`extremely important`**
* session **`gets sent along to the server`**; **only HTTP request have access to this session Id**
* -> **`Client can't access this cookie at all`** - **can't access it by javascript** (_không thể dùng javascript để get ra cookie đó chứ đừng nói đến sữa_)
* -> you can't actually accidentally leak this data
* -> nobody that has access to javascript on your site has access to this

* => this is why we should always **`store these type of sessions in cookie instead of LocalStorage or SessionStorage`**

```js - Tạo 2 Database cho "user info" và "user-session"; Login API    
// Allow Credential to be passed - allow Cookies
app.use(cors({ origin: "http://127.0.0.1:5500", credentials: true }));

app.use(cookieParser());

// Database để lưu User Info (hiện có 2 user):
const USERS = new Map()
USERS.set("WDS", { id: 1, username: "WDS", role: "Admin" })
USERS.set("Kyle", { id: 2, username: "Kyle", role: "User" })

// Thường dùng Memory để lưu Session (cũng có lúc s/d Database):
const SESSIONS = new Map();

// User gửi request form login page
app.post("/login", (req, res) => {
    const user = USERS.get(req.body.username); // Tìm username này trong Database
    if (user === null) { // Nếu không có thì trả về "401 - Not Authentication"
        res.sendStatus(401);
        return
    }

    const sessionId = crypto.randomUUID(); // if có, tạo 1 session cho user
    SESSIONS.set(sessionId, user); // gắn session này với user
    
    // sending down a cookie
    res.cookie("sessionId", sessionId, {
        secure: true, // can only be used on "https" sites
        httpOnly: true, 
        sameSite: "none" // nếu Server và Client nằm trên URL khác nhau
    })
    .send(`Auth as ${req.body.username}`)
})
```

* _H nếu ta gửi request để login thì `trong DevTool/Application/Cookies sẽ có 1 sessionId`_
* _trong request Header sẽ có `Set-Cookie` header với "sessionId=....;Path=/; HttpOnly; Secure; SameSite=None"_

### Helmet library:
* set a  bunch of HTTP Headers on our sites and secure them
* prevent clickjacking, ...

### Don't Roll Your Own
* nên s/d nhưng thư viện như: Passport, Node-Login, Aqua, Okta
