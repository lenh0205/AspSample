
# object type
// -> All types in C# inherit directly or indirectly from "object" type (alias for System.Object);
// => include reference types and value types; reference types and value types 
// =>  can assign values of any type to variables of type "object"
// ->  require boxing and unboxing for conversion (it makes it slow)
// -> the C# compiler checks types during compile-time
// -> has only a few operation/members (Equals, GetHashCode, ToString) and doesn’t provide much flexibility.
object test = "hello"; // Boxing no error
test = 10; // Boxing no error
test = test + 10; // Compile time error; "object" not contain "+" operation
var myInt = (int) test // Unboxing


# dynamic type
// -> functions like "object" type in most cases
// -> compiler assumes a "dynamic" element supports any operation (whether the object gets its value from a COM API, from a dynamic language such as IronPython, from the HTML Document Object Model (DOM), from reflection, or from somewhere else in the program,...)
// -> is a static type, but an object of type dynamic bypasses static type checking
// -> not require boxing and unboxing
// -> checks types during runtime (All errors on dynamic variables are discovered at runtime only)
dynamic dyn = "Hello, World!";
int len = dyn.Length; // No error at compile time, runtime  
dyn = 123;
int len2 = dyn.Length; // not an error at compile time, but runtime error 

# var keyword
// -> compiler infers the type of the variable from its initialization expression
var test = 10; // "test" integer type
test = test + 10; // No error
test = "hello"; // Compile time error as test is an integer type

# BoxingvsvsvsUnBoxing
## Boxing 
// -> process of converting a "value type" to the "type object" or to any interface type implemented by this value type. 
// -> When CLR boxes a value type, it wraps the value inside a "System.Object" instance and stores it on the managed heap. 
// -> Boxing is implicit

## Unboxing
// -> extracts the value type from the object
// -> Unboxing is explicit