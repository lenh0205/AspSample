=============================================================

## MapControllerRoute
* -> most often used in an **MVC application**
* -> uses **conventional routing**, and **`sets up the URL route pattern`**

* -> but it's entirely possible to use MapControllerRoute (and by proxy MapDefaultControllerRoute) **`along side attribute routing`** as well
* -> if the user does not supply attributes, it will **`use the defined default pattern`**
*_về cơ bản thì thằng này có thể làm hết, không cần `route attribute` và còn cho ta `default route` nếu không tìm thấy_

```cs - Example:
endpoints.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);
```
* -> the above pattern is basically **`{{root_url}}/{{name_of_controller}}/{{name_of_action}}/{{optional_id}}`** (_if controller and action are not supplied, it defaults to home/index_)
* -> but we could set this to whatever we wanted (within reason) and our routes would follow this pattern

## MapDefaultControllerRoute
* -> this is the above, but it **`shorthands the configuration of the default pattern`** that we displayed above

## MapControllers
* -> most commonly used in **WebAPI controllers**
* -> this doesn't make any assumptions about routing and will **`rely on the user doing attribute routing`** to get requests to the right place