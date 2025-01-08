
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