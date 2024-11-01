# Khi gặp 1 lỗi có liên quan đến việc lấy dữ liệu
* -> bước đầu tiên cần làm là **`bắt được chính xác câu lệnh SQL đã thực thi lên database`**, nếu database là _SQL Server_ thì ta có thể dung phần mềm **SQL Server Profiler**
* -> kiểm tra tham số đầu vào, cũng như đầu ra của câu SQL

=======================================================================
# Bắt đầu join 1 dự án:
* **Nghiệp vụ**: đối tượng s/d product; có mấy loại user, họ tương tác với nhau thế nào ?
* **Database**: mình cần tương tác với những Database nào; có cần viết mới Database không

# Khởi đầu 1 dự án:
* Bóc tách thành component, estimate thời gian (reused-component, component, thời gian gắn component)
* Tách thời gian viết Frontent, Backend
* tính toán cấu trúc Folder Structure   

## Mock Data
* Frontend nên tạo file Mock data chung cho API, vì viết service rất lâu
* Việc viết Mock data giúp ta hiểu được flow của frontend

## Freature
* Nên list những feature cũng như output mình đã viết
* List ra những lỗi mình đã fix trên feature đó

## Viết Service
* Khi viết Service nên viết đầy đủ method: GetAll, GetById, Update, Create, Delete để việc test dễ dàng hơn  
* Ta có thể làm việc này bằng cách viết 1 Generic Service và kế thừa nó (trừ connected service)

=======================================================================
# Refactor code từ WebForm sang React; từ .NET Framework sang .NET Core:

## Step 1: Trước khi đọc logic code cần ""sắp xếp cấu trúc code" nếu cấu trúc code cũ chưa hợp lý
* -> **`đưa variables đến sát chỗ cần s/d`** - các biến nhiều khi được khai báo rất sớm nhưng phải chạy qua 1 đống logic xử lý không liên quan khác mới đc s/d => gây confuse; ta cần 
* -> những **`logic code dùng chung 1 biến`** thì không được đổi thứ tự trước sau của chúng; vì state của 1 biến sẽ thay đổi ở nhiều chỗ ta không biết được
* -> ta chỉ được thay đổi vị trí các biến; **`không được thay đổi thứ tự xử lý logic`**
* -> **`nhóm những code`** hoặc là cùng không liên quan hoặc là liên quan theo từng step thành 1 nhóm để gần nhau

## Step 2: Từ những khối code ta đã nhóm, phân tích ngược từ Output
* Cần biết **`output`** của từng khối code và những **`step`** cần để đạt được những output đó
* **`Tìm hiểu về sơ về những class type được s/d`**

## Step 3: lần manh mối nếu không hiểu code
* Đối với 1 **action method** cũ mà ta không hiểu nghiệp vụ của nó; ta nên thử tương tác với Page rồi dùng **SQL Profile** để xem nó gửi dữ liệu gì ? tác động với Database nào ?

## Step 4: Phân tách code nào để ở "SER", code nào để ở "HelperCommon"
* **`HelperCommon`** s/d **.NET framework**; **`SER`** s/d **.NET Core** .Vậy nên: 
* -> những code xử lý với Connected Service **WCF** hoặc interact database directly ta sẽ bỏ vào `SER`
* -> những code khác của **.NET Framework, WebForm** ta nên để trên `HelperCommon`
* -> còn đối với những code sử dụng `element control` của WebForm để viết logic, thì ta cần bỏ nó đi và lấy chính xác dữ liệu gì đang được giữ trong element control đó (_VD: "var btn = (ImageButton)sender;" trong đó System.Web.UI.WebControls.ImageButton_) 

## Step 5: Xây dựng flow code mới
* Đối với 1 trang **`control`** có nhiều `text field` và `datagrid` gắn với nhau, ta sẽ cần phân thích 1 `Flow` đầy đủ: binding data, binding text field lần đầu, quản lý State giữa text field và DataGrid,...
* Vẽ `Flow` của code hiện tại ra -> Phân tích
* Vẽ `Flow` của code mới ra

## Refactor từ "Server-side rendering" sang "Client-side rendering"
* Ta nên xem xét dữ liệu trả về
* vì với `Server-side rendering` toàn bộ dữ liệu được giấu đi dưới server, chỉ những dữ liệu render UI mới public
* Ta có thể sẽ cần `Mapping` lại filter đi những `personal field`, map lại `field name`,...
* đồng thời giảm load cho băng thông

=======================================================================
# Viết 1 trang UI phải tái sử dụng nhiều lần (ẩn hiện, disable các control khác nhau)
* -> nếu các logic để điều chỉnh display của từng control trở nên phức tạp, ta nên gom tất cả logic bên trong 1 function và expose các biến display cho từng control

* -> nếu ta sử dụng 1 biến mode để điều chỉnh UI cho từng trang thì cần thiết kế dạng object phân cấp 
* _Ví dụ mode `Parent` sẽ chứa mode `Children` và mode `SecondChilren`: const modeVanBan = { Parent: ["Chilren", "SecondChilren"] }_ 
* -> thì khi đó nếu một trang đang chạy với mode `Children` bình thường, nếu ta h muốn ẩn một nút cụ thể ta chỉ cần tạo ra mode `Parent` và gán cho trang đó
* -> ta sẽ tạo 1 method check phải là mode là `Parent` thì sẽ ẩn nút đó; còn các nút còn lại sẽ chạy như bình thường vì các nút hiện tại chỉ đang check dựa trên mode `Children` mà `Parent` là cha của mode `Children`