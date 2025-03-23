
# Singleton
* -> ensure that **`a class has only one instance`**, while **`providing a global access point to this instance`** (_đây là 2 đặc điểm tối quan trọng để để gọi thứ gì đó là **singleton**_)
* => the most common reason for this is to **control access to some shared resource** - for example, a database or a file
* => nó sẽ giống global variables là cho phép **access some object from anywhere in the program**; nhưng **protects that instance from being overwritten by other code**

## To archieve 'Singleton' ?
* -> make the **`default constructor private`**, to **prevent other objects from using the `new` operator with the Singleton class**
* -> create **`a static creation method that acts as a constructor`** - under the hood, this method **calls the private constructor to create an object** and **saves it in a static field**
* => all following calls to this method return the **`cached object`** - this is also the the **only way of getting the Singleton object**
