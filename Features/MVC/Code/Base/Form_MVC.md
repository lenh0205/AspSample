

```cs - _Layout.cshtml
<ul class="nav navbar-nav">
    <li>@Html.ActionLink("Home", "Index", "Home")</li>
    <li>@Html.ActionLink("Login", "Index", "Login")</li> // navigate
</ul>
```

```cs - Controller
public class LoginController : Controller
{
    [HttpGet] // for redirect to "Login" page
    public ActionResult Index()
    {
        return View();
    }

    [HttpPost] // for submit form at current "Login" page
    public ActionResult Index(User user)
    {
        ViewBag.error = "";
        // check if user exist
        var db = new ApplicationDbContext();
        User aUser = db.Users
            .Where(u => u.UserName =  user.UserName && u.Password == user.Password)
            .FirstOrDefault();
        if (aUser == null) {
            ViewBag.error = "Invalid username and/or password";
        }
        else {
            Session["username"] = user.UserName;
            // -> hiểu đơn giản thì đây là 1 dấu check để biết user login hay chưa
            // => trong trường hợp này, chỉ cần get ra Session["username"] != null là đã login rồi
            // -> ta có thể viết kiểu này Session["isLoggedIn"] = true; để thể hiện điều tương tự
            // -> nhưng thường ta sẽ lưu 1 user info (VD: userId) để sau này truy xuất Database nếu cần
            // -> và tất nhiên trước khi có thể check thì ta cần Authen người dùng trước 
        }

        return View(user);
    }
}
```

```cs - Model
public class User
{
    public int Id { get; set; }

    [Required][StringLength(100)]
    public string UserName { get; set; }

    [Required]
    public string Password { get; set; }
}
```

```cs - View
@model User // using "User" object as model

<h2>Login</h2>

@if (ViewBag.error != "")
{
    <div>
        <h1 style="color:red;">@ViewBag.error</h1>
        @using (Html.BeginForm("Index", "Login", FormMethod.Post))
        {
            <span>Enter your name:</span> 
            @Html.TextBoxFor(m => m.UserName, new { @class = "form-control mb-4" })
            <br/>
            <span>Enter your password:</span> @Html.TextBoxFor(m => m.Password)<br/>
            <input id="Submit" type="submit" value="submit">
        }
        <hr/>
        <strong>User Name: </strong> @Html.DisplayFor(m => m.UserName)<br/>
        <strong>Password: </strong> @Html.DisplayFor(m => m.Password)<br/>
    </div>
}
else 
{
    <h1 style="color:blue;">Login thành công</h1>
}

// -> "GET /Login" thì ".DisplayFor" chưa hiển thị gì ra UI; ta phải "submit" form tới "POST /Login" thì action mới truyền model View(user) thì lúc này mới hiện thông tin ra UI
// -> Note: giá trị của UI textbox @Html.TextBoxFor() sẽ không đổi sau khi submit dù ta có sửa lại data của "user" trước khi bind ngược lại view
```
