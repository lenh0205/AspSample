> về cơ bản, các mục tiêu cốt lõi của Clean Architecture cũng giống với đối với Ports & Adapters (Hexagonal) và Onion Architectures
> thực tế là nó không phải 1 mô hình đột phá mới mà nó khôi phục, làm rõ các khái niệm, quy tắc, khuôn mẫu quan trọng 

========================================================================

# Clean Architecture
* _i **`loại bỏ sự lệ thuộc trực tiếp`** giữa các **object** cũng như các **layer** trong ứng dụng - **`hướng tâm`**: các layer ở trong không biết gì về các layer bên ngoài_

* về cơ bản thì gồm **4 layer** được đại diện thông qua các vòng tròn đồng tâm
* -> **Enitites** , **Use Cases** , **Interface Adapters** (**`Controllers, Gateways, Presenters`**), **Frameworks & Drivers** (**`UI, Web, DB, Framework, devices`**)
* -> để các layer trong Clean Architecture có thể làm việc được nhưng lại độc lập với nhau thì chúng sẽ dùng các **interfaces**
* -> mỗi vòng sẽ có tập hợp các "Dependencies", chỉ có **`objects của vòng ngoài mới phụ thuộc vào dependencies của vòng trong không có chiều ngược lại`**
<br/>

* => nó sẽ **đòi hỏi viết nhiều class, interface**; nhưng nhờ sự độc lập giữa các tầng mà việc **Debug và Testing trở nên dễ dàng** hơn
* _tách biệt `presentation logic`, `business logic`, `data access logic`_
* _vẫn test được dù thiếu `Database`, `Web server`_

## Note
* -> bởi việc đảm bảo business rules và core domain của ta bên trong vòng tròn là hoàn toàn **không có bất kì sự phụ thuộc nào bên ngoài hoặc các thư việc bên thứ 3 (3rd party libraries)**
* -> có nghĩa là chúng phải **`sử dụng code C# thuần`** vì sẽ dễ dàng hơn trong việc test

* -> khi chúng ta truyền dữ liệu qua một ranh giới (Boundary), nó **luôn ở dạng thuận tiện nhất cho vòng tròn phía trong**

* -> `Clean Architecture` là kiến trúc tham khảo, nên trên thực tế **không nhất thiết phải là 4 tầng**

```r - Ex: 
// 1 business của API update "product"
// gồm 3 tầng: "Transport" -> "Business" -> "Repository/Storage"
// chứa tương ứng: "HTTP Handler methods; parse & validate data from request" -> "implement business logic" -> "store, retrieve data"
// business: "API update Product" -> "UpdateProduct(product)" -> "FindProductById(id); UpdateProduct(product)"
```

========================================================================
![Robert C. Martin 2012, The Clean Architecture](/nonrelated/cleanarchitecture1.jpg)

# Layers

## Entities (`Enterprise Business Rules`)
* -> Entities là layer trong cùng, cũng là **`layer quan trọng nhất`**
* -> Entity chính là các thực thể hay từng **đối tượng cụ thể và các rule business logic của nó**
* -> trong OOP, đây chính là Object cùng với **`các method và properties tuân thủ nguyên tắc Encapsulation`** - chỉ bên trong Object mới có thể thay đổi trạng thái (State) của chính nó

```r 
// -> trong object Person thì thuộc tính age không thể bé hơn 1
// -> khi cần thay đổi age, ta cần phải thông qua hàm public setAge - hàm này cũng chịu trách nhiệm check điều kiện liên quan tới age

// -> các business logic của layer Entities sẽ không quan tâm hay lệ thuộc vào các business logic ở các layer bên ngoài như "Use Cases"
// -> giả sử với trường hợp người dùng phải từ 18 tuổi trở lên mới được phép tạo tài khoản thì rule thuộc tính Age trong Entities vẫn không đổi
```

## Use Cases (`Application Business Rules`)
* -> layer chứa các **business logic ở cấp độ cụ thể từng Use Case (hay application)**

```r - Ex:
// -> "Use Case" đăng ký tài khoản (tạo mới một Person/Account) sẽ cần tổ hợp một hoặc nhiều Entities tuỳ vào độ phức tạp của Use Case
// -> các business logic của Use Case sẽ không quan tâm và lệ thuộc vào việc dữ liệu đến từ đâu, dùng các thư viện nào làm apdapter, dữ liệu thể hiện thế nào,... vì đấy là nhiệm vụ của layer Interface Adapters
```

## Interface Adapters
* -> layer phụ trách việc **chuyển đổi các format dữ liệu để phù hợp với từng Use Case và Entities**, các format dữ liệu này có thể dùng cho cả bên trong hoặc ngoài ứng dụng

```r - Ex:
// -> thông tin người dùng sẽ có một số thông tin rất nhạy cảm như Email, Phone, Address
// -> không phải lúc nào dữ liệu cũng về đầy đủ để phục vụ GUI (Web, App); tương tự với tuỳ vào hệ thống Database mà các adapter phải format dữ liệu hợp lý.
```

