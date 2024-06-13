# String to DateTime Conversion
* -> có **`5 built-in methods`** luôn được sử dụng: **Convert.ToDateTime()**, **DateTime.Parse()**, **DateTime.ParseExact()**, **DateTime.TryParse()**, and **DateTime.TryParseExact()**
* => _vậy câu hỏi là `Tại sao lại có nhiều method để parse a string to DateTime object như vậy?` và `khi nào thì dùng cái nào ?`_

## Ví dụ các valid date mà chắc chắn là parse thành công
```r
// "1/1/2010"
// "01/10/2015"

```

## Convert.ToDateTime()
* -> converts **`specified string data`** to _equivalent date and time_
* -> it contains a couple of overload methods, but **`2 most important`**: **ToDateTime(string value)** and **oDateTime(string value, IFormatProvider provider)**

* -> **value**: **`a string representation of date and time`**; 
* -> **provider**: an object which provides **`culture-specific info`**

* -> if the **`string value is not null`**, then it **internally calls 'DateTime.Parse()'** to give the result
* -> if the **`string value is null`**, then it **gives 'DateTime.MinValue' as "1/1/0001 12:00:00 AM"**

* => this method always tries to **`parse the value completely`** and **avoid the FormatException issue**

```cs
// "en-US" is cultural information about the United States of America
CultureInfo culture = new CultureInfo("en-US"); 
DateTime tempDate = Convert.ToDateTime("1/1/2010 12:10:15 PM", culture);

string dateString = null;
var dateTime = Convert.ToDateTime(dateString); // Output: 1/1/0001 12:00:00 AM

string dateString = "not a date"; // Hoàn toàn không thể parse
var dateTime = Convert.ToDateTime(dateString); 
// Exception: The string was not recognized as a valid DateTime (There is an unknown word starting at index 0)

string dateString = "Tue Dec 30, 2015"; // Có thể parse nếu sửa lại 1 xíu
DateTime dateTime12 = Convert.ToDateTime(dateString);
// Exception: String was not recognized as a valid DateTime (because the day of week was incorrect)
```

## DateTime.Parse()
* -> converts **`specified string data`** to _equivalent date and time_
* -> **`overload methods`**: **DateTime.Parse(String value)**, **DateTime.Parse(String value, IFormatProvider provider)**, **DateTime.Parse(String value, IFormatProvider provider, DateTypeStyles styles)**

* -> **value** - **`a string representation of date and time`**
* -> **provider** - an object which provides **`culture-specific info`**
* -> **styles** - defines **`the formatting options`** that **customize string parsing** for _some date and time parsing methods_ (_Ex: **AllowWhiteSpaces** helps **`ignore all spaces`** in the string while it parses_)

* -> if the **`string value is null`**, it **throw 'ArgumentNullException'**
* -> if the **`string value contains some invalid date format`**, it **throw 'FormatException'**

```cs
string dateString = null;
DateTime dateTime = DateTime.Parse(dateString);
// Exception: Argument null exception

string dateString = "not a date";
DateTime dateTime = DateTime.Parse(dateString); 
// Exception: The string was not recognized as a valid DateTime (there is an unknown word starting at index 0)

string dateString = "Tue Dec 30, 2015";
DateTime dateTime = DateTime.Parse(dateString);
// Exception: String was not recognized as a valid DateTime (because the day of week was incorrect)
```

## DateTime.ParseExact()
* -> converts **`a specified string`** to _an equivalent DateTime_ with a **specified format** and **culture**; 
* -> the **`format's string value`** **must match a string value of datetime**
* -> overload methods: **DateTime.ParseExact(string value, string format, IFormatProvider provider)**, **DateTime.ParseExact(string value, string format, IFormatProvider provider, DateTimeStyles style)**, **DateTime.ParseExact(string value, string[] formats, IFormatProvider provider, DateTimeStyles style)**

