
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
# Linear Algebra (đại số tuyến tính)

## Vectors
* -> is defined under the operation of summation and the multiplication by a scalar

```py
import numpy as np

x = np.array([1, 2, 5, 8, 9])
x = np.array([1, 2, 4, 6, 7])
print("x + y: ", x + y) 
print("x - y: ", x + y)
print("x * y: ", x + y)
```
