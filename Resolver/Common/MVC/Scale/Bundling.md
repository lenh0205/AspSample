======================================================================
# Bundling and Minification
* -> _bundling and minification techniques_ were introduced in MVC 4 to **improve request load time**
* -> **`bundling`** allows us to **load the bunch of static files from the server in a single HTTP request**
* _tức là thay vì mỗi request lấy 1 file .js thì gom tất cả file .js thành 1 rồi request file đó thôi_

## Minification
* ->  **removing unnecessary `white space` and `comments` and `shortening variable names` to one character**
* => **`optimizes script or CSS file size`** - improve the loading time of the page

```js
// normally
sayHello = function(name){
    //this is comment
    var msg = "Hello" + name;
    alert(msg);
}

// minify version
sayHello=function(n){var t="Hello"+n;alert(t)}
```

## Bundle Types
* _MVC 5 includes following bundle classes in **`System.web.Optimization`** namespace:_
* -> **ScriptBundle** - responsible for **`JavaScript minification of single or multiple script files`**
* -> **StyleBundle** - responsible for **`CSS minification of single or multiple style sheet files`**
* -> **DynamicFolderBundle** - represents **`a Bundle object`** that ASP.NET creates from a folder that contains **`files of the same type`**

* -> we can create style or script bundles in BundleConfig class under App_Start folder in an ASP.NET MVC project. (you can create your own custom class instead of using BundleConfig class, but it is recommended to follow standard practice.)

======================================================================
