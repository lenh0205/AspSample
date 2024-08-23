===================================================================
# 'DateTime.Now' vs 'DateTime.Today'

## DateTime.Now 
* -> returns **`a DateTime value`** that consists of the **local date and time of the computer where the code is running**
* -> it has **DateTimeKind.Local** assigned to its **`Kind`** property

* _it is equivalent to calling any of the following:_
* -> **DateTime.UtcNow.ToLocalTime()**
* -> **DateTimeOffset.UtcNow.LocalDateTime**
* -> **DateTimeOffset.Now.LocalDateTime**
* -> **TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.Local)**
* -> **TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.Local)**

## DateTime.Today 
* -> returns **`a DateTime value`** that has the **same year, month, and day components as any of the above expressions**, but with the **`time components set to zero`**
* -> it also has **DateTimeKind.Local** in its **`Kind`** property

* _it is equivalent to any of the following:_
* -> **DateTime.Now.Date**
* -> **DateTime.UtcNow.ToLocalTime().Date**
* -> **DateTimeOffset.UtcNow.LocalDateTime.Date**
* -> **DateTimeOffset.Now.LocalDateTime.Date**
* -> **TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.Local).Date**
* -> **TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.Local).Date**

## Usage
* -> we **`shouldn't use either one of these or any of the above equivalents`**
* -> the **DateTime.Now** doesn't have any information about the clock of the computer that the code is running on; the only valuable information we can get is **`DateTime.Now.Kind == DateTimeKind.Local`**
* -> but whose **Local** is it ? and this **`information will also lost`** after we use it value such as **store it in a database**, **display it on screen**, or **transmit it using a web service**

* -> the best thing we should use is **`Utc`** (_Ex: using **DateTimeOffset**_)
```cs
// This will always be unambiguous.
DateTimeOffset now = DateTimeOffset.Now;
```

## Note
* _**DateTime.Now.ToUniversalTime()** is more expensive than **DateTime.UtcNow**_
* -> because internally, **`the system clock is in terms of UTC`** - when we call _DateTime.Now_ it **first gets the UTC time** (via the "GetSystemTimeAsFileTime" function in the Win32 API)
* -> then it **converts the value to the local time zone**

* _**DateTimeOffset.Now.DateTime** will have **`similar values`** to **DateTime.Now**, but it will have **`DateTimeKind.Unspecified`** rather than **`DateTimeKind.Local`**_

* _there are some places in this world (such as Brazil) where the "spring-forward" transition happens exactly at Midnight_
* -> the clocks go from 23:59 to 01:00
* -> this means that the value we get for DateTime.Today on that date, does not exist! Even if we use DateTimeOffset.Now.Date, you are getting the same result, and we still have this problem
* -> it is because traditionally, there has been no such thing as a Date object in .Net. So regardless of how we obtain the value, once we strip off the time - we have to remember that it doesn't really represent "midnight", even though that's the value we're working with

===================================================================
# 'DateTime.UtcNow' vs 'DateTime.Now'

## DateTime.UtcNow 
* -> is the date and time as it would be in **`Coordinated Universal Time`** (_which is also called the `Greenwich Mean Time` time zone_) 

* => so using _DateTime.UtcNow_ when we want to **`store dates`** (_in **Database**_)
* => or use them for **`later calculations`** that way (in a client-server model); our calculations **don't become confused by clients in different time zones from our server** or from each other

* _this is important so we have to **make sure that the database doesn't add its own timezone** to "dates that don't give explicit timezones"_

## DateTime.Now 
* -> gives the date and time as it would appear to someone in our **`current locale`**

* => so the _DateTime.Now_ is usually used when we want **`displaying a date to a human being`**
* _that way they're comfortable with the value they see - it's something that they can **easily compare to what they see on their watch or clock**_

===================================================================
# Converting a DateTime object to 'Coordinated Universal Time'
* -> the easiest way to convert a time to UTC is to call the static method **`TimeZoneInfo.ConvertTimeToUtc(dateTimeObj)`**

