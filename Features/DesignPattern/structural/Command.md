
# Command


## Problem
```cs
// Ex: toolbar has a bunch of buttons look similar, they're all supposed to do different things (cùng là 1 "click" event nhưng lại có những logic xử lý khác nhau)
// where would we put the code for the various click handlers of these buttons? the simplest solution is to create tons of subclasses for each place where the button is used
```

* _có 2 vấn đề ở đây_
* -> khi ta có hàng tá subclasses "Button" cho mỗi trường hợp, nghĩa là code trong những subclasses này rất có thể breaking khi ta modify base class - nghĩa là GUI sẽ phải phụ thuộc vào những business logic code rất dễ bị thay đổi này
* -> thứ 2 - the ugliest part - là có những components thực hiện những behavior giống nhau, nghĩa là ta phải duplicate code cho mỗi subclasses này; hoặc ý tưởng tệ hơn là cho tất cả components phụ thuộc vào component duy nhất thực hiện chức năng đó

## Solution     
* -> good software design is often based on the principle of **`separation of concerns`**, which usually results in **`breaking an app into layers`**
* => the most common example: a layer for the graphical user interface and another layer for the business logic (_tức là ta không để business logic rải rác trong các subclasses của button, mà nhiệm vụ chính của những subclasses này là show các UI khác nhau ở từng nơi khác nhau_)
* => in the code it might look like this: a GUI object calls a method of a business logic object, passing it some arguments - this process is usually described as **`one object sending another a request`**

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
* => **parameters required to execute a method on a receiving object** (những tham số của cái method của business logic mà thằng Command cần gọi) can be declared as **`fields in the concrete command`**;
* => we can make command objects **`immutable`** by only allowing the initialization of these fields via the constructor.
* => the **`Client creates and configures concrete command objects`** (_Ex: Command1(receiver, params)_) - it must must pass all of the **`request parameters`**, including **`a receiver instance`**, into the **`command's constructor`**
* => most commands only handle the details of how a request is passed to the receiver, while the receiver itself does the actual work; however, for the sake of simplifying the code, **these classes can be merged**

* -> after we apply the Command pattern, we are no longer need all the subclasses to implement various "click" behaviors; put **`a single field`** into the **`base class`** that **`stores a reference to a command object`** and **execute that command** on a "click"
* -> we'll **`implement a bunch of command classes for every possible operation`** and **`link them with particular "buttons"`** (_depending on the buttons'intended behavior_)
* -> the sender isn't responsible for creating the command object; usually, it gets **`a pre-created command from the client via the constructor`**
* => **`the elements related to the same operations will be linked to the same commands`**, preventing any code duplication
* => "commands" become **a convenient middle layer** that reduces coupling between the GUI and business logic layers.

## Classic Example
* -> the Command pattern helps to track the history of executed operations and makes it possible to revert an operation if needed
* -> although there are many ways to implement undo/redo, the Command pattern is perhaps the most popular of all.
