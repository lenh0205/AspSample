=======================================================================
> Phân tích cài này: https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-date-and-time-format-strings

## "Standard date and time format string" and "Custom date and time format string"
* -> _`a standard date and time format string`_ uses a single character as the **format specifier** to define the text representation of **`a 'DateTime' or a 'DateTimeOffset' value`**
* -> _`a custom date and time format string`_ - any date and time format string that **contains more than one character, including white space**

## Table of "format specifiers" 
* -> hiện những **standard date and time format specifiers** - nói chung chúng là những **`alias`** đại diện cho những định dạng tiêu chuẩn, những định dạng này có thể khác nhau tuỳ **`culture`**
* -> vậy nên **in any cases** (_unless otherwise noted_), với 1 thằng format specifier cụ thể (trong bảng) thì nó sẽ **`luôn trả về cùng 1 chuỗi "string presentation"`** dù ta dùng với **`DateTime`** hoặc **`DateTimeOffset`**

```r - VD:
// nếu normal format là "2009-06-15T13:45:30"  
// thì với standard date and time format specifiers là "d" nó sẽ là "6/15/2009 (en-US)"
```

## How standard format strings work
* -> in a formatting operation, **`a standard format string`** is simply **an alias for a custom format string**
* -> the advantage of using an alias to refer to a custom format string is that, **`although the alias remains invariant, the custom format string itself can vary`**
* -> this is important because the **`string representations`** of date and time values **typically vary by culture**

```r - VD:
// the "d" standard format string use a "short date pattern"
// -> for "invariant culture", this pattern is "MM/dd/yyyy"
// -> but for "fr-FR culture", the pattern is "dd/MM/yyyy"
// -> and for "ja-JP culture", the pattern is "yyyy/MM/dd"
```
