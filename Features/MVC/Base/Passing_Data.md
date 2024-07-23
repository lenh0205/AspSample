=========================================================================
# Model 

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
# ViewBag
* -> used to **transfer temporary data** (_which is **`not included in the model`**_) from the **controller to the view, not visa-versa** 
* -> it is a **dynamic** type - so we can **assign any number of properties and values to ViewBag** (_if we assign the `same property multiple times`, it will only **`consider last value assigned`**_)
* -> property of the **ControllerBase** class - means it is **`available in any controller or view`** in the ASP.NET MVC application 

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

# Note 
* -> internally, `ViewBag` is a wrapper around **ViewData**; it will **`throw a runtime exception`**, **if the ViewBag property name matches with the key of ViewData** (_in case using both_)
* -> ViewData's life only lasts **during the current HTTP request**, ViewBag values will be **`null if redirection occurs`**
* -> _ViewBag_ **doesn't require typecasting** while **`retrieving values from it`** - this can throw a run-time exception if the wrong method is used on the value
* -> _ViewBag_ is a dynamic type and **`skips compile-time checking`**; so, **ViewBag property names must match between controller and view** while writing it manually

=========================================================================
# ViewData
* -> similar to **`ViewBag`**, but is a **Dictionary** type (_not `dynamic` like ViewBag_) - contain **key-value pair** where **`each 'key' is a string`** 
* -> however, both **store data in the same dictionary internally**; if the **`ViewData Key matches with the property name of ViewBag`** it will throw a runtime exception
* -> in view, to **`retrieve the value using ViewData`**, we need to **typecasting it to an appropriate data type**
* -> **`ViewBag` internally inserts data into `ViewData` dictionary**, so **`the key of ViewData and property of ViewBag must NOT match`**

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
* -> **`stores the data temporarily`** and **automatically removes key-value after retrieving a value**
* -> however, we can still **`keep it for the subsequent request`** by calling **TempData.Keep(keyName)** method (_if not specific **`keyName`**, it will keep all_)

```cs - Transfer data from one "Action Method" to another "Action Method"
// user visit "Index" page first and then to the "About" page and assume that Index.chtml not retrieve TempData 
// -> add data in the 'TempData' in the "Index()" action method and access it in the "About()" action method
// -> sau khi About() retrive data từ TempData thì nó sẽ tự động bị remove, Contact() không thể retrieve nữa

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

```cs - Transfers data from an "Action method" to a "View"
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

```cs - Transfer data from a "View" to "Action method"
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

```cs - retain TempData value for the subsequent requests even after accessing it
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