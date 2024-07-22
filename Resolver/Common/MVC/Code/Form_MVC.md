

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
<div>
    @using (Html.BeginForm("Index", "Login", FormMethod.Post))
    {
        <span>Enter your name:</span> @Html.TextBoxFor(m => m.UserName)<br/>
        <span>Enter your password:</span> @Html.TextBoxFor(m => m.Password)<br/>
        <input id="Submit" type="submit" value="submit">
    }
    <hr/>
    <strong>User Name: </strong> @Html.DisplayFor(m => m.UserName)<br/>
    <strong>Password: </strong> @Html.DisplayFor(m => m.Password)<br/>
</div>

// -> "GET /Login" thì ".DisplayFor" chưa hiển thị gì ra UI; ta phải "submit" form tới "POST /Login" thì action mới truyền model View(user) thì lúc này mới hiện thông tin ra UI
// -> Note: giá trị của UI textbox @Html.TextBoxFor() sẽ không đổi sau khi submit dù ta có sửa lại data của "user" trước khi bind ngược lại view
```