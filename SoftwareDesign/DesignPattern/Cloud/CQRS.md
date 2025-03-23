> là 1 dạng architect pattern chứ không phải traditional design pattern 
> áp CQRS cho 1 app sử dụng 1 DB duy nhất và 1 app sử dụng 2 DB đọc và ghi thì có giống nhau không ? và sẽ handle Transaction như nào ?
> 1 logic cần cả đọc và ghi thì như nào, Command hay Query ? ta sẽ dựa vào Http Action GET là Query; còn POST, PUT, DELETE là Command

==============================================================
# 'CQRS' pattern (Command and Query Responsibility Segregation) 
* -> tức là nó để seperate trách nhiệm của **`Query`** và **`Command`** - hay **`separates read and update operations for a data store`**

## Reason
* có sự khác biệt trong việc scale của 2 việc Command và Query  
* -> khác nhau nhu cầu, khối lượng 2 việc
* -> khác biệt về security, permission

* => Optimized data schemas bằng cách tạo schema khác nhau cho việc đọc và ghi
* => Command nên được dựa trên task thay vì tập trung vào dữ liệu. ("Đặt phòng khách sạn", không thiết lập "ReservationStatus thành Reserved")
* => Command có thể đặt trong một queue cho xử lý bất đồng bộ (asynchronous) thay vì được xử lý đồng bộ (synchronous).
* => Query không bao giờ sửa đổi dữ liệu của cơ sở dữ liệu. Một query trả về một DTO mà không gói gọn trong bất kì hiểu biết của domain nào

* => có thể sử dụng 2 loại Database khác nhau, Relation DB cho việc ghi và Document NoSql cho việc đọc; hoặc DB đọc có thể là 1 read-only replication của DB ghi 
* => nhưng sự đồng bộ dữ liệu luôn cần thiết (VD: tạo transaction cho việc ghi data và bắn event để cập nhật DB đọc) 
* => cho phép mở rộng 2 DB theo cách khác nhau để chịu tải khác nhau, vì thường DB đọc có tải cao hơn DB ghi rất nhiều 
* => ngăn các lệnh cập nhật gây xung đột trên cùng một domain


## Pros and Cons
* -> Các models được tách biệt đọc và ghi giúp quá trình thiết kế và triển khai trở nên đơn giản hơn. 

* ->Tuy nhiên, một nhược điểm của CQRS là code không thể tự động sản sinh ra từ schema của cơ sở dữ liệu sử dụng cơ chế scaffolding giống như công cụ ORM.
* -> có thể không đảm bảo dữ liệu consistence khi lấy ra
* -> độ phức tạp cao và càng phức tạp nếu có cả event sourcing

## Command

==============================================================
# MediatR
* -> make implement the **`Mediator pattern`** easier; allow to conform **`CQRS pattern`** more easily