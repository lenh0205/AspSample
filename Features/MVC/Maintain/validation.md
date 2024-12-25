=================================================================
# Implement Data Validation in MVC
* -> include **`data validation`** and **`display validation messages`** on the **violation of business rules** in an ASP.NET MVC applicatio

## add validation metadata by 'Data Annotation' Attributes
* -> ASP.NET MVC includes **`built-in attribute classes`** in the **System.ComponentModel.DataAnnotations** namespace - used to **`define metadata`** for ASP.NET MVC and ASP.NET data controls
* -> we can apply these attributes to **`the properties of the model class`** to **display appropriate validation messages** to the users
```r - all data annotation attributes used for validation
+-------------------+---------------------------------------------------------------------------------------------------+
|    Attribute      |                                           Usage                                                   |
+-------------------+---------------------------------------------------------------------------------------------------+
| Required          | Specifies that a property value is required                                                       |
+-------------------+---------------------------------------------------------------------------------------------------+
| StringLength      | Specifies the minimum and maximum length of characters that are allowed in a string type property |
+-------------------+---------------------------------------------------------------------------------------------------+
| Range             | Specifies the numeric range constraints for the value of a property                               |
+-------------------+---------------------------------------------------------------------------------------------------+
| RegularExpression | Specifies that a property value must match the specified regular expression                       |
+-------------------+---------------------------------------------------------------------------------------------------+
| CreditCard        | Specifies that a property value is a credit card number                                           |
+-------------------+---------------------------------------------------------------------------------------------------+
| CustomValidation  | Specifies a custom validation method that is used to validate a property                          |
+-------------------+---------------------------------------------------------------------------------------------------+
| EmailAddress      | Validates an email address                                                                        |
+-------------------+---------------------------------------------------------------------------------------------------+
| FileExtension     | Validates file name extensions                                                                    |
+-------------------+---------------------------------------------------------------------------------------------------+
| MaxLength         | Specifies the maximum length of array or string data allowed in a property                        |
+-------------------+---------------------------------------------------------------------------------------------------+
| MinLength         | Specifies the minimum length of array or string data allowed in a property                        |
+-------------------+---------------------------------------------------------------------------------------------------+
| Phone             | Specifies that a property value is a well-formed phone number                                     |
+-------------------+---------------------------------------------------------------------------------------------------+
```

## Validate Model state
* -> however, these attributes **only define the metadata for the validations** of the model class 
* -> we need to **`check whether the submitted data - the model state`** is satisfies the requirement specified by all the **data annotation attributes** in the controller by using **ModelState.IsValid**

## implement validation in View
* -> calls the HTML Helper method **ValidationSummary()** **`at the top`** and **ValidationMessageFor()**  for **`every field`** 
* -> the **`ValidationMessageFor()`** **displays a validation message** if **`an error exists for the specified field`** in the **ModelStateDictionary** object
* -> the **`ValidationSummary()`** **displays a list of all the error messages** for all the fields
* => in this way, we can display the default validation message when we submit a form

