
```cs
1. Simplest:

public seal class Singleton
{
    private static Singleton _instance;

    private Singleton()
    {
        Console.WriteLine("Singleton Instance Created");
    }

    public static Singleton Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new Singleton();
            }
            return _instance;
        }
    }

    public void DoSomething()
    {
        Console.WriteLine("Testing...");
    }
}


2. Thread safe:
public seal class ThreadSafeSingleton
{
    private static ThreadSafeSingleton _instance;
    private static readonly object _lock = new object();

    private ThreadSafeSingleton()
    {
        Console.WriteLine("Thread-Safe Singleton Instance Created");
    }

    public static ThreadSafeSingleton Instance
    {
        get
        {
            lock (_lock) // Ensure that only one thread can access this block at a time
            {
                if (_instance == null)
                {
                    _instance = new ThreadSafeSingleton();
                }
            }
            return _instance;
        }
    }

    public void DoSomething()
    {
        Console.WriteLine("Thread-Safe Singleton Instance is Doing Something!");
    }
}

3. Lazy initialization
public class LazySingleton
{
    private static readonly Lazy<LazySingleton> _instance = 
        new Lazy<LazySingleton>(() => new LazySingleton());

    private LazySingleton()
    {
        Console.WriteLine("Lazy Singleton Instance Created");
    }

    public static LazySingleton Instance => _instance.Value;

    public void DoSomething()
    {
        Console.WriteLine("Lazy Singleton Instance is Doing Something!");
    }
}
```

```cs

--------------------------------------------------------------------------------------------------
1. Single Responsibility Principle (SRP) - A class should have only 1 reason to change.

Bad:
public class UserService
{
    public void AddUser(string name, string email)
    {
        // Add user to the database
        Console.WriteLine("User added to database.");

        // Send a welcome email
        Console.WriteLine("Email sent to " + email);
    }
}

Good:
public class UserService
{
    private readonly EmailService _emailService;

    public void AddUser(string name, string email)
    {
        Console.WriteLine("User added to database.");
        _emailService.SendEmail(email, "hello!");
    }
}

public class EmailService
{
    public void SendEmail(string to, string message)
    {
        Console.WriteLine($"Email sent to {to}: {message}");
    }
}

-----------------------------------------------------------------------------
2. Open/Closed Principle (OCP) - open for extension but closed for modification

Bad:
public class Provider
{
    public string Connect(string providerType, string baseUrl)
    {
        if (providerType == "XPM")
            return baseUrl + '/XPM/connect';
        if (providerType == "XERO")
            return baseUrl + '/xero/connect';

        return string.Empty;
    }
}

Good:
public interface IProvider
{
    string Connect(string baseUrl);
}

public class XPMProvider : IProvider
{
    public string Connect(string baseUrl => baseUrl + '/XPM/connect';
}

public class XEROProvider : IProvider
{
    public string Connect(string baseUrl => baseUrl + '/xero/connect';
}

public class Greatestsoft : IProvider
{
    public string Connect(string baseUrl => baseUrl + '/Greatestsoft/connect';
}

Extension:
namespace CustomExtensions
{
    //Extension methods must be defined in a static class
    public static class StringExtension
    {
        // This is the extension method.
        // The first parameter takes the "this" modifier
        // and specifies the type for which the method is defined.
        public static string TrimAndReduce(this string str)
        {
            return ConvertWhitespacesToSingleSpaces(str).Trim();
        }

        public static string ConvertWhitespacesToSingleSpaces(this string value)
        {
            return Regex.Replace(value, @"\s+", " ");
        }
    }
}

using CustomExtensions;

string text = "  I'm    wearing the   cheese.  It isn't wearing me!   "; // => "I'm wearing the cheese. It isn't wearing me!";
text = text.TrimAndReduce();
text = text.Trim();

-----------------------------------------------------------------------------
3. Liskov Substitution Principle (LSP) - Subclasses should be substitutable for their base classes without altering the correctness of the program.

Bad:
public class Rectangle
{
    public virtual int Width { get; set; }
    public virtual int Height { get; set; }
    public int Area => Width * Height;
}

public class Square : Rectangle
{
    public override int Width
    {
        set { base.Width = base.Height = value; }
    }

    public override int Height
    {
        set { base.Height = base.Width = value; }
    }
}

var square = new Square();
square.Width = 10;
square.Height = 5;

var rec = new Rectangle();
rec.Width = 10;
rec.Height = 5;


Good:
public abstract class Shape
{
    public abstract int Area { get; }
}

public class Rectangle : Shape
{
    public int Width { get; set; }
    public int Height { get; set; }
    public override int Area => Width * Height;
}

public class Square : Shape
{
    public int Side { get; set; }
    public override int Area => Side * Side;
}


-----------------------------------------------------------------------------
// 4. Interface Segregation Principle (ISP) - A class should not be forced to implement interfaces it does not use.

// Bad:
public interface IAnimal
{
    void Fly();
	void Run();
}

public class Bird : IAnimal
{
    public void Fly() { Console.WriteLine("Fly"); }
    public void Run() { throw new NotImplementedException(); }
}

public class Dog : IAnimal
{
    public void Run() { Console.WriteLine("Run"); }
    public void Fly() { throw new NotImplementedException(); }
}

// Good:
public interface IFlyable
{
    void Fly();
}

public interface IRunable
{
    void Run();
}

public interface IEatable
{
    void Eat();
}

public class Bird : IFlyable, IEatable
{
    public void Fly() { Console.WriteLine("Fly"); }
    public void Eat() { Console.WriteLine("Eat"); }
}

public class Dog : IRunable
{
    public void Run() { Console.WriteLine("Run"); }
}


// -----------------------------------------------------------------------------
// 5. Dependency Inversion Principle (DIP) - High-level modules should not depend on low-level modules. Both should depend on abstractions.

// Bad:
public class EmailService
{
    public void SendMessage(string message)
    {
        Console.WriteLine($"Sending email: {message}");
    }
}

public class UserService
{
    private EmailService _emailService = new EmailService();

    public void Notify(string message)
    {
        _emailService.SendMessage(message);
    }
}


// Good:
public interface IMessageService
{
    void SendMessage(string message);
}

public class EmailService : IMessageService
{
    public void SendMessage(string message)
    {
        Console.WriteLine($"Sending email: {message}");
    }
}

public class SmsService : IMessageService
{
    public void SendMessage(string message)
    {
        Console.WriteLine($"Sending SMS: {message}");
    }
}

public class UserService
{
    private readonly IMessageService _messageService;

    public UserService(IMessageService messageService)
    {
        _messageService = messageService;
    }

    public void Notify(string message)
    {
        _messageService.SendMessage(message);
    }
}
```