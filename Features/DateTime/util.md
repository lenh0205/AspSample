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