## Example
```cs - Example
// -> users can not save empty "StudentName" or "Age" value
// -> Age should be between 10 to 20

public class Student
{
    public int StudentId { get; set; }
     
    [Required]
    public string StudentName { get; set; }
       
    [Range(10, 20)]
    public int Age { get; set; }
}

public class StudentController : Controller
{
    public ActionResult Edit(int id)
    {
        // var stud = (get the data from the DB using Entity Framework)
        return View(stud);
    }

    [HttpPost]
    public ActionResult Edit(Student std)
    {
        if (ModelState.IsValid) { //checking model state
            //update student to db
            return RedirectToAction("Index");
        }
        return View(std);
    }
}

```
```cs - Edit.cshtml
@model MVC_BasicTutorials.Models.Student

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
                @Html.ValidationMessageFor(model => model.Age, "", new { @class = "text-danger" })
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

=================================================================
# Data Annotation Model Binder
https://learn.microsoft.com/en-us/aspnet/mvc/overview/older-versions-1/models-data/validation-with-the-data-annotation-validators-cs

=================================================================
## ValidationMessageFor
* **`a strongly typed extension method`** 
* -> **`first parameter`** - a lambda expression to **specify a property** for which we want to show an error message
* -> **`second parameter`** is for **custom error message** if any
* -> **`third parameter`** is for **HTML attributes** such as CSS, style, ...

*  -> when the user **`submits a form that violate the validation`** then ASP.NET MVC **uses the `data-` attribute** of HTML5 for the validation 
* -> and the **default validation message will be injected** when validation error occurs

```cs
@model Student  
@Html.EditorFor(m => m.StudentName) <br />
@Html.ValidationMessageFor(m => m.StudentName, "", new { @class = "text-danger" })
```
```html - result
<input id="StudentName" name="StudentName" type="text" value="" />
<span class="field-validation-valid text-danger" data-valmsg-for="StudentName" data-valmsg-replace="true"></span>
```

```html - when the user submits a form without entering a "StudentName"
<span 
    class="field-validation-error text-danger" 
    data-valmsg-for="StudentName" 
    data-valmsg-replace="true"
>
    The StudentName field is required
</span>
```

## Custom Error Message
* -> display custom error messages **`instead of the default error message`**
* -> we can **`provide a custom error message`** either in **ErrorMessage parameter of the data annotation attribute** or in the **ValidationMessageFor()** method

```cs - ErrorMessage
public class Student
{
    public int StudentId { get; set; }
    [Required(ErrorMessage="Please enter student name.")]
    public string StudentName { get; set; }
    public int Age { get; set; }
}
```

```cs - specify a message in the "ValidationMessage()" method
@model Student  
    
@Html.Editor("StudentName") <br />
@Html.ValidationMessageFor(m => m.StudentName, "Please enter student name.", new { @class = "text-danger" })
```

=================================================================
# 'ValidationSummary' Extension
* -> **displays a summary of all validation errors** on a web page as **`an unordered list element`**, it can also be used to **`display custom error messages`**
* -> the **`ValidationMessageFor`** displays an error message for **an individual field**, whereas the **`ValidationSummary`** displays **all the error messages**
* _tức là hiển thị tất cả error message của tất cả fields tại vị trí `Html.ValidationSummary` được gọi_

* -> if the **`first parameter`** is false, so it will **display the field level errors as a summary**
* -> the **`second parameter`** is for the **message**
* -> the **`third parameter`** is for **HTML attributes**  

* -> we **`added a custom error message`** using the **ModelState.AddModelError()** method
* -> the **`ValidationSummary()`** method will **automatically display all the error messages added into the ModelState**

```cs
public class Student
{
    public int StudentId { get; set; }
    [Required]
    public string StudentName { get; set; }
    [Range(10, 20)]
    public int Age { get; set; }
}
```

```cs - uses the "ValidationSummary()" method to display all the error messages
@model Student  
@Html.ValidationSummary(false, "", new { @class = "text-danger" })

@Html.HiddenFor(model => model.StudentId)    

@Html.EditorFor(m => m.StudentName) <br />
@Html.EditorFor(m => m.Age) <br />

// Nếu ta submit form without populate "Age" and "Name" control, nó sẽ hiện message trên UI:
// The Name field is required
// The Age field is required
```

```cs
public class StudentController : Controller
{
    public ActionResult Edit(int id)
    {
        var stud = ... get the data from the DB using Entity Framework

        return View(stud);
    }

    [HttpPost]
    public ActionResult Edit(Student std)
    {
        if (ModelState.IsValid) { //checking model state
            
            //check whether name is already exists in the database or not
            bool nameAlreadyExists = * check database *       
        
            if(nameAlreadyExists)
            {
                //adding error message to ModelState
                ModelState.AddModelError("name", "Student Name Already Exists.");
    
                return View(std);
            }
            
            return RedirectToAction("Index");
        }

        return View(std);
    }
}
```

