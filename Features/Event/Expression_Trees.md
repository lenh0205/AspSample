
# Expression Trees
* -> **represent code** in a **tree-like** **`data structure`**, where **each node** is **`an expression`** 
* -> for example, a method call or a binary operation such as x < y

## Common use case: Expression Trees with LINQ

* _in LINQ_
* -> we write **`function arguments`** (_typically using Lambda Expressions_) when we create `LINQ queries`
* -> in a typical LINQ query, those `function arguments` are **transformed into a delegate** the compiler creates (**Func type**) 

* _Entity Framework's LINQ APIs_ 
* -> accept **Expression trees as the arguments** for the **`LINQ Query Expression Pattern`** 
* -> that enables `Entity Framework` to **`translate the query`** we wrote in **C# into SQL** that executes in the database engine

## Purpose
* when you want to have **a richer interaction**, you need to use Expression Trees
* -> _Expression Trees_ represent **code as a structure** that we **`examine, modify, or execute`**
* -> these tools give us the power to **manipulate code during run time** (_we write code that **`examines running algorithms`**, or **`injects new capabilities`**_)
* -> _in more advanced scenarios, we modify running algorithms and even translate C# expressions into another form for execution in another environment_

* _we compile and run code represented by expression trees_
* -> building and running expression trees enables **`dynamic modification of executable code`**, the **`execution of LINQ queries in various databases`**, and the **`creation of dynamic queries`**

* **`Expression trees`** are also used in the **DLR - dynamic language runtime** 
* -> to provide **`interoperability`** between **`dynamic languages and .NET`**
* -> and to enable **compiler writers** to **`emit expression trees instead of MSIL - Microsoft intermediate language`**

## Create Expression Trees
* -> we can have the **C# compiler** **`create an expression tree for us`** based on an anonymous lambda expression, 
* -> or we can create expression trees **manually** by **`using the System.Linq.Expressions namespace`**

* when **a lambda expression** is assigned to a variable of type **`Expression<TDelegate>`**, 
* -> the **compiler emits code** to **`build an expression tree`** that **`represents the lambda expression**
* -> the **`C# compiler generates expression trees`** only from **expression lambdas**, it can't parse **statement lambdas**

```c# - to have the C# compiler create an expression tree that represents the lambda expression 
// lambda expression: num => num < 5
Expression<Func<int, bool>> lambda = num => num < 5;
```

### Step
* **https://learn.microsoft.com/en-us/dotnet/csharp/advanced-topics/expression-trees/expression-trees-building**
* -> we create expression trees in our code
* -> we build the tree by creating each node and attaching the nodes into a tree structure

* **Expression trees are immutable**
* -> if we want to modify an expression tree, we must construct a new expression tree by **`copying the existing one and replacing nodes in it`** 

* we use an **expression tree visitor** to traverse the existing expression tree

* => once you build an expression tree, you execute the code represented by the expression tree.