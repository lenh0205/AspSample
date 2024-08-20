
# TimeSpan
* -> **a TimeSpan object** represents **`a time interval (duration of time or elapsed time)`** that is measured as a positive or negative number of **days, hours, minutes, seconds, and fractions of a second**

```cs - initiate using constructor
TimeSpan interval = new TimeSpan();
interval.Equals(TimeSpan.Zero); // True

TimeSpan interval = new TimeSpan(2, 14, 18);
interval.ToString(); // 02:14:18 (2 giờ 14 phút 18 giây) 

new TimeSpan(Int32, Int32, Int32, Int32); // number of days, hours, minutes, and seconds
new TimeSpan(Int32, Int32, Int32, Int32, Int32); // number of days, hours, minutes, seconds, and milliseconds
new TimeSpan(Int32, Int32, Int32, Int32, Int32, Int32); // number of days, hours, minutes, seconds, milliseconds, and microseconds
new TimeSpan(Int64); // number of ticks
```

* -> **TimeSpan class** provides **.FromDays(), .FromHours(), .FromMinutes(), .FromSeconds(), and .FromMilliseconds()** static methods to **`create TimeSpan objects`** from **`days, hours, minutes, seconds, and milliseconds`**
```cs
TimeSpan ts1 = TimeSpan.FromDays(12); // 12.00:00:00
TimeSpan ts2 = new TimeSpan(12, 0, 0, 0, 0); // 12.00:00:00

TimeSpan ts2 = TimeSpan.FromHours(8);   
TimeSpan ts3 = TimeSpan.FromMinutes(20);
TimeSpan ts4 = TimeSpan.FromMilliseconds(2300);
```

* -> the **.Add(), .Subtract(), .Multiply(), .pide(), and .Negate() methods** to **`add, subtract, pide, multiply, and negate`** **TimeSpan objects**
```cs - get "TimeSpan" between 2 specific dates
DateTime date1 = new DateTime(2010, 1, 1, 8, 0, 15);
DateTime date2 = new DateTime(2010, 8, 18, 13, 30, 30); // 8/18/2010 1:30:30 PM
TimeSpan interval = date2 - date1;
TimeSpan interval2 = date2.Subtract(date1);

interval.ToString(); // 229.05:30:15 (229 ngày 5 giờ 30 phút và 15 giây)
interval2.ToString(); // 229.05:30:15 

interval.Days; // 229
interval.Hours; // 5
interval.Minutes; // 30
interval.Seconds; // 15
interval.Milliseconds; // 0
interval.Ticks; // 198,054,150,000,000 - a tick is 100 nanoseconds

interval.TotalDays; // 229.229340277778
interval.TotalHours; // 5501.50416666667
interval.TotalMinutes; // 330090.25
interval.TotalSeconds; // 19,805,415
interval.TotalMilliseconds; // 19,805,415,000
```

```cs - caculate time interval between 2 time points
string startTime = "7:00 AM";
string endTime = "2:00 PM";

DateTime dtStart =  DateTime.Parse(startTime); // 2024-08-20T07:00:00 (nếu hôm nay là 20/08/2024)
DateTime dtEnd =  DateTime.Parse(endTime); // 2024-08-20T14:00:00 (nếu hôm nay là 20/08/2024)

TimeSpan duration = dtEnd.Subtract(dtStart); // 07:00:00


string startTime = "7:00";
string endTime = "14:00";
TimeSpan duration = DateTime.Parse(endTime).Subtract(DateTime.Parse(startTime)); // 07:00:00
```