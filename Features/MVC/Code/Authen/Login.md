
# Database
* -> tạo bảng **Account** gồm 2 field là **UserName** và **Password**
* -> tạo **store-procedure** là **`Account_Login`** để kiểm trả username-password có tồn tại không

* _store-procedure giúp tối ưu về nghiệp vụ, cũng như giảm thiểu duplicate processing_

# Tạo tầng truy cập dữ liệu
* -> Add new Project -> chọn template là **Class Library** tên là **`Models`** -> add nó vô project chính của ta
* -> install **EntityFramework** và **Entity Framework Tool** để **`generate code từ Database`**

# Tạo Entity 
* -> tạo thư mục "Framework" -> add new Item -> chọn **ADO.NET Entity Data Model** -> tên DbContext: **`OnlineShopDbContext`** -> chọn **Code First from Database** -> nhập **`connect string`**
* _nó sẽ tạo cho ta `class DbContext`, cũng như các file `class Entity`_

# Tạo class để tương tác với từng bảng

```cs - ~/AccountModel.cs
public class AccountModel
{
    private OnlineShopDbContext context = null;

    public AccountModel()
    {
        context = new OnlineShopDbContext();
    }

    public bool Login(string userName, string password)
    {
        object[] sqlParams = {
            new SqlParameter("@UserName", userName),
            new SqlParameter("@Password", password)
        };

        // gọi store procedure mà ta đã tạo sẵn
        var res = context.Database
            .SqlQuery<bool>("Sp_Account_Login @UserName,@Password", sqlParams)
            .SingleOrDefault();
        
        return res;
    }
}
```

# Login Model

```cs - ~/Areas/Admin/Models/LoginModel.cs
public class LoginModel
{
    [Required]
    public string UserName { get; set; }
    public string Password { get; set; }
    public bool RememberMe { get; set; }
}
```

# Set up Session
```cs - ~/Areas/Admin/Code/UserSession.cs
[Serializable] // tuần tự hoá nhị phân - để lưu thông tin vào session
public class UserSession
{
    public string UserName { get; set; }
}
```

```cs - ~/Areas/Admin/Code/SessionHelper.cs
public class SessionHelper
{
    public static void SetSession(UserSession session)
    {
        HttpContext.Current.Session["loginSession"] = session;
    } 
    public static UserSession GetSession()
    {
        var session = HttpContext.Current.Session["loginSession"];
        if (session == null) 
        {
            return null;
        }
        else
        {
            return session as UserSession;
        }
    }
}
```

# Login Controller

```cs - ~/Areas/Admin/Controllers/LoginController.cs
public class LoginController : Controller
{
    [HttpGet]
    public ActionResult Index()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken] 
    // -> đảm bảo token sinh ra bởi server và clien gửi tới đảm bảo request-reponse đồng bộ
    // -> chống việc request liên tục
    public ActionResult Index(LoginModel model)
    {
        var result = new AccountModel().Login(model.UserName, model.Password);
        if (result && ModelState.IsValid)
        {
            SessionHelper.SetSession(new UserSession() { UserName = model.UserName });
            return RedirectToAction("Index", "Home");
        }
        else 
        {
            ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng");
        }
        return View(model);
    }
}
```

# Login view

```cs - ~/Areas/Views/Login/Index.cshtml
@ {
    Layout = null;
}
@model LoginModel

@using (Html.BeginForm())
{
    @Html.AntiForgeryToken()
    @Html.ValidationSummary(true, null, new { @class = "alert alert-danger" })
    <fieldset>
        <div class="form-group">
            @Html.TextBoxFor(model => model.UserName, new {
                @class = "form-control", @placeholder="UserName", @autofocus = "autofocus" })
        </div>
        <div class="form-group">
            @Html.TextBoxFor(model => model.Password, new {
                @class = "form-control", @placeholder="Password", @type = "password" })
        </div>
        <div class="checkbox">
            <label>
                @Html.CheckBoxFor(model => model.RememberMe)
                Remember Me
            </label>
        </div>
        <button type="submit" class="btn">Login</button>
    </fieldset>
}
```