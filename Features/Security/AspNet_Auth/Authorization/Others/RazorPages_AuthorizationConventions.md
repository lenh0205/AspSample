===================================================================
# Razor Pages authorization conventions in ASP.NET Core
* -> one way to **`control access in your Razor Pages app`** is to use **authorization conventions** at **startup**
* -> these conventions allow us to **authorize users** and **allow anonymous users** to **`access individual pages or folders of pages`**
* -> the **conventions** automatically apply **`authorization filters`** to control access
* _this can apply to app using ASP.NET Core Identity also_


```cs
public void ConfigureServices(IServiceCollection services)
{
    services.AddControllers();

    #region
    services.AddRazorPages(options =>
    {
        options.Conventions.AuthorizePage("/Contact");
        options.Conventions.AuthorizeFolder("/Private");
        options.Conventions.AllowAnonymousToPage("/Private/PublicPage");
        options.Conventions.AllowAnonymousToFolder("/Private/PublicPages");
    });
    #endregion

    services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie();
}
```

===================================================================
## config "authorization convention" 

* -> the **`AuthorizePage` convention** to add an **`AuthorizeFilter`** to the **page** at the specified path
* _the **`specified path`** is the "View Engine path", which is **the Razor Pages root relative path without an extension** and containing only forward slashes_

* -> the **`AuthorizeFolder` convention** to add an **`AuthorizeFilter`** to **all of the pages in a folder** at the specified path
* _the **`specified path`** is the View Engine path, which is the **Razor Pages root relative path**_

* -> the **`AuthorizeAreaPage` convention** to add an **`AuthorizeFilter`** to the **area page** at the specified path
* _the **`page name`** is the **path of the file without an extension relative to the pages root directory for the specified area**_
* _for example: the page name for the file Areas/Identity/Pages/Manage/Accounts.cshtml is /Manage/Accounts_

* -> the **`AuthorizeAreaFolder` convention** to add an **`AuthorizeFilter`** to **all of the areas in a folder** at the specified path
* _the **`folder path`** is **the path of the folder relative to the pages root directory** for the specified area_
* _For example, the folder path for the files under Areas/Identity/Pages/Manage/ is /Manage._

* -> to specify **`an authorization policy`**, use overload:
```cs
options.Conventions.AuthorizePage("/Contact", "AtLeast21");
options.Conventions.AuthorizeFolder("/Private", "AtLeast21");
options.Conventions.AuthorizeAreaPage("Identity", "/Manage/Accounts", "AtLeast21");
options.Conventions.AuthorizeAreaFolder("Identity", "/Manage");
options.Conventions.AuthorizeAreaFolder("Identity", "/Manage", "AtLeast21");
```

===================================================================
# Allow anonymous access to a page

* -> the **`AllowAnonymousToPage` convention** to add an **`AllowAnonymousFilter`** to **a page** at the specified path
* _the **`specified path`** is the View Engine path, which is **the Razor Pages root relative path without an extension** and containing only forward slashes_

* -> the **`AllowAnonymousToFolder` convention** to add an **`AllowAnonymousFilter`** to **all of the pages** in a folder at the specified path
* _the **`specified path`** is the View Engine path, which is the **Razor Pages root relative path**_

===================================================================
# Note on combining authorized and anonymous access
* -> it's **`valid`** to specify that **a folder of pages `requires authorization`** and then **specify that a page within that folder `allows anonymous access`**:
```cs
.AuthorizeFolder("/Private").AllowAnonymousToPage("/Private/Public") // this works
```

* -> however, the reverse **`isn't valid`** - can't declare a folder of pages for anonymous access and then specify a page within that folder that requires authorization
* -> when both the **AllowAnonymousFilter** and **AuthorizeFilter** are applied to the page, the **`AllowAnonymousFilter takes precedence and controls access`**
```cs
.AllowAnonymousToFolder("/Public").AuthorizePage("/Public/Private") // this doesn't work!
```