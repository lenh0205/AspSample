
# NET Application Life Cycle
* Người dùng gửi một yêu cầu truy cập vào dữ liệu của ứng dụng. Trình duyệt sẽ gửi yêu cầu này đến Web Server.
* Một đối tượng cũa lớp ApplicationManager được tạo
* Một đối tượng của lớp HostingEnvironment được tạo để cung cấp thông tin về nguồn dữ liệu
* Các thành phần đầu của ứng dụng sẽ được biên dịch
* Các đối tượng như HttpContext, HttpRequest và HttpResponse được khởi tạo và cài đặt.
* Một thể hiện của đối tượng HttpApplication được tạo và gắn cho yêu cầu.
* Các yếu cầu được xử lí bởi lớp HttpApplication, các sự kiện khác nhau được kích hoạt bới lớp này để xử lí các yêu cầu

# ASP.NET Page Life Cycle
## Về cơ bản:
* Khi một trang được yêu cầu 
* -> nó sẽ được load vào bộ nhớ của Server 
* -> xử lí và gửi lại Browser
* -> Sau đó nó sẽ bị giải phóng khỏi bộ nhớ. 
* Vào mỗi bước, các phương thức và sự kiện luôn có sẵn, ta có thể viết lại cách xử lí tương ứng cho mỗi ứng dụng.

Bước 1: Khởi tạo
Khi một trang được yêu cầu, nó sẽ được tải vào bộ nhớ của máy chủ. Trong bước này, các phương thức sau sẽ được gọi:

Page_Load: Phương thức này được gọi khi trang được tải lần đầu tiên.
Init: Phương thức này được gọi trước khi Page_Load để khởi tạo trang.
InitComplete: Phương thức này được gọi sau khi Page_Load để hoàn tất quá trình khởi tạo trang.
Bước 2: Khởi tạo các điều khiển trên trang

Trong bước này, các điều khiển trên trang sẽ được khởi tạo. Các phương thức sau sẽ được gọi cho mỗi điều khiển:

Control_Load: Phương thức này được gọi khi điều khiển được tải lần đầu tiên.
Init: Phương thức này được gọi trước khi Control_Load để khởi tạo điều khiển.
InitComplete: Phương thức này được gọi sau khi Control_Load để hoàn tất quá trình khởi tạo điều khiển.
Bước 3: Phục hồi và duy trì trạng thái

Trong bước này, trạng thái của trang sẽ được phục hồi. Nếu trang đã được yêu cầu trước đó, trạng thái của nó sẽ được lưu vào bộ nhớ của máy chủ.

Bước 4: Thực hiện mã trình xử lý sự kiện

Trong bước này, mã trình xử lý sự kiện sẽ được thực hiện. Điều này bao gồm các sự kiện do người dùng gây ra, chẳng hạn như nhấp chuột hoặc nhấn phím.

Bước 5: Hiển thị trang

Trong bước này, trang sẽ được hiển thị cho người dùng.

Lưu ý: Để xem cây điều khiển của trang, bạn có thể thêm dòng sau vào phần chỉ thị trang:

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="WebForm.aspx.cs" 
Inherits="ASP_P119_Caching.WebForm" Trace="true" %>
Dòng này sẽ bật tính năng theo dõi cho trang, cho phép bạn xem danh sách tất cả các điều khiển trên trang và thứ tự mà chúng được khởi tạo.

Dưới đây là một ví dụ về cách sử dụng vòng đời của một trang ASP.NET:

// Trong bước 1, chúng ta khởi tạo một biến để lưu trữ trạng thái của trang.
private int _count = 0;

// Trong bước 2, chúng ta thêm mã để tăng biến này mỗi khi điều khiển button được nhấp.
protected void Button1_Click(object sender, EventArgs e)
{
// Tăng biến trạng thái.
_count++;

// Hiển thị giá trị mới của biến trạng thái.
Label1.Text = _count.ToString();
}

// Trong bước 3, chúng ta khôi phục trạng thái của trang.
protected void Page_Load(object sender, EventArgs e)
{
// Nếu trang đã được yêu cầu trước đó, chúng ta lấy giá trị của biến trạng thái từ bộ nhớ của máy chủ.
if (IsPostBack)
{
_count = int.Parse(Request.Form["count"]);
}
}

Trong ví dụ này, chúng ta khởi tạo một biến để lưu trữ số lần người dùng nhấp vào nút. Trong bước 2, chúng ta thêm mã để tăng biến này mỗi khi điều khiển button được nhấp. Trong bước 3, chúng ta khôi phục trạng thái của trang. Nếu trang đã được yêu cầu trước đó, chúng ta lấy giá trị của biến trạng thái từ bộ nhớ của máy chủ.

Vòng đời của một trang ASP.NET là một khái niệm quan trọng cần hiểu để có thể phát triển các ứng dụng web ASP.NET hiệu quả