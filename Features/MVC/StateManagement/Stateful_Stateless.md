> ta cần nhớ rằng việc sử dụng session (`stateful`) là của các Web App trước đây theo kiến trúc monolothic giúp thuận tiện hơn trong việc sử dụng thông tin được cá nhân hoá
> sau này các hệ thống phân tán ngày càng nhiều nên việc thiết kế Web API (`stateless`) trở nên phát triển hơn
> 1 khi đã gọi là stateful, tức là request sau có thể sẽ phải phụ thuộc vào request trước đó; và điều này khá nguy hiểm nếu request trước đó bị lỗi

> cách làm chức năng shopping cart trong stateless web app
> sẽ có trường hợp mà our Web App use standard tradition session-based for user log-in; but on one pages, the javascript need some payload to authenticate with API that's using `OAuth2`

======================================================================
## Stay away from sessions for authentication
As you anticipated in your question, avoid using a session to authenticate users. Sessions are peremptory and it's very difficult to replicate it consistently in a distributed, scalable infrastructure.

Also, load balancers don't work well with sessions: see Problem with Session State timeing out on Load Balanced Servers.

## Use a token-based authentication system
A stateless app will preferably use a token-based authentication system. Firebase is a good example. Map the immutable user ID extracted from the token to the user data persisted in whatever storing mechanism you want to use. Since this user ID won't change, you'll be fine in a distributed database.

## Don't confuse stateless with 'data persistence'-less
Sometimes people think that, by mapping a user ID to user data in a database, you are making a stateful app. It's not true. Let me make it clear:

An application that persists user information in a database and has dynamic responses for authenticated users IS NOT NECESSARILY STATEFUL. Stateless means the app won't have to distribute mutable authentication sessions across multiple servers and won't change its internal state to a particular client depending on session data.

The trick of stateless is: once a user validated its token by logging in, the server don't have to distribute anything new across the database servers and it won't change its state to that client. It can extract user info from the token and carry out what's needed to answer the request. If the token expires, the client will require a new authentication, which will generate a new token, but this is isolated from the app server, since the user ID will remain the same.

## Use cookies for caching while remaining stateless
If caching in cookies some frequently requested data will improve performance, that's fine, go ahead and cache. Just make sure the cookie isn't connected to any server state and your app will not break if the client loses the cookie.

======================================================================
# Categorizing Web APIs (`stateful-stateless` and `synchronous-asynchronous`)
https://documentation.gravitee.io/platform-overview/before-you-begin/api-fundamentals/categorizing-web-apis

======================================================================
# 'stateless' and 'stateful' key difference
* -> **`maintain states through multiple request`**

# Stateless API
* -> treats **`each request`** as an **independent** transaction that is unrelated to any previous request; doesn't contain **client-specifc data** (_like **`cookie sessionID`**_)
* -> each **`request`** from the client **must contain all the necessary data** for the server to process the request (_server does not store any context or session data between requests_)

* -> are generally easier to **scale horizontally** (**`distributed system`**) because a server instance doesn't require session data save in other server
* -> also more **fault-tolerant** because if one server instance fails, another instance can seamlessly handle the next request without any loss of context
* -> responses from stateless APIs can be **cached** more effectively, improving performance and reducing server load

# Stateful API
* -> maintains **`server-side state or session information`** related to **`each client sequence of requests`**
* -> the server **uses that data from previous requests to influence the response to subsequent requests**
* -> require a mechanism to **identify and associate each request with a specific client or session**; this is often done using **`session identifiers, tokens, or cookies`**
* -> this state or session data can include **`user authentication, shopping carts, user preferences, or any other relevant information`** that needs to persist across multiple requests

* -> maintaining state on the server increases complexity, introduce potential scalability and fault-tolerance issues (_as server instances need to share and manage information_)

======================================================================
# 'RESTful' web service
* -> is generally **stateless**, they can also be designed to **`maintain state if needed`**
* -> typically done by **storing state information on the client-side** (_Ex: using tokens, cookies, or local storage_) 
* -> or by **leveraging separate stateful components like `databases`** 
* -> or **caching systems** to store and retrieve session data
* => means the **API itself remains stateless**, but the application **`leverages stateful components or client-side mechanisms`** to maintain and manage the required state information across requests

* _Web service, whether it is Rest or SOAP, is by default, designed stateless_