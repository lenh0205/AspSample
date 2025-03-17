> Prototype là để giải quyết việc clone object, nhưng có 1 vấn đề đằng sau nó là tại sao phải cloning 1 object ?
> ví dụ, when your objects have dozens of fields and hundreds of possible configurations, cloning them might serve as an alternative to subclassing.
> sử dụng abstract class nếu ta chắc rằng tất cả các subclasses đều thực hiện 1 behavior theo cách giống nhau trong mọi trường hợp, việc đổi logic của concrete behavior trong base class không làm ảnh hưởng đến tính đúng đắn
>  You can get rid of repeated initialization code in favor of cloning pre-built prototypes ?

# Prototype
* -> cách tiếp cận đầu tiên là First, you have to create a new object of the same class
* ->  Then you have to go through all the fields of the original object and copy their values over to the new object
* ->  But there’s a catch. Not all objects can be copied that way because some of the object’s fields may be private and not visible from outside of the object itself. 
* ->  also Since you have to know the object’s class to create a duplicate, your code becomes dependent on that class
* -> Ngoài ra, trong trường hợp code của ta phụ thuộc vào interface thì ta sẽ không thể biết chính xác nên sử dụng concrete class nào để khởi tạo object

* -> The Prototype pattern delegates the cloning process to the actual objects that are being cloned. The pattern declares a common interface for all objects that support cloning. This interface lets you clone an object without coupling your code to the class of that object. Usually, such an interface contains just a single clone method
* -> The implementation of the clone method is very similar in all classes. The method creates an object of the current class and carries over all of the field values of the old object into the new one

## Note
* -> An object that supports cloning is called a prototype. 
* -> you create a set of objects, configured in various ways. When you need an object like the one you’ve configured, you just clone a prototype instead of constructing a new object from scratch.

## Implementation
* -> object của ta chắc chắn là phải có thêm **`1 constructor có 1 tham số để ta có thể truyền this (an object of that class as an argument) vào`** và **`clone()`** method
* -> vì trong trường hợp object của ta có private field, thì khi clone() method khởi tạo 1 object mới bằng new thì giá trị của các private field không thể init bằng setter được 
* -> nên bắt buộc ta phải pass **`prototype`** (**`this`** của origin object) vào constructor của object mới này và bắt đầu map lại tất cả các field bằng cách **this.field1 = prototype.field1** (**`this`** ở đây là object ta mới tạo)
