
# Reflection
* -> _Reflection_ được sử dụng để **`truy cập đến metadata`** của **`kiểu đối tượng`** bất kỳ tại thời điểm **`runtime`** của chương trình
* -> _về cơ bản, include getting infomation: classes, interfaces, and value types (that is, structures and enumerations); create type instances, invoke, access, ... them at run time_

* -> `classes` in the **System.Reflection** namespace, together with **System.Type**, enable us to **`obtain information`** about **`loaded assemblies`** and **`types`** 

## "System.Type" class
* -> là lớp chính để thực hiện các **`cơ chế Reflection`**
* -> 1 _abstract class_ đại diện **`cho mọi kiểu dữ liệu trong .Net`**
* => by this class, we can take all information about: data type, method,...; create instance, execute method
* => _to define Type_, use **GetType()** for **`initialized oject`** and use **typeof** for **`DataType`** 

```c# - "typeof" for "int" datatype
public class ReflectionCSharp
{
    public void Run()
    {
        var fullName = string.Empty;
        var assemblyName = string.Empty;
        var constructors = new List<string>();

        Type? type = typeof(int);
        fullName = type.FullName; 
        assemblyName = type.Assembly.FullName; 
        var listConstructors = type.GetConstructors().ToList();
        foreach (var item in listConstructors) constructors.Add(item.Name);

        Console.WriteLine("Type.FullName: " + fullName);
        Console.WriteLine("Type.Assembly.FullName: " + assemblyName);
        Console.WriteLine("Type.GetConstructors: " + string.Join(", ", constructors));
        Console.WriteLine("==============================");
    }
}

// Type.FullName: System.Int32
// Type.Assembly.FullName: System.Private.CoreLib, Version=6.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e
// Type.GetConstructors:
// ==============================

// => tên class/struct (gồm cả namespace): Struct Int32 của namespace "System"
// => assembly info (tên file dll hoặc exe, phiên bản, ...): file mscorlib.dll - built-in .Net library
// => danh sách các constructor của class: int là struct nên không có constructor
```

```cs - "GetType()" of class
Type mType1 = Type.GetType("System.Int32");
Console.WriteLine(mType1.FullName); // System.Int32
```

```cs - "GetType()" for "double" instance
double i = 100d;
type = i.GetType();
fullName = type.FullName;
assemblyName = type.Assembly.FullName;
constructors.Clear();
listConstructors = type.GetConstructors().ToList();
foreach (var item in listConstructors) constructors.Add(item.Name);

Console.WriteLine("Type.FullName: " + fullName);
Console.WriteLine("Type.Assembly.FullName: " + assemblyName);
Console.WriteLine("Type.GetConstructors: " + string.Join(", ", constructors));
Console.WriteLine("==============================");

// Type.FullName: System.Double
// Type.Assembly.FullName: System.Private.CoreLib, Version=6.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e
// Type.GetConstructors:
// ==============================
```

```cs - "GetType()" for a complex instance
var reflectionInfo = new ReflectionInformation("Name", "Value");
type = reflectionInfo.GetType();
fullName = type.FullName;
assemblyName = type.Assembly.FullName;
constructors.Clear();
listConstructors = type.GetConstructors().ToList();
foreach (var item in listConstructors) constructors.Add(item.ToString());

Console.WriteLine("Type.FullName: " + fullName);
Console.WriteLine("Type.Assembly.FullName: " + assemblyName);
Console.WriteLine("Type.GetConstructors: " + string.Join(", ", constructors));

// Type.FullName: ConsoleApp1.Example.ReflectionInformation
// Type.Assembly.FullName: ConsoleApp1, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// Type.GetConstructors: Void .ctor(Int32), Void .ctor(System.String, System.String)

// => ConsoleApp1 assembly: file ConsoleApp1.exe - file build ứng dụng
```


## MetaData
* -> class **Type** cung cấp phương thức dạng **`GetXXX()`** trả về **`1 hay 1 mảng đối tượng`** lưu trữ thông tin của mỗi member trong data type với hậu tố là **`Info`**
* -> bao gồm các `class` nằm trong **System.Reflection** namespace (_`ConstructorInfo, MethodInfo, ...`_)
* -> mặc định giúp ta lấy những thông tin với access modify là **`public`**; để lấy thông tin **`private, protected, internal`** ta cần truyền tham số **enum BindingFlags** (_thuộc namspace System.Reflection_)

