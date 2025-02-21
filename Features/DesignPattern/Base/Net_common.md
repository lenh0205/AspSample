https://medium.com/@serasiyasavan14/5-powerful-design-patterns-every-c-developer-should-master-b0a214617375
https://www.csharp.com/article/the-importance-of-design-patterns-in-net-core-development/
https://www.csharp.com/UploadFile/bd5be5/design-patterns-in-net/

======================================================================
# The Singleton Pattern
* -> ensure that **`only one instance of a class is created`** and that this **`instance can be accessed globally`**
* -> a common use case for this pattern is when we need to **`control access to a shared resource`** (_such as logging, configuration settings or a database connection_)
* => avoid inconsistencies, resource wastage, or slowdowns

## Implementation
* -> create **`a private static variable`** that **holds the instance**
* -> the class has **`a private constructor`** to prevent external instantiation
* -> **`a public method`** **returns the instance**

```cs - Example
public class Singleton
{
    private static Singleton _instance;
    private Singleton() { }
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
}
```

## Use Case
* _Global logging systems, configuration managers, and thread pools_

```cs - Example:

```

# The Factory Pattern
* -> is used to create objects without exposing the instantiation logic to the client
* -> this allows for greater flexibility and maintainability, as the client can be decoupled from the specific class being instantiated. Here’s an example of a factory class in C#:

```cs
public abstract class Creator
{
    public abstract Product FactoryMethod();
}

public class ConcreteCreator : Creator
{
    public override Product FactoryMethod()
    {
        return new ConcreteProduct();
    }
}
```

# The Observer Pattern

The Observer Pattern The observer pattern is used to create a one-to-many relationship between objects, where one object (the subject) maintains a list of its dependents (the observers) and notifies them of any changes. This pattern is commonly used in event-driven systems, such as GUI applications. Here’s an example of an observer class in C#:

```cs
public class Subject
{
    private List<Observer> _observers = new List<Observer>();
    public void Attach(Observer observer) { _observers.Add(observer); }
    public void Detach(Observer observer) { _observers.Remove(observer); }
    public void Notify()
    {
        foreach (Observer observer in _observers)
        {
            observer.Update();
        }
    }
}
```

# The Decorator Pattern

The decorator pattern is used to add new functionality to an existing object without changing its structure. This pattern is useful when you need to add functionality to a class, but don’t want to create a new class for each combination of functionality. Here’s an example of a decorator class in C#:

```cs
public abstract class Component
{
    public abstract void Operation();
}

public class ConcreteComponent : Component
{
    public override void Operation()
    {
        // Original functionality
    }
}

public abstract class Decorator : Component
{
    protected Component _component;

    public void SetComponent(Component component)
    {
        _component = component;
    }

    public override void Operation()
    {
        if (_component != null)
        {
            _component.Operation();
        }
    }
}

public class ConcreteDecoratorA : Decorator
{
    public override void Operation()
    {
        base.Operation();
        // Additional functionality
    }
}
```

# The Template Method Pattern
The template method pattern is used to define the skeleton of an algorithm, and allow subclasses to fill in the details. This pattern is useful when you have a common algorithm that can be reused, but with different implementation details. Here’s an example of a template method class in C#:

```cs
public abstract class AbstractClass
{
    public void TemplateMethod()
    {
        Method1();
        Method2();
    }

    public abstract void Method1();

    public virtual void Method2()
    {
        // Default implementation
    }
}

public class ConcreteClass : AbstractClass
{
    public override void Method1()
    {
        // Implementation details
    }
}
```

## The Command Pattern
The Command pattern is used to encapsulate a request as an object, which allows for greater flexibility and separation of concerns. This pattern is useful when you need to handle different requests in a similar way or need to support undo/redo functionality. Here’s an example of a Command class in C#:

```cs
public interface ICommand
{
    void Execute();
    void Undo();
}

public class ConcreteCommand : ICommand
{
    private Receiver _receiver;
    private string _state;

    public ConcreteCommand(Receiver receiver)
    {
        _receiver = receiver;
    }

    public void Execute()
    {
        _state = _receiver.Action();
    }

    public void Undo()
    {
        _receiver.Undo(_state);
    }
}
```

## The Adapter Pattern
The Adapter pattern is used to adapt an interface of a class to another interface that the client expects. This pattern is useful when you need to integrate with existing systems or frameworks that have a different interface than what you are expecting. Here’s an example of an Adapter class in C#:

```cs
public interface ITarget
{
    void Request();
}

public class Adaptee
{
    public void SpecificRequest()
    {
        // Original functionality
    }
}

public class Adapter : ITarget
{
    private Adaptee _adaptee;

    public Adapter(Adaptee adaptee)
    {
        _adaptee = adaptee;
    }

    public void Request()
    {
        _adaptee.SpecificRequest();
    }
}
```

======================================================================
# Example
https://stackoverflow.com/questions/3252499/what-design-patterns-are-used-throughout-the-net-framework
https://learn.microsoft.com/en-us/archive/msdn-magazine/2005/july/discovering-the-design-patterns-you-re-already-using-in-net
