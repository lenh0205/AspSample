=======================================================================
# Scaffold Identity in ASP.NET Core projects
* -> **ASP.NET Core provides ASP.NET Core Identity** as **`a Razor class library (RCL)`**
* -> **Applications that include Identity** can apply the **`scaffolder`** to **`selectively add the source code contained in the Identity RCL`** 
* -> install **`Microsoft.VisualStudio.Web.CodeGeneration.Design`** Nuget package
* => we might want to generate source code so we can modify the code and change the behavior

* -> **`generated code takes precedence over the same code in the Identity RCL`**
* _For example, we could instruct the scaffolder to generate the code used in registration_

> Bên dưới là cách ta sẽ dùng `Identity scaffolder` lên project (cả **Razor pages** và **MVC**) **`chưa setup Authorization`** và **`đã được setup Authorization`**
> Ex: khi ta khởi tạo 1 project bằng template `ASP.NET Core Web App (Razors Page)` và chọn `Authentication type` option là "Individual Account" thì nó sẽ tạo project được setup sẵn **ASP.NET Core Identity**

=======================================================================
# Scaffold Identity into a Razor project without existing authorization
* -> trong phần **Choose files to override** ta có thể chọn page mà ta sẽ muốn ghi đè 
* -> hoặc không chọn gì cũng được nếu chỉ muốn nó sử dụng **`ASP.NET Core Identity`** để setup Authentication và sử dụng default UI 

* -> **Process**: right-click on the project > Add > New Scaffolded Item > chọn Identity > chọn Identity > rồi click "Add" và chờ vài giây > chọn view mà ta muốn override (hoặc có option để override tất cả)

* -> chọn **Layout** - nếu ta đã có existing, customized layout page for Identity; select it to **`avoid overwriting our layout with incorrect markup by the scaffolder`**
* Ex: **Pages/Shared/_Layout.cshtml** for Razor Pages or Blazor Server projects
* Ex: **Views/Shared/_Layout.cshtml** for MVC projects or Blazor Server projects with existing MVC infrastructure

* -> ngoài ra ta bắt buộc phải chọn ít nhất 1 **DbContext** (_class với đầy đủ namespace_) cho **`data context`** để chạy được chức năng Identity Scaffold
* -> ta có thể chọn DbContext đã có (nếu có)
* -> hoặc nếu ta muốn tạo mới 1 **`data context`** hoặc 1 **`user Class`** cho Identity, ta sẽ select **"+" button**

* -> cuối cùng nhấn nút "Add" để run scaffolder

* => _về cơ bản thì nó sẽ_
* -> cài cho ta những Nuget Package để làm việc với **`ASP.NET Core Identity`** và **`Entity Framework`**
* -> update file **`Program.cs`** để thêm phần cấu hình Authentication với **AddDefaultIdentity** và **AddDbContext**
* -> update file **`appsettings.cs`** để thêm **ConnectStrings**
* -> thêm partial view **`_LoginPartial.cshtml`**
* -> thêm 2 thư mục **`Data`** (**DbContext** và **User** class) và **`Pages`** vào **Areas/Identity**   

## Migrations, UseAuthentication, and layout
* -> the **`generated Identity database code`** requires Entity Framework (EF) Core **Migrations**
* -> if a migration to generate the **Identity schema** hasn't been created and applied to the database, ta sẽ sử dụng **Code First** để create a migration and update the database

* -> in **Solution Explorer**, double-click **Connected Services** > in the **SQL Server Express LocalDB area** of Service Dependencies, select the ellipsis **...** > select **`Add migration`**
* -> ta sẽ đặt tên cho **Migration** và wait for the database context to load in the **DbContext class names** field (_nếu nó không load được ta hãy tắt Visual Studio đi rồi mở lại_)
* -> Finish > Close
* -> tiếp tục chọn **...** trong **SQL Server Express LocalDB area** để chọn **`Update database`**
* -> wait for the DbContext class names field to update and for prior migrations to load > Finish > Close

## Update Layouts
* -> ta sẽ thêm partial view **`_LoginPartial.cshtml`** vào Layout của ta để dễ hàng Register và Login

=======================================================================
# Scaffold Identity into a Razor project with authorization
* -> thằng này lúc khởi tạo đã được cấu hình sẵn **`ASP.NET Core Identity`** rồi; ta chỉ cần **Identity Scaffold** để ghi đè thôi

=======================================================================
# Scaffold Identity into an MVC project without existing authorization
* -> tương tự như _`Scaffold Identity into a Razor project without existing authorization`_ thôi
* -> nhưng vì **ASP.NET Core Identity** được cung cấp dưới dạng **`a Razor class library (RCL)`**, nên trong **Program.cs** ta cần thêm **`MapRazorPages()`**:
```cs
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
```

=======================================================================
