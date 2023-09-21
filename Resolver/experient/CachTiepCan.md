# Bắt đầu join 1 dự án:
* **Nghiệp vụ**: đối tượng s/d product; có mấy loại user, họ tương tác với nhau thế nào ?
* **Database**: mình cần tương tác với những Database nào; có cần viết mới Database không

## Mock Data
* Frontend nên tạo file Mock data chung cho API, vì viết service rất lâu
* Việc viết Mock data giúp ta hiểu được flow của frontend

## Freature
* Nên list những feature cũng như output mình đã viết
* List ra những lỗi mình đã fix trên feature đó

## Viết Service
* Khi viết Service nên viết đầy đủ method: GetAll, GetById, Update, Create, Delete để việc test dễ dàng hơn  
* Ta có thể làm việc này bằng cách viết 1 Generic Service và kế thừa nó (trừ connected service)

# Refactor code:
* Đối với 1 trang **`control`** có nhiều `text field` và `datagrid` gắn với nhau, ta sẽ cần phân thích 1 `Flow` đầy đủ: binding data, binding text field lần đầu, quản lý State giữa text field và DataGrid,...
* Vẽ `Flow` của code hiện tại ra -> Phân tích
* Vẽ `Flow` của code mới ra

## Refactor từ "Server-side rendering" sang "Client-side rendering"
* Ta nên xem xét dữ liệu trả về
* vì với `Server-side rendering` toàn bộ dữ liệu được giấu đi dưới server, chỉ những dữ liệu render UI mới public
* Ta có thể sẽ cần `Mapping` lại filter đi những `personal field`, map lại `field name`,...
* đồng thời giảm load cho băng thông