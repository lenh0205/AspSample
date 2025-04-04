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
* -> requirements.txt - is a text file được đặt ở root của project, **lists all the dependencies (packages) required for a Python project**
* => mỗi lần tạo mới 1 project ta nên tạo mới 1 Virtual Enviroment để tránh, hay nói cách khác "requirements.txt" được dùng để **managing dependencies in a virtual environment**

```bash
# navigate to project
cd my_project

# create a virtual environment
$ python -m venv myenv

# activate the environment in Linux
$ source myenv/bin/activate

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
# Numby
* -> provides support for large, multi-dimensional arrays and matrices, along with mathematical functions to offer fast, optimized array operations that enhances performance in numerical computing

Key features of NumPy:
● N-dimensional Array Object: NumPy is a multi-dimensional array that can store elements of the
same data type. These arrays are allowed for vectorized operations.
● Array Operations: NumPy offers a vast array of mathematical operations that can be performed
directly on arrays without the need for explicit looping, known as vectorization.
● Broadcasting: NumPy 's broadcasting capability allows arrays with different shapes to be
combined in arithmetic operations.
● Integration with other Libraries: NumPy is often used in conjunction with other libraries like
SciPy (scientific computing), Matplotlib (plotting), Pandas (data manipulation), and others, forming
the basis of the scientific Python ecosystem.

# Pandas
* -> build on top of Numby; provides flexible data structures for structured data manipulation and analysis efficiently

Key Features of Pandas:
● DataFrame Object: The core of Pandas is the DataFrame, a two-dimensional labeled data
structure similar to a table or spreadsheet. It consists of rows and columns, allowing easy
manipulation and analysis of data.
● Data Exploration and Analysis: It offers powerful methods for grouping, aggregating, sorting,
filtering, and analyzing data.
● Input/Output Operations: Pandas supports reading and writing data from various file formats
such as CSV, Excel, SQL databases, JSON, and more.
● Integration with Other Libraries: Pandas integrates well with other Python libraries like NumPy,
Matplotlib, and Scikit-learn, forming a robust ecosystem for data analysis and machine learning.

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
