# Checking "null"
* **`!= null`** 
* **`is not null`** (_`is null` hoặc `is not null` không depend on custom implementation of `!= and == operators`_)

* **define custom == and != operators**: khi default behavior of checking for reference equality không đủ tốt 
```
public class MyClass
{
    public int Value { get; set; }

    // Custom "==" operation of reference
    public static bool operator ==(MyClass a, MyClass b)
    {
        return a.Value == b.Value;
    }

    // Custom "!=" operation of reference
    public static bool operator !=(MyClass a, MyClass b)
    {
        // Custom inequality check
        return !(a == b);
    }
}

MyClass a = new MyClass { Value = 1 };
MyClass b = new MyClass { Value = 2 };
MyClass c = new MyClass { Value = 1 };

Console.WriteLine(a == b); // False
Console.WriteLine(a != b); // True
Console.WriteLine(a == c); // True
```

# Checking "string"
* **`string.IsNullOrEmpty(<variable>)`** -> true if a string is null or empty
* **`string.IsNullOrWhiteSpace(<variable>)`** -> true if the string is null, empty, or contains only white-space characters

# Checking "collection"
* **`.Any()`**

# Checking type of object:
* **myObject `is` MyClass**
* **myObject `as` MyClass** -> safely convert myObject to an object of type MyClass

# safely access 
* **access members of object**: **`?.`**
* **provide a default value for a nullable variable**: **`??`**
* **safely convert a string to other type**: **`.TryParse()`** (_VD: DateTime.TryParse_)