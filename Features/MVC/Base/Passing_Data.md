> know how to passing data from controller to view, view to controller, controller to controller, view to view

=========================================================================
# Strongly Typed Model (Recommended Approach)

```cs
// Models
namespace HelloWorld.Models
{
    public class MessageModel 
    {
        public string Welcome { get; set; }
    }
}

// HomeController
public ActionResult Index()
{
    var message = new MessageModel();
    message.Welcome = "welcome to the world";
    return View(message);
}

// ~/Views/Home/Index.cshtml
@{
    Layout = null;
}
@model HelloWorld.Models.MessageModel // this one

<!DOCTYPE html>
<html>
<head>
    <meta name="viewport" content="width-device-width"/>
    <title>Index</title>
</head>
<body>
    <div>
        @Model.Welcome
    </div>
</body>
</html>
```

=========================================================================
# common point of 'ViewBag' and 'ViewData'
* _**`'ViewBag' is a wrapper around 'ViewData'`**- internally, ViewBag uses ViewData (ViewDataDictionary) to store values dynamically_
* -> used to transfer temporary data (_which is not included in the model_) from the **`controller to the view, not visa-versa`** 
* -> if we assign value to the **same property/key multiple times** (regardless using ViewData, ViewBag or both), it will only **`consider last value assigned`**
* -> both are **`limited to the current HTTP request`** (_if a **`redirection`** occurs, their values will not persist_)
  
# ViewBag
* -> it is a **`dynamic`** type - so we can **assign any number of properties and values to ViewBag** 
* -> **`doesn't require typecasting`** while **retrieving values from it** (Razor view will automatically call **`.ToString()`** when we directly render the value of it) (_ensure we know the actual type before calling an method of it or it will throw exception at runtime_) 
* _Note: since it is a dynamic type (skips compile-time checking), **`mismatch in naming between controller and view will cause a runtime error`**_

```cs - action method using "ViewBag"
namespace MVC_BasicTutorials.Controllers
{
    public class StudentController : Controller
    {
        IList<Student> studentList = new List<Student>() { 
            new Student(){ StudentID=1, StudentName="Steve", Age = 21 },
            new Student(){ StudentID=2, StudentName="Bill", Age = 25 }
        };

        // GET: Student
        public ActionResult Index()
        {
            ViewBag.TotalStudents = studentList.Count();
            return View();
        }

    }
}

// Index.cshtml
<label>Total Students:</label>  @ViewBag.TotalStudents
```

# ViewData
* -> **stores key-value pairs** in a **`Dictionary<string, object>`** (ViewDataDictionary) 
* -> **`requires typecasting`** when **retrieving values in the View** and **`require case-sensitive key access`** (_while ViewBag properties are case-insensitive_)

```cs
public ActionResult Index()
{
    IList<Student> studentList = new List<Student>();
    studentList.Add(new Student(){ StudentName = "Bill" });
    studentList.Add(new Student(){ StudentName = "Steve" });
    studentList.Add(new Student(){ StudentName = "Ram" });

    ViewData["students"] = studentList;
  
    return View();
}

// or
public ActionResult Index()
{
    ViewData.Add("Id", 1);
    ViewData.Add(new KeyValuePair<string, object>("Name", "Bill"));
    ViewData.Add(new KeyValuePair<string, object>("Age", 20));

    return View();
}

// Index.cshtml
<ul>
    {/* require type casting */}
    @foreach (var std in ViewData["students"] as IList<Student>)
    {
        <li>
            @std.StudentName
        </li>
    }
</ul>
```

=========================================================================
# TempData
* -> is used to **`transfer data`** from **view to controller**, **controller to view**, or **from one action method to another action method** of the same or a different controller
* -> **`stores the data temporarily`** and **automatically removes key-value after retrieving a value** (_tức là sau khi đọc data từ nó thì data đó sẽ mất_)
* -> but able to **`keep it for the subsequent request`** by calling **TempData.Keep(keyName)** method (_if not specific **`keyName`**, it will keep all_)

