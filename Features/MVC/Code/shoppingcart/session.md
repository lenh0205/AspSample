> việc sử dụng session rất thích hợp khi ta muốn persist giỏ hàng từ lúc user chưa login rồi sau đó login
https://xuanthulab.net/asp-net-core-mvc-xay-dung-gio-hang-cart-voi-net-core.html

=================================================================
# "Shopping cart" feature by 'session'
* -> dùng session để lưu 1 list `CartItem`
* _ta sẽ thêm nút `Add to Cart` vào trang chi tiết của từng sản phẩm_
* _tạo 1 trang để xem chi tiết giỏ hàng_

```cs - Register using Session
// program.cs
services.AddSession();

app.UseSession();
```

```cs - hoặc
services.AddDistributedMemoryCache();  // đăng ký dịch vụ lưu cache trong bộ nhớ (Session sẽ sử dụng nó)
services.AddSession(cfg => { // Đăng ký dịch vụ Session
    cfg.Cookie.Name = "mySession";   // tên Session - sử dụng ở Browser (Cookie)
    cfg.IdleTimeout = new TimeSpan(0,30, 0); // Thời gian tồn tại của Session
});
```

## write "Extension" to interact with session easier
```cs - Session Extension
// ví 'session' chỉ hỗ trợ kiểu "string" và "int"; vậy nên ta sẽ viết extension để hỗ trợ kiểu phức tạp hơn
public static class SessionExtensions
{
    public static void Set<T>(this ISession session, string key, T value)
    {
        session.SetString(key, JsonSerializer.Serialize(value));
    }
    public static T Get<T>(this ISession session, string key)
    {
        var value = session.GetString(key);
        return value == null ? default : JsonSerializer.Deserialize<T>(value);
    }
} 
```

## Model
```cs - create model
public class CartItem
{
    public int MaHh { get; set; } // id 
    public string TenHh { get; set; }
    public string Hinh { get; set; }
    public double DonGia { get; set; }
    public int SoLuong { get; set; }
    public double ThanhTien => SoLuong * DonGia;
}
```

## Action
```cs - controller
public class CartController : Controller
{
    private readonly MyStore2020Context _context;
    public const string CARTKEY = "GioHang";

    public List<CartItem> Carts
    {
        get 
        {
            var data = HttpContext.Session.Get<List<CartItem>>(CARTKEY);
            if (data == null) data = new List<CartItem>();
            return data;
        }
    }

    public CartController(MyStore2020Context context)
    {
        _context = context;
    }

    public IActionResult Index() // trang giỏ hàng hiển thị các item
    {
        return View(Carts); 
    }
    public IActionResult AddToCart(int id, int SoLuong)
    {
        var myCart = Carts;
        var item = myCart.SingleOrDefault(p => p.MaHh == id);

        if (item == null) 
        {
            var hangHoa = _context.HangHoa.SingleOrDefault(p => p.MaHh == id);
            item = new CartItem { 
                MaHh = id,
                TenHh = hangHoa.TenHh,
                DonGia = hangHoa.DonGia.Value,
                SoLuong = SoLuong,
                Hinh = hangHoa.Hinh
            };
            myCart.Add(item);
        }
        else 
        {
            item.SoLuong += SoLuong;
        }
        HttpContext.Session.Set(CARTKEY, myCart);

        return RedirectToAction("Index");
    }
}
```

```js - ~/Views/Product/Detail.cshtml
<form
    asp-action="AddToCart"
    asp-controller="Cart"
    asp-route-id="@Model.MaHh" // gửi query param là "id" vào action
>
    <input type="text" name="SoLuong">
    <button class="buy-now btn btn-sm btn-primary">Add To Cart</button>
</form>
```

```cs - ~/Views/Cart/Index.cshtml
@model IEnumerable<CartItem>

@{
    ViewData["Title"] = "Index";
    Layout = "~/Views/Shared/_Frontend.cshtml";
}

@section breadcrum {
    <div class="bg-light py-3">
        <div class="container">
            <div class="row">
                <div class="col-md-12 mb-0">
                    <a href="index.html">Home</a>
                    <span class="mx-2 mb-0">/</span>
                    <strong class="text-back">Cart</strong>
                </div>
            </div>
        </div>
    </div>    
}

@foreach(var item in Model)
{
}
```

=================================================================
# add item to shopping cart using 'AJAX'
* _trong trang show các sản phẩm dưới dạng 1 grid gồm nhiều cards; mỗi card ta sẽ cho thêm `Add to Cart`_
* _đồng thời s/d `AJAX` để tăng số lượng trong `bage của shopping cart icon` mà không phải reload lại trang_ 

```cs - ~/Views/Product/Index.cshtml
@{
    ViewData["Title"] = "Index";
    Layout = "~/Views/Shared/_Frontend.cshtml";
}

@foreach(var item in Model)
{
    ...
    <button data-id="@item.MaHh" class="ajax-add-to-cart">
        Add To Cart
    </button>
}

@section scripts {
    <script>
        $(document).ready(function() {
            $(".ajax-add-to-cart").click(function() {
                $.ajax({
                    url: "/Cart/AddToCart",
                    data: {
                        id: $(this).data("id");
                        SoLuong: 1,
                        type: "ajax"
                    },
                    success: function(data) {
                        $("#cart_count").html(data.soLuong);
                    },
                    error: function() {
                        console.error("Them gio hang that bai");
                    }
                })
            })
        });
    </script>
}
```
```cs - ~/Views/Shared/_Frontend.cshtml
<a href="cart.html" class="site-cart">
    <span class="icon icon-shopping-cart"></span>
    <span class="count" id="cart_count">
        @{
            var data = HttpContext.Session.Get<List<CartItem>>("GioHang");
            if (data == null) data = new List<CartItem>();
        }
        @(data.Sum(c => c.SoLuong))
    </span>
</a>

@RenderSection("scripts", required: false)
```

```cs - CartController
public IActionResult AddToCart(int id, int SoLuong, string type = "Normal")
{
    // ....
    HttpContext.Session.Set(CARTKEY, myCart);

    if (type == "ajax") return Json(new {
        SoLuong = Carts.Sum(c => c.SoLuong)
    });
}
```