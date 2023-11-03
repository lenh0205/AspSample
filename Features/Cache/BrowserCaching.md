
* all these Storage are being store on the user's actual Browser that they're using
* -> so for example if Local Storage saved on Chrome will not available on FireFox
 
* user doesn't share cookies, localStorage between them
* -> so if we set the local Storage for a certain user, none of other users of that site will be able to see that because it's stored on that user's computer only


=========================================
# Cookies
* can store only small amount of information - 4kb for most browser
* while cookies stored in the browser, they send to server everytime a user requests from server
* -> that while cookie have small capacity - avoid slow down the request and response because of a lot of cookies

* cookies is available for any window (tabs) inside the browser
* Have to set when Cookie expire
* => thường ta chỉ dùng Cookie để lưu data nếu muốn gửi luôn data đó cho Server

# Local Storage
* can store 10mb
* local storage is available for any window (tabs) inside the browser
* Never Expired; just until user delete it or use code to delete it

# Session Storage 
* can store 5mb
* Session Storage is only available in the single tab that you have open that you set in it
* expired when we close the Tab where that session was set 
