> như ta thấy 1 số Identit Platform sẽ hỗ trợ UI cho phần login và consent; việc này có cần thiết không ? nó khá là tiện khi ta redirect từ app này sang appp khác để xác nhận việc delegate permission
> theo motip sau khi thông qua consent Authorization Server sẽ redirect về trang đã yêu cầu delegate permission và đống thời đính kèm token trong reponse
> điều này chứng tỏ, các trang Client yêu cầu delegate cần hiểu được cách delegate authorize của Service bên kia; bởi vậy ta thấy 1 trang Client chỉ hỗ trợ 1 số Identity Provider nhất định
> t nghĩ đó là lý do UI login và consent cần được thống nhất cũng như cung cấp bởi Authorization Server 

> mấy cài ClientID, Client Secret, Authorization Code thực sự dùng để làm gì ? 

> sao lúc hoàn thành Consent, response không bao gồm trực tiếp Access Token luôn mà chỉ bao gồm Authorization Code rồi Client cần cần request đến Authorization Server 1 lần nữa để lấy được Access Token


==============================================================

# Sharing information between services
* -> _in the stone-age days of the internet_ **`sharing information between services`** was easy, we simply gave our usename and password for one service to another
* -> so they could log into our account and grab whatever information they wanted
* => we should **never be required to shared our credentials**, username and password to another service
* => there's no guarantee the organization will **`keep our credential safe`**; or guarantee their service won't **`access more of our personal information than necessary`** 

* => today we have agreed-upon **`standards to securely allow one service to access data from another`**: the **OAuth 2.0** and **OpenID Connect**

# OAuth 2.0
* -> is a security standard where we **give one application permission to access our data in another application**
* -> _instead of giving them our credential_, we can essentially give one app a key that gives them **`specific information to access our data`** or **`do thing on our behalf in another application`**
* -> the steps to **`grant permission or consent`** are often referred to as **authorization** or even **delegated authorization**
* -> we authorize one application to **`access our data or use feature`** in another application **on our behalf** without give them our password; and we can **take back that key whenever we wish**

## OAuth flow
* _the OAuth flow is made of **`visible steps to grant consent`** as well as some **`invisible step where the two services agree on a secure way of exchanging information`**_
* _the **`most common OAuth 2.0 flow`** is the **Authorization Code Flow**_

```r - Example:
// we create an account on a website to have it send us a joke as a text message every day to our phone
// we love it so much so we decide to share this site with everyone we have ever met online
// however writing an email to every person in our "contacts" list sound like a lot of work

// the good thing is this website (the "sending joke" website) has a feature to invite our friends
// -> the first step is to choose our "email provider"; when we click on our email provider (VD: Outlook) we get "redirected to our email service"
// -> our email service than checks to see if we are currently "logged in"; if not we get a prompt to log in
// -> after we login (or if we already have an active login session), we will be presented with a question similar to "do you want to give this website access to your contants ?"
// -> when we click "allow", we get redirected back to the "sending joke" website
// -> the application can now "read our contacts", and that is the "only thing it can do"
// -> the website can now send emails to everyone we know 
```

* -> the **`Resource Owner`** want to **allow the Client to access their resource**
* -> the Client **redirect the Browser to the Authorization Server** and includes with the request **`the ClientID, Redirect URI, Response Type and one or more Scopes it needs`**
* -> the Authorization Server verifies "who you are ?" and if necessary prompts for a login  
* -> the Authorization Server then presents with the **`consent form`** **based on the Scopes requested by the Client**
* -> Resource Owner have the **opportunity to grant or deny permission**
* -> the Authorization Server redirects back to the Client using **`Redirect URI`** along with a temporary **Authorization Code** 
* -> the Client then **contacts the Authorization Server directly**, it's **`doesn't use the Resource Owner Browser`** and **`securely sends its ClientID, Client Secret and Authorization Code`** 
* -> the **`Authorization Server verify the data`** and responses with an **Access Token**
* -> the Access Token is a value the client doesn't understand, as far as the **`Client is concerned the Access Token is just a string of gibberish`** 
* -> however, the Client can **`use the Access Token to send requests`** to the **Resource Server**
* -> the **Resource Server verifies the Access Token** and if valid responses with the contacts requested 

* long before we gave the Client permission to access our contacts, the **Client and the Authorization Server established a working relationship**
* -> the **`Authorization Server generated`** a **ClientID** and **Client Secret**, then gave them to the Client to use for all future exchanges 
* -> the **`Client Secret must be kept secret`** so that only Client and Authorization Server know

============================================================
* -> OAuth 2.0 is designed only for **Authorization** - for granting access to data and features from one application to another
* -> giving the Client a key, that key is useful but it doesn't tell the Client "who you are?" or anything about "you" 

# OIDC - OpenID Connect
* -> is a thin **`layer that sits on top of OAuth 2.0`** thats **add funtionality around login** and **profile information about the person who is logged in**
* _OIDC can not work without the underlying OAuth Framework_
* -> _instead of a key, OIDC is like giving the client application a badge_
* -> the badge not only gives the **Client specific permisions**, it also provides some **basic information** about "who you are ?"
* -> OAuth enables Authorization from one app to another, OIDC enables **a Client to establish a login session** often referred to as **`Authentication`** as well to gain **`information about the person logged in`** (_Resource Owner_) which is often called **Identity**
* -> when **`an Authorization Server supports OIDC`**, it's sometimes called an **Identity Provider** since it provides information about the Resource Owner back to the Client

* -> OpenID Connect enables scenarios where **`one login can be used across multiple applications`**, also know as **Single Sign On (SSO)**

```r - For example
// an application could support SSO with social networking services such as Fackbook, Twitter,...
// so users can choose to leverage a login they already have and are comfortable using 
```

## OpenID Flow
* -> the **`OpenID Connect flow`** looks the same as **`OAuth flow`**; 
* -> the only **differences are in the inital request**, a specific **Scope of OpenID is used** - this lets the **`Authorization Server`** knows this will be **`an OpenID Connect exchange`**
* -> the **`Authorization Server`** goes through all the same steps as before and **issues an "Authorization Code" back to the Client** using the **`Resource Owner Browser`**
* -> when the **`Client exchanges the Authorization Code`** for an Access Token, the Client **receives both an "Access Token" and "ID Token"**
* -> the **`ID Token`** is a **specifically formatted string of characters** known as a **`JSON Web Token`** 
* -> the **`Client`** can **extract information embeded in the JWT** (_such as userId, name, when we logged in, the ID Token expiration,..._) and it can tell if anyone has tried to **tamper with JWT** 
* -> the **`data inside ID Token`** called **Claims**

* _with OIDC, there's also a **`standard way the Client can request additinal Identity information`** from the Authorization Server (such as email,...) using the Access Token_ 