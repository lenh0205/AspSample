==================================================================
# 'Basic Auth' for Web API in MVC project
* -> trong thực tế, thì ta chỉ gửi **`username-password`** lần đầu thôi sau đó ta sẽ sử dụng **`token`** (token-base authentication)

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

        config.Filters.Add(new BasicAuthenticationAttribute());
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

# Using Token in 'Basic Auth'
* _lần đầu gửi lên `username:password` thực sự, server sẽ trả về 1 `token` đồng thời lưu `token` này vào trường `Token` của `User`_
* _lần sau gửi lên `token: tokenValue` theo dạng `username: password` của Basic Auth; so sánh với token của user trong database_

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
                User user = db.Users.SingleOrDefault(w => w.userName == userName && w.password == password);
                if (user != null)
                {
                    isValid = true;
                    user.token = fakeToken;
                    db.SaveChanges();
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