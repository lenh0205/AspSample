==============================================================================
# Python
* -> data types: Numbers (int, float, complex), Strings (immutable), List/Tuple/Set, Dictionary
* -> OOP: Inheritance, Encapsulation, Polymorphism

## Module
* -> is simply **`a python file (.py)`** that contains functions, classes, and variables
* -> helps organize code into reusable parts

```py
# create "math_operations.py" as a 'module'
def add(a, b):
    return a + b

def multiply(a, b):
    return a * b


# use the module in another python script:
import math_operations

result = math_operations.add(5, 3)
print(result)  # Output: 8
```

## Packages
* -> is a **`collection of modules inside a directory`** that contains an **`__init__.py`** file to makes it easier to **organize related modules**
* -> _in Python 3.3 and later versions, the __init__.py file is not required to define a package; if a directory contains modules but lacks an __init__.py file, it's still considered a package_

```py
# create "utilities" directory as a 'package'
utilities/  
│-- __init__.py  (makes it a package)  
│-- math_operations.py  
│-- string_operations.py

# inside "string_operations.py" module of "utilities" package
def to_uppercase(text):
    return text.upper()

# import from the package
from utilities import math_operations, string_operations

print(math_operations.add(2, 3))  # Output: 5
print(string_operations.to_uppercase("hello"))  # Output: HELLO
```

## venv (Virtual Environment) and 'requirements.txt' file
* -> Virtual Environment - is an **`isolated Python environment`** where we can **install dependencies without affecting the system-wide Python installation**
* _nhớ là sau khi tạo Virtual Enviroment rồi thì ta cần activate nó, để khi cài đặt package nó sẽ chỉ ở trong venv nếu không nó sẽ được cài đặt global_
* 
* -> requirements.txt - is a text file được đặt ở root của project, **lists all the dependencies (packages) required for a Python project**
* => mỗi lần tạo mới 1 project ta nên tạo mới 1 Virtual Enviroment để tránh, hay nói cách khác "requirements.txt" được dùng để **managing dependencies in a virtual environment**

```bash
# navigate to project
cd my_project

# create a virtual environment - tạo thư mục tên "myenv" tai root project
$ python -m venv myenv

# activate the environment 
$ myenv/bin/activate # in Window
$ source myenv/bin/activate # in Linux

# deactivate the virtual enviroment
deactivate
```

```bash
# creating requirements.txt from an existing virtual environment
# Ex: if we have installed packages inside a virtual environment, we can generate "requirements.txt"
$ pip freeze > requirements.txt

# using requirements.txt to install dependencies in a new environment
# Ex: if another developer (or you on another machine) wants to install the same dependencies
$ pip install -r requirements.txt
```

```txt
// content of "requirements.txt" file
requests==2.31.0
Flask==2.2.3
numpy==1.24.2
```

## Package Management
* -> Python uses **`pip`** to install, remove, and manage packages from the **Python Package Index (PyPI)**
* -> trong "C:\Users\Default\AppData\Local\Programs\Python\Python38-32\Scripts" ta có thể có nhiều pip khác nhau như pip.exe, pip3.exe, pip3.8exe; nhưng nếu ta thử check version (e.g., pip --version) thì nhiều khi chúng như nhau

```bash
# Install a package:
$ pip install requests

# List installed packages:
$ pip list

# Uninstall a package:
$ pip uninstall requests

# Freeze dependencies into a file:
$ pip freeze > requirements.txt

# Install dependencies from a file:
$ pip install -r requirements.txt
```

==============================================================================
# Data visualization - Matplotlib & Seaborn

==============================================================================
# Installation - Cài python runtime rồi những kiểm tra thì Not Found
* -> tình huống là sau khi cài đặt xong, ta vào thử cmd gõ "python" để check xem install success chưa nhưng nó báo "Pythong not found"
* -> check lại đường dẫn ta chọn để install lúc chạy setup wizard
* -> vô đường dẫn đó xem có file file python.exe không và chạy nó lên xem có code được không
* -> nếu được thì rất có thể là do **PATH** Enviroment variable đang trỏ đến 1 thư mục khác cũng chưa file "python.exe", nhưng file này không chạy được
* -> vào Enviroment variable -> trong "System variables" + click chọn "PATH" -> bấm "Edit" -> xóa đường dẫn đang trỏ sai, thêm đường dẫn đúng vào
* -> mở cmd -> chạy lại "python" thử 

# Lỗi khi install package
* -> rất có thể do phiên bản chưa phù hợp để cài package này, ta có thể update pip version (hoặc tải python version cao hơn, nó sẽ có pip version cao hơn)
```bash
$ python -m pip install --upgrade pip
```

# Đã install package nhưng không xài được
* -> ví dụ "pip install jupyter" nhưng khi chạy "jupyter notebook" thì lại không được 
* -> đây có thể là do không tìm thấy "jupyter.exe" trong "PATH" enviroment variable
* -> ta cần đọc lại log trong quá trình install xem nó được install vô đâu và thêm path đó vào "PATH" là xong 
