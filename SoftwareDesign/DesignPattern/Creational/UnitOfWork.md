
=======================================================================
## Unit of Work
* -> a unit of work class serves one purpose **`ensure that multiple repositories share a single database context`**
* -> when we used multiple repositories and each one **uses a separate database context instance**; then when we update 2 different entity types as part of the same transaction, **`one might succeed and the other might fail`**
* -> that way, **when a unit of work is complete** we can call the **SaveChanges** method on **that instance of the context** and be **`assured that all related changes will be coordinated`**

* => this is really important when dealing with **`transaction`**

```r - Ex:
// creating a repository class for each entity type could result in "partial updates"
// suppose we have to update two different entity types as "part of the same transaction"
// -> if each uses a separate database context instance, one might succeed and the other might fail
// -> "unit of work" ensure that "all repositories" use the "same database context"
// => and thus coordinate all updates
```

## Implementation
* -> the **`unit of work class`** **coordinates the work of multiple repositories** by **`creating a single database context class shared by all of them`**
* -> all that the _UnitOfWork class_ needs is a **`Save method`** and **`a property for each repository`**
* -> _each repository property_ **returns a repository instance** that has been **`instantiated using the same database context instance`** as the other repository instances
* => the _Unit of Work object_ will responsible for initiating database operations, tracking changes, and committing or rolling back the transaction

=======================================================================
# Repository and Unit of Work Design Patterns
* -> to **`create an abstraction layer`** between the **Data Access layer** and the **Business Logic layer** of an application
* => help **`insulate`** our **application** from **changes in the data store** and can facilitate **`automated unit testing`** or **`test-driven development (TDD)`**
