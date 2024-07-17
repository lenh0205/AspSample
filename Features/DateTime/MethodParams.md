===============================================================================
# Note
* -> if not specific the **`IFormatProvider`** in parsing method; then it will use the **culture associated with the current thread**
* -> if **`the CultureInfo associated with the current culture can't parse the input string`**, it **might throw a FormatException**
* -> we can specific one of the standard **DateTimeFormatInfo** objects returned by the **CultureInfo.DateTimeFormat** property to **`precludes whatever setting`** is in the **CurrentCulture** of the **CurrentThread**

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


