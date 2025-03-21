https://refactoring.guru/design-patterns/abstract-factory

===============================================================
# Abstract Factory
* -> is a creational design pattern that lets you **produce families of `related objects` without specifying their concrete classes**

## Example

## Summary
* -> **Abstract Factory inteface** thực chất là 1 abstraction để **`decoupling logic creation of related objects`** khỏi các logic khác 
* -> nó là tập hợp các **`Factory Method`**, mỗi cái sẽ return 1 **`abstract product`** riêng; nhớ là **concrete factory's methods** cũng return **abstract product** nhưng bên trong sẽ initiate **concrete product**
* -> each distinct product of a product family should have a base interface; all variants of the product must implement this interface (_để client code sử dụng behavior của 1 product 1 cách consitent_)

* -> thế nào là **related products** ? thì 1 business logic hoàn chỉnh sẽ cần sử dụng nhiều object khác nhau; thế nên tập hợp các object này sẽ là "related" - hay gọi **`product family`**
* -> thì những product nào hay thường được collaborate với nhau thì ta nên để nó trong 1 variant of product family; 
* -> products của variant này sẽ incompatible với products của variant kia; còn factory sẽ đảm bảo những products trong 1 variant sẽ compatible với nhau

* -> each **`Concrete Factory`** phải tạo ra 1 **variant** riêng cho **product family** 
* -> sẽ có nhiều **`variant`** (biến thể) của **product family**, và trong từng variant **`each abstract product must be implemented in all`** 

* => client code sẽ communicate với objects nhưng không coupled to the specific variant thông qua abtract interface - Abstract Factory; việc sửa đổi hoặc thêm mới 1 product sẽ không breaking existing client code
* => và nó đảm bảo khi client code sử dụng 1 concrete factory thì tất cả các objects/products/elements bên trong nó là consitent với nhau (tránh việc ta tự implement sai loại product)
* => tất cả những gì ta cần modify là app's initialization code để chọn concrete factory phù hợp

## Use case
* -> Use the Abstract Factory when your code needs to work with various families of related products, but you don’t want it to depend on the concrete classes of those products
* -> Consider implementing the Abstract Factory when you have a class with a set of Factory Methods that blur its primary responsibility

## Consideration
* -> ta thường biết sẽ tạo concrete type nào vào thời điểm start app dựa vào configuration hoặc enviroment settings, vậy nên ta sẽ chỉ sử dụng 1 concrete type cho 1 Abstract Factory trong toàn Application  
* -> vậy trong trường hợp ta chỉ biết ta cần cái concrete type nào hoặc nhiều concrete type khác loại vào thời điểm runtime dựa vào 1 dynamic parameter thì sao ?

===============================================================
# Arise
* -> in **`Dependency Injection`**, we typically **put our dependencies in the constructor**; when class is created, the dependencies get created and injected
* -> however, there're times we want to **create our dependencies more often or in a different way**
* => one solution is **`Factory pattern`** - create instances properly from dependency injection

```cs - we can create "factories" for various needs in project
// an abstract factory for initialize class instances with startup data
// a factory for instantiating different implementation classes based upon passing parameters
```

## Simple Factory

```cs
// Ví dụ: ta có trang UI có 1 button để lấy thời gian hiện tại hiển thị ra màn hình
// ta có 1 class "Sample1" chứa property thể hiện thời gian hiện tại; nếu ta inject class này vào 1 view
// thì mỗi lần bấm button nó sẽ không cập nhật thời gian mới mà chỉ lấy đúng thời gian đã được inject lúc load trang lần đâu
// giờ chỉ còn cách là trong callback của button event, ta khởi tạo class "Sample1" thủ công và mỗi lần click button là mỗi lần access property để lấy thời gian mới nhất
// điều này là 1 vấn đề nhất là khi class "Sample1" cũng đang có những Dependencies mà bình thường nó sẽ lấy từ DI
// => vậy nên ta sẽ cần 1 factory

// program.cs
builder.Services.AddTransient<ISample1, Sample1>();

// a simple factory
builder.Services.AddSingleton<Func<ISample1>>(x => () => x.GetService<ISample1>()!);

// Index.razor
@page "/"
@inject Func<ISample1> factory

<h2>@currentTime?.CurrentDateTime</h2>
<button class="btn btn-primary" @onclick="GetNewTime">Get New Time</button>

@code {
    ISample1? currentTime;
    private void GetNewTime()
    {
        // giờ mỗi lần bấm nút nó sẽ hiển thị cho ta thời gian mới nhất ra màn hình
        currentTime = factory();
    }
}

// model
public interface ISample1
{
    string CurrentDateTime { get; set; }
}
public class Sample1 : ISample1
{
    public string CurrentDateTime { get; set; } = DateTime.Now.ToString();
}
```

