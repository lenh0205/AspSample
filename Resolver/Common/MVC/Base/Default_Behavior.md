
# Standard rules for view lookup

# Rule for _ViewStart.cshtml lookup
* -> the **`default _ViewStart.cshtml file`** is included in the **Views** folder, but can also be created in **`all other folders/sub-folder of root (but not the root itself)`**
* -> it is used to **specify common settings for all the `views` under a folder and sub-folders where it is created**

* -> we can create **multiple "_ViewStart.cshtml" files** in **`different folder locations`** within the **`Views folder`** hierarchy
* -> when a view is rendered, ASP.NET will first look for a **`_ViewStart.cshtml`** file in the same folder as the view, and then **search up the folder hierarchy until it finds one**

* _For example, we render `Index.cshtml` view that's located inside `~/Views/Home` folder;_
* _then first, ASP.NET will find the `_ViewStart.cshtml` file in the `Home` folder; if not found, if we continue find it in next hierachy - `Views` folder; if not found, it stop because next one is the root_
* _so in this case, if we have only one `_ViewStart.cshtml` file locate in `~/Views/Shared` folder then the `Home/Index.cshtml` will render without any setting (Ex: Layout) take from `_ViewStart.cshtml` file_