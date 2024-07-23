================================================================================
# 'HtmlHelper' class 
* -> **renders `HTML elements` in the razor view** 
* -> **binds the `model` object to HTML controls** to **`display the value of model properties into those controls`**
* -> and also **assigns the value of the controls to the model properties** while **`submitting a web form`**
* => **always** use the **`HtmlHelper class in razor view`** instead of **`writing HTML tags manually`**
* _tức là sự khác biết giữa chúng là `HtmlHelper` method được thiết kế để dễ dàng **bind to view data or model data**_

```cs
@Html.ActionLink("Create New", "Create")
// -> will generate anchor tag <a href="/Student/Create">Create New</a>
```

## 'Html' property
* -> **Html** is **`a property of the 'HtmlHelper' class included in base class of razor view 'WebViewPage'`**

```r - HtmlHelper methods and HTML control each method renders
+-----------------------+-------------------------+---------------------------------------+
|  Extension Method	    |  Strongly Typed Method  |             Html Control              |
|-----------------------|-------------------------|---------------------------------------|
|  Html.ActionLink()    | NA	                  | <a></a>                               |
|-----------------------|-------------------------|---------------------------------------|
|  Html.TextBox()	    | Html.TextBoxFor()	      | <input type="textbox">                |
|-----------------------|-------------------------|---------------------------------------|
|  Html.TextArea()	    | Html.TextAreaFor()	  | <input type="textarea">               |
|-----------------------|-------------------------|---------------------------------------|
|  Html.CheckBox()	    | Html.CheckBoxFor()	  | <input type="checkbox">               |
|-----------------------|-------------------------|---------------------------------------|
|  Html.RadioButton()	| Html.RadioButtonFor()	  | <input type="radio">                  |
|-----------------------|-------------------------|---------------------------------------|
|  Html.DropDownList()  | Html.DropDownListFor()  |	<select><option></select>             |
|-----------------------|-------------------------|---------------------------------------|
|  Html.ListBox()	    | Html.ListBoxFor()	      | multi-select list box: <select>       |
|-----------------------|-------------------------|---------------------------------------|
|  Html.Hidden()	    | Html.HiddenFor()	      | <input type="hidden">                 |
|-----------------------|-------------------------|---------------------------------------|
|  Html.Password()	    | Html.PasswordFor()	  | <input type="password">               |
|-----------------------|-------------------------|---------------------------------------|
|  Html.Display()	    | Html.DisplayFor()	      | HTML text: ""                         |
|-----------------------|-------------------------|---------------------------------------|
|  Html.Label()	        | Html.LabelFor()	      | <label>                               |
|-----------------------|-------------------------|---------------------------------------|
|  Html.Editor()	    | Html.EditorFor()	      | Generates Html controls based on      |
|                       |                         | data type of specified model property |
|-----------------------|-------------------------|---------------------------------------|
```

================================================================================
# TextBox
* -> **creates <input type="text"> control**

* -> the **TextBoxFor<TModel, TProperty>()** is the **`generic extension method of 'HtmlHelper'`** that the lambda expression specifics the property to bind with a textbox - **less error prons and performs fast**
* -> the **TextBox()** method creates HTML control with the **`specified name, value, and other attributes`** - **not a strongly typed extension method`**

```cs - dll
public static MvcHtmlString TextBoxFor<TModel,TProperty> (this HtmlHelper<TModel>> htmlHelper, Expression<Func<TModel,TProperty>> expression, object htmlAttributes);

public static MvcHtmlString TextBox(this HtmlHelper htmlHelper, string name, string value, object htmlAttributes)
``` 

```cs - Ex:
// render a textbox for the "StudentName" property of the "Student" model

@model Student
@Html.TextBoxFor(m => m.StudentName, new { @class = "form-control" }) 
// Output: <input class="form-control" id="StudentName" name="StudentName" type="text" value="" />

@model Student
@Html.TextBox("StudentName") 
// Output: <input id="StudentName" name="StudentName" type="text" value="" />
```

# Text Area
* -> render **multi-line <textarea> HTML control**
* _sử dụng **`TextArea()`** hoặc **`TextAreaFor<TModel, TProperty>()`** như TextBox_

```cs
@model Student
@Html.TextAreaFor(m => m.Description)
// Output: <textarea cols="20" id="Description" name="Description" rows="2"></textarea>

@model Student
@Html.TextArea("Description", "This is dummy description.", new { @class = "form-control" })
// Output: <textarea class="form-control" id="Description" name="Description" rows="2"cols="20">This is dummy description.</textarea>
```

# Checkbox
* -> **generate a <input type="checkbox"> HTML control** in a razor view; also **generated an additional hidden field** with the **`same name`**
* -> when we **`submit a form with a checkbox`**, the **value is posted only if a checkbox is checked**; the **`hidden input with the same name`** will **send 'false' value to the server if checkbox is unchecked**
* -> _using **`Html.CheckBox()`** or **`Html.CheckBoxFor()`**_

```cs
@model Student
@Html.CheckBoxFor(m => m.isActive)
// Output: 
// <input data-val="true" data-val-required="The isActive field is required." id="isActive" name="isActive" type="checkbox" value="true" />
// <input name="isActive" type="hidden" value="false" />

@Html.CheckBox("isActive", true)
// Output: <input checked="checked" id="isActive" name="isActive" type="checkbox" value="true" />
```

# Radio
* -> **generate a <input type="radio"> HTML control** in a razor view
* -> _using **`Html.RadioButtonFor<TModel, TProperty>()`** and **`RadioButton()`**_

```cs
@model Student
@Html.RadioButtonFor(m => m.Gender,"Male") // Ouput: <input checked="checked" id="Gender" name="Gender" type="radio" value="Male" />
@Html.RadioButtonFor(m => m.Gender,"Female") // Output: <input checked="checked" id="Gender" name="Gender" type="radio" value="FeMale" />

