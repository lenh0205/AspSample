
# Note
* -> ta nên hướng đến việc client code của ta chỉ sử dụng những consistence interface thay vì phụ thuộc trực tiếp vào các concrete type
* -> ta cần hiểu client code ở đây là các class đại diện cho business của ta, ta sẽ inject dependecies theo interface trong constructor của những client code này 
* -> trong App ta có thể khởi tạo các instances từ nhiều concrete type khác nhau có chung interface để sử dụng để pass cho các class constructor khác nhau tùy scenarios
* -> tuy nhiên, ta chỉ có thể chọn 1 concrete type để inject vào constructor của 1 class có interface tương ứng;
* -> vậy nên constructor của những class này trong App đều nhận interface giống nhau sử dụng behavior chung nhưng logic của behavior sẽ khác nhau dựa trên concrete type mà client code sử dụng đế tạo instance rồi pass vào constructor 
* => việc cần khởi tạo instances từ 1 concrete type và coupling với nó trong 1 phần của application là không thể tránh khỏi
* => cái ta cần làm là thu gọn việc khởi tạo instance vào một chỗ cho dễ quản lý

* -> Ví dụ trong 1 base class nó nhiều business logic cần sử dụng instance của cùng 1 concrete type thì thay vì mỗi behavior ta sẽ cần khởi tạo 1 instance thì ta tách việc khởi tạo này vào một method riêng khác
* => các logic sẽ sử dụng consistence **`Factory Method`** này để lấy instance và object được return sẽ có type là **interface** của concrete type đó
* => việc này đảm bảo khi tất cả các logic của 1 subclass muốn sử dụng instance của 1 concrete type khác có cùng interface, ta không cần đi sửa từng behavior mà chỉ đổi phần implementation của "Factory Method" đi là được
* => việc này có thể thực hiện bằng cách ta tạo thêm 1 subclass nữa rồi **method overriding "Factory Method"**, bên trong khởi tạo instance từ 1 concrete type khác mà ta đang cần

* -> sử dụng abstract class nếu ta chắc rằng tất cả các subclasses đều thực hiện 1 behavior theo cách giống nhau trong mọi trường hợp, việc đổi logic của concrete behavior trong base class không làm ảnh hưởng đến tính đúng đắn