```cs - VD: dùng "GetMembers()" để get tất cả public members của 1 class
Type mType = typeof(MyClass);
 
MemberInfo[] members = mType.GetMembers();
Array.ForEach(members,mem =>
    Console.WriteLine(mem.MemberType.ToString().PadRight(12) + ": " + mem)
);
```

### "ConstructorInfo" abstract class
* -> xác định **`danh sách các constructors`** (_mặc định là public constructors_)
* -> dùng **.GetConstructors(Type[] types)**
* -> required a Type array với đúng kiểu, trình tự, số lượng tham số của constructor

```cs - constructor không có tham số
public class ReflectionCSharp
{
    public void Run()
    {
        var reflectionInfo = new ReflectionInformation();
        var type = reflectionInfo.GetType();

        var constructors = type.GetConstructors();
        foreach (var item in constructors) Console.WriteLine(item);

        object obj = conInfo.Invoke(null);
        mType.InvokeMember("SayHello", BindingFlags.InvokeMethod, null, obj, null);
    }
}
// Void .ctor()
// Void .ctor(Int32)
// Void .ctor(Int32, System.String)
```

```cs - constructor có tham số
class Program
{
    class MyClass
    {
        string _name;
        public MyClass(string name)
        {
            _name = name;
        }
        public void SayHello()
        {
            Console.WriteLine("Hello, "+_name);
        }
    }
    static void Main()
    {
        Type mType = Type.GetType("ConsoleApplication1.Program+MyClass"); // "+" for children class of "Program" class

        ConstructorInfo conInfo = mType.GetConstructor(new Type[] { typeof(string) });

        object obj = conInfo.Invoke(new object[] { "Yin Yang" });

        mType.InvokeMember("SayHello", BindingFlags.InvokeMethod, null, obj, null);

        Console.ReadLine();
    }
}
```

### MethodInfo
* -> phương thức **GetMethod(string methodName)** trả về một đối tượng kiểu **System.Reflection.MethodInfo**

```cs - method has "no param"
class MyClass
{
    public void SayHello()
    {
        Console.WriteLine("Hello"); // Output: Hello
    }
}
static void Main()
{
    MyClass myClass = new MyClass();
    Type myType = myClass.GetType();

    MethodInfo myMethodInfo = myType.GetMethod("SayHello"); // Lấy về phương thức SayHello
    myMethodInfo.Invoke(myClass, null); // Thực thi phương thức "SayHello" của myClass với tham số là null
}
```

* _if the method required param, we need to pass an **`object array`** to Invoke()_
```cs - method has "params"
class MyClass
{
    public void SayHello(string name)
    {
        Console.WriteLine("Hello, {0}", name); // Output: Hello, Yin Yang
    }
}
static void Main()
{
    MyClass mClass = new MyClass();
    Type mType = mClass.GetType();
    
    MethodInfo mMethodInfo = mType.GetMethod("SayHello"); // Lấy về phương thức SayHello

    object[] mParams = new object[] { " Yin Yang" };
    mMethodInfo.Invoke(mClass, mParams); // Thực thi phương thức "SayHello" với tham số là mParam
}
```

```cs - Method Overloaded
// use another method overloaded of "GetMethod"
class MyClass
{
    public void SayHello()
    {
        Console.WriteLine("Hello");
    }
    public void SayHello(string name)
    {
        Console.WriteLine("Hello, {0}", name); // Output: Hello, Yin Yang
    }
}
static void Main()
{
    MyClass myClass = new MyClass();
    Type myType = myClass.GetType();

    // "SayHello" method có 1 tham số kiểu string
    MethodInfo myMethodInfo = myType.GetMethod("SayHello", new Type[] { typeof(string) });

    // thực thi phương thức SayHello của myClass với tham số
    myMethodInfo.Invoke(myClass, new object[] { "Yin Yang" });

    Console.Read();
}
```

```cs - use "InvokeMember()" with "BindingFlags. InvokeMethod" instead of "GetMethod" and "Invoke"
class MyClass
{
    public void SayHello()
    {
        Console.WriteLine("Hello");
    }
}
static void Main()
{
    MyClass mClass = new MyClass();
    Type mType = mClass.GetType();

    mType.InvokeMember("SayHello", BindingFlags.InvokeMethod, null, mClass, null);
}
```

### Other
* _Ngoài ra_, còn có **MemberInfo**, **InterfaceInfo**, **EventInfo**, **PropertyInfo**, **FieldInfo**, ...