* -> như vậy dữ liệu đầu vào và ra ở tầng Interface Apdapter chỉ cần đủ và hợp lý
* -> nó sẽ không quan tâm việc dữ liệu sẽ được hiển thị cụ thể như thế nào cũng như được thu thập như thế nào. Vì đó là nhiệm vụ của tầng Frameworks & Drivers

## Frameworks & Drivers
* -> là tầng ngoài cùng, **tổ hợp các công cụ cụ thể phục vụ cho từng nhu cầu của end user** như: thiết bị (devices), web, application, databases,... 
* -> trong kiến trúc Clean Architecture thì ở tầng này là "nhẹ" nhất vì chúng ta không cần phải viết quá nhiều code
* -> trên thực tế thì đây là nơi "biết tất cả" cụ thể các tầng là gì thông qua việc chịu trách nhiệm khởi tạo các objects cho các tầng bên trong (hay còn gọi là Setup Dependencies)

# Flow of control giữa các control
* _biểu đồ nhỏ góc dưới bên phải của mô hình Clean Architect_
![Flow of control](/nonrelated/cleanarchitecturedesign2.png)

* -> trong sơ đồ ở trên, ở phía bên trái, chúng ta có View và Controller của MVC
* -> mọi thứ bên trong / giữa các đường kẻ đôi màu đen đại diện cho Model trong MVC
* -> mô hình này cũng đại diện cho kiến trúc **`EBI`** (với **Boundary**, **Interactor** và the **Entities**), **Application** trong **`Hexagonal Architecture`**, **Application Core** trong **`Onion Architecture`**, **Entities** và **Use Cases** layer trong **`Clean Architecture`**

* _Theo biểu đồ luồng tương tác, chúng ta có một yêu cầu HTTP đến các Controller. Controller sau đó sẽ:_
* -> Phân tích Request;
* -> Tạo một Request Model với các dữ liệu có liên quan;
* -> Execute một method trong Interactor (đã được đưa (inject) vào Controller bằng cách sử dụng interface của Interactor là Boundary), chuyển nó cho Request Model;
* -> Interactor sẽ:
* -> Sử dụng implementation của Entity Gateway (được đưa vào Interactor bằng cách sử dụng Entity Gateway Interface) để tìm các Entities liên quan;
* -> Phối hợp các tương tác giữa các Entities;
* -> Tạo Response Model với kết quả dữ liệu trả về;
* -> Tạo ra Presenter chứa Response Model;
* -> Trả lại Presenter cho Controller;
* -> Dùng Presenter để tạo ra một ViewModel;
* -> Bind ViewModel với View;
* -> Trả View về cho Client.

========================================================================
# Compare

## Layers in 'Onion Architecture' and 'Hexagonal Architecture'
* -> Sơ đồ Kiến trúc **Hexagonal Architecture** chỉ hiển thị cho chúng ta hai layer: **`Bên trong ứng dụng`** và **`bên ngoài của ứng dụng`**

* -> mặt khác, **Onion Architecture** mang đến sự kết hợp các layer ứng dụng được xác định bởi **`DDD (Domain Driven Design)`**
* -> **Entities**
* -> **Value Objects**
* -> **Application Services** chứa các **`use-case logic`**
* -> **Domain Services** đóng gói **`domain logic không thuộc về các Entities hoặc Value Objects…`**

* -> khi so sánh với Onion Architecture, **Clean Architecture** sẽ duy trì Application Services layer (Use Cases) và Entities layer nhưng dường như nó quên mất **`Domain Services layer`**
* -> tuy nhiên thực tế trong Clean Architecture, **Entities** không chỉ là và Entity theo ý nghĩa của DDD mà bất cứ Domain object nào; tức là **`2 layer bên trong đã được sát nhập để đơn giản sơ đồ`**
* _"Một entity có thể là một đối tượng với các phương thức, hoặc nó có thể là một tập hợp các cấu trúc dữ liệu và các hàm"_

========================================================================

# Triển khai với ASP.NET
* _thường thì 4 layer khi triển khai 'Clean Architect" trong ASP.NET: `Domain, Application, Infrastructure, Presentation` (từ trong ra ngoài)_
* -> "Application Layer" và "Domain Layer are always **the core** of system's design; the core will be **independent of the `data access` and `infrastructure` concerns**
* -> we can achieve this goal by **`using the Interfaces and abstraction`** **within the core system**, but **`implementing them`** **outside of the core system**
https://www.c-sharpcorner.com/article/clean-architecture-in-asp-net-core-web-api/

https://topdev.vn/blog/lam-the-nao-de-sap-xep-clean-architecture-theo-modular-patterns-trong-10-phut/
https://tuhocict.com/lesson/web-application-architectures/#google_vignette