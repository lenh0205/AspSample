# The Singleton Pattern
The Singleton Pattern The singleton pattern is used to ensure that only one instance of a class is created, and that this instance can be accessed globally. A common use case for this pattern is when you need to control access to a shared resource, such as a database connection. Here’s an example of a singleton class in C#:

```cs
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

# The Factory Pattern

The Factory Pattern The factory pattern is used to create objects without exposing the instantiation logic to the client. This allows for greater flexibility and maintainability, as the client can be decoupled from the specific class being instantiated. Here’s an example of a factory class in C#:

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

# 5. The Template Method Pattern
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
