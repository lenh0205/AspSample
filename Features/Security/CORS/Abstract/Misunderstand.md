> Auth is used for protect resource, so if we have Auth do we need CORS ?
> 2 điều mà duy nhất mình có thể nghĩ ra là: hạn chế sự truy cập vào các resource không cần Auth; hoặc chặn việc 1 clientB nhưng lại có Auth của ServerA để truy cập vào resource của ServerA

> 1 điều nữa ta cần hiểu CORS là cơ chế bảo vệ được hổ trợ từ Browser

================================================================
# Control Access to Resource
* -> when it comes to controlling access to resources, there are 3 important concepts to understand: **CORS**, **Authentication**, and **Authorization**

* **CORS (Cross-Origin Resource Sharing)**
* -> is **`a security feature implemented by web browsers`** that controls **`which web pages can access resources on different domains`**
* -> CORS acts as a gatekeeper that only **allows requests from certain domains to access resources on other domains**

* **Authentication**
* -> is the process of **`verifying the identity of a user or client`**

* **Authorization**
* -> is the process of determining **`what actions a user or client is allowed to perform`**

==========================================================
CORS prevents any random website from using our authenticated cookies to send an API request to our resource server
By default, browsers only allow api calls on the same .com or IP address