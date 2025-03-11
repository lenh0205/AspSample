# Array
If M is too big, then the unused spaces are wasted.
If M is too small, then we will run out of space easily
* => implements this doubling-when-full strategy - So when the array is full, we create a larger array (usually two times larger) and move the elements from the old array to the new array
* => However, the classic array-based issues of space wastage and copying/shifting items overhead are still problematic

## Các phương thức tiêu biểu

* -> get(i), just return A[i]
* Just one access, O(1)
```cs
if empty, do nothing
return A[k]

// Select "min" from sorted array
if empty, do nothing
return A[0]

// Select max from sorted array
if empty, do nothing
return A[lenFilled - 1]
```


* -> search(v), we check each index i ∈ [0..N-1] one by one to see if A[i] == v
*  O(1) -> O(N)
```cs

```

* -> insert(i, v), we shift items ∈ [i..N-1] to [i+1..N] (from backwards) and set A[i] = v
*  O(1) -> O(N)
```cs
if (lenFilled === lenArray)

  double the size of A

A[lenFilled] = v

for (i = lenFilled - 1; i >= 0; i--)

  if (A[i] < A[i + 1]): break

  else: swap(A[i], A[i + 1])

lenFilled++
```

* -> remove(i), we shift items ∈ [i+1..N-1] to [i..N-2], overwriting the old A[i]
*  O(1) -> O(N)
```cs
if empty, do nothing

remove A[i]

for (j = i + 1; j < lenFilled; j++)

  A[j - 1] = A[j]

lenFilled--
```

# Application
* -> Searching for a specific value v in array A,
* -> Finding the min/max or the k-th smallest/largest value in (static) array A,
* -> Testing for uniqueness and deleting duplicates in array A,
* -> Counting how many time a specific value v appear in array A,
* -> Set intersection/union between array A and another sorted array B,
* -> Finding a target pair x ∈ A and y ∈ A such that x+y equals to a target z,
* -> Counting how many values in array A is inside range [lo..hi], etc.

## for unsorted Array
* -> We can use O(N) linear search (leftmost to rightmost or vice versa) to find v,
* -> For min/max, we can use O(N) linear search again; for k-th smallest/largest, we may need to use O(kN) algorithm1,
* -> We can use O(N^2) nested-loop to see if any two indices in A are the same,
* -> We may need to use Hash Table to do this in O(N),
* -> O(N^2) nested-loop is needed,
* -> O(N^2) nested-loop is needed,
* -> We may need to use Hash Table to do this in O(N)

## for sorted Array
* -> We can use O(log N) binary search on a sorted array,
* -> A[0]/A[k-1]/A[N-k]/A[N-1] are the min/k-th smallest/k-th largest/max value in (static sorted) array A,
* -> Duplicates, if any, will be next to each other in a sorted array A,
* -> Same as above,
* -> We can use modifications of merge routine of Merge Sort,
* -> We can use two pointers method,
* -> The index of y - the index of x + 1 (use two binary searches).

# Dynamic Array
Approach: When we enter an element in array but array is full then you create a function, this function creates a new array double size or as you wish and copy all element from the previous array to a new array and return this new array. Also, we can reduce the size of the array. and add an element at a given position, remove the element at the end default and at the position also.

Key Features of Dynamic Array
Add Element: Add element at the end if the array size is not enough then extend the size of the array and add an element at the end of the original array as well as given index. Doing all that copying takes O(n) time, where n is the number of elements in our array. That’s an expensive cost for an append. In a fixed-length array, appends only take O(1) time. But appends take O(n) time only when we insert into a full array. And that is pretty rare, especially if we double the size of the array every time we run out of space. So in most cases appending is still O(1) time, and sometimes it’s O(n) time. In dynamic array you can create fixed-size array when required added some more element in array then use this approach:
