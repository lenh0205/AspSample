> những thằng pattern này tăng độ phức tạp cho project nên chỉ nên sử dụng trong project lớn

==============================================================
# 'Mediator' pattern
* -> **`reduce chaotic dependencies between objects`**
* -> by **restricts direct communications** between the objects and forces them to **depend only on a single mediator object**
* _nó kiểu đổi quan hệ many-to-many thành one-to-many bằng cách tạo ra 1 thằng trung gian; tránh những giao tiếp phức tạp giữa các component_

* => **`Single Responsibility Principle`** - communication giữa các component được tổng hợp về một nơi
* => **`Open/Closed Principle`** - trường hợp cần mở rộng, có thể dễ dàng tạo ra mediator mới (_vì các component chỉ phụ thuộc vào "Mediator Interface"_)

## Members
* -> **`Components`** là các class chứa logic xử lí business; ta sẽ DI **Mediator Interface** đồng thời establish connection trong constructor của nó
* -> **`Mediator Interface`** khai báo những method phục vụ **giao tiếp với components** - thường bao gồm 1 method là **notification method**
* -> **`ConcreteMediator`** **giữ tham chiếu tới tất cả component cũng như đóng gói relation giữa chúng**, đôi khi còn **quản lý vòng đời của component**

## Others
* -> if we implement **`Mediator`** in **subscribe and unsubscribe**, nó sẽ khá giống **`Observer pattern`**

==============================================================
# 'CQRS' pattern (Command and Query Responsibility Segregation) 
* -> tức là nó để seperate trách nhiệm của **`Query`** và **`Command`** - hay **`separates read and update operations for a data store`**

## Reason
* -> có thể là do nhu cầu truy vấn thường nhiều hơn update dẫn đến việc khác biệt trong cách tối ưu, xử lý; hay sự khác biệt về quyền trong thao tác dữ liệu

==============================================================
# MediatR
* -> make implement the **`Mediator pattern`** easier; allow to conform **`CQRS pattern`** more easily