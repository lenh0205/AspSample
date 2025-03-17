
# Note
* -> ta nên hướng đến việc client code của ta chỉ sử dụng những consistence thay vì phụ thuộc trực tiếp vào các concrete type
* -> ta cần hiểu client code ở đây là các class đại diện cho business của ta, ta sẽ inject các dependencies thông qua interface trong constructor của class
* -> chứ việc cần khởi tạo instance từ 1 concrete type và coupling với nó trong 1 application là không thể tránh khỏi, và thường ta sẽ tập trung phần code này lại một chỗ trong method Main chẳng hạn

* -> khi ta định nghĩa 1 interface, nghĩa là có thể có nhiều class implement interface này
* -> nhưng thực tế DI chỉ cho ta chọn 1 concrete type để inject khi class của ta phụ thuộc vào interface tương ứng
* -> vậy nên nếu muốn khởi tạo các instance khác nhau từ các concrete type khác nhau mà có chung interface, thì ta cần phải khởi tạo manually  