=============================================================

# "System.Activator" class
* -> cách tạo instance như trên khá dài dòng, rắc rối, khó nhớ; trong khi yêu cầu **`tạo một instance từ tên kiểu`** khá phổ biến

```cs - tạo đối tượng với constructor không và có tham số
class Program
{
    class MyClass1
    {
        public void SayHello()
        {
            Console.WriteLine("Hello");
        }
    }
    class MyClass2
    {
        string _name;

        public MyClass2(string name)
        {
            _name = name;
        }
        public void SayHello()
        {
            Console.WriteLine("Hello, "+_name);
        }
    }
    static void Main(string[] args)
    {
        Type mType1 = Type.GetType("ConsoleApplication1.Program+MyClass1");
        Type mType2 = Type.GetType("ConsoleApplication1.Program+MyClass2");

        object obj1 = Activator.CreateInstance(mType1);
        object obj2 = Activator.CreateInstance(mType2,"Yin Yang");

        mType1.InvokeMember("SayHello", BindingFlags.InvokeMethod, null, obj1, null);
        mType2.InvokeMember("SayHello", BindingFlags.InvokeMethod, null, obj2, null);

        Console.Read();
    }
}
```

================================================================
# "System.Reflection.Assembly" class

* **Net Assembly** 
* -> có thể được hiểu là **kết quả của quá trình biên dịch** từ **`mã nguồn sang tập tin nhị phân`** dựa trên **`.Net framework`**
* -> là `thành phần cơ bản nhất .Net framework`, _assemly_ là thành phần `không thể thiếu trong bất kì ứng dụng .Net nào` 
* -> được thể hiện dưới hai dạng tập tin là **EXE** (**`process assembly`**) và **DLL** (**`library assembly`**)
* -> Assembly có thể được lưu trữ dưới dạng **`single-file hoặc multi-file`**, tùy theo kiểu dự án mà ta làm việc.

* **System.Reflection.Assembly** class
* -> có thể được coi là **`một kiểu mô phỏng chi tiết về các assembly`**
* -> _Assembly class_ chứa đầy đủ các thông tin cho phép chúng ta **`truy xuất thông tin và thực thi các phương thức lấy được từ các assembly`**
* -> _Assembly class_ cung cấp các **`phương thức tĩnh để nạp một assembly`** thông qua **AssemblyName**, đường dẫn tập tin hoặc từ tiến trình đang chạy

* _ta cũng có thể **`lấy Assembly`** dễ dàng từ property của lớp Type - **typeof(int).Assembly**
* _có thể hiểu Lớp **Type** đại diện cho các **`kiểu dữ liệu`**, lớp **Assembly** đại diện cho các **`assembly`**_

```cs - s/d memthod "Assembly.GetExecutingAssembly()" để lấy về assembly hiện tại và in ra màn hình các kiểu dữ liệu của nó
class Program
{
    class MyClass { }
 
    static void Main()
    {
 
        Assembly ass = Assembly.GetExecutingAssembly();
 
        Type[] mTypes = ass.GetTypes();
 
        Array.ForEach(mTypes,type => Console.WriteLine(type.Name));
        // Program
        // MyClass
    }
}
```

=============================================================

# Apply "Reflection" in pratical

## Initialize Object, call method() at runtime
* -> để tạo đối tượng thay vì s/d keyword **new** thì ta có thể s/d **System.Activator**
* -> mặc định hàm **Activator.CreateInstance** trả về kiểu **`object`**; **`first param`** là **`loại đối tượng tạo`**, **`second param`** là **`list parameters of a specific constructors`**
* => ta sẽ cần hiện ép kiểu để đưa về đối tượng mong muốn

* _để gọi hàm ta còn có thể GetMethod() rồi truyền đối số vào hàm cần gọi; nhưng `không khuyến khích`_

```cs
public class ReflectionCSharp
{
    public void Run()
    {
        var reflectionInfo = new ReflectionInformation();
        var type = reflectionInfo.GetType();

        // khởi tạo đối tượng dựa trên 2 constructor của class "ReflectionInformation"
        var firstReflection = (ReflectionInformation) Activator.CreateInstance(type, new object[] { 10 }); 
        var secondReflection = (ReflectionInformation) Activator.CreateInstance(type, new object[] { 10, "Name" });

        // gọi Method "Write" của "ReflectionInformation" instance
        firstReflection.Write();    //  Output: Id: 10. Name:
        secondReflection.Write();    //  Output: Id: 10. Name: Name
    }
}
```

