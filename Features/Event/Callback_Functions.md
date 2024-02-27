
# Callback Function
* -> _a callback function_ is **`code within a managed application`** that **helps an unmanaged DLL function complete a task**
* -> **`calls to a callback function`** pass indirectly **`from a managed application`**, **through a DLL function**, and **`back to the managed implementation`** 
* -> some of the many DLL functions called with platform invoke **`require a callback function in managed code`** to run properly

* => Callback functions are ideal for use in situations in which **a task is performed repeatedly** 
* => another common usage is with **enumeration functions**, such as **`EnumFontFamilies`**, **`EnumPrinters`**, and **`EnumWindows`** in the Windows API

* the **EnumWindows** function **enumerates through all existing windows** on our computer, **`calling the callback function`** to **perform a task on each window**

## Mechanism
* to **`call most DLL functions from managed code`**:
* -> we **create a managed definition of the function** and then **call it** - the process is straightforward

* **`using a DLL function that requires a callback function`** has some additional steps
* -> first, we must **determine whether the function requires a callback** by looking at **`the documentation for the function`**
* -> next, we have to **`create the callback function`** **in our managed application**
* -> finally, we **call the DLL function**, passing a pointer to the callback function as an argument