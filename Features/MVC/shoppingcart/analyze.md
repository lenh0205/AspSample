# use 'LocalStorage' or 'Cookies' for shopping cart items
* -> ta cần nhớ là cookies are sent to the server with (almost) every request and can store less data than LocalStorea
* -> ngoài ra cookie can be **`created/accessed across subdomains`**, for example store.domain.com and checkout.domain.com

## For basic feature
* -> generally, localStorage and only client rendering the cart would be the easiest first move to get the base feature done

## Common 
* -> use **cookie** to store **`sessionId`**; 
* -> server use this **session object** (may be **`Redis`**) of that "sessionId" to store a list of **`cartId`** 
* -> and use "cartId" to retrieve **`full cart information`** in **database**; 
* -> also do cookie **HTTP secure** would be the best practice
* (_because there might be a ton of value in holding the cart in a DB for abandoned cart follow-ups, general reporting, etc_) 

* -> có 1 vấn đề là mỗi lần user add 1 item vào cart thì sẽ đòi hỏi server update database

## authenticate (consider)
* -> Local Storage should be fine if the user is adding things to the cart as a "Guest" (unauthenticated)
* -> However, if the user is "Authenticated / Logged In", I would recommend storing stuff in the backend so that if the user signs in from another browser, mobile device, or other device, he/she can see previously added products in the cart.
