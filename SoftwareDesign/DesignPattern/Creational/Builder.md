> ta sẽ cần làm sao để client code chỉ sử dụng các consistence interface  
> đặc điểm riêng biệt của Builder là nó có thể tạo multiple product mà không cần có common base interface

# Builder
* -> is a creational design pattern that lets you **construct `complex objects` + `step by step`**
* -> the pattern allows you to produce different types and representations of an object using the same construction code
* => mục đích là để reuse the same object construction code (tái sử dụng logic initiate 1 object);

## Summary
* -> để initiate 1 complex object đôi khi sẽ đòi hỏi step-by-step initialization of many fields and nested objects
* -> ví dụ ta có 1 complex object là "House" ngoài việc construct những thành phần cơ bản (tường, sàn, cửa, mái), còn cho phép enable các tiện ích khác (hồ bơi, vườn, ga-ra, ....)
* -> có hàng trăm cách để construct 1 "House"
* -> điều này sẽ gắn liền với việc có 1 constructor khổng lồ với nhiều parameter; hoặc tệ hơn nhiều khi nó sẽ nằm rải rác trong khắp client code (VD: dùng setter để gán giá trị cho public properties)
* => in most cases most of the parameters will be unused (VD: tạo 10 căn nhà mà chỉ có 1 căn nhà cần hồ bơi, thì parameters related to swimming pools will be useless 9 times)
* => việc tạo overloading cho constructor thì cũng vậy, vì nó cũng refer tới cái main constructor khổng lồ và pass vào default value

* -> cách đơn giản nhất đề giải quyết chuyện này là chỉ cho "House" construct những thành phần cơ bản; sau đó dùng inheritance để tạo những subclasses cover all combinations of the parameters
* => nhưng việc này sẽ dẫn đến một lượng lớn subclasses; cũng như the growing of hierarchy khi có 1 tham số mới

* -> cách tiếp cận thứ 2 là **`extract the object construction code out of its own class`** and **`move it to separate objects called builders`**
* -> **builder interface** sẽ defines all possible construction steps - ứng với từng method - để construct 1 **product**; mỗi step sẽ construct từng part của "House"
* -> sau đó create a **concrete builder** class for each of the product representations and implement their construction steps theo cách riêng biệt để phù hợp với **concrete product**
* -> dựa vào những step ta và thứ tự gọi từng step của builder trong client code mà object sẽ được initiate theo cách khác nhau
* -> builder interface sẽ có method **reset** để khởi tạo instance với constructor không có tham số (từ đó bắt đầu các build step khác) 
* -> mặc dù các concrete builder là có chung builder nhưng các product chúng construct might not have a common interface; việc tạo ra entirely different products là hoàn toàn oke
* => vậy nên property chứa instance của product và getResult method để lấy object sau khi construct xong được cần được đặt trong concrete builder thay vì builder interface (trừ phi sử dụng dynamic - Ví dụ generic)
* => còn nếu muốn đặt trong getResult trong builder interface thì ta cần đảm bảo ta chỉ  dealing with products from a single hierarchy

* -> bình thường ta có thể sử dụng client code để control builders directly
* -> nhưng ta cũng có thể tái sử dụng configuration bằng Director - the director class guides the order of construction
* -> Director thường chứa logic để construct những most common model of the "House" từ đó ta có thể reuse an existing construction process
* -> nó không thể chứa **getResult** vì nó sẽ làm Direct coupling with **concrete product**
* -> Directory có thể gồm nhiều construction behavior để thực hiện constructions logic khác nhau trên Builder

## Structure
* -> Concrete builders may produce products that don't follow the common interface

* -> the Client must associate one of the builder objects with the director. Usually, it’s done just once, via parameters of the director's constructor. Then the director uses that builder object for all further construction. 
* -> however, there's an alternative approach for when the client passes the builder object to the production method of the director. In this case, you can use a different builder each time you produce something with the director.
* => với cách 1 thì nó cho phép ta viết thêm các behavior của Director dựa trên cùng 1 builder instance; còn cách 2 thì mỗi lần gọi method ta sẽ có 1 instance mới hoàn toàn cho phép Director có 2 method sử dụng 2 builder khác nhau
* -> tại sao concrete builder có getResult mà builder interface lại không

## Applicability
* -> Use the Builder pattern to get rid of a "telescoping constructor"
* -> Use the Builder pattern when you want your code to be able to create different representations of some product (for example, stone and wooden houses)
* -> Use the Builder to construct Composite trees or other complex objects