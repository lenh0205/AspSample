> thay vì phải sử dụng sesion trực tiếp để thực hiện login thì ASP.NET hỗ trợ Membership

==================================================================
# Custom Membership Provider

## Cấu hình web.config
```xml - web.config
<authentication mode="Forms">
    <forms loginUrl="~/Admin/Login" timeout="2880" />
</authentication>
<membership defaultProvider="CustomMembershipProvider">
    <providers>
        <add 
            name="CustomMembershipProvider" 
            type="OnlineShop.Areas.Admin.Code.CustomMembershipProvider"
            connectionStringName="OnlineShopDbContext"
            enablePasswordRetrieval="false"
            enablePasswordReset="true"
            requiresQuestionAndAnswer="false"
            requiresUniqueEmail="false"
            maxInvalidPasswordAttemps="5"
            minRequiredPasswordLength="6"
            minRequiredNonalphanumbericCharacters="0"
            passwordAttemptWindow="10"
            applicationName="OnlineShop"
        >
    </providers>
</membership>
```

## Tạo CustomMembershipProvider 

```cs - ~/Areas/Admin/Code/
public class CustomMembershipProvider : MembershipProvider // abstract class of "System.Web.Security"
{
    // implement all ....

    public override bool ValidateUser(string username, string password) // kiểm tra login
    {
        return new AccountModel().Login(username, password);
    }
}
```

# Usage
* -> giờ ta sẽ không cần sử dụng **`session`** nữa

```cs
public class LoginController : Controller
{

    [HttpPost]
    [ValidateAntiForgeryToken] 
    public ActionResult Index(LoginModel model)
    {
        if (Membership.ValidateUser(mode.UserName, model.Password) && ModelState.IsValid)
        {
            FormsAuthentication.SetAuthCookie(mode.UserName, model.RememberMe);
            return RedirectToAction("Index", "Home");
        }
        else 
        {
            ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng");
        }
        return View(model);
    }

    public ActionResult Logout()
    {
        FormAuthentication.SignOut(); // huỷ tất cả session, cookie nó tạo
        return RedirectToAction("Index", "Login");
        // nếu ta sign out rồi cố gắng về trang chủ thì nó không cho
    }
}

// giờ ta có thể sử dụng các attribute [Authorize] hoặc [AllowAnonymous] để Authen
[Authorize]
public class HomeController : Controller
{
    public ActionResult Index()
    {
        return View();
    }
    
    public ActionResult About()
    {
        return View();
    }
}
```

```cs - ~/Areas/Admin/Views/Shared/_Layout.cshtml
<li>
    @Html.ActionLink("Đăng xuất", "Logout", "Login");
</li>
```