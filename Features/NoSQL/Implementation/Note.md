
## Basic Implement
* -> https://www.mongodb.com/developer/languages/csharp/create-restful-api-dotnet-core-mongodb/

## Các Attribute để dùng cho model của MongoDB
* -> https://mongodb.github.io/mongo-csharp-driver/2.14/apidocs/html/T_MongoDB_Bson_Serialization_Attributes_BsonIdAttribute.htm
* -> https://mongodb.github.io/mongo-csharp-driver/2.14/apidocs/html/T_MongoDB_Bson_Serialization_Attributes_BsonRepresentationAttribute.htm
* -> https://mongodb.github.io/mongo-csharp-driver/2.14/apidocs/html/T_MongoDB_Bson_Serialization_Attributes_BsonElementAttribute.htm
* -> https://mongodb.github.io/mongo-csharp-driver/2.14/apidocs/html/T_MongoDB_Bson_ObjectId.htm

## MongoClient
* -> https://mongodb.github.io/mongo-csharp-driver/2.14/reference/driver/connecting/#re-use
* -> theo microsoft, MongoClient should be registered in DI with a singleton service lifetime (https://learn.microsoft.com/en-us/aspnet/core/tutorials/first-mongo-app?view=aspnetcore-8.0&tabs=visual-studio)

```cs
services.AddSingleton<IMongoClient>(s => 
    new MongoClient(Configuration.GetConnectionString("MongoDb"))
);
```

## basic operation of 'MongoCollection' (MongoDatabase)
* -> DeleteOneAsync: Deletes a single document matching the provided search criteria.
* -> Find<TDocument>: Returns all documents in the collection matching the provided search criteria.
* -> InsertOneAsync: Inserts the provided object as a new document in the collection.
* -> ReplaceOneAsync: Replaces the single document matching the provided search criteria with the provided object.

# Cannot use "UnitOfWork" with MongoDb
* -> MongoDB does not have transaction control in the .NET driver.
* -> Unlike SQL Server, you can't enlist multiple queries in a transaction that can be rolled back if one fails.
* -> because Mongo already has that with the **IClientSessionHandle**

# UnitOfWork
* -> Unit Of Work will be responsible for performing the transactions that the Repositories have made. For this work to be done, a Mongo Context must be created
* -> this Context will be the connection between the Repository and UoW.
* -> https://github.com/brunobritodev/MongoDB-RepositoryUoWPatterns
* -> https://blog.jmorbegoso.com/post/unit-of-work-pattern-in-csharp-using-mongodb/

## Draft
* -> khi ta call .InsertOneAsync() trên mongo collection, nó sẽ ghi vào database ngay lập tức
* -> vì vậy ta thay vì Repository sẽ thực thi trực tiếp, nó sẽ làm nhiệm vụ add Func mà thực thi .InsertOneAsync() vào 1 List<Func>
* -> và khi UnitOfWork SaveChange ta sẽ thực thi toàn bộ List callback này trong MongoClient.StartSessionAsync().StartTransaction();

* -> đầu tiên là ta sẽ xây dựng 1 lớp MongoDbContext, 
* -> bao gồm các state là MongoClient, Database, Session, List<Func<Task>>
* -> nó sẽ cần có logic khởi tạo MongoClient() rồi truy cập đến database
* -> tạo 1 Method để cung cấp cho Repository các mongo Collection
* -> tạo 1 Method để add các Func của Repository chứa các database operation 
* -> tạo Method SaveChange để tạo Session đồng thời tạo Transaction để thực thi các Func

* -> ta chỉ cần inject cái DbContext này vào UnitOfWork để nó có thể s/d hàm SaveChange; và inject DbContext vào Repository để truy cập vào Collection tương ứng

# Transaction
https://www.mongodb.com/developer/languages/csharp/transactions-csharp-dotnet/
https://www.mongodb.com/docs/drivers/csharp/current/fundamentals/transactions/

