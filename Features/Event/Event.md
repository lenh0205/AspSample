# Events
* -> enable **`a class or object`** to **notify other classes or objects** when **`something of interest occurs`**
* -> **`the class that sends/raises the event`** is called the **publisher** and **`the classes that receive/handle the event`** are called **subscribers** 

* in a typical C# Windows Forms or Web application, we subscribe to events raised by controls such as buttons and list boxes
* -> we can use **`the Visual C# integrated development environment (IDE)`** to **browse the events** that **`a control publishes`** and **select the ones** that **`we want to handle`**
* -> the IDE provides an easy way to automatically add an empty event handler method and the code to subscribe to the event

## Event Properties
* -> the **publisher** determines **`when an event is raised`**; the **subscribers** determine **`what action is taken in response to the event`**
* -> **`an event`** can have **multiple subscribers**; **`a subscriber`** can **handle multiple events from multiple publishers**
* -> **`Events`** that have **no subscribers** are **never raised**
* -> Events are typically used to **signal user actions** such as **`button clicks or menu selections`** in GUI - graphical user interfaces
* -> when **`an event has multiple subscribers`**, the event handlers are **invoked synchronously when an event is raised**
* -> to invoke **events asynchronously**, we need to **`call Synchronous Methods Asynchronously`**
* -> in the **`.NET class library`**, events are based on the **EventHandler delegate** and the **EventArgs base class**