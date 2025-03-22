> những thằng pattern này tăng độ phức tạp cho project nên chỉ nên sử dụng trong project lớn
> từ **communication** sử dụng trong pattern này có giống từ **dependence** không ? No, "communication" là nói về việc exchange information dù chúng có **`dependent directly or not`**;
> nên là "Mediator" come in khi 1 thằng object nó chỉ muốn notify event, nó không cần biết về những thằng khác chứa logic xử lý cho event này; mediator mới là thằng biết/refer tất cả những thằng nó cần communicate để xử lý event này;
> vậy nên mỗi object là self-contain không reference lẫn nhau; chỉ có refer đến Mediator để thực hiện việc communicate nhằm xử lý những business logic thuộc về class khác

==============================================================
# Mediator
* -> **`reduce chaotic dependencies between objects`**
* -> by **restricts direct communications** between the objects and forces them to **depend only on a single mediator object**

* _nó kiểu đổi quan hệ many-to-many thành one-to-many bằng cách tạo ra 1 thằng trung gian; tránh những giao tiếp phức tạp giữa các component_

## Problem
* -> Elements can have lots of relations with other elements. Hence, changes to some elements may affect the others.
* -> By having this logic implemented directly inside the code of the form elements you make these elements’ classes much harder to reuse in other forms of the app

* => the components depend only on a single mediator class instead of being coupled to dozens of their colleagues
* => encapsulate a complex web of relations between various objects inside a single mediator object. The fewer dependencies a class has, the easier it becomes to modify, extend or reuse that class.
* => we can go further and make the dependency even looser by extracting the common interface for all types of dialogs - so we can reuse the component in other programs by linking it to a different mediator

## Implement
* -> Previously, each time a user clicked the button, it had to validate the values of all individual form elements. Now its single job is to notify the dialog about the click. 
* -> Upon receiving this notification, the dialog itself performs the validations or passes the task to the individual elements
* -> the interface declares methods of communication with components, which usually include just a single notification method - which all form elements can use to notify the dialog about events happening to those elements
* -> Concrete mediators often keep references to all components they manage and sometimes even manage their lifecycle
* -> When the mediator receives the notification, it can easily identify the sender, which might be just enough to decide what component should be triggered in return


## Pros and Cons
* => **`Single Responsibility Principle`** - communication giữa các component được tổng hợp về một nơi
* => **`Open/Closed Principle`** - trường hợp cần mở rộng, có thể dễ dàng tạo ra mediator mới (_vì các component chỉ phụ thuộc vào "Mediator Interface"_)

## Others
* -> if we implement **`Mediator`** in **subscribe and unsubscribe**, nó sẽ khá giống **`Observer pattern`**

==============================================================
# 'CQRS' pattern (Command and Query Responsibility Segregation) 
* -> tức là nó để seperate trách nhiệm của **`Query`** và **`Command`** - hay **`separates read and update operations for a data store`**

## Reason
* -> có thể là do nhu cầu truy vấn thường nhiều hơn update dẫn đến việc khác biệt trong cách tối ưu, xử lý; hay sự khác biệt về quyền trong thao tác dữ liệu

==============================================================
# MediatR
* -> make implement the **`Mediator pattern`** easier; allow to conform **`CQRS pattern`** more easily