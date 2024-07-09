
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

```
_____________________________________________________________
|  Extension Method	Strongly Typed Method	Html Control
|  Html.ActionLink()	 NA	<a></a>
|  Html.TextBox()	     |Html.TextBoxFor()	<input type="textbox">
|  Html.TextArea()	     |Html.TextAreaFor()	<input type="textarea">
|Html.CheckBox()	     |Html.CheckBoxFor()	<input type="checkbox">
|Html.RadioButton()	 |Html.RadioButtonFor()	<input type="radio">
|Html.DropDownList()	 |Html.DropDownListFor()	<select><option></select>
|Html.ListBox()	     |Html.ListBoxFor()	multi-select list box: <select>
|Html.Hidden()	     |Html.HiddenFor()	<input type="hidden">
|Html.Password()	     |Html.PasswordFor()	<input type="password">
|Html.Display()	     |Html.DisplayFor()	HTML text: ""
|Html.Label()	     |Html.LabelFor()	<label>
|Html.Editor()	     |Html.EditorFor()	Generates Html controls based on data type of specified model property e.g. textbox for string property, numeric field for int, double or other numeric type.

```