## Abstract Factory
* -> làm việc inject factory 1 cách rõ ràng hơn
* -> lý do mà ta không inject trực tiếp **`IServiceCollection`** và dùng nó luôn, mà viết trong 1 factory để ta có thể làm rõ factory này có những dependencies nào cụ thể

```cs
// Register
builder.Services.AddAbstractFactory<ISample1, Sample1>();

// Define
public static class AbstractFactoryExtension
{
    public static void AddAbstractFactory<TInterface, TImplementation>(this IServiceCollection services)
    where TInterface : class
    where TImplementation : class, TInterface
    {
        services.AddTransient<TInterface, TImplementation>();
        services.AddSingleton<Func<TInterface>>(x => () => x.GetService<TInterface>()!);
        services.AddSingleton<IAbstractFactory<TInterface>, AbstractFactory<TInterface>>();
    }
}

public class AbstractFactory<T> : IAbstractFactory<T>
{
    private readonly Func<T> _factory;

    public AbstractFactory(Func<T> factory)
    {
        _factory = factory;
    }

    public T Create()
    {
        return _factory();
    }
}

public interface IAbstractFactory<T>
{
    T Create();
}

// Usage
@inject IAbstractFactory<ISample1> factory
<h2>@currentTime?.CurrentDateTime</h2>
<button class="btn btn-primary" @onclick="GetNewTime">Get New Time</button>

@code {
    ISample1? currentTime;
    private void GetNewTime()
    {
        currentTime = factory.Create();
    }
}
```

## Populate data when initiate instance
* -> when using DI, constructor was passed in the dependencies only
* -> to pass data to constructor at startup, we will create a factory that is custom built and **`only used for one particular type`**


```cs
// Register
builder.Services.AddGenericClassWithDataFactory();

// Define 
public static class GenerateClassWithDataFactoryExtension
{
    public static void AddGenericClassWithDataFactory(this IServiceCollection services)
    {
        services.AddTransient<IUserData, UserData>();
        services.AddSingleton<Func<IUserData>>(x => () => x.GetService<IUserData>()!);
        services.AddSingleton<IUserDataFactory, UserDataFactory>();
    }
}

public interface IUserDataFactory
{
    IUserData Create(string name);
}

public class UserDataFactory : IUserDataFactory
{
    private readonly Func<IUserData> _factory;

    public UserDataFactory(Func<IUserData> factory)
    {
        _factory = factory;
    }

    public IUserData Create(string name)
    {
        var output = _factory();
        output.Name = name;
        return output;
    }
}

// Usage
<h1>Hello @user?.Name</h1>

@code {
    IUserData? user;
    protected override void OnInitialized()
    {
        user = userDataFactory.Create("Sue Storm");
    }
}

// model
public interface IUserData
{
    string? Name { get; set; }
}

public class UserData : IUserData
{
    public string? Name { get; set; }
}
```

## Interface with multiple Implementation

```cs
// Usage
builder.Services.AddVehicleFactory();

// Define
public static class DifferentImplementationsFactoryExtension
{
    public static void AddVehicleFactory(this IServiceCollection services)
    {
        services.AddTransient<IVehicle, Car>();
        services.AddTransient<IVehicle, Truck>();
        services.AddTransient<IVehicle, Van>();

        services.AddSingleton<Func<IEnumerable<IVehicle>>>(x => () => x.GetService<IEnumerable<IVehicle>>()!);
        services.AddSingleton<IVehicleFactory, VehicleFactory>();
    }
}

public interface IVehicleFactory
{
    IVehicle Create(string name);
}

public class VehicleFactory : IVehicleFactory
{
    private readonly Func<IEnumerable<IVehicle>> _factory;

    public VehicleFactory(Func<IEnumerable<IVehicle>> factory)
    {
        _factory = factory;
    }

    public IVehicle Create(string name)
    {
        var set = _factory();
        // Lưu ý cách này có thể là vấn đề về performance nếu có hàng ngàn implementation của IVehicle
        IVehicle output = set.Where(x => x.VehicleType == name).First();
        return output;
    }
}

// Usage
@inject IVehicleFactory vehicleFactory

<h1>Hello @user?.Name (who drives a @vehicle?.VehicleType)</h1>

@code {
    IVehicle? vehicle;

    protected override void OnInitialized()
    {
        vehicle = vehicleFactory.Create("Truck");
    }
}

// model
public interface IVehicle
{
    string VehicleType { get; set; }

    string Start();
}

public class Car : IVehicle
{
    public string VehicleType { get; set; } = "Car";
    public string Start()
    {
        return "The car has been started.";
    }
}

public class Truck : IVehicle
{
    public string VehicleType { get; set; } = "Truck";
    public string Start()
    {
        return "The truck has been started.";
    }
}

public class Van : IVehicle
{
    public string VehicleType { get; set; } = "Van";
    public string Start()
    {
        return "The van has been started.";
    }
}
```

## Other
* -> không phải lúc nào ta cũng s/d factory; sẽ có những trường hợp cần thiết để ta xài
* -> việc ta calling API trong C# app, thực chất là sử dụng **HttpClientFactory** - nó tạo các HttpClient, handle life span, reuse, closing them


