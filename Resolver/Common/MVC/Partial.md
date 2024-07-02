# Partial view:
* -> a **Razor markup (.cshtml) file** that **`render HTML output within another markup file`**
* -> it's not a complete view, but instead used for **the `reusability` of the HTML markup** 
* -> include **break up large markup** files into smaller components; **reduce the duplication** of common markup content across markup file; **`shouldn't be used to maintain common layout elements`**

* -> _there are different ways of rendering partial view in MVC Razor: **RenderPartial**, **RenderAction**, **Partial**, **Action**_

=========================================================================
# Html.RenderPartial