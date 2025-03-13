==================================================================
# Factory Method (or Virtual Constructor)
* -> is a creational design pattern that **`provides an interface for creating objects in a superclass`** and  **`subclasses will alter the type of objects that will be created`**

## Example
* -> tại sao cần 1 "Creator" (abstract class/interface/base class) có 1 method trả về 1 interface để tạo instance implement cái interface do, mà không đơn giản định nghĩa interface rồi tạo instance dựa trên DI
* -> ta cần hiểu ta gọi là nó là "Creator" vì nó chứa factory method, nhưng thực tế những "Creator" này thường là những business hoàn chỉnh nó sẽ chứa nhiều logic khác nữa
* -> và những logic này cần phụ thuộc vào những instance khác nhau của cùng interface (Ví dụ: để thực hiện 1 hành động consistence), ta nên **`decoupling`** việc create instance và việc thực sự sử dụng nó 
* -> khi dùng DI để khởi tạo instance thì các tham số trong constructor của nó cũng phải được DI, nhưng trong nhiều trường hợp ta sẽ muốn kiểm soát tham số được truyền vô để tạo instance cho những trường hợp cụ thể
* -> ngoài ra sẽ có trường hợp ta muốn factory method trả về 1 instance có sẵn thay vì phải tạo mới 1 instance giống hệt
* => subclass của "Creator" sẽ cần cung cấp implementation cho những factory method này để có những business logic cụ thể
* => điều này cho phép user extend internal components của library or framework bằng cách reduce the code that constructs components across the framework into a single factory method
* => 1 "Creator" càng có nhiều factory method thì nó sẽ càng gần với **`Abstract Factory`** pattern

## Implementation
Make all products follow the same interface. This interface should declare methods that make sense in every product.

Add an empty factory method inside the creator class. The return type of the method should match the common product interface.

In the creator’s code find all references to product constructors. One by one, replace them with calls to the factory method, while extracting the product creation code into the factory method.

You might need to add a temporary parameter to the factory method to control the type of returned product.

At this point, the code of the factory method may look pretty ugly. It may have a large switch statement that picks which product class to instantiate. But don’t worry, we’ll fix it soon enough.

Now, create a set of creator subclasses for each type of product listed in the factory method. Override the factory method in the subclasses and extract the appropriate bits of construction code from the base method.

If there are too many product types and it doesn’t make sense to create subclasses for all of them, you can reuse the control parameter from the base class in subclasses.

For instance, imagine that you have the following hierarchy of classes: the base Mail class with a couple of subclasses: AirMail and GroundMail; the Transport classes are Plane, Truck and Train. While the AirMail class only uses Plane objects, GroundMail may work with both Truck and Train objects. You can create a new subclass (say TrainMail) to handle both cases, but there’s another option. The client code can pass an argument to the factory method of the GroundMail class to control which product it wants to receive.

If, after all of the extractions, the base factory method has become empty, you can make it abstract. If there’s something left, you can make it a default behavior of the method.

==================================================================
# Application

## Use the Factory Method when you want to provide users of your library or framework with a way to extend its internal components
```cs
// Example
Imagine that you write an app using an open source UI framework. Your app should have round buttons, but the framework only provides square ones. You extend the standard Button class with a glorious RoundButton subclass. But now you need to tell the main UIFramework class to use the new button subclass instead of a default one.

To achieve this, you create a subclass UIWithRoundButtons from a base framework class and override its createButton method. While this method returns Button objects in the base class, you make your subclass return RoundButton objects. Now use the UIWithRoundButtons class instead of UIFramework. And that’s about it!
```

## Use the Factory Method when you want to save system resources by reusing existing objects instead of rebuilding them each time
* -> You often experience this need when dealing with large, resource-intensive objects such as database connections, file systems, and network resources.
```cs
// Example
Let’s think about what has to be done to reuse an existing object:

First, you need to create some storage to keep track of all of the created objects.
When someone requests an object, the program should look for a free object inside that pool.
… and then return it to the client code.
If there are no free objects, the program should create a new one (and add it to the pool).
That’s a lot of code! And it must all be put into a single place so that you don’t pollute the program with duplicate code.

Probably the most obvious and convenient place where this code could be placed is the constructor of the class whose objects we’re trying to reuse. However, a constructor must always return new objects by definition. It can’t return existing instances.

Therefore, you need to have a regular method capable of creating new objects as well as reusing existing ones. That sounds very much like a factory method.
```

## 