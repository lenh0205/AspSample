
# Bridge
* -> split a large class or a set of closely related classes into **`two separate hierarchies - "abstraction" and "implementation"`** - which can be developed independently of each other
* => tập trung xử lý vấn đề hierachy khi inheritance bằng cách tách nhỏ thành từng hierachy riêng và hierachy này sẽ reference đến hierachy kia
* => **`Abstraction`** don't do any real work on its own, it delegate the work to **`Implementation`** interface

## Problem
* -> the **`inheritance hierarchy will grow exponentially`** is a common issue when we try to **`extend class in multiple different dimensions`**

```bash
$ Example: try to extend a "Shape" by form (like Cricle, Square) and by color (like Red, Blue)
$ will produce subclasses like "RedCircle", "RedSquare", "BlueCircle", "BlueSquare"
```

## Solution
* -> the Bridge pattern attempts to solve this problem by switching from inheritance to the object composition
* -> means is that we extract one of the dimensions into a separate class hierarchy, so that the original classes will reference an object of the new hierarchy
* -> instead of having all of its state and behaviors within one class

```cs
$ Trong trường hợp này ta không cần phải tạo 1 lớp như "RedCircle" để xử lý logic liên quan đến "Red" và "Circle"
$ Thay vào đó ta tạo 2 hierachy riêng biệt: 1 là "Shape" class sẽ có subclassses như "Circle", "Square"; 2 là "Color" sẽ có subclasses như "Red", "Blue" 
$ "Circle" chỉ cần có một "reference field" pointing to "Red" object và delegate any actual work (color-related) to the linked color object "Red"
$ => that reference will act as a "Bridge" between the "Abstraction" (Shape) and "Implementation" (Color)
```

## Example
```bash
$ Problem
When talking about real applications, the abstraction can be represented by a graphical user interface (GUI), and the implementation could be the underlying operating system code (API) which the GUI layer calls in response to user interactions
you can extend such an app in two independent directions:
Have several different GUIs (for instance, tailored for regular customers or admins).
Support several different APIs (for example, to be able to launch the app under Windows, Linux, and macOS).
In a worst-case scenario, this app might look like a giant spaghetti bowl, where hundreds of conditionals connect different types of GUI with various APIs all over the code.

Solution 1:
You can bring order to this chaos by extracting the code related to specific interface-platform combinations into separate classes. However, soon you’ll discover that there are lots of these classes. The class hierarchy will grow exponentially because adding a new GUI or supporting a different API would require creating more and more classes.

Solution 2:
Bridge pattern suggests that we divide the classes into two hierarchies: Abstraction: the GUI layer of the app, Implementation: the operating systems’ APIs
* -> The abstraction object controls the appearance of the app, delegating the actual work to the linked implementation object. Different implementations are interchangeable as long as they follow a common interface, enabling the same GUI to work under Windows and Linux.
* -> As a result, you can change the GUI classes without touching the API-related classes. Moreover, adding support for another operating system only requires creating a subclass in the implementation hierarchy
```
