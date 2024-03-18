# Bitwise Operators
* **&** - **`AND`**: _sets each bit to 1 if both bits are 1_
* **|**	- **`OR`**:	_sets each bit to 1 if one of two bits is 1_
* **^** - **`XOR`**: _sets each bit to 1 if only one of two bits is 1_
* **~** - **`NOT`**: _Inverts all the bits_
* **<<** - **`Zero fill left shift`**: _shifts left by pushing zeros in from the right and let the leftmost bits fall off_
* **>>** - **`signed right shift`**: _shifts right by pushing copies of the leftmost bit in from the left, and let the rightmost bits fall off_
* **>>>** -	**`Zero fill right shift`**: _shifts right by pushing zeros in from the left, and let the rightmost bits fall off_

## Note:
* **`JavaScript`** **stores numbers as 64 bits** floating point numbers, but all **`bitwise operations`** are performed on **32 bits binary numbers**
* -> before a bitwise operation is performed, JavaScript converts numbers to 32 bits signed integers.
* -> after the bitwise operation is performed, the result is converted back to 64 bits JavaScript numbers

```r - "NOT" bitwise operator
// if using 4 bits unsigned binary numbers, the result of "~ 5" returns 10 (~0101 = 1010)
// but since JavaScript uses 32 bits signed integers, it will not return 10. It will return -6.

00000000000000000000000000000101 (5)
11111111111111111111111111111010 (~5 = -6)
```

## Example
```js
5 <=> 00000000000000000000000000000101
1 <=> 00000000000000000000000000000001

5 & 1 <=> 

5 | 1 <=>

5 ^ 1 <=>

~5	<=> 11111111111111111111111111111010

5 << 2 <=> 00000000000000000000000000010100
// chèn thêm 2 số "0" vô bên phải (bỏ 2 số "0" bên trái đi)

-5 <=> 11111111111111111111111111111011
-5 >> 1 <=>	11111111111111111111111111111101 (-3)
// lấy bit bên trái ngoài cùng ("1" in this case) chèn vô bên trái (bỏ số "1" ở cuối đi)

5 >>> 2 <=>	00000000000000000000000000000001 (1)
// chèn thêm  2 số "0" vô bên phải (bỏ số "01" bên phải đi)  
```

## Binary number
```js
00000000000000000000000000000001 <=> 1
00000000000000000000000000000100 <=> 4
00000000000000000000000000000101 <=> 5 = 4 + 1
```

* **`a negative number`** is the **bitwise NOT of the number plus 1**
```js
00000000000000000000000000000101 <=> 5
11111111111111111111111111111010 <=> ~5 = -6
11111111111111111111111111111011 <=> -5
```

## Converting Decimal to Binary, Binary to Decimal
```js
// dec2bin
const output = (dec >>> 0).toString(2);

// bin2dec
const output = parseInt(bin, 2).toString(10);
```