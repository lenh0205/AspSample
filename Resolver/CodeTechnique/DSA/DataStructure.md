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