## Từ "base class" và "interface" tìm ra list những "derived class"

```cs
public interface IValidate
{
    bool IsOk(string text);
}
public class TextNotEmpty : IValidate
{
    public bool IsOk(string text)
    {
        return !string.IsNullOrEmpty(text);
    }
}
public class TextAtLeast8Chars : IValidate
{
    public bool IsOk(string text)
    {
        return text.Length >= 8;
    }
}

public class ReflectionCSharp
{
    public void Run()
    {
        var type = typeof(IValidate);

        IEnumerable<Type>? needValids = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => type.IsAssignableFrom(p) && !p.IsInterface && !p.IsAbstract);

        foreach (var item in needValids) Console.WriteLine(item);
        // ConsoleApp1.Example.TextNotEmpty
        // ConsoleApp1.Example.TextAtLeast8Chars
    }
}
```

```cs - h ta có thể apply tất cả loại "validate" cho bất kỳ "text" nào 
var text = string.Empty;
foreach (var item in needValids)
{
    IValidate? o = Activator.CreateInstance(item, null) as IValidate;
    var ok = o.IsOk(text);

    Console.WriteLine(item + "==" + text + "==" + ok);
}
Console.WriteLine("==================");

// ConsoleApp1.Example.TextNotEmpty====False
// ConsoleApp1.Example.TextAtLeast8Chars====False
// ==================


text = "WTF";
foreach (var item in needValids)
{
    var o = Activator.CreateInstance(item, null) as IValidate;
    var ok = o.IsOk(text);

    Console.WriteLine(item + "==" + text + "==" + ok);
}
Console.WriteLine("==================");

// ConsoleApp1.Example.TextNotEmpty==WTF==True
// ConsoleApp1.Example.TextAtLeast8Chars==WTF==False
// ==================


text = "WTF WTF WTF";
foreach (var item in needValids)
{
    var o = Activator.CreateInstance(item, null) as IValidate;
    var ok = o.IsOk(text);

    Console.WriteLine(item + "==" + text + "==" + ok);
}
// ConsoleApp1.Example.TextNotEmpty==WTF WTF WTF==True
// ConsoleApp1.Example.TextAtLeast8Chars==WTF WTF WTF==True
```

============================================================
### Attribute
* -> cho phép định nghĩa những **`thông tin ngoài mặc định của .Net`**
* -> thông tin khai báo thêm phải nằm trong class kế thừa lớp **System.Attribute**
* -> những thông tin thêm này có thể được gán cho class, field, property,...
* -> nguyên tắc đặt tên 1 attribute: **<Tên thuộc tính>Attribute** (_`<Tên thuộc tính>` để gán cho lớp, thuộc tính, methods..._)

```cs - tạo "Custom" Attribute
public class CustomAttribute : Attribute
{
    public string Name { get; set; }

    public void Write()
    {
        Console.WriteLine("Hello CustomAttribute.");
    }
}

[Custom]
public class ReflectionInformation
{
    [Custom]
    private int _id;
    private string _name;

    [Custom]
    public ReflectionInformation()
    {
    }
    public ReflectionInformation(int id)
    {
        Id = id;
    }
    public ReflectionInformation(int id, string name)
    {
        Id = id;
        Name = name;
    }

    [Custom]
    public int Id { get; set; }
    public string Name { get; set; }

    [Custom]
    public void Write()
    {
        Console.WriteLine("Id: " + Id);
        Console.WriteLine("Name: " + Name);
    }
    public void Write(string name)
    {
        Console.WriteLine("Name: " + name);
    }
}

public class ReflectionCSharp
{
    public void Run()
    {
        var reflectionInfo = new ReflectionInformation();
        var type = reflectionInfo.GetType();

        var attrs = type.Attributes;
        Console.WriteLine("Class.Attribute: " + attrs); // list of .NET default attribute 

        var customAttrs = type.CustomAttributes;
        Console.WriteLine("Class.Custom.Attribute: " + customAttrs); // list of attribute defined by user
    }
}

// Class.Attribute: AutoLayout, AnsiClass, Class, Public, BeforeFieldInit
// Class.Custom.Attribute: System.Collections.ObjectModel.ReadOnlyCollection`1[System.Reflection.CustomAttributeData]
```