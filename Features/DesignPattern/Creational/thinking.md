
# Note
* -> ta nên hướng đến việc client code của ta chỉ sử dụng những consistence interface thay vì phụ thuộc trực tiếp vào các concrete type
* -> ta cần hiểu client code ở đây là các class đại diện cho business của ta, ta sẽ inject các interface của dependencies trong constructor của class
* -> chứ việc cần khởi tạo instance từ 1 concrete type và coupling với nó trong 1 phần của application là không thể tránh khỏi (ví dụ tập trung phần code này tại method Main)

* -> trong App ta có thể khởi tạo các instances từ nhiều concrete type khác nhau có chung interface để sử dụng tùy scenarios
* -> tuy nhiên, ta chỉ có thể chọn 1 concrete type để inject vào constructor của 1 class có interface tương ứng; vậy nên constructor của những class này đều nhận interface giống nhau sử dụng behavior chung nhưng logic của behavior sẽ khác nhau dựa trên concrete type mà client code sử dụng đế tạo instance rồi pass vào constructor 

* -> sử dụng abstract class nếu ta chắc rằng tất cả các subclasses đều thực hiện 1 behavior theo cách giống nhau trong mọi trường hợp, việc đổi logic của concrete behavior trong base class không làm ảnh hưởng đến tính đúng đắn
