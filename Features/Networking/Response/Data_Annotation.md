
# Action Selectors
* -> _Action selector_ is the **`attribute`** that can be applied to the **`action methods`**
* -> it **helps the `routing engine` to select the `correct action method` to handle a particular request**
* -> MVC 5 includes the following action selector attributes: **ActionName**, **NonAction**, **ActionVerbs**

## ActionName
* -> allows us to **specify a different action name than the method name**

```cs
[ActionName("Find")]
public ActionResult GetById(int id)
{
    // get student from the database 
    return View();
}
// it'll be invoked on http://localhost/student/find/1 instead of http://localhost/student/getbyid/1 
```

## NonAction
* -> used when we want **`public method in a controller`** but **do not want to treat it as an action method**

```cs
public string Index() // is Action method
{
        return "This is Index action method of StudentController";
}

[NonAction]
public Student GetStudent(int id) // is not Action method
{
    return studentList.Where(s => s.StudentId == id).FirstOrDefault();
}
```

## ActionVerbs
* -> to **`handle different type of Http requests`**
* -> includes **HttpGet**, **HttpPost**, **HttpPut**, **HttpDelete**, **HttpOptions**, **HttpHead**, **HttpOptions**, **HttpPatch** action verbs
* -> we can **apply one or more action verbs to an action method** to handle different HTTP requests
* _if we don't apply any action verbs to an action method, then it will **`handle HttpGet request by default`**_

```cs
[AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)] // apply multiple action verb by using "AcceptVerbs"
public ActionResult GetAndPostAction()
{
    return RedirectToAction("Index");
}

[HttpDelete]
public ActionResult DeleteAction() // handles DELETE requests by default
{
    return View("Index");
}

[HttpHead]
public ActionResult HeadAction() // handles HEAD requests by default
{
    return View("Index");
}
    
[HttpOptions]
public ActionResult OptionsAction() // handles OPTION requests by default
{
    return View("Index");
}
```