
# Session
* -> cơ chế để **lưu lại dữ liệu của phiên làm việc** cho của ứng dụng 
* -> **`ứng với từng khách truy cập`**

* _Ví dụ nếu người dùng đã đăng nhập, thì thông tin đăng nhập được lưu lại và chuyển cho các trang khác nhau trong phiên làm việc để khỏi mỗi lần gửi request lại phải đăng nhập_
* _hay người dùng chọn đựa mặt hàng vào giỏ hàng thì phải nhớ khi chuyển đến trang thanh toán ..._

# Nơi lưu trữ dữ liệu Session trên Server
* có thể là ở `bộ nhớ Cache`, có thể là ở `CSDL SQLServer` hoặc những nguồn lưu cache khác nhau

# Kích hoạt Session trong ASP.NET
* -> Thêm **ISession** `service` cho phép App làm việc với Session
* -> **`DistributedSession`** class trong App là implementation của `ISession`

```cs - Lưu Session trong Memory
// Thêm Package để App sử dụng Session:
dotnet add package Microsoft.AspNetCore.Session
dotnet add package Microsoft.Extensions.Caching.Memory

// Kích hoạt Session:
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        // Đăng ký dịch vụ lưu cache trong bộ nhớ (Session sẽ sử dụng nó) :
        services.AddDistributedMemoryCache(); 

        // Đăng ký dịch vụ Session:
        services.AddSession(cfg => {                    
            cfg.Cookie.Name = "Lee"; // Đặt tên Session - tên này sử dụng ở Browse (Cookie)
            cfg.IdleTimeout = new TimeSpan(0,60, 0); // Thời gian tồn tại của Session
        });
    }
    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
        // Đăng ký Middleware Session vào Pipeline
        app.UseSession();  
    }
}

// Truy xuất Session thông qua property của HttpContext:
DistributedSession session = context.Session;
```

# Lưu và đọc dữ liệu Session
* các đối tượng sẽ được lưu vào Session **`dưới dạng chuỗi Json`** (_Newtonsoft.Json_) có **`key`** tương ứng

```cs - lưu thông tin một khách truy cập vào trang /Product gồm dữ liệu là "số lần truy cập" và "thời điểm cuối truy cập"
// ProductController.cs:
public void CountAccess(HttpContext context) 
{
    var session = context.Session;
    string key_access = "info_access";
    string json = session.GetString(key_access); // Getter

    // định nghĩa cấu trúc dữ liệu lưu trong Session
    var accessInfoType = new  {
        count = 0, // số lần truy cập
        lasttime = DateTime.Now // thời điểm cuối truy cập
    };

    // check có chưa; nếu chưa có thì khởi tạo
    dynamic lastAccessInfo;
    if (json != null) { 
        lastAccessInfo = JsonConvert.DeserializeObject(json, accessInfoType.GetType());
    }
    else { 
        lastAccessInfo  = accessInfoType; 
    }

    // cập nhật thông tin:
    var accessInfoSave = new {
        count = lastAccessInfo.count + 1,
        lasttime = DateTime.Now
    }; 

    // convert accessInfoSave thành chuỗi Json và lưu lại vào Session:
    string jsonSave = JsonConvert.SerializeObject(accessInfoSave);
    session.SetString(key_access, jsonSave);

    // Output:
    string thongtin = $"Số lần truy cập /Product: {lastAccessInfo.count} 
        + $"lần cuối: {lastAccessInfo.lasttime.ToLongTimeString()}";
    return thongtin;
}
```

==================================================
# Session in WebForm
* -> Session is a **`State Management technique`**
* -> stores some information or data on Server and is used to **`pass from one page to another`**

* _the data is stored for every user separately and is secured also because it's on the server_

# Background
* HTTP protocol is a **`stateless protocol`**
* -> a client sends a request to the server
* -> an instance of the page is created
* -> the page is converted to HTML format
* -> the server provides the response
* -> the instance of the page and the value of the control get destroyed

# Session Timeout
* Session's **`default timeout is 20 minutes`**

* có 2 cách set session timeout:
* -> **`programmatically`**:
```cs
Session.Timeout = 10;  
```
* -> **`web.config`**:
```xml
<configuration>  
    <system.web>  
        <sessionState timeout="30"></sessionState>  
    </system.web>  
</configuration> 
```

# Create Session:
* create an event on **`Login`** WebForm page
```cs - nút "Login" trên form đăng nhập Email, Password
// Login.aspx.cs:
protected void btnLogin_Click(object sender, EventArgs e)  
{  
    Session["UserName"] = txtEmail.Text;  
    Session["PassWord"] = txtPassword.Text;  
    Response.Redirect("User.aspx");  
}

// User.aspx.cs: (truy cập vào session trên 1 trang khác)
protected void Page_Load(object sender, EventArgs e)  
  {  
     if (!IsPostBack)  
       {  
          if (Session["UserName"] != null)  // true 
              {  
                lblUser.Text = "Welcome to" + "  " + Session["UserName"].ToString();  
              }                  
       }  
  }  
```

# Session Mode: InProc vs OutProc

## InProc - The Default session state mode
* Sessions state are stored **`in the memory space inside of the ASP.NET app worker process on the webserver - aspnet_wp.exe`** in the application domain

* _Vì nó là default nên không cần config cũng tự hiểu là "InProc" mode_
```xml - web.config
<configuration>  
   <system.web>  
      <sessionState mode="InProc"></sessionState>  
   </system.web>  
</configuration>
```

## OutProc
* _Session data_ are stored **`in a different memory location`**: _SQL Server, State Server_

* **SQL Server**:
* -> Session data are stored in **`different Servers`** using **SQL Server database**
* -> connects through **`ConnectionString`**
```xml - web.config
<configuration>  
   <system.web>  
      <sessionState mode="SQLServer" sqlConnectionString="ConnectionString"></sessionState>  
   </system.web>  
</configuration>
```

**State Server**
* -> Session data are stored in **`different servers`** using **State Server windows service**
```xml - web.config
<configuration>  
   <system.web>  
      <sessionState mode="StateServer" stateConnectionString="ConnectionString"></sessionState>  
   </system.web>  
</configuration>
```