# nvm 
## Install
* Vô Github -> chọn "Download" (ngay trên cùng)
## Không thể sử dụng "npm":
* **`nvm on`**
* Hoặc update the **environment variables** to set the **PATH** to where node.js is installed

=============================================================

# Code First

# Khi chỉnh sửa 1 domain model:
* Sửa lại RequestModel, Model, Mapping Profile, fluent API
* Trước tiên là cứ dùng lệnh **`Add-Migration <commit>`**
* Nhớ lại ta đã chỉnh sửa gì trong model, chỉ giữ lại những thay đổi đó trong Migration còn lại xoá hết

# Database First:
```shell - inside "Package Manage Console":
Scaffold-DbContext 'Server=192.168.1.3\\sql2k16,1436;Initial Catalog=VI_QLVB;user=vietinfo;password=Vietinfo@#@!;TrustServerCertificate=True;' Microsoft.EntityFrameworkCore.SqlServer -Tables DM_DONVI_NGUOIDUNG -Context ApplicationDbContext -OutputDir "Domain/Entities/SqlServerCCKL" -ContextDir "Infrastructure/DatabaseFirst"
```

=============================================================

# Để chạy nhiều project trong 1 solution bằng Visual Studio
* right-click vào **`Solution`** -> chọn **`Configure Startup projects`** -> check **`Multiple startup projects`** -> chuyển Action thành **`Start`** -> click **`Apply`** -> click **`OK`**