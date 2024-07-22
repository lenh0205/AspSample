=============================================================================

# Model in ASP.NET MVC
* -> represents the **`shape of the domain-specific data as public properties`** and **`business logic as methods`** in MVC application
* -> the model class can be **`used in the view to populate the data`**, as well as **`sending data to the controller`**

* _usually, we put these model in `~/Model` folder_

# Views
* -> a view is used to **display data using the `model` class object**
* -> derived from **WebViewPage** class included in **`System.Web.Mvc`** namespace
* -> these view are contained inside **Views** folder 

* -> the MVC framework requires **`a separate sub-folder for each controller with the same name as a controller, under the "Views" folder`**
* -> it's good practice to **`keep the view name the same as the action method name`** so that you don't have to **explicitly specify the view name in the action method** while returning the view

* -> the **Shared** folder contains **`views, layout views, and partial views`** - which will be **shared among multiple controllers**

=============================================================================

# Razor View Engine
* -> to **compile a view with a mix of HTML tags and server-side code (C#)** - maximizes the speed of writing code
* -> the **`C# Razor view`** is **.cshtml** file

* -> the razor view uses **@** character to **`include the server-side code inline`** instead of the traditional **<% %>** of ASP
* -> use **@{ ... }** code block for **`multi-statement`**
* -> use **@:** or **<text>/<text>** to **`display texts within code block`**
* -> so to **`declare a variable`** - we need to declare it in code block **@ { ... }** and use that variable in HTML by **@**
* -> use **@model** to use model object anywhere in the view
* -> other like: **@if { ... } else { ... }**, **@for (...) {...}**

```cs - server-side code
<h2>@DateTime.Now.ToShortDateString()</h2>

@{
    var date = DateTime.Now.ToShortDateString();
    var message = "Hello World";
}
<h2>Today's date is: @date </h2>
<h3>@message</h3>

@{
    var date = DateTime.Now.ToShortDateString();
    string message = "Hello World!";
    @:Today's date is: @date <br />
    @message                               
}
```
```cs - if-else condition
@if(DateTime.IsLeapYear(DateTime.Now.Year) )
{
    @DateTime.Now.Year @:is a leap year.
}
else { 
    @DateTime.Now.Year @:is not a leap year.
}
```
```cs - for loop
@for (int i = 0; i < 5; i++) { 
    @i.ToString() <br />
}
```
```cs - model
@model Student

<h2>Student Detail:</h2>
<ul>
    <li>Student Id: @Model.StudentId</li>
    <li>Student Name: @Model.StudentName</li>
    <li>Age: @Model.Age</li>
</ul>
```

# Integrate Model, View, Controller
* -> the **View()** method is **`defined in the BaseController class`**, which **automatically binds a model object to a view**

```cs - Controller
public class StudentController : Controller
{
     // GET: Student
    public ActionResult Index()
    {
        // we need to pass a model object with IEnumerable<Student> as a model type
        // because the "Index.cshtml" require it
        return View(studentList);
    }

    static IList<Student> studentList = new List<Student> { 
                new Student() { StudentId = 1, StudentName = "John", Age = 18 } ,
                new Student() { StudentId = 2, StudentName = "Steve",  Age = 21 } ,
                new Student() { StudentId = 3, StudentName = "Bill",  Age = 25 } 
    };
}
```
```cs - Model
public class Student
{
    public int StudentId { get; set; }

    [Display( Name="Name")]
    public string StudentName { get; set; }
    public int Age { get; set; }
}
```
```cs - View
// Index.cshtml
@model IEnumerable<MVC_BasicTutorials.Models.Student>

@{
    ViewBag.Title = "Index";
    Layout = "~/Views/Shared/_Layout.cshtml";
}

<h2>Index</h2>
<p>@Html.ActionLink("Create New", "Create")</p>

<table class="table">
    <tr>
        <th>
            @Html.DisplayNameFor(model => model.StudentName)
        </th>
        <th>
            @Html.DisplayNameFor(model => model.Age)
        </th>
        <th></th>
    </tr>

@foreach (var item in Model) {
    <tr>
        <td>
            @Html.DisplayFor(modelItem => item.StudentName)
        </td>
        <td>
            @Html.DisplayFor(modelItem => item.Age)
        </td>
        <td>
            @Html.ActionLink("Edit", "Edit", new { id=item.StudentId }) |
            @Html.ActionLink("Details", "Details", new { id=item.StudentId  }) |
            @Html.ActionLink("Delete", "Delete", new { id = item.StudentId })
        </td>
    </tr>
}
</table>
```

=============================================================================
# Model Binding
* -> refers to **converting the `HTTP request data` to an `action method parameters`** - so there're 2 step in model binding:
* -> **`collects values from the incoming HTTP request`** - responsible by  **Value providers**
* -> **`populates primitive type or a complex type with these values`** - responsible by **Model Binders**

## Value Provider
* _i **`default value provider collection`** evaluates values from the following sources:_
* -> **Querystring parameters** (Request.QueryString)
* -> The property values in the **`JSON Request body`** (_Request.InputStream_), but only when the request is an **AJAX request**
* -> **Form fields** (_Request.Form_)
* -> previously bound action parameters, when the action is a **child action**
* -> **Route data** (_RouteData.Values_)
* -> **Posted files** (_Request.Files_)

## Binding to Primitive Type
* -> **`the HTTP GET request embeds data`** into a **query string**
* -> MVC framework automatically converts a query string to the action method parameters provided their **names are matching**

```cs
// http://localhost/Student/Edit?id=1&name=John
public ActionResult Edit(int id, string name)
{            
    return View();
}
```

## Binding to Complex Type
* -> it will automatically convert **`the input fields data on the view`** to **`the properties of a complex type parameter of an action method`** in **HttpPost** request if the **properties's names match with the fields on the view**
* -> thus, the **`MVC framework`** **automatically binds form fields to the complex type parameter of action method**

```cs - Ex:
// automatically map "Form collection values" to the "Student" type parameter 
// when the form submits an HTTP POST request to the Edit() action method
[HttpPost]
public ActionResult Edit(Student std)
{
    var id = std.StudentId;
    var name = std.StudentName;
    var age = std.Age;
    var standardName = std.standard.StandardName;

    return RedirectToAction("Index");
}

public class Student
{
    public int StudentId { get; set; }

    [Display( Name="Name")]
    public string StudentName { get; set; }

    public int Age { get; set; }
    public Standard standard { get; set; }
}
```

### FormCollection
* -> include the FormCollection type parameter in the action method **`instead of a complex type`** to **retrieve all the values from view form fields**

```cs
[HttpPost]
public ActionResult Edit(FormCollection values)
{
   var name = values["StudentName"];
   var age = values["Age"];

    return RedirectToAction("Index");
}
```

## [Bind] attribute
* -> **specify which properties of a model class we want to bind**
* -> specify the exact properties of a model should **`include`** or **`exclude`** in binding - can **improve performance** 

```cs
//  the Edit() action method will only bind StudentId and StudentName properties of the Student model class
[HttpPost]
public ActionResult Edit([Bind(Include = "StudentId, StudentName")] Student std)
{
    var name = std.StudentName;
            
    return RedirectToAction("Index");
}
```

=============================================================================
# Edit View (or Detail view for updating data of single item)

```cs - StudentController.cs
// Trang "Index.cshtml" của ta đang render 1 list "Students", ta bấm vào nút "Edit" trên 1 "Student"
// send "HttpGet" request "http://localhost/student/edit/{Id}"
public ActionResult Edit(int Id) 
{
    var std = students.Where(s => s.StudentId == Id).FirstOrDefault(); // get particular "Student" by Id
    return View(std); // navigate to Edit view and passing particular "Student" data
}

// User now edit data and click "Save" button
// send a HttpPOST request http://localhost/Student/Edit with the Form data collection
[HttpPost]
public ActionResult Edit(Student std)
{
    var name = std.StudentName;
    var age = std.Age;
    // update data to database

    return RedirectToAction("Index"); // render "Index" page with the refreshed datanew data
}
```

```cs - ~/View/Student/Edit.cshtml
@model MVCTutorials.Models.Student
@{
    ViewBag.Title = "Edit";
    Layout = "~/Views/Shared/_Layout.cshtml";
}

<h2>Edit</h2>

@using (Html.BeginForm())
{
    @Html.AntiForgeryToken()
    
    <div class="form-horizontal">
        <h4>Student</h4>
        <hr />
        @Html.ValidationSummary(true, "", new { @class = "text-danger" })
        @Html.HiddenFor(model => model.StudentId)

        <div class="form-group">
            @Html.LabelFor(model => model.StudentName, htmlAttributes: new { @class = "control-label col-md-2" })
            <div class="col-md-10">
                @Html.EditorFor(model => model.StudentName, new { htmlAttributes = new { @class = "form-control" } })
                @Html.ValidationMessageFor(model => model.StudentName, "", new { @class = "text-danger" })
            </div>
        </div>

        <div class="form-group">
            @Html.LabelFor(model => model.Age, htmlAttributes: new { @class = "control-label col-md-2" })
            <div class="col-md-10">
                @Html.EditorFor(model => model.Age, new { htmlAttributes = new { @class = "form-control" } })
                @Html.ValidationMessageFor(model => model.Age, "", new { @class = "text-danger"< })
            </div>
        </div>

        <div class="form-group">
            <div class="col-md-offset-2 col-md-10">
                <input type="submit" value="Save" class="btn btn-default" />
            </div>
        </div>
    </div>
}

<div>
    @Html.ActionLink("Back to List", "Index")
</div>
```

=============================================================================
