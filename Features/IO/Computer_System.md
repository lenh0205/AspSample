
# "System.IO.DriveInfo" class - làm việc với drive
* -> giúp **`đọc thông tin các ổ đĩa có trong hệ thống`** thông qua những thuộc tính như: **`isReady`**, **`DriveType`**, **`AvailableFreeSpace`**, ...
* -> phương thức static **DriveInfo.GetDrives()** trả về mảng gồm các đối tượng **`DriveInfo`** 

```c# - get drives info
DriveInfo[] drives = DriveInfo.GetDrives();

foreach (DriveInfo drive in drives)
{
    Console.WriteLine($"Drive: {drive.Name}");
    Console.WriteLine($"Drive type: {drive.DriveType}");

    if (drive.IsReady)
    {
        Console.WriteLine($"Total size of drive: {drive.TotalSize} bytes");
        Console.WriteLine($"Total available free space: {drive.TotalFreeSpace} bytes");
        Console.WriteLine($"Available free space to current user: {drive.AvailableFreeSpace} bytes");
        Console.WriteLine($"File system: {drive.DriveFormat}");
        Console.WriteLine($"Volume label: {drive.VolumeLabel}");
    }
    else
    {
        Console.WriteLine("Drive is not ready.");
    }

    Console.WriteLine();
}
Console.ReadLine();
```
```r - result
Drive: C:\
Drive type: Fixed
Total size of drive: 127376101376 bytes
Total available free space: 11501146112 bytes
Available free space to current user: 11501146112 bytes
File system: NTFS
Volume label: System

Drive: D:\
Drive type: Fixed
Total size of drive: 128558559232 bytes
Total available free space: 112932896768 bytes
Available free space to current user: 112932896768 bytes
File system: NTFS
Volume label: Data
```

## "Drive" in a Window System
* -> **`a physical or logical`** **storage device** (_like a **HDD** - **`hard disk drive`** or **SSD** - **`solid-state drive`**_) used for storing and retrieving data
* ->  _in the context of storage_, a **`drive`** and a **disk** are often used **interchangeably** (but no all like `a network share drive` or `a virtual drive`)
* ->  a single **`physical storage device`** (**a drive**) can be divided into **`multiple partitions or logical  volumes`** (_if needed_)
* -> _each partition_ is typically assigned a separate **drive letter** (_like **`C:`**, **`D:`** in Window OS)
* -> However, in Windows system we can refer the term **`drive`** as **`a specific partition or logical volume`** on that storage device, which is represented by a drive letter
* _nói đơn giản ta có thể gọi ổ đĩa C: và ổ đĩa D: là drive cũng được_
* -> The _separating data onto different drives or partitions_ **`is not mandatory`**, depends on specific system requirements and preferences

### Tại sao nên tách "drive" thành "multiple partitions or logical  volumes" 
* **a single large partition** is especially sufficient in systems with **`ample storage capacity`** and **`simpler data management requirements`**

* mặt khác việc **separate storage into different partitions or drives** sẽ có 1 số lợi ích
* -> VD: để **`Operation system and programs`** trong **`C: drive`** và **`user data`** trong **`D: drive`** 
* -> **reinstall** the operating system without affecting user data
* -> ta có thể sử dụng **các system configuration khác nhau** để triển khai những mức độ **`data protection, backup, recovery strategies, system maintenance performance`** khác nhau
* -> **improve performance**: distributed disk I/O operations giúp **`tránh sự tranh chấp`** giữa những dữ liệu truy cập thường xuyên (files, databases) với operating system and program files 
* -> **compatibility** with **`older operating systems and software`** 

## Driver
* -> is **`a software component`** that enables **communication** between the **`operating system`** and **`a specific hardware device or peripheral`**, such as a disk drive
* -> **`provide`** the **necessary instructions and interfaces** for the **`operating system to interact with hardware devices`** effectively

===========================================================
# "System.IO.Path" class - làm việc với đường dẫn
* -> hỗ trợ **cross-platform** trong việc **`quản lý, tạo`** các **`đường dẫn đến file, thư mục`** 
* -> thông qua các **`static method`** như **`PathSeparator`**, **`ChangeExtension`**, **`GetPathRoot`**, ... 

* để lấy đường dẫn đến một số **`thư mục đặc biệt`** của hệ thống, có thể dùng phương thức **Environment.GetFolderPath** (_VD: lấy thư mục MyDocument_)
```c# - lấy thư mục "MyDocument" của User trên hệ thống
var path_mydoc = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
```

============================================================
# "System.IO.File" class - làm việc với tệp
* ->  làm việc với các tệp như **`copy, xóa, di chuyển, lưu text vào file, đọc nội dung file, kiểm tra sự tồn tại, tra cứu thông tin về file ...`**
* -> thông qua các **`static method`** như WriteAllText, ReadAllText, 

## File.WriteAllText(filePath, stringContent, Encoding)
* -> **`creates a new file`** + **`write string contents to the files`** + **`close file`** (_truncated and overwritten if the target file already exists_)

```c# - lưu một nội dung vào file test.txt với đường dẫn đến thư mục MyDocument của hệ thống
var filename = "test.txt";
string contentfile = "Xin chào! xuanthulab.net";
var directory_mydoc = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

var fullpath = Path.Combine(directory_mydoc, filename); // tạo đường dẫn
File.WriteAllText (fullpath, contentfile); // ghi file

Console.WriteLine ($"File lưu tại {directory_mydoc}{Path.DirectorySeparatorChar}{filename}");
// "DirectorySeparatorChar" sẽ là "\" trên Windows, "/" trên Unix
``` 

## File.ReadAllText
* -> **`opens a text file`** + **`reads all the text in file into a string`** + **`close file`**

```c#
string s = File.ReadAllText(fullpath);
Console.Write(s);
```

=========================================================
#  "System.IO.Directory" class - làm việc với directory

```c# - lấy tất cả các thư mục, file trong một thư mục
var directory_mydoc = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
String[] files = System.IO.Directory.GetFiles(directory_mydoc);
String[] directories = System.IO.Directory.GetDirectories(directory_mydoc);

foreach (var file in files)
{
    Console.WriteLine(file);
}

foreach(var directory in directories)
{
    Console.WriteLine(directory);
}
```

```c# - đệ quy liệt kê tất cả các file, thư mục con trong một thư mục
static void ListFileDirectory(string path)
{
    String[] directories = System.IO.Directory.GetDirectories(path);
    String[] files = System.IO.Directory.GetFiles(path);
    foreach (var file in files)
    {
        Console.WriteLine(file);
    }
    foreach (var directory in directories)
    {
        Console.WriteLine(directory);
        ListFileDirectory(directory); // Đệ quy
    }
}

// Run:
var directory_mydoc = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
ListFileDirectory(directory_mydoc);
```