
# Delegate
* -> _a delegate_ is **`a type`** that represents **references to methods** with a _particular parameter list_ and _return type_
* -> a delegate is a type that **safely encapsulates a method**
* -> **a delegate object** is normally constructed by providing the **`name of the method`** the delegate will wrap, or with a **`lambda expression`**

## Using Delegate
* -> Delegates are used to **pass methods as arguments to other methods**
* => this makes delegates ideal for defining **callback methods**
 
* -> we can associate **`delegate instance`** **with any method** (_either static or an instance method from any accessible class or struct_) with _`a compatible signature and return type`_
* -> we can **`invoke (or call) the method`** through the delegate instance
* => be able to **change method calls, or plug new code into existing classes**

```cs - VD:
// Khai báo delegate:
public delegate void ShowLog(string message);

// dùng "delegate" như 1 kiểu dữ liệu 
// giá trị gán cho biến là 1 hàm có sự tương đồng với delegate
ShowLog showLog = Info;
// hoặc
var showLog = new ShowLog(Info);

static public void Info(string s)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine(string.Format("Info: {0}", s));
    Console.ResetColor();
}

// thực thi:
showLog("Thông báo"); // Info: Thông báo

// tương tự:
showLog = Warning; // gán lại giá trị cho biến bằng 1 method khác
showLog("Thông báo");   // Waring: Thông báo

// chaning delegate
showLog = null;
showLog += Warning;         
showLog += Info;      
showLog += Warning;     
showLog?.Invoke("TestLog");         
// Output: Waring: TestLog  Info: TestLog   Waring: TestLog

// combine delegate:
ShowLog showLog1 = (x)=> {Console.WriteLine($"{x}");};
ShowLog showLog2 = Warning;
ShowLog showLog3 = Info;
var all = showLog1 + showLog2 + showLog3 + showLog1;
all("Xin Chào"); // Output: Xin Chào  Waring: Xin Chào  Info: Xin Chào  Xin Chào
```

### Event handlers
* -> are nothing more than **`methods that are invoked through delegates`**
* -> we create a custom method, and a class such as a windows control can **`call your method when a certain event occurs`**

### Function pointers
* -> support similar scenarios, where we need more control over the **`calling convention`** 
* -> The code associated with a delegate is invoked using a virtual method added to a delegate type
* -> using function pointers, you can specify different conventions

### Lambda Expression
* -> Lambda expressions are a more concise way of **`writing inline code blocks`**
* -> Lambda expressions (in certain contexts) are **compiled to delegate types**

## Action & Func
* là 2 mẫu delegate định nghĩa sẵn giúp ta định nghĩa biến kiểu delegate (_mà không phải khai báo kiểu delegate 1 cách rõ ràng_) 
* -> **Func** đại diện cho **`delegate có kiểu trả về`** 
* -> **Action** đại diện cho **`delegate không có kiểu trả về`** 

```cs - Func
delegate bool DelegateName(int a, string b);
DelegateName bien1;
// tương đương với:
Func<int, string, bool> bien1;

public static void TestFunc(int x, int y)
{
    Func<int,int,int> tinhtong;         
    tinhtong = Sum;                    

    var ketqua = tinhtong(x, y);
    Console.WriteLine(ketqua);
}
static int Sum(int x, int y)
{
    return x + y;
}
```

```cs - Action
Action<string> showLog = null;
showLog += Logs.Info;           
showLog += Logs.Warning;         
showLog -= Logs.Warning; //  chỉ có thể "-" phương thức ở cuối delegate   
showLog("TestLog"); // Output: Info: TestLog
```

## Compare delegate
* -> comparing delegates of **`two different types assigned`** at **compile-time** will **`result in a compilation error`**
* -> if the **`delegate instances`** are **statically of the type System.Delegate**, then the comparison is **`allowed`**, but will **`return false`** at **run time**

```c#
delegate void Callback1();
delegate void Callback2();

static void method(Callback1 d, Callback2 e, System.Delegate f)
{
    // Compile-time error.
    //Console.WriteLine(d == e);

    // OK at compile-time. False if the run-time type of f
    // is not the same as that of d.
    Console.WriteLine(d == f);
}
```

