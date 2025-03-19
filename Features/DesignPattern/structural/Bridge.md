
# Bridge
* -> split a large class or a set of closely related classes into two separate hierarchies—abstraction and implementation—which can be developed independently of each other
* => tập trung xử lý vấn đề hierachy khi inherit bằng cách tách nhỏ thành từng hierachy riêng

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
* -> The abstraction object controls the appearance of the app, delegating the actual work to the linked implementation object. Different implementations are interchangeable as long as they follow a common interface, enabling the same GUI to work under Windows and Linux.

* -> As a result, you can change the GUI classes without touching the API-related classes. Moreover, adding support for another operating system only requires creating a subclass in the implementation hierarchy
