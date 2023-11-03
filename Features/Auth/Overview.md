# Overview
* **Authentication** - **`you are who you claim you are ?`** (đã có trong hệ thống chưa + user A thì hệ thống không được nhầm thành user B)
* -> include claims (sự khẳng định) about an identity + verification of the claimed identity of the user

* **Authorization** (Access Control) - **`permission to do`**
* -> sau khi Authen
* -> thường hệ thống sẽ cấp cho ta 1 cái Token -> trong token sẽ có những cái mã -> khi ta truy xuất thì hệ thống sẽ kiểm tra những cái mã đó 

=================================================
# Authen/Author with Client or Server ?
* **Authentication and Authorization happen in "Server"** , **`not Client`**
* -> Lý do là **can not trust Client** 
```cs - VD: Auth on client 
// như kiểu ta có 1 user, what button can they click ?
``` 

* nhưng nó **vẫn hoàn toàn OK to show and hide UI base on actual user datas or user information** để nói cho user what they can do (_VD: ta s/d 1 Boolean variable like "isAdmin" to able to show and hide different part of UI_) (_nhưng verification và check phải ở server_)
```cs - VD:
// Nếu User là "Admin", ta sẽ show "delete" and "edit" button while "normal user" don't have that
// Kể cả khi ta change UI/ change Client để "delete" button hiện ra cho "normal user" 
// => Server vẫn không cho phép ta thực hiện hành động đó
```

* ta có thể vào DevTool/Application để view Session trong Cookie, nó chỉ là 1 đoạn text **it doesn't matter what that is, it's just a unique identifier**
* -> nếu ta xoá nó thì giống như ta logout

* và ta cần đảm bảo ta **`securely store sessionID`** vì nếu **someone else has access to sessionID, they can logged in as user without knowing "user name" and "password"**
```cs - VD: Ta có được sessionID của user
// Ta vào DevTool/Application/Cookies của 1 trang web ta đang login copy lại value của "session"
// Ta mở tab ẩn danh -> mở trang web -> DevTool/Application -> paste lại cặp key/value vừa copy vào "Cookies"
// Refresh lại trang và ta sẽ thấy trang được login
```

* Vậy nên it a good idea to **periodically remove current sessions** from our session map
* -> if a user have **`inactive for a certain period of time, we'll remove that session`**, means that user is `logout`

==================================================
> _`Cookie-based server-side sessions`_

# Authentication process - Login process
* -> _Client_ **`send request`** to _Server_ with **credential infomation** (_email and password_) 
* -> Server do the **Authentication** to confirm **`a valid user`** and if so **`who exactly are they`** ?
* -> if the user is correct with the email and password, Server sẽ **`store that User`** inside the **session** which is stored in **Server memory**
* -> Server also create **a sessionID** - **`a unique Id that correspond withs with that part of Memory`** (it's how Server determines **this is always you** `when you make further request`)
* -> Server **send back response with that ID** (_"John"'s uuid - saying this is "John" and you good to go_); and **`tell Browser to store infomation in a Cookie`**
* -> **Client take the ID and saves in a Cookie**
* -> every time we make **`a request requires authorization`**, **the cookie has the specific ID for user is going to send along with request**
* -> The Server will know that this request coming from John

* In senario **`user is not exist`** , Server notify "Incorrect email or password. Try again!"

# Authorization process
* -> Client  make request to Server that **attach the cookie** (the ID said that "I am john") says "does John have **`permission to do`** that" ?
* -> Server go into **session memory** lookup to the User on memory have `correspond to this particular ID`
* -> if correct it say this is user that corresponds with that ID; are they authorized to access this information ?
* -> if they're authorized then send the response with information browser looking for
* ->  if not, send and error "John doesn't have ability to do this"
* -> if we try to **`query a list of information`**, it'll **only return the items that we are allowed to access** from that list of information (_VD: 1 teacher chủ nhiệm thì chỉ trả về học sinh trong lớp đó thay vì toàn trường_)

================================================
# HttpOnly - store "Session" in a "Cookie" 
* _HttpOnly_ is **`extremely important`**
* session *`gets sent along to the server`**; **only HTTP request have access to this session Id**
* -> **`Client can't access this cookie at all`** (**can't access it by javascript**)
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

* làm sao để đặt password đủ phức tạp mà lại dễ nhớ ?
* -> password based on passphrase - tức là ta viết lái đi so với mật khẩu bình thường (_VD: IloveU2_)
* -> tỷ lệ bẻ khoá tương đương với việc đặt mật khẩu với 8 random characters `nhưng dễ nhớ hơn`
* -> search `leet speak` để biết thêm chi tiết về cách viết leet speak cho mật khẩu