Male:   @Html.RadioButton("Gender","Male")  
Female: @Html.RadioButton("Gender","Female")  
// Male: <input checked="checked" id="Gender" name="Gender" type="radio" value="Male" />
// Female: <input id="Gender"name="Gender" type="radio" value="Female" />
```

# DropdownList
* -> **generate the dropdownlist (<select>) HTML control** using the **`HtmlHelper`** in a razor view
* -> _i **`DropDownListFor()`** and **`DropDownList()`**_

```cs
public class Student
{
    public int StudentId { get; set; }
    public string StudentName { get; set; }
    public Gender StudentGender { get; set; }
}
public enum Gender
{
    Male,
    Female    
}

@using MyMVCApp.Models
@model Student
@Html.DropDownListFor(m => m.StudentGender, new SelectList(Enum.GetValues(typeof(Gender))), "Select Gender")
// Output:
// <select class="form-control" id="StudentGender" name="StudentGender">
//    <option>Select Gender</option> 
//    <option>Male</option> 
//    <option>Female</option> 
// </select>

@using MyMVCApp.Models
@model Student
@Html.DropDownList("StudentGender", new SelectList(Enum.GetValues(typeof(Gender))), "Select Gender",new { @class = "form-control" })
// <select class="form-control" id="StudentGender" name="StudentGender">
//    <option>Select Gender</option> 
//    <option>Male</option> 
//    <option>Female</option> 
// </select>
```

# Hidden Field
* -> **generate hidden field** using the **`HtmlHelper`** (_i **`Html.HiddenFor()`** or **`Html.Hidden()`**_) in razor view
* -> the generated <input> has **data-** HTML5 attribute - used for **`validation` ASP.NET MVC**

```cs
@model Student
@Html.HiddenFor(m => m.StudentId)
// <input 
//      data-val="true" 
//      data-val-number="The field StudentId must be a number." 
//      data-val-required="The StudentId field is required." 
//      id="StudentId" 
//      name="StudentId" 
//      type="hidden" 
//      value="" />

@model Student
@Html.Hidden("StudentId")
// <input 
//      id="StudentId" 
//      name="StudentId" 
//      type="hidden" 
//      value="1" />
```

# Password
* -> **generate a password field <input type="password"> element** in a razor view
* _using **`Html.PasswordFor()`** and **`Html.Password()`**_

```cs
public class User
{
    public int UserId { get; set; }
    public string Password { get; set; }
}

@model User
@Html.PasswordFor(m => m.Password)
// Output: <input id="Password" name="Password" type="password" value="" />

@model User
@Html.Password("Password")
// <input 
//      id="Password" 
//      name="Password" 
//      type="password" 
//      value="" />
```

# Display HTML String
* -> **generate html `string literal` for the model object property** in razor view 
* -> using **`Html.DisplayFor()`** and **`Display()`**

```cs
public class Student
{
    public int StudentId { get; set; }
    public string StudentName { get; set; }
    public int Age { get; set; }
}

@model Student
@Html.DisplayFor(m => m.StudentName)
// Output: "Steve"
// -> it generates a html string with the "StudentName" value - Steve

@Html.Display("StudentName")
// Output: "Steve"
```

# Label
* -> **generate HTML <label> element ** - using **`Html.LabelFor()`** and **`Label()` for a specified property of model object**
```cs
public class Student
{
    public int StudentId { get; set; }
    [Display(Name="Name")]
    public string StudentName { get; set; }
    public int Age { get; set; }
}

@model Student
@Html.LabelFor(m => m.StudentName)
// Output: <label for="StudentName">Name</label>

@Html.Label("StudentName","Student Name")
// Output: <label for="StudentName">Student Name</label>
```

# Editor (input element)
* -> **generates HTML `input` elements based on the datatype**
* -> using **`Html.EditorFor()`** or **`Html.Editor()`**

```cs
public class Student
{
    public int StudentId { get; set; }
    [Display(Name="Name")]
    public string StudentName { get; set; }
    public int Age { get; set; }
    public bool isNewlyEnrolled { get; set; }
    public string Password { get; set; }
    public DateTime DoB { get; set; }
}

@model Student
StudentId:      @Html.EditorFor(m => m.StudentId) <br />
Student Name:   @Html.EditorFor(m => m.StudentName) <br />
Age:            @Html.EditorFor(m => m.Age)<br />
Password:       @Html.EditorFor(m => m.Password)<br />
isNewlyEnrolled: @Html.EditorFor(m => m.isNewlyEnrolled)<br />
DoB:            @Html.EditorFor(m => m.DoB)
// StudentId:      <input data-val="true" data-val-number="The field StudentId must be a number." data-val-required="The StudentId field is required." id="StudentId" name="StudentId" type="number" value="" /> 
// Student Name:   <input id="StudentName" name="StudentName" type="text" value="" />
// Age:            <input data-val="true" data-val-number="The field Age must be a number." data-val-required="The Age field is required." id="Age" name="Age" type="number" value="" />
// Password:       <input id="Password" name="Password" type="text" value="" />
// isNewlyEnrolled:<input class="check-box" data-val="true" data-val-required="The isNewlyEnrolled field is required." id="isNewlyEnrolled" name="isNewlyEnrolled" type="checkbox" value="true" />
// <input name="isNewlyEnrolled" type="hidden" value="false" />
// DoB:            <input data-val="true" data-val-date="The field DoB must be a date." data-val-required="The DoB field is required." id="DoB" name="DoB" type="datetime" value="" />
```

================================================================================
