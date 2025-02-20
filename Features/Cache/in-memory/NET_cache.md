> https://docs.digitalocean.com/glossary/sticky-session/
> https://topdev.vn/blog/distributed-cache-la-gi-dieu-gi-khien-no-tro-nen-manh-me/

# In-memory caching
* -> giảm thiểu khối lượng công việc trong quá trình lấy những dữ liệu **`truy cập thường xuyên`** và **`ít thay đổi`** 
* -> làm tăng tốc độ tải trang 

## Note: clear cache by default
* **`MemoryCache sẽ được xóa`** mỗi khi IIS app pool được làm mới (**recycle**)
* Ta sẽ cần **`lấy data từ Database thay vì Cache`** (sau đó gán lại data vào cache) nếu WebAPI của ta đạt những default value:
* -> không nhận được bất cứ request nào trong vòng hơn 20 phút
* -> thời gian mặc định cho IIS làm mới Pool là 1740 phút
* -> ta deploy một bản build mới lên thư mục Web API được triển khai trên IIS

## Implement Caching in ASP.NET Web API
* -> sử dụng built-in class **MemoryCache** của **System.Runtime.Caching** (.NET < 4.7)

```cs
// Set cache:
MemoryCacheHelper.Add("topProducts",new List<string>() { "A"}, DateTimeOffset.UtcNow.AddHours(1));

// Get cache:
var topProducts = MemoryCacheHelper.GetValue("topProducts");
```
```cs
public class MemoryCacheHelper
{
    /// <summary>
    /// Get cache value by key
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static object GetValue(string key)
    {
        return MemoryCache.Default.Get(key);
    }

    /// <summary>
    /// Add a cache object with date expiration
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="absExpiration"></param>
    /// <returns></returns>
    public static bool Add(string key, object value, DateTimeOffset absExpiration)
    {
        return MemoryCache.Default.Add(key, value, absExpiration);
    }

    /// <summary>
    /// Delete cache value from key
    /// </summary>
    /// <param name="key"></param>
    public static void Delete(string key)
    {
        MemoryCache memoryCache = MemoryCache.Default;
        if (memoryCache.Contains(key))
        {
            memoryCache.Remove(key);
        }
    }
}
```

==========================================================
# Setup In-Memory Cache Service in ASP.NET Core
* -> ta sẽ s/d **IMmemoryCache** của **Microsoft.Extensions.Caching.Memory** trong **`ASP.NET Core`**

## Enable In-memory caching
* _`add AddMemoryCache() service` để inject vào những service khác_
```cs
IServiceCollection services;
services.AddMemoryCache();
```

## Usage
```cs
using Microsoft.Extensions.Caching.Memory;

public class HomeController : Controller
{
    private IMemoryCache cache;

    public HomeController(IMemoryCache cache)
    {
        this.cache = cache;
    }
    // ....
}
```

## Get(), Set(), TryGet(), GetOrCreate() cache
* -> **`Set()`** - thêm một entry vào cache 
* -> **`Get()`** - lấy dữ liệu từ cache
* -> **TryGetValue()** - **`built-in method`** của .NET kiểm tra sự tồn tại của cache key
* -> **TryGetValue()** - **`built-in method`** của .NET kiểm tra sự tồn tại của cache key

```cs
// Set:
if (!cache.TryGetValue<string>("timestamp", out string timestamp))
{
        cache.Set<string>("timestamp", DateTime.Now.ToString());
}

// Get:
public IActionResult Show()
{
    string timestamp = cache.GetOrCreate<string>("timestamp", entry => { 
        return DateTime.Now.ToString(); 
    });

    return View("Show",timestamp);
}
```

## 