===============================================================
> https://stackoverflow.com/questions/13029261/design-patterns-factory-vs-factory-method-vs-abstract-factory

# Factory Design Pattern
* -> the pattern **`encapsulates object generation`** - separates clients from the creation process, allowing dynamic object instantiation based on runtime requirements
* -> enables us to **generate objects without defining the specific type of object** that will be created 
* => by using a **factory method** or **factory class**, it allows us to **`abstract away the instantiation process`** - resulting in a more modular and scalable solution

# Factory Method Design Pattern
* -> a _design pattern_ that **`provides an interface for creating objects`** while **`allowing subclasses to determine the type of objects to be made`**
* -> **bypassing the instantiation process to subclasses** encourages **`loose coupling`**

```r - Ex:
// consider "a logistics company" that uses a variety of "transportation methods" - such as trucks, ships, and planes
// the company may utilize a factory method to "generate the proper transportation object" depending on the "individual requirements"
// => allowing for "simple extension or modification" of transportation kinds without affecting the core logistics code
```

===============================================================
# Problems of not using the 'Factory Design Pattern'

## Tight coupling: 
* -> this occurs when our code is tightly bound to specific classes, making it **`impossible to change or extend the types of objects created without modifying`** the current code
* -> this can result in decreased flexibility and more maintenance effort

## Code Duplication:
* -> without a factory, **`object creation code is frequently duplicated across the program`**
* -> this redundancy can cause inconsistencies and problems in the code, making it more challenging to manage

## Difficulty in Testing: 
* -> Testing components based on specific implementations might be tricky
* -> using a factory allows us to **`simply change out implementations for testing`**, making our code more testable and reusable

## Scalability Issues:
* -> as our application expands, managing object creation directly in client code can become inefficient
* -> The Factory Design Pattern **`centralizes object generation`**, making it more scalable and maintainable

```cs
using System;

public abstract class Document
{
    public abstract void Open();
}

public class WordDocument : Document
{
    public override void Open()
    {
        Console.WriteLine("Opening a Word document.");
    }
}

public class PDFDocument : Document
{
    public override void Open()
    {
        Console.WriteLine("Opening a PDF document.");
    }
}

public class DocumentManager
{
    public void OpenDocument(string type)
    {
        Document doc;

        // the DocumentManager class uses conditional logic to create proper Document
        // -> this ties DocumentManager to specific document types 
        // -> and necessitates modification of the OpenDocument function whenever new document types are added
        if (type == "Word")
        {
            doc = new WordDocument();
        }
        else if (type == "PDF")
        {
            doc = new PDFDocument();
        }
        else
        {
            throw new ArgumentException("Invalid document type");
        }

        doc.Open();
    }
}

class Program
{
    static void Main()
    {
        DocumentManager manager = new DocumentManager();

        manager.OpenDocument("Word"); // Output: Opening a Word document.
        manager.OpenDocument("PDF");  // Output: Opening a PDF document.
    }
}
```

# Implementing the Factory Design Pattern

```cs - Example 1:
// Abstract Product
public abstract class Document
{
    public abstract void Open();
}

// Concrete Product 1
public class WordDocument : Document
{
    public override void Open()
    {
        Console.WriteLine("Opening a Word document.");
    }
}

// Concrete Product 2
public class PDFDocument : Document
{
    public override void Open()
    {
        Console.WriteLine("Opening a PDF document.");
    }
}

public class DocumentFactory
{
    public abstract Document CreateDocument(string type);
}

public class ConcreteDocumentFactory : DocumentFactory
{
    // centralizes the generation of Document objects
    public override Document CreateDocument(string type)
    {
        switch (type)
        {
            case "Word":
                return new WordDocument();
            case "PDF":
                return new PDFDocument();
            default:
                throw new ArgumentException("Invalid document type");
        }
    }
}

// Client Code
class Program
{
    static void Main()
    {
        try
        {
            // using Factory allowing the client code to request documents without knowing which classes are involved
            // simplifies object generation
            // increases flexibility by eliminating reliance on specific implementations
            // makes document types easier to manage and extend

            DocumentFactory documentFactory = new ConcreteDocumentFactory();

            // Use the factory to create a WordDocument
            Document wordDoc = documentFactory.CreateDocument("Word");
            wordDoc.Open(); // Output: Opening a Word document.

            // Use the factory to create a PDFDocument
            Document pdfDoc = documentFactory.CreateDocument("PDF");
            pdfDoc.Open(); // Output: Opening a PDF document.

            // Attempting to create an invalid document type
            Document invalidDoc = documentFactory.CreateDocument("Excel");
            invalidDoc.Open(); // This line will throw an exception
        }
        catch (ArgumentException ex)
        {
            Console.WriteLine(ex.Message); // Output: Invalid document type
        }
    }
}
```
