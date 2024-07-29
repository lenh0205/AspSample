==================================================================
# 'Basic Auth' for Web API in MVC project
* _đòi hỏi mỗi lần thực hiện Action đều cần gửi `username-password`_

```cs - ~/App_Start/WebApiConfig.cs
public static class WebApiConfig
{
    public static void Register(HttpConfiguration config)
    {
        // Web API configuration and services

        // Web API routes
        config.MapHttpAttributeRoutes();

        config.Routes.MapHttpRoute(
            name: "DefaultApi",
            routeTemplate: "api/{controller}/{id}",
            defaults: new { id = RouteParameter.Optional }
        );

        config.Filters.Add(new BasicAuthenticationAttribute()); // Add this filter
    }
}
```

```cs - ~/Filters/BasicAuthenticationAttribute.cs
// -> using 'Basic Auth' scheme for 'Authorization' request header
// -> the 'Basic Auth' only decode credential to base64; so [RequireHttps] is needed to encrypt it
public class BasicAuthenticationAttribute : AuthorizationFilterAttribute
{
    public override void OnAuthorization(HttpActionContext actionContext)
    {
        var authHeader = actionContext.Request.Headers.Authorization;

        if (authHeader != null)
        {
            var authenticationToken = action.Request.Headers.Authorization.Parameter;
            var decodeAuthenticationToken = Encoding.UTF8.GetString(
                Convert.FromBase64String(authenticationToken));
            var usernamePasswordArray = decodeAuthenticationToken.Split(":");
            var userName = usernamePasswordArray[0];
            var password = usernamePasswordArray[1];

            var isValid = useName == "Lee" && password == "123";
            if (isValid)
            {
                var principal = new GenericPrinciple(new GenericIdentity(userName), null);
                Thread.CurrentPrincipal = pricipal;
                return;
            }
        }
        HandleUnauthorized(actionContext);
    }

    private static void HandleUnauthorized(HttpActionContext actionContext)
    {
        actionContext.Response = actionContext.Request.CreateResponse(
            HttpStatusCode.Unauthorized, "Unauthorized");
        actionContext.Response.Headers.Add(
            "WWW-Authenticate", 
            "Basic Scheme='Data' location = 'http://localhost:'"
        );
    }
}
```

```cs - Usage:
[BasicAuthentication]
[RequireHttps]
public class ValuesController : ApiController
{
    public IEnumerable<Item> Get()
    {
        var db = new ApplicationDbContext();
        List<Item> items = db.Items.ToList();
        return items;
    }
}
```

```js - calling API:
// send request with "Authorization" header type "Basic Auth":
await axios.post(Url, {}, { auth: { username, password } });
```

==================================================================
# Using Token in 'Basic Auth'
* -> trong thực tế, thì ta chỉ gửi **`username-password`** lần đầu thôi sau đó ta sẽ sử dụng **`token`** (token-base authentication)
* _lần đầu gửi lên `username:password` thực sự, server sẽ trả về 1 `token` đồng thời lưu `token` này vào trường `Token` của `User`_
* _lần sau gửi lên `token: tokenValue` theo dạng `username: password` của Basic Auth; so sánh với token của user trong database_

* **BCrypt** - thuật toán **`Hashing`**
* -> cùng 1 input nhưng mỗi lần hashing sẽ ra 1 output khác nhau (_khác ở phần `salt` và `hashing password`_)
* -> nhưng hàm **`verify()`** của nó có thể kiểm tra 1 output bất kỳ và input có khớp nhau không
* -> chuỗi output hasing sẽ trông như này: 
```r
// [Algorithm][cost][salt][hashed password]
// VD: "$2y$10$6z7GKa9kpDN7KC3ICW1Hi.fd0/to7Y/x36WUKNPOIndHdkdR9Ae3K"
```

```cs - User.cs
public partial class User 
{
    public int id { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string Token { get; set; } // thêm trường này
}
```

```cs - ~/Filters/BasicAuthenticationAttribute.cs
public class BasicAuthenticationAttribute : AuthorizationFilterAttribute
{
    public override void OnAuthorization(HttpActionContext actionContext)
    {
        var authHeader = actionContext.Request.Headers.Authorization;

        if (authHeader != null)
        {
            var authenticationToken = action.Request.Headers.Authorization.Parameter;
            var decodeAuthenticationToken = Encoding.UTF8.GetString(
                Convert.FromBase64String(authenticationToken));
            var usernamePasswordArray = decodeAuthenticationToken.Split(":");
            var userName = usernamePasswordArray[0];
            var password = usernamePasswordArray[1];

            var isValid = false;
            string fakeToken = "";

            if (userName == "token")
            {
                isValid = db.Users.Select(w => w.token == password).FirstOrDefault();
            }
            else {
                fakeToken = RandomString(100);
                User user = db.Users.SingleOrDefault(w => w.userName == userName);
                if (user != null)
                {
                    // hash and save a password
                    int costParameter = 12;
                    string hashedPassword = BCrypt.Net.Bcrypt.HashPassword(password, costParameter);
                    bool test = BCrypt.Net.BCrypt.Verify(password, hashedPassword);

                    // check a password
                    isValid = BCrypt.Net.BCrypt.Verify(password, user.password);
                    if (isValid) {
                        user.token = fakeToken;
                        db.SaveChanges();
                    }
                }
            }
            
            if (isValid)
            {
                var principal = new GenericPrinciple(new GenericIdentity(userName), null);
                Thread.CurrentPrincipal = pricipal;
            
                if (fakeToken != "")
                {
                    actionContext.Response = actionContext.Request.CreateResponse(
                        HttpStatusCode.Ok, "Fake token [" + fakeToken + "]");
                }
                return;
            }
        }
        HandleUnauthorized(actionContext);
    }

    private static void HandleUnauthorized(HttpActionContext actionContext)
    {
        actionContext.Response = actionContext.Request.CreateResponse(
            HttpStatusCode.Unauthorized, "Unauthorized");
        actionContext.Response.Headers.Add(
            "WWW-Authenticate", 
            "Basic Scheme='Data' location = 'http://localhost:'"
        );
    }
}
```

=======================================================================
# Use case:
https://security.stackexchange.com/questions/180357/store-auth-token-in-cookie-or-header
https://softwareengineering.stackexchange.com/questions/400551/how-to-combine-session-based-authentication-and-stateless-rest-api
https://www.baeldung.com/cs/tokens-vs-sessions