===============================================================================
# Note
* -> if not specific the **`IFormatProvider`** in parsing method; then it will use the **culture associated with the current thread**
* => if **`the CultureInfo associated with the current culture can't parse the input string`**, it **might throw a FormatException**
* -> for more clarity, we can **`explicitly define the culture`** whose formatting conventions are currently using by date string when we use parsing method
* -> to do that, we can specific one of the standard **DateTimeFormatInfo** objects returned by the **CultureInfo.DateTimeFormat** property 
* => this will **`precludes whatever setting`** is in the **CurrentCulture** of the **CurrentThread**
```cs
var cultureInfo = new CultureInfo("de-DE"); 
var dateTime = DateTime.Parse(dateString, cultureInfo);
```

* -> **Parse()** overloading method support specifying **custom format providers**; however, it **doesn't support parsing `non-standard` formats**
* -> to **`parse a date and time expressed in a non-standard format`**, use the **ParseExact** method instead
* _tức là `DateTime.Parse` cùng lắm là cho ta custom format provider để làm những thứ như `DateTimeStyles.NoCurrentDateDefault, ...` nhưng vẫn format của nó vẫn phải theo tiêu chuẩn_

===============================================================================
# IFormatProvider - parameter of parsing method
* -> if we want **a specific culture or custom settings**, we specify **`the IFormatProvider parameter`** of **`a parsing method`**
* -> _for the `IFormatProvider` parameter_, specify a **CultureInfo** object or a **DateTimeFormatInfo** object

* -> **`the format provider`** is also used to **interpret an ambiguous numeric date**
* -> _Ex: it's unclear which components of the date represented by the string "02/03/04" are the month, day, and year_
* -> the components are **`interpreted according to the order of similar date formats`** in the format provider

## DateTimeFormatInfo
* -> **`the current DateTimeFormatInfo object`** provides more control over how text should be interpreted as a date and time
* -> **`properties of a DateTimeFormatInfo`** describe the **date and time separators, the names of months, days, and eras, and the format for the "AM" and "PM" designations**

## CultureInfo
* -> the **`CultureInfo`** returned by **CultureInfo.CurrentCulture** has a **CultureInfo.DateTimeFormat** property 
* -> this property is a **DateTimeFormatInfo** object that **`represents the current culture`**

===============================================================================
# 'DateTimeKind' Enum
* -> specifies whether **`a DateTime object`** represents a **local time**, a **Coordinated Universal Time (UTC)**, or is **not specified as either local time or UTC**
* -> used in **`conversion operations between local time and Coordinated Universal Time (UTC)`**

```cs
var kind0 = DateTimeKind.Unspecified; // Output: Unspecified
var kind1 = DateTimeKind.Utc; // Output: Utc
var kind2 = DateTimeKind.Local; // Output: Local

var dt = DateTime.Now;
DateTimeKind kind = dt.Kind; // Output: Local
```
