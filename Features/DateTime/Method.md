====================================================================
# Requirement
* -> để parse **`string`** thành **`DateTime`** object đòi hỏi ta cần **specific information about how the dates and times are represented as text**
* _`different cultures` use different orders for day, month, and year_
* _some `time representations` use a 24-hour clock, others specify "AM" and "PM"_
* _some applications `need only the date`; others need only the time; still others need to specify both the date and time_ 

* _so there are 3 subtasks to `correctly converting text into a DateTime`:_
* -> must **specify the expected format** of the text representing a date and time
* -> can **specify the culture** for the format of a date time.
* -> can **specify how missing components** in the text representation are set in the date and time

* => the methods that convert strings to DateTime objects enable us to **provide detailed information about the formats we expect** and **the elements of a date and time our application needs**

====================================================================
# String to DateTime Conversion
* -> có **`5 built-in methods`** luôn được sử dụng: **Convert.ToDateTime()**, **DateTime.Parse()**, **DateTime.TryParse()**, **DateTime.ParseExact()**, **DateTime.TryParseExact()**

* => sự khác biệt giữa _`Convert.ToDateTime`, `DateTime.Parse`, `DateTime.TryParse`_ nằm ở việc xử lý đối với **string input value is `null`** và **string input value is `invalid`**
* => sự khác biệt giữa _`DateTime.Parse`, `DateTime.ParseExact`_ nằm ở việc _`DateTime.ParseExact`_ cho phép ta truyền pattern của datetime string value () giúp việc **parse những invalid format trở nên khả thi**
* (_về cơ bản `DateTime.Parse` và `DateTime.TryParse` chỉ dùng để parse những common presentations of date and time, đọc `~/Features/DateTime` để hiểu rõ hơn_)

## Valid datetime format in any case
* -> Format should be **MM-dd-yyyy**

```r
// "1/1/2010"
// "01/10/2015"
```

====================================================================
## DateTime.Parse()
* -> converts **`specified string data`** to _equivalent date and time_
* -> **`overload methods`**: **DateTime.Parse(String value)**, **DateTime.Parse(String value, IFormatProvider provider)**, **DateTime.Parse(String value, IFormatProvider provider, DateTypeStyles styles)**

* -> **value** - **`a string representation of date and time`**
* -> **provider** - an object which provides **`culture-specific info`**
* -> **styles** - defines **`the formatting options`** that **customize string parsing** for _some date and time parsing methods_ (_Ex: **AllowWhiteSpaces** helps **`ignore all spaces`** in the string while it parses_)

* -> if the **`string value is null`**, it **throw 'ArgumentNullException'**
* -> if the **`string value contains some invalid date format`** (_include empty string_), it **throw 'FormatException'**

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

```cs
string dateInput = "Jan 1, 2009";
var parsedDate = DateTime.Parse(dateInput);
Console.WriteLine(parsedDate); // output on a system whose culture is en-US:   1/1/2009 00:00:00

// uses a format provider to parse a German string into a DateTime:
// -> creates a CultureInfo representing the "de-DE" culture
// -> this CultureInfo object ensures successful parsing of the given string below
var cultureInfo = new CultureInfo("de-DE"); 
string dateString = "12 Juni 2008";
var dateTime = DateTime.Parse(dateString, cultureInfo);
// -> by using the standard "DateTimeFormatInfo" objects that we specific 
// -> it precludes whatever setting is in the "CurrentCulture" of the "CurrentThread"
Console.WriteLine(dateTime); // output:   6/12/2008 00:00:00

var cultureInfo = new CultureInfo("de-DE");
string dateString = "12 Juni 2008";
var dateTime = DateTime.Parse(dateString, cultureInfo,DateTimeStyles.NoCurrentDateDefault);
// -> uses the 'DateTimeStyles' enumeration to specify that the current date and time information shouldn't be added to the DateTime for unspecified fields
Console.WriteLine(dateTime); // output if the current culture is en-US: 6/12/2008 00:00:00
```

====================================================================
## Convert.ToDateTime()
* -> converts **`specified string data`** to _equivalent date and time_
* -> it contains a couple of overload methods, but **`2 most important`**: **ToDateTime(string value)** and **ToDateTime(string value, IFormatProvider provider)**

* -> **value**: **`a string representation of date and time`**; 
* -> **provider**: an object which provides **`culture-specific info`**

* -> if the **`string value is not null`**, then it **internally calls 'DateTime.Parse()'** to give the result
* -> if the **`string value is null`**, then it **gives 'DateTime.MinValue' as "1/1/0001 12:00:00 AM"**

* => tức là nó khác với **`DateTime.Parse()`** ở chỗ là nó **không throw Exception nếu string value null**, cũng như **`không có tham số 'DataTimeSyles'`**

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

====================================================================
## DateTime.TryParse()
* -> **`converts specified string data to equivalent datetime`** and **always returns the Boolean value** after parsing - _indicating that parsing has succeeded or fail_ 
* -> overload methods: **DateTime.TryParse (String value, out DateTime result)**, **DateTime.TryParse(String value, IFormatProvider provider, DateTimeStyles styles, out DateTime result)**

