
# Convert monolith to microservices
* -> bring out the most easily separable component from the Monolith first
* _find the most isolated component and easily brought out; Ex: Authen and Author, User Management_

* -> is we using SQL Server or Oracle, our first option might be to make a service responsible for table set instead of an entire database
* _**isolating table** meaning all the tables are in same database but for a set of tables a single microservice is responsible for that_
* _it's really hard to ask for 3 more SQL Server because the cost associated_
* _if we use something cheaper like Postgres, there're a lot of work associated with it_

* -> when segregated all data into different database, getting a consolidated view of the data or report is hard
* _**API Gateway** support data aggregation; or we could have an aggregated service to get data from multiple services and aggregate them_
* _other option is creat a **`Data Stream`** for all the data and a reports microservice reading data from Data Stream, aggregating the data for data visualization and putting that into a **read-only database**_

* -> if we are using cloud, use cloud native managed databases like **DynamoDB** in AWS or **CosmosDB** in Azure 
* _easy to startup, easy to put data in_
* _however, these not work for relationship database_

* -> think about the resilience during design
* _Ví dụ ta make a call from "Inventory" service to "Product" service, nếu "Product" service fail thì "Inventory" service sẽ phản ứng thế nào_

* -> **`Logging and Monitoring`** is probably the **most critical aspect of building microservices**, cannot be an afterthought 
* _Cloud based centralised logging is extremely critical; also passing a correlation log id accross microservices would save us lot of debug time in future_

# Example:
```bash - Example:
// ta có 1 Monolith Ecommerce App
// bao gồm các bảng cơ bản: Inventory, Product, User, Order

// in Testing cost aspect
// nếu ta cần change aspect nào đó của app thì như là Product, Inventory thì ta cần regression test để test toàn bộ app 
// vì tụi nó là all part of the same deployable unit - một thằng change sẽ có thể ảnh hưởng những thằng còn lại
// ngoài ra, cần functional test riêng cho feature thay đổi 
// => makes the cost of testing higher

// in Scaling aspect
// ví dụ việc search 1 product trong Inventory cụ thể sẽ thường xuyên hơn là việc show các Orders
// vậy nên việc Scaling for product sẽ là khác hoàn toàn với Scaling for Orders
```

## First: separate "Order" into single reposibility microservice
```bash
// => vậy nên trước tiên ta sẽ tách khối Order ra khỏi app vì Orders is pretty isolated
// -> easily move Order, OrderDetail and user can still remain in app because UserId can be passed along to the service to figure out what the order is 

// => nhưng ta sẽ không tách database bây giờ
```

```bash
// -> đầu tiên ta sẽ dựng 1 project ASP.NET Core Web Application + Enable Docker Support
// -> sau đó cấu hình IHttpClientFactory để tầng Business của main App gọi đến "Order" service WebAPI thay vì truy cập trực tiếp Database thông qua tầng DAL như hiện tại

// -> hiện tại DAL của Order service đang cần join với 1 bảng từ monolithic để lấy data cần thiết
// -> what we need now is refactoring database so that it dont have any database dependency with the monolithic service; ta có 2 options có thể sử dụng kết hợp
// -> Option 1: ta có thể thêm 1 số columns cho các tables của Order
// -> data redundancy its not a concern when we come to microservice because microservice support to contain their data in their own context
// -> Option 2: nhưng nếu VD ta cần nhiều trường hơn từ Product service thì ta nên gọi Product service để get information for the product
// => now we can break the tables belong to Order and bring them into their own Database
// => the Database can be a cloud native database so that cost is much more smaller 

// -> đối với thông tin về User, in real life scenario these information should anyway be part of some sort of Single Sign-on
```