* -> **value** - **`a string representation of date and time`**
* -> **format** - **a format specifier** that defines **`what a date looks like`** **after conversion**
* -> **formats** - (like "format" but) a string array contains **`a list of formats`**; **at least one format must match the string(value)** to convert the DateTime object
* -> **provider** - an object which specifies **`cultural info`**
* -> **style** - defines the **`formatting options`** that **customize string parsing** for some date and time parsing methods

* -> if **`the value is null`**, it **throw ArgumentNullException**
* -> if the **`value contains some invalid date format`**, it **throw 'FormatException'**
* => to overcome this issue, we can the **formats** array with some possibilities; however, the `format` **must match with string date time**, or it **`throws FormatException`**
* _Ex: suppose our date might be in a format `"12/12/2015" or "12-12-2015"` here; to parse it success in both situation we need to pass a string array with a format like `"MM/dd/yyyy" and "MM-dd-yyyy"`_

```cs
CultureInfo provider = CultureInfo.InvariantCulture;

string dateString = null;
DateTime dateTime = DateTime.ParseExact(dateString, "mm/dd/yyyy", provider);
// Exception: Argument null exception

string dateString = "not a date";
DateTime dateTime = DateTime.ParseExact(dateString, "mm/dd/yyyy", provider);
// Exception: The string was not recognized as a valid DateTime (there is an unknown word starting at index 0)

string dateString = "Tue Dec 30, 2015";
DateTime dateTime = DateTime.ParseExact(dateString, "mm/dd/yyyy", provider);
// Exception: String was not recognized as a valid DateTime (because the day of week was incorrect)

string dateString = "10-22-2015";
DateTime dateTime = DateTime.ParseExact(dateString, "MM-dd-yyyy", provider);
string temp = dateTime.ToString(); // Output: 10/22/2015 12:00:00 AM

string dateString = "10-12-2015";
DateTime dateTime = DateTime.ParseExact(
    dateString, 
    new string[] { "MM.dd.yyyy", "MM-dd-yyyy", "MM/dd/yyyy" }, 
    provider, 
    DateTimeStyles.None
);
string temp = dateTime.ToString(); // Output: 10/22/2015 12:00:00 AM
```

## DateTime.TryParse()
* -> **`converts specified string data to equivalent datetime`** and **always returns the Boolean value** after parsing - _indicating that parsing has succeeded or fail_ 
* _it like **`DateTime.Parse()`**, but doesn't throw any exception when the conversion fails_
* -> overload methods: **DateTime.TryParse (String value, out DateTime result)**, **DateTime.TryParse(String value, IFormatProvider provider, DateTimeStyles styles, out DateTime result)**

* -> **value** - **`a string representation of date and time`**
* -> **provider** - an object which provides **`culture-specific info`**
* -> **styles** - defines the **`formatting options`** that customize string parsing for some date and time parsing methods
* -> **result** - **`holds the DateTime value after parsing`**

* -> TryParse() **always tries to parse the string value datetime** 
* -> if **`the conversion fails`** or **`the string value is null or empty`**, it **returns MinValue(1/1/0001 12:00:00 AM)**

## DateTime.TryParseExact()
* -> converts **`a specified string`** to an equivalent DateTime with **a specified format** and **culture**; the `format's string value` **must match a string value of datetime**
* -> methods overloading: **DateTime.ParseExact(string value, string format, IFormatProvider provider, DateTimeStyles style)**, **DateTime.ParseExact(string value, string[] formats, IFormatProvider provider, DateTimeStyles style)**

* -> **value** - a string representation of date and time.
* -> **format** - a format specifier that defines what a date looks like after conversion.
* -> **formats** - (like "format" but) a string array contains **`a list of formats`**, and **at least one format must match the string (value)** to convert the DateTime object
* -> **provider** - an object which specifies **`cultural info`**
* -> **style** - defines **`the formatting options`** that **customize string parsing** for some date and time parsing methods

* -> if **`the value is null / empty / incorrect date / not matched with the 'format' provided`**

https://www.c-sharpcorner.com/UploadFile/manas1/string-to-datetime-conversion-in-C-Sharp/
