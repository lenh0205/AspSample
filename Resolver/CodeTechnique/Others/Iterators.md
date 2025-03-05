> https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/iterators
> https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/collections

# Iterators 

## Mechanism
* -> An **iterator method** uses the **`yield return`** statement to return each element one at a time 
* -> when a "yield return" statement is reached, the **`current location in code is remembered`**; so the execution is **`restarted from that location the next time the iterator function is called`**
* -> the **return type of an "iterator method" or "get accessor"** can be **`IEnumerable, IEnumerable<T>, IEnumerator, or IEnumerator<T>`**

## Usage
* -> we **`consume an iterator from client code`** by using a **`foreach`** statement (_or by using a **`LINQ query`**_)
* -> we can use a **`yield break`** statement to **end the iteration**

```cs
static void Main()
{
    // khi foreach gọi SomeNumbers() nó sẽ trả về 1 IEnumerable, và foreach sẽ lặp qua IEnumerable này
    foreach (int number in SomeNumbers())
    {
        Console.Write(number.ToString() + " ");
    }
    // Output: 3 5 8
    Console.ReadKey();
}

// this is a iterator method
public static System.Collections.IEnumerable SomeNumbers()
{
    yield return 3;
    yield return 5;
    yield return 8;
}

// -> foreach lặp qua IEnumerable giống như lặp qua 1 collection bình thường, nhưng cách thức hơi khác 1 xíu
// -> the first iteration of the foreach loop causes execution to proceed in the SomeNumbers iterator method until the first yield return statement is reached
// -> This iteration returns a value of 3, and the current location in the iterator method is retained
// -> On the next iteration of the loop, execution in the iterator method continues from where it left off, again stopping when it reaches a yield return statement
// -> This iteration returns a value of 5, and the current location in the iterator method is again retained
// -> The loop completes when the end of the iterator method is reached
```
