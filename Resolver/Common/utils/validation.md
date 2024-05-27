# safely access 
* **_access members of object_**: **`?.`**
* **_provide a default value for a nullable variable_**: **`??`**
* **_safely convert a string to other type_**: **`.TryParse()`** (_VD: DateTime.TryParse_)

# LinQ to SQL return null:
* `able to return null`: .FirstOrDefault() , .SingleOrDefault() , .LastOrDefault(), .ElementAtOrDefault()
* `only return empty list`: .ToList(), .ToArray() , .ToDictionary()

# Checking "null"
* **`!= null`** 
* **`is not null`** (_`is null` hoặc `is not null` không depend on custom implementation of `!= and == operators`_)

* **define custom == and != operators**: khi default behavior of checking for reference equality không đủ tốt 
```cs
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

## Avoid "Escape Sequences" in a string
* **@** symbol - create a verbatim string literal
* -> string is **`interpreted exactly as it is written`** (_including any **`escape characters`**_)
* -> useful for strings that **`contain special characters`** or **`contain multiple lines`**
```cs
string sourceDirectory = @"c:\sourceDirectory"; 

string multilineString = @"This is a
multiline string.";
```

## Compare 2 strings
* **StringComparison.Ordinal**
* -> the fastest type of string comparison, but it is also the least sensitive to the culture and regional settings of the machine

# Checking "collection"
## Checking a List<T> is empty 
_Nhưng vẫn cần check null của List<T> trước_

* **`.Count` property** - best if using a `List<T>`, since it knows its size (luôn đc update khi list thay đổi)
* **`.Any()` method** - best when using an `IEnumerable`, faster since it stops after checking one item
* **`.Count()` method** - method of `IEnumerable<T>` iterate entire sequence; ta có thể cho codition vào 

# Checking type of object:
* **myObject `is` MyClass**
* **myObject `as` MyClass** -> safely convert myObject to an object of type MyClass



