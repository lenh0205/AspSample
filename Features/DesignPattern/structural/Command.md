
# Command


## Problem
* _có 2 vấn đề ở đây_
* -> ta phải tạo hàng tá subclasses "Button" cho mỗi trường hợp, nghĩa là code trong những subclasses này rất có thể breaking khi ta modify base class - nghĩa là GUI sẽ phải phụ thuộc vào những business logic code rất dễ bị thay đổi này
* -> thứ 2 là có những components thực hiện những behavior giống nhau, nghĩa là ta phải duplicate logic cho mỗi subclasses này; hoặc ý tưởng tệ hơn là cho tất cả components phụ thuộc vào component duy nhất thực hiện chức năng đó

## Solution     
* -> good software design is often based on the principle of **`separation of concerns`**, which usually results in **`breaking an app into layers`**
* => the most common example: a layer for the graphical user interface and another layer for the business logic
* => a GUI object calls a method of a business logic object, passing it some arguments - this process is usually described as **`one object sending another a request`**

* -> **Command** pattern suggests that GUI objects shouldn't send these requests directly
* -> the GUI object doesn't need to know what business logic object will receive the request and how it'll be processed
* => the GUI object **just triggers the command, which handles all the details**
* => Command objects serve as **links** between various GUI and business logic objects
* => we will **`extract all of the request details`** (_such as the object being called, the name of the method and the list of arguments_) into **`a separate 'command' class`** with **`a single method that triggers this request`**

## Implementation
* -> make our **`commands implement the same interface`**, usually it has **`just a single execution method that takes no parameters`**
* => this interface lets us **use various commands with the same request sender, without `coupling` it to concrete classes of commands**
* => as a bonus, now we can switch command objects linked to the sender, effectively **changing the sender's behavior at runtime**

* -> so how would we pass the request details to the receiver? it turns out the command should be either **`pre-configured with this data, or capable of getting it on its own`**
* => **parameters required to execute a method on a receiving object** can be declared as **`fields in the concrete command`**;
* => must pass all of the **`request parameters`**, including **`a receiver instance`**, into the **`command's constructor`** - most commands only handle the details of how a request is passed to the receiver, while the receiver itself does the actual work
* => we can make command objects **`immutable`** by only allowing the initialization of these fields via the constructor.

* -> first, we are no longer need all the subclasses; put **`a single field`** into the **`base class`** that **`stores a reference to a command object`** and execute that command when needed
* => Ex: we'll implement **a bunch of command classes for every possible operation** and link them with particular buttons, depending on the buttons' intended behavior
* => **the elements related to the same operations will be linked to the `same commands`**, preventing any code duplication.
