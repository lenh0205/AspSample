# Lambda expressions 
* -> to **create an anonymous function**

* -> **`Expression lambda`**: (input-parameters) => expression
* -> **`Statement lambda`**: (input-parameters) => { <sequence-of-statements> }

## Convert "Lambda expression" to a specific type
* we can use lambda expressions in **`any code that requires instances of delegate types or expression trees`** (_VD: Task.Run(Action), LINQ_)

```c# - use lambda expressions when writing LINQ 
int[] numbers = { 2, 3, 4, 5 };
var squaredNumbers = numbers.Select(x => x * x);
Console.WriteLine(string.Join(" ", squaredNumbers)); // 4 9 16 25
```

### Delegate
* -> **`any lambda expression`** can be converted to a **delegate type** (_same types of parameters and return value_)
* -> if a lambda expression **`doesn't return a value`**, it can be converted to one of the **Action** delegate types
* -> if a lambda expression **`return a value`**, it can be converted to one of the **Func** delegate types

```c# - convert lambda to delegate type
// the lambda expression "x => x * x"  is assigned to a variable of a delegate type "Func<int, int>"
Func<int, int> square = x => x * x;
Console.WriteLine(square(5)); // 25

// lambda expression for Action
Action line = () => Console.WriteLine();
```

### Expression Tree
* -> **`Expression lambda`** can also be converted to the **expression tree types** (_Statement lambda_)

```c# - convert lambda to the expression tree type
using System.Linq.Expressions;

Expression<Func<int, int>> e = x => x * x;
Console.WriteLine(e); // x => (x * x)
```

### Notice:
* _when we call the **`Enumerable.Select`** method in the `System.Linq.Enumerable` class_
* -> the parameter is a delegate type **System.Func<T,TResult>** 
* -> for example in _LINQ to Objects_, _LINQ to XML_ 

* _when we call the **`Queryable.Select`** method in the `System.Linq.Queryable` class_
* -> the parameter type is an expression tree type **Expression<Func<TSource,TResult>>** 
* -> for example in _LINQ to SQL_

* => in both cases, you can use the same lambda expression to specify the parameter value
* => that makes the two Select calls to **`look similar`** but in fact the **type of objects created from the lambdas is different**

=================================================
# Input parameters of a lambda expression
* in some case, the compiler **can't infer** the types of input parameters; we can **`specify the parameters types explicitly`**
```c#
Func<int, string, bool> isTooLong = (int x, string s) => s.Length > x;
```

* Lambda **discard** parameters specify **`two or more input parameters`** of a lambda expression that **`aren't used in the expression`** 
* -> may be useful when you use a lambda expression to provide **an event handler**
* -> if **`only a single input parameter`** is named **`_`**, then, within a lambda expression, _ is treated as the **name of that parameter**