* -> **value** - **`a string representation of date and time`**
* -> **provider** - an object which provides **`culture-specific info`**
* -> **styles** - defines the **`formatting options`** that customize string parsing for some date and time parsing methods
* -> **result** - **`holds the DateTime value after parsing`**

* -> _DateTime.TryParse()_ **không throw Exception** in any cases (**`always tries to parse the string value datetime`**) and **return a boolean value** (_indicating whether the conversion succeeded or failed_)
* -> if **`the conversion fails (invalid format)`** or **`the string value is null`**, it **out a MinValue(1/1/0001 12:00:00 AM)**

```cs
string dateString = null;
DateTime dateTime; // 1/1/0001 12:00:00 AM
bool isSuccess = DateTime.TryParse(dateString, out dateTime); 

string dateString = "not a date";
DateTime dateTime; // 1/1/0001 12:00:00 AM
bool isSuccess = DateTime.TryParse(dateString, out dateTime); 

string dateString = "Tue Dec 30, 2015";
DateTime dateTime; // 1/1/0001 12:00:00 AM
bool isSuccess = DateTime.TryParse(dateString, out dateTime); 
```

====================================================================
## DateTime.ParseExact()
* -> converts **`a specified string`** to _an equivalent DateTime_ with a **specified format** and **culture**; 
* -> overload methods: **DateTime.ParseExact(string value, string format, IFormatProvider provider)**, **DateTime.ParseExact(string value, string format, IFormatProvider provider, DateTimeStyles style)**, **DateTime.ParseExact(string value, string[] formats, IFormatProvider provider, DateTimeStyles style)**

* -> **value** - **`a string representation of date and time`**
* -> **format** - **a format specifier** that defines **`what a date looks like`** **after conversion**
* -> **formats** - (like "format" but) a string array contains **`a list of formats`**; **at least one format must match the string(value)** to convert the DateTime object
* -> **provider** - an object which specifies **`cultural info`**
* -> **style** - defines the **`formatting options`** that **customize string parsing** for some date and time parsing methods

* -> if **`the value is null`**, it **throw ArgumentNullException**
* -> if the **`value contains some invalid date format`**, it **throw 'FormatException'**
* => to overcome this issue, we can the **formats** array with some possibilities; however, the `format` **must match with string date time**, or it **`throws FormatException`**
* (_Ex: suppose our date might be in a format `"12/12/2015" or "12-12-2015"` here; to parse it success in both situation we need to pass a string array with a format like `"MM/dd/yyyy" and "MM-dd-yyyy"`_)

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

====================================================================
## DateTime.TryParseExact()
* -> converts **`a specified string`** to an equivalent DateTime with **a specified format** and **culture**; the `format's string value` **must match a string value of datetime**
* -> methods overloading: **DateTime.ParseExact(string value, string format, IFormatProvider provider, DateTimeStyles style)**, **DateTime.ParseExact(string value, string[] formats, IFormatProvider provider, DateTimeStyles style)**

* -> **value** - a string representation of date and time.
* -> **format** - a format specifier that defines what a date looks like after conversion.
* -> **formats** - (like "format" but) a string array contains **`a list of formats`**, and **at least one format must match the string (value)** to convert the DateTime object
* -> **provider** - an object which specifies **`cultural info`**
* -> **style** - defines **`the formatting options`** that **customize string parsing** for some date and time parsing methods

* -> if **`the value is null / empty / incorrect date / not matched with the 'format' provided`**, returns **MinValue( 1/1/0001 12:00:00 AM)**
* -> it **only throws an exception** if the **`DateTimeStyle value`** is not valid

```cs
CultureInfo provider = CultureInfo.InvariantCulture;

string dateString = null;
DateTime dateTime; // 1/1/0001 12:00:00 AM
bool isSuccess1 = DateTime.TryParseExact(dateString, "MM/dd/yyyy", provider, DateTimeStyles.None, out dateTime); // False

string dateString = "not a date";
DateTime dateTime11; // 1/1/0001 12:00:00 AM
bool isSuccess2 = DateTime.TryParseExact(dateString, "MM/dd/yyyy", provider, DateTimeStyles.None, out dateTime11); // False

string dateString = "Tue Dec 30, 2015";
DateTime dateTime12; // 1/1/0001 12:00:00 AM
bool isSuccess3 = DateTime.TryParseExact(dateString, "MM/dd/yyyy", provider, DateTimeStyles.None, out dateTime12); // False

string dateString = "10-22-2015";
DateTime dateTime13; // 1/1/0001 12:00:00 AM
bool isSuccess4 = DateTime.TryParseExact(dateString, "MM/dd/yyyy", provider, DateTimeStyles.None, out dateTime13); // False

string dateString = "10-22-2015";
DateTime dateTime15; // 10/22/2015 12:00:00 AM
bool isSuccess5 = DateTime.TryParseExact(dateString, "MM-dd-yyyy", provider, DateTimeStyles.None, out dateTime15); // True

string dateString = "10-12-2015";
DateTime dateTime14; // 10/22/2015 12:00:00 AM
bool isSuccess6 = DateTime.TryParseExact(dateString, new string[]{ "MM/dd/yyyy", "MM-dd-yyyy", "MM.dd.yyyy"}, provider, DateTimeStyles.None, out dateTime14); // True
```


