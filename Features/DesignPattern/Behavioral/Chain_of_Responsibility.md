> the common point of other **behavioral design patterns** is that they rely on **converting specific behaviors** into **`standalone objects called "handlers"`** 

# Chain of Responsibility / CoR / Chain of Command
* -> lets us **`pass requests along a chain of handlers`** - upon receiving a request, each handler decides either to process the request or to pass it to the next handler in the chain

## Summary
* -> tức là thay vì 1 class với nhiều behavior ta nên tách những behavior này ra; bỏ vào class riêng biệt (gọi là **`Handler`**) với 1 method duy nhất để handling requests (nhưng mà nhiều khi cũng có method khác để setting the next handler on the chain) 
* -> it's crucial that **`all handler classes implement the same interface`** - each concrete handler should only care about the following one having the "execute" method
* -> rồi sau đó ta **link these handlers into a chain** - by **`passing a next handler to the constructor (or setter) of handler`** and has **`a field for storing a reference to the next handler`**
* -> **a handler** is **`usually self-contained and immutable`** can **`decide whether to process it, pass the request further down the chain, or effectively stop any further processing`**  

## Extend 
* -> ta có thể thêm Base Handler - an optional class where we can put the boilerplate code that's common to all handler classes; for example, implement the default handling behavior: it can pass execution to the next handler after checking for its existence
* -> 
