===============================================================================
# Rules
* -> if the **date** is present in the string, it **`must include the month and one of the day or year`**
* -> if the **time** is present, it **`must include the hour, and either the minutes or the AM/PM designator`**

* -> we can specify the **NoCurrentDateDefault** constant to **`override these defaults`**
* -> when we **`use that constant, any missing year, month, or day properties`** are **set to the value 1**. The last example using Parse demonstrates this behavior.

===============================================================================
# DateTime string missing infomation:
* _`Parsing methods` handle text representing a date or time that is missing some information by **`using reasonable defaults`**:_
* -> when **only the time is present**, the date portion **`uses the current date`**
* -> when **only the date is present**, the time portion is **`midnight`**
* -> when the **year isn't specified in a date**, the **`current year`** is used
* -> when the **day of the month isn't specified**, the **`first day of the month`** is used

```r - For example
// most people would assume the date "March 12" represents the current year
// Similarly, "March 2018" represents the month of March in the year 2018
// text representing time often does only include "hours, minutes, and an AM/PM designation"
```

===============================================================================
# Offset
* -> in addition to **`a date`** and **`a time`** component, the **`string representation`** of a date and time can include an **offset**
* -> **indicates how much the time differs from `UTC` (Coordinated Universal Time)**

```r - For example:
 the string "2/14/2007 5:32:00 -7:00" defines a time that is "seven hours earlier than UTC"
``` 

* -> if an **offset is omitted** from the string representation of a time, **`parsing returns`** a DateTime object with its **Kind** property set to **DateTimeKind.Unspecified**
* -> if an **offset is specified**, **`parsing returns`** a DateTime object with its **Kind** property set to **DateTimeKind.Local** - its **`value`** is also adjusted to the **local time zone of our machine**
* _we can modify this behavior by using a **DateTimeStyles** value with the parsing method_

===============================================================================
# UTC