=======================================================
# "Delegate" as "Callback"
* -> **`delegate types`** are derived from the **Delegate** class in .NET.
* -> **`delegate types`** are **sealed** — cannot be derived from 
* => Because the **`instantiated delegate`** is **`an object`**, it can be **passed as an argument** or **assigned to a property**

* => _this allows a method to accept a **`delegate as a parameter`**, and **`call the delegate at some later time`**_
* -> this is known as **an asynchronous callback**, and is a common method of **`notifying a caller`** (the delegate) when a long process has completed
* -> the code using the delegate **`does not need any knowledge of the implementation of the method`** being used; the functionality is similar to the encapsulation interfaces provide

* _another common use of callbacks is defining **a custom comparison method** and **`passing that delegate to a sort method`**_
* -> it allows the caller's code to become part of the sort algorithm

```c#
public delegate void Callback(string message); // define delegate

Callback handler = DelegateMethod; // Instantiate the delegate - the callers
MethodWithCallback(1, 2, handler); // Output: The number is: 3

public static void DelegateMethod(string message) // Create a method for a delegate
{
    Console.WriteLine(message);
}
public static void MethodWithCallback(int param1, int param2, Callback callback)
{
    callback("The number is: " + (param1 + param2).ToString());
}

// => using the delegate as an abstraction
// -> "MethodWithCallback" does not need to call the console directly, it does not have to be designed with a console in mind
// -> "MethodWithCallback" does is simply prepare a string and pass the string to another method
```

# "instance method" and "a static method" for delegate
* when a delegate is constructed to **`wrap an instance method`** 
* -> the delegate **references both the instance and the method** 
* -> a delegate has **`no knowledge of the instance type`** aside from the method it wraps, 
* => so a delegate **can refer to any type of object** as long as there is a method on that object that matches the delegate signature. 

* when a delegate is constructed **`to wrap a static method`**, it **only references the method**

=========================================================
# Multicase Delegate - chained delegate for call multiple methods on a single event
* -> a delegate can **`call more than one method when invoked`**
* -> to **`add an extra method`** to the delegate's list of methods — **the invocation list**; using the addition or addition assignment operators (**+** or **+=**)
* -> when the delegate is invoked, all methods are **`called in order`**

* _if the delegate uses **reference** parameters_, the reference is **`passed sequentially to each of the methods in turn`**, 
* -> any changes by one method are **`visible to the next method`**

* _when any of the methods **throws an exception** that is not caught within the method_
* -> that exception is passed to the caller of the delegate and **`no subsequent methods in the invocation list are called`**

* _if the delegate has **a return value** and/or **out** parameters_
* -> it returns the return value and parameters **`of the last method invoked`**

```c#
//Both types of assignment are valid.
Callback allMethodsDelegate = d1 + d2;
allMethodsDelegate += d3;

Callback d1 = obj.Method1;
Callback d2 = obj.Method2;
Callback d3 = DelegateMethod;

var obj = new MethodClass();
public class MethodClass
{
    public void Method1(string message) { }
    public void Method2(string message) { }
}

// At this point allMethodsDelegate contains three methods in its invocation list — Method1, Method2, and DelegateMethod
// The original three delegates, d1, d2, and d3, remain unchanged

allMethodsDelegate -= d1; //remove Method1
Callback oneMethodDelegate = allMethodsDelegate - d2; // copy AllMethodsDelegate while removing d2
```

## System.Delegate & System.MulticastDelegate
* -> **`delegate types`** are derived from **System.Delegate**, the methods and properties defined by that class **`can be called on the delegate`**
* -> Delegates with more than one method in their invocation list derive from **MulticastDelegate**, which is **`a subclass of System.Delegate`**

```c#
int invocationCount = d1.GetInvocationList().GetLength(0);
```

## Event Handling with "delegate"
* **`Multicast delegates`** are used extensively in **event handling**
* -> **Event source objects** **`send event notifications`** to **recipient objects** that have **`registered to receive that event`**

* **to register for an event**, 
* -> _the recipient_ **`creates a method designed to handle the event`**,
* -> **`creates a delegate for that method`** and **`passes the delegate to the event source`** 
* -> _the source_ **`calls the delegate when the event occurs`**

* => the delegate then **`calls the event handling method`** on the recipient, **`delivering the event data`** 
* -> the **`delegate type for a given event`** is **defined by the event source**