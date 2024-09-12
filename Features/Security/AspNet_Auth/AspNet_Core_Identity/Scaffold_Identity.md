=======================================================================
# Scaffold Identity in ASP.NET Core projects
* -> **ASP.NET Core provides ASP.NET Core Identity** as **`a Razor class library (RCL)`**
* -> **Applications that include Identity** can apply the **`scaffolder`** to **`selectively add the source code contained in the Identity RCL`** 
* -> install **`Microsoft.VisualStudio.Web.CodeGeneration.Design`** Nuget package
* => we might want to generate source code so we can modify the code and change the behavior

* -> **`generated code takes precedence over the same code in the Identity RCL`**
* _For example, we could instruct the scaffolder to generate the code used in registration_

=======================================================================
## Run the 'Identity scaffolder' - Scaffold Identity into a Razor project without existing authorization
* -> hiện tại các view như _Register, Login, LogOut, and RegisterConfirmation,... của Identity_ ta đều đang sử dụng mặc định, để ghi đè nó thì ta sẽ chạy **Identity Scaffold** 
* -> để nó làm hiện ra những view mặc định này trong project của chúng ta (vì bình thường khi mới khởi tạo project chúng sẽ bị giấu đi), từ đó ta có thể sửa code trực tiếp trong những view này

* -> **Process**: right-click on the project > Add > New Scaffolded Item > chọn Identity > chọn Identity > rồi click "Add" và chờ vài giây > chọn view mà ta muốn override (hoặc có option để override tất cả)

* -> chọn **Layout** - nếu ta đã có existing, customized layout page for Identity; select it to **`avoid overwriting our layout with incorrect markup by the scaffolder`**
* Ex: **Pages/Shared/_Layout.cshtml** for Razor Pages or Blazor Server projects
* Ex: **Views/Shared/_Layout.cshtml** for MVC projects or Blazor Server projects with existing MVC infrastructure

* -> ngoài ra ta cần chọn ít nhất 1 **DbContext** (_class với đầy đủ namespace_) cho **`data context`**

* -> nếu ta muốn create a **`data context`** hoặc 1 **`user class`** mới cho Identity, ta sẽ select **"+" button**

* -> cuối cùng nhấn nút "Add" để run scaffolder

## Migrations, UseAuthentication, and layout