
# Programming Language
* -> 

# Mathematics and Statistics
# Machine Learning Fundamentals
# Libraries and Tools
# Workshop: Project and Presentation

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
* -> requirements.txt - is a text file được đặt ở root của project, **lists all the dependencies (packages) required for a Python project**
* => mỗi lần tạo mới 1 project ta nên tạo mới 1 Virtual Enviroment để tránh, hay nói cách khác "requirements.txt" được dùng để **managing dependencies in a virtual environment**

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

```bash
# navigate to project
cd my_project

# create a virtual environment
$ python -m venv myenv

# activate the environment in Linux
# source myenv/bin/activate

# deactivate the virtual enviroment
deactivate
```

## Package Management
* -> Python uses **`pip`** to install, remove, and manage packages from the **Python Package Index (PyPI)**

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
