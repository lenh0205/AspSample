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
