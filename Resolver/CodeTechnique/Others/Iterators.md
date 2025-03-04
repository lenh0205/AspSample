> https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/iterators
> https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/collections

# Iterators 

## Mechanism
* -> An iterator method uses the yield return statement to return each element one at a time. 
* -> when a "yield return" statement is reached, the **`current location in code is remembered`**; so the execution is **`restarted from that location the next time the iterator function is called`**
