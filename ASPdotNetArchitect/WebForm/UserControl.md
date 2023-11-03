# User Control
* tạo ra bằng cách customize, kết hợp nhiều control
* Behaves like miniature ASP.NET pages or web forms, which `could be used by many other pages`
* Have an ``.ascx` extension

## Create 
* right-click on project file -> add new item -> chọn `Web User Control` template
* nó sẽ tạo 1 .ascx file: ta sẽ không thấy "Master" directive , MasterPage page directiv;chỉ thấy "control" directive 
```cs
// FileUploader.ascx
<%@ Control Language="C#" AutoEventWireup="true" CodeFile="FileUploader.ascx.cs" Inherits="FileUploader" %>
```