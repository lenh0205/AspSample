
# "private" vs "protected"
* -> khác biệt chính nằm ở việc **`access member khi inherit`**
* -> **a derived class has access** to the **`public`**, **`protected`**, **`internal`**, and **`protected internal`** **members of a base class**
* -> _about **private** member_, even though a **derived class inherits the those members of a base class**, it **cannot access those members**

* -> _so all those **private** members are `must present in the derived class` and `can do the same work they would do in the base class` itself_
* -> _imagine `a protected base class method accesses a private field`; that field `has to be present in the derived class` in order **for the inherited base class method to work properly**_

```cs
namespace LenhASP.Domain.Services
{
    class BaseModel
    {
        public int _id = 0;
        private string _name = string.Empty;
        protected long _age = 0;
        public BaseModel(int id, string name, long age) { 
            _id = id;
            _name = name;
            _age = age;
        }

        public string AccessPrivate () => _name;
        public long AccessProtected () => _age;

        protected string AccessPrivateForDerived () => _name;
    }

    class DerivedModel : BaseModel
    {
        public DerivedModel (int id, string name, long age) : base(id, name, age) { }

        public long AccessBaseDirectly() => _age + _id; // Error if access "_name" directly
        public string AccessPrivateOfBase() => AccessPrivateForDerived() + AccessPrivate(); // access indirectly
    }

    public class TestAccessModifier
    {
        public void LogTestingResult ()
        {
            var baseModel = new BaseModel(1, "Lee", 1);
            Debug.WriteLine(baseModel._id); // Error nếu access: "baseModel._name" hoặc "baseModel._age"
            Debug.WriteLine(baseModel.AccessPrivate() + baseModel.AccessProtected()); // Lee1

            var derivedModel = new DerivedModel(2, "Zed", 2);
            Debug.WriteLine(derivedModel._id); // 2
            Debug.WriteLine(derivedModel.AccessPrivate() + derivedModel.AccessProtected()); // Zed2
            Debug.WriteLine(derivedModel.AccessBaseDirectly() + derivedModel.AccessPrivateOfBase()); // 4ZedZed
        }
    }
}
// Kết luận 1: đối với "base class"
// -> các member bên trong class có thể truy cập trực tiếp lẫn nhau dù "access modifier" là gì

// Kết luận 2: đối với "instance" của base class
// -> chỉ có thể truy cập trực tiếp đến "public" member;
// -> tuy nhiên, có thể dùng những "public" member này để truy cập đến "private/protected" members

// Kết luận 3: đối với "derived class"
// -> các member bên trong chỉ có thể truy cập trực tiếp "public/protected" members của "base class";
// -> tuy nhiên, có thể dùng những "public/protected" members này để access đến "private" members của "base class"

// Kết luận 4: đối với "instance" của derived class
// -> chỉ có thể truy cập đến "public" member của "base class"
// -> tuy nhiên, có thể dùng "public" member của "base class" để truy cập đến "private/protected" members của "base class"
// -> hoặc dùng "public" member của chính "derived class" để truy cập đến "private/protected" members của base class

// => từ đó ta đưa ra 2 kết luận:
// -> Kết luận 1: đối với "instance" của 1 class, chỉ có thể truy cập đến "public" members
// -> Kết luận 2: những member là "private" và "protected" chỉ có thể dùng trong phạm vi class (not instance)
```