```cs
// Transfer data from one "Action Method" to another "Action Method" (Controller to Controller)
// -> user visit "Index" page first and then to the "About" page and assume that Index.chtml not retrieve TempData 
// -> add data in the 'TempData' in the "Index()" action method and access it in the "About()" action method
// -> sau khi About() retrieve data bởi key đó từ TempData thì nó sẽ tự động bị remove, Contact() không thể retrieve nữa
// -> nếu sau đó ta truy cập vào Contact() và cố retrieve data bằng key đó thì nó sẽ trả về null

public class HomeController : Controller
{
    public ActionResult Index()
    {
        TempData["name"] = "Bill";

        return View();
    }

    public ActionResult About()
    {
        string name;
        
        if(TempData.ContainsKey("name")) {
            name = TempData["name"].ToString(); // returns "Bill" 
        }

        return View();
    }

    public ActionResult Contact()
    {
        string name;

        //the following throws exception as TempData["name"] is null 
        //because we already accessed it in the About() action method
        name = TempData["name"].ToString(); 

        return View();
    }
}
```

```cs
// Ví dụ khác cho "Controller to Controller"

public ActionResult Index() {  
    Customer data = new Customer() { CustomerID = 1, CustomerName = "Abcd", Country = "PAK" };
    TempData["mydata"] = data;
    return RedirectToAction("Index", "Home2");  
}  

// inside the Index() of Home2:
public ActionResult Index() {  
    Customer data = TempData["mydata"] as Customer;  
    return View(data);  
} 
```

```cs
// Transfers data from an "Action method" to a "View" (Controller to View)
// -> added data in the TempData in the Index() action method, so we can access it in the Index.cshtml view
// -> because we have accessed it in the index view first, we cannot access it anywhere else

public class HomeController : Controller
{
    public ActionResult Index()
    {
        TempData["name"] = "Bill";

        return View();
    }

    public ActionResult About()
    {
        string name;

        //the following throws exception as TempData["name"] is null 
        //because we already accessed it in the Index.cshtml view
        name = TempData["name"].ToString(); 

        return View();
    }

    public ActionResult Contact()
    {
        string name;

        //the following throws exception as TempData["name"] is null 
        //because we already accessed it in the Index.cshtml view
        name = TempData["name"].ToString(); 

        return View();
    }
}

// Index.cshtml
@{
    ViewBag.Title = "Index";
    Layout = "~/Views/Shared/_Layout.cshtml";
}

@TempData["name"]
```

```cs
// Transfer data from a "View" to "Action method" (View to Controller)

// Index.cshtml
@{
    ViewBag.Title = "Index";
    Layout = "~/Views/Shared/_Layout.cshtml";
}

@{
    TempData["name"] = "Steve";
}

public class HomeController : Controller
{
    public ActionResult Index()
    {
        return View();
    }

    public ActionResult About()
    {
        string name;

        if(TempData.ContainsKey("name"))
            name = TempData["name"].ToString(); // returns "Bill" 

        return View();
    }

    public ActionResult Contact()
    {
        string name;

        //the following throws exception as TempData["name"] is null 
        //because we already accessed it in the About() action method
        name = TempData["name"].ToString(); 

        return View();
    }
}
```

```cs
// sử dụng Keep() để retain TempData value for the subsequent requests even after accessing it

public class HomeController : Controller
{
    public ActionResult Index()
    {
        TempData["name"] = "Bill";
        return View();
    }

    public ActionResult About()
    {
        string name;
        
        if(TempData.ContainsKey("name"))
            name = TempData["name"] as string;
        
        TempData.Keep("name"); // Marks the specified key in the TempData for retention.
        
        //TempData.Keep(); // Marks all keys in the TempData for retention

        return View();
    }

    public ActionResult Contact()
    {
        string name;
        
        if(TempData.ContainsKey("name"))
            data = TempData["name"] as string;
            
        return View();
    }
}
```

