
# Why RabbitMQ
* -> **`Asynchronous`** - với những hành động không cần thực hiện và phản hổi kết quả ngay thời điểm đó (TCP request-response) thì ta cứ cho nó vô queue cho các Consumer tự lấy, 1 service không cần phải chờ thực hiện xong tất cả hành động để mà cứ phản hổi đã làm thành công về cho user luôn cho nhanh
* -> **`Decoupling`** - thay vì 1 server phải quản lý việc giao tiếp với nhiều server khác, thì nó chỉ cần giao tiếp với RabbitMQ rồi RabbitMQ sẽ quản lý việc giao tiếp với những thằng còn lại
* -> **`Rate Limit`** - (Ví dụ: chương trình sale một sản phẩm bắt đầu, số lượng request trên giây tăng đột biến lên 5000; nhưng MySQL chỉ có thể xử lý 2000 request per second Database thì nó sẽ sập ngay); Lúc này ta cứ ném hết 5000 request cho RabbitMQ nó sẽ chỉ gửi 2000 request per second; còn 3000 request còn lại cứ từ từ xử lý sau
* -> **`Scalibility`** - nếu có hàng tá tin nhắn mà 1 Consumer không thể xử lý hết được làm queue bị đầy, ta có thể nhân đôi Consume lên lấy message

```cs
// Ví dụ user gửi request cho 1 WebAPI, WebAPI này phải gửi 5 HTTP request đến 5 services khác để hoàn thành business logic
// thì nó sẽ cần chờ tất cả 5 Service khác response thì WebAPI này mới có thể response cho user
// và trường hợp này sẽ tệ hơn nếu 1 Service nó gọi tới bị chết, nó sẽ phải request over and over again và chờ cho Service này ok thì mới phản hồi cho user được
// hoặc trường hợp WebAPI gửi một lượng request đến nỗi Service kia không chịu nổi, nó sẽ làm sập Service
```

## Một số vấn đề trong RabbitMQ 
* -> nhưng RabbitMQ không đảm bảo 100% là không bị miss message; một vấn đề nữa là tuy RabbitMQ đã gửi message thành công và báo về những việc xử lý Data Persitance có thành công không thì chưa chắc

# RabbitMQ, Kafka, ActiveMQ, RocketMQ
* -> thông lượng của Kafka cao hơn RabbitMQ
* -> Kafka ít mất dữ liệu hơn
* -> độ tin cậy của tin nhắn thì RabbitMQ là toàn vẹn; còn Kafka cao nhưng vẫn có thể bị mất
* -> Lantency của RabbitMQ rất thấp dưới mức milisecond; còn Kafka độ trễ trong vòng milisecond 
