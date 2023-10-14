
# Viewstate
* use Viewstate to **maintain user input on a form**    
* Without a methodology for tracking page state, a web application **by default is stateless**

```Ví dụ:
A Page with a form that after submitting and finding out we finding out we enter something incorrectly we have to type everything all over again
-> that Page is not maintaining its page state; and in its page lifecycle the Page is destroyed and recreated each time   
```

* In ASP.NET when we submit, we `aren't submitting data from one page to another page`, we are **submitting a page to itself** - this is call **Postback** (_the page posting data back to itselft_)
* -> In ASP.NET WebForm, we use `Master Template`; 
* -> our `Form tag` is in _Master Template_ and not in each page

* **Viewstate** is how ASP.NET WebForms maintain the **page state between postback**
* -> such as: Text input, select value, ... between postback
* -> **make forms "sticky"** (_so that the field values don't disappear after submitting the form_)

* For each of server control on the page, their **state is encoded and send to server** everytime the Page is submitted
* This encoded data is stored in a **hidden field** with the id **`_VIEWSTATE`**

```html
<!-- khi view page source của 1 ASP.NET WebForm page, ta sẽ thấy "hidden field" với encoded data như này: -->
<input type="hidden" name="_VIEWSTATE" id="_VIEWSTATE" value="mfMx....+VX8=" />
```

# ASP.NET View State can affect performance 
* If we have a large amount of controls on a page => we also have a large amount of viewstate being stored

* should we limit the amount of controls on the Page?
* -> No, `Viewstate Tracking could be turn off on the entire pages or individual control` (_ViewStateMode_)
* -> In most case, **disable Viewstate isn't necessary**
* -> because Viewstate `won't be large enough to affect perfomance`
```aspx
<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" ViewStateMode="Disabled">

// ta có thể thêm thuộc tính này vào để không track những "TextBox" như "name", "email",...
```