* -> the conversion will depend on the **`value "Kind" property of DateTime object`**
* -> if its value is **DateTimeKind.Local**, it'll **`converts local time to UTC`**
* -> if its value is **DateTimeKind.Utc**, it'll **`returns the dateTime parameter unchanged`**
* -> if its value is **DateTimeKind.Unspecified**, it **`assumes the dateTime parameter is local time and converts local time to UTC`**

```cs
DateTime dateNow = DateTime.Now;
TimeZoneInfo.ConvertTimeToUtc(dateNow);
```

## Convert fail
* -> **the date and time value doesn't represent the local time or UTC**, the ToUniversalTime method will likely return an **`erroneous result`**
* -> but we can provide **`TimeZoneInfo`** object to convert the date and time from a **specified time zone**

```cs - convert Eastern Standard Time to UTC
try {
    string easternZoneId = "Eastern Standard Time";
    TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById(easternZoneId);

    DateTime easternTime = new DateTime(2007, 01, 02, 12, 16, 00);
    var dt = TimeZoneInfo.ConvertTimeToUtc(easternTime, easternZone);
}
catch (TimeZoneNotFoundException)
{
    Console.WriteLine($"Unable to find the {easternZoneId} zone in the registry.");
}
catch (InvalidTimeZoneException)
{
    Console.WriteLine($"Registry data on the {easternZoneId} zone has been corrupted.");
}
catch (ArgumentException)
{
    // occur if the DateTime object's 'Kind' property and the time zone are mismatched
    // Ex: the Kind property is DateTimeKind.Local but the TimeZoneInfo object doesn't represent the local time zone
}
```

## Using 'DateTimOffset'
* ->  the _DateTimeOffset structure_ has a **.ToUniversalTime()** instance method that **`converts the date and time of the current instance to UTC`**

```cs
// define local time in local time zone
DateTimeOffset localTime = new DateTimeOffset(new DateTime(2007, 6, 15, 12, 0, 0)); // 6/15/2007 12:00:00 PM -07:00

// convert local time to offset 0 and assign to otherTime
DateTimeOffset otherTime = localTime.ToOffset(TimeSpan.Zero); // 6/15/2007 7:00:00 PM +00:00

localTime.Equals(otherTime); // true
localTime.EqualsExact(otherTime); // false

// convert other time to UTC
DateTimeOffset universalTime = localTime.ToUniversalTime(); // 6/15/2007 7:00:00 PM +00:00

universalTime.Equals(otherTime); // true
universalTime.EqualsExact(otherTime); // true
```

===================================================================
# Converting 'UTC' to a designated time zone
* -> to convert UTC to local time, see the Converting UTC to local time section that follows. To convert UTC to the time in any time zone that you designate, call the ConvertTimeFromUtc method. The method takes two parameters:

The UTC to convert. This must be a DateTime value whose Kind property is set to Unspecified or Utc.

The time zone to convert the UTC to.

# Converting UTC to local time
* -> _to convert UTC to local time_, call the **`.ToLocalTime()`** method of the DateTime object 

* -> the conversion will depend on the **`value "Kind" property of DateTime object`**
* -> if its value is **DateTimeKind.Local**, it'll **`returns the DateTime value unchanged`**
* -> if its value is **DateTimeKind.Utc**, it'll **`converts the DateTime value to local time`**
* -> if its value is **DateTimeKind.Unspecified**, it **`assumes that the DateTime value is UTC and converts the UTC to local time`**

# Converting between any two time zones
```cs - converts UTC to Central Standard Time
try
{
    DateTime timeUtc = DateTime.UtcNow;
    TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
    DateTime cstTime = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, cstZone);
}
catch (TimeZoneNotFoundException)
{
    Console.WriteLine("The registry does not define the Central Standard Time zone.");
}
catch (InvalidTimeZoneException)
{
    Console.WriteLine("Registry data on the Central Standard Time zone has been corrupted.");
}
```