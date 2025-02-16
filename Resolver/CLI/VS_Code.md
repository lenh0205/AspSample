# Visual Studio Code
* -> Tạo project web api: 
```bash
$ dotnet new webapi -n MyWebApi
# -n thì nó sẽ tạo 1 thư mục chứa file .csproj có cùng tên, không có thì nó sẽ gen code bao gồm cả file .csproj có tên giống với folder đang chứa nó
```

* -> Tạo 1 file solution:
```bash
$ dotnet new sln -n MySolution
```

* -> add project to solution
```bash
$ dotnet sln MySolution.sln add some_path/MyProject.csproj
```

* -> setup git
```bash
$ git init
$ dotnet new gitignore
```

* -> add reference của projectB cho projectA
```bash
$ cd path_to_projectA.csproj
$ dotnet add reference some_path/projectB.csproj
```

* -> Run thử project vừa tạo (xem nó có mở browser vs swagger không)
```bash
$ dotnet watch run
$ dotnet run --project some_path/MyProject.csproj
```

* -> cài Postgres - password: 12345 port: 5432

* -> cài Nuget package:
```bash
$ dotnet add package Microsoft.EntityFrameworkCore --version 6.0.0
$ dotnet add package Microsoft.EntityFrameworkCore.Design --version 6.0.0
$ dotnet add package Microsoft.EntityFrameworkCore.Tools --version 6.0.0
$ dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL --version 6.0.0
```

* -> để remove package: 
```bash
$ dotnet remove package Newtonsoft.Json
```

* -> để check version những package ta đã cài đặt cũng như target của project:
```bash
$ dotnet list package
```

* -> nếu cài package success thì chạy thử "dotnet watch run" xem 
- nếu bị lỗi thì thử đọc log trong console lúc cài package xem có gì không
- Ví dụ: trong trường hợp của "Microsoft.AspNetCore.Identity"
- khi ta search trên "nuget.org" ta sẽ thấy version mới nhất là 2.3.0 và nó sẽ support
từ .NET 5,.NET Core 2.0, .NET Standard 2.0, .NET Framework 4.6.1 trở lên
- đồng thời dependencies của nó support Microsoft.AspNetCore.Cryptography.KeyDerivation 
(>= 2.3.0)
- nhưng khi ta cài đặt nó vào 1 .NET 6 web API project thì sẽ log ra là "Detected 
package downgrade: Microsoft.AspNetCore.Cryptography.KeyDerivation from 3.0.0 to 
2.3.0"
- ta chỉ cần chỉ định lại phiên bản cho "Microsoft.AspNetCore.Cryptography.KeyDerivation"
là được: dotnet add package Microsoft.AspNetCore.Cryptography.KeyDerivation --version 3.0.0

* -> chạy migration
// cài dotnet-ef nếu chưa có: dotnet tool install --global dotnet-ef --version 6.0.0
// để uninstall: dotnet tool uninstall dotnet-ef --global 
// để update: dotnet tool update dotnet-ef --version 6.0.8 --global
// check xem cài thành công chưa: dotnet ef 

// để chạy dotnet-ef đòi hỏi project phải có "Microsoft.EntityFrameworkCore.Design"
// đồng thời nhớ tắt "dotnet watch run" và chạy thử "dotnet build" trước khi migrate
// dotnet ef migrations add InitialCreate
// dotnet ef database update

* -> build project:
// dotnet build
// dotnet build --configuration Release
// dotnet build -p:Version=1.2.3.4

* -> clean project:
dotnet clean

* -> Authen:
// dotnet add package Microsoft.AspNetCore.Identity --version 2.3.0
// dotnet add package Microsoft.AspNetCore.Identity.EntityFrameworkCore --version 6.0.0

 