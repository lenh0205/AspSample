> **`.NET`** uses a **task-based asynchronous pattern** to do asynchronous programming
> **`Node.js`** uses an **event-driven, non-blocking I/O model with an event loop** to do asynchronous programming

=======================================================
# Problem
* khi trong code của ta gọi đến 1 Method, thì khi thực thi Method nó sẽ **khoá Thread gọi đến phương thức đó**; làm việc thực thi những dòng code tiếp theo sẽ phải chờ
* => đặc biết đối với những thao tác mất nhiều thời gian như đọc stream, đọc file, kết nối web, kết nối CSDL, ... 
* => dẫn đến 2 vấn đề:
* -> **`tài nguyên vẫn đủ để làm các việc khác`** - thì chương trình vẫn cứ phải chờ phương thức trên kết thúc 
* -> đặt biệt là khi gọi phương thức trong các **`tiến trình UI`**, giao diện người dùng không tương tác được

* => **Lập trình bất đồng bộ / Lập trình đa tiến trình, đa luồng**
* => **`.NET Framework 4.5`** introduce the **Task-based Asynchronous Pattern** (TAP)   

======================================================
# "Task" class
* -> represents **`an asynchronous operation`**

## System.Threading.Tasks.Task
* -> để tạo ra một Task ta cần **`tham số`** là một hàm **delegate** (**`Func`** hoặc **`Action`**)
* -> use **Start()** to **`run method call`** as a Task 
* -> use **Wait()** to **`wait for Task to completed`**
* -> use **Result()** to **`read the return result`** (also Wait) of the Task

```c# - khởi tạo "Task" instance
// first argument is a "Func" return "T"; second argument is "argument of that Func"
Task<T> task = new Task<T>(myfunc, object);

// if not return then it's an "Action" as first argument
Task task = new Task(myfunc);
```

```c# - sử dụng Task để tạo 2 tiến trình con chạy đồng thời:
// Hàm main
Console.WriteLine($"{' ',5} ThreadId:{Thread.CurrentThread.ManagedThreadId,3} MainThread");

Task<string> t1 = TaskSample.Async1("A", "B");
Task t2 = TaskSample.Async2();

Console.WriteLine("Lam gi đo o thread chinh sau khi 2 task chay");

string s = t1.Result; // block the main thread until Task completed 
TaskSample.WriteLine(s, ConsoleColor.Red);

t2.Wait(); // block the main thread until Task completed

Console.ReadKey(); // ngăn main thread kết thúc cho đến khi user nhấn 1 phím bất kỳ

Console.WriteLine("The main Thread is completed");


internal class TaskSample
{
    // Sử dụng delegate "Func" (có kiểu trả về) để tạo "Task"
    public static Task<string> Async1(string thamso1, string thamso2)
    {
        Func<object, string> myfunc = (object thamso) => {
            dynamic ts = thamso;
            for (int i = 1; i <= 10; i++)
            {
                // "Thread.CurrentThread.ManagedThreadId" trả về ID của thread đang chạy
                WriteLine($"Func - Time:{i,5} ThreadId:{Thread.CurrentThread.ManagedThreadId,3} Tham so {ts.x} {ts.y}", ConsoleColor.Green);
                Thread.Sleep(300);
            }
            return $"Ket thuc Async1! {ts.x}";
        };

        Task<string> task = new Task<string>(myfunc, new { x = thamso1, y = thamso2 }); // Tạo Task
        task.Start(); // khởi chạy một thread mới  

        Console.WriteLine("Async1: Lam gi đo sau khi task chay");
        return task;
    }

    // Sử dụng delegate "Action" (không kiểu trả về) để tạo "Task"
    public static Task Async2()
    {
        Action myaction = () =>
        {
            for (int i = 1; i <= 10; i++)
            {
                WriteLine($"Action - Time:{i,5} ThreadId:{Thread.CurrentThread.ManagedThreadId,3}",
                    ConsoleColor.Yellow);
                Thread.Sleep(500);
            }
            WriteLine("Ket thuc Async2!", ConsoleColor.Red);
        };

        Task task = new Task(myaction); // Tạo Task
        task.Start(); // chạy Task

        Console.WriteLine("Async2: Lam gi đo sau khi task chay");
        return task;
    }

    public static void WriteLine(string s, ConsoleColor color)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(s);
    }
}
```
```r - kết quả
    ThreadId:  1 MainThread
Async1: Lam gi do sau khi task chay
Async2: Lam gi do sau khi task chay
Lam gi do o thread chinh sau khi 2 task chay
Action - Time:    1 ThreadId:  8
Func - Time:    1 ThreadId:  6 Tham so A B
Func - Time:    2 ThreadId:  6 Tham so A B
Action - Time:    2 ThreadId:  8
Func - Time:    3 ThreadId:  6 Tham so A B
Func - Time:    4 ThreadId:  6 Tham so A B
Action - Time:    3 ThreadId:  8
Func - Time:    5 ThreadId:  6 Tham so A B
Action - Time:    4 ThreadId:  8
Func - Time:    6 ThreadId:  6 Tham so A B
Func - Time:    7 ThreadId:  6 Tham so A B
Action - Time:    5 ThreadId:  8
Func - Time:    8 ThreadId:  6 Tham so A B
Action - Time:    6 ThreadId:  8
Func - Time:    9 ThreadId:  6 Tham so A B
Func - Time:   10 ThreadId:  6 Tham so A B
Action - Time:    7 ThreadId:  8
Ket thuc Async1! A
Action - Time:    8 ThreadId:  8
Action - Time:    9 ThreadId:  8
Action - Time:   10 ThreadId:  8
Ket thuc Async2!
```

======================================================
# async/await

## Problem: 
* -> khi ta truy cập **`Task.Result`** để đọc kết quả trả về; nó sẽ **`block thread lại`**
* -> vấn đề xảy ra khi ta **truy cập Task1.Result trước Task2.Result** nhưng thực tế **Task2 lại hoàn thành xong trước Task1**
* => ta chỉ có thể đợi đến khi Task1 chạy xong, **`Task2.Result mới có thể nhận về kết quả`**
* => những **`xử lý sau đó cần sử dụng kết quả từ Task2`** phải đợi 1 cách vô ích
* => lợi ích của đa luồng, bất đồng bộ mất đi

* => **Solution**: ta sẽ cần làm sao đó khi gọi 1 AsyncMethod() mà:
* -> nó phải trả về ngay lập tức - **`không block thread`** 
* -> dù bên trong nó **`chạy 1 Task`** + **`đợi kết quả trả về`** + **`sử dụng kết quả đó thực hiện xử lý khác sau đó`**

## Mechanism:
* ta sẽ cần sử dụng từ khoá **async** cho **`Async Method`** và **await** cho **`Task`**
* -> when execute an async method if **Thread counter await**, **`the execution of current method is suspended`** 
* -> while **`waiting for the Async Method to complete`**, the **thread is freed up** to **`perform other tasks`** or operations within the application (_but not within the same method scope_)
* -> once the **`Task is completed`**, the **execution resumes** at the line **`after the await statement`**
* => allowing **multiple tasks to run concurrently**

* _nói đơn giản là **`1 Thread có thể thực hiện nhiều Task cùng lúc`**_
* _bằng cách là khi thực thi 1 Task nếu gặp từ khoá await bên trong, Thread sẽ chạy sang Task khác để thực thi cho đỡ tốn thời gian_
* _khi nào Task ban đầu xong, thì Thread sẽ quay trở lại và làm tiếp những gì còn dang dỡ_

```c# - Vẫn là VD trên nhưng s/d "async/await"
static async Task Main(string[] args)
{
    var t1 = TestAsyncAwait.Async1("x", "y");
    var t2 = TestAsyncAwait.Async2();

    Console.WriteLine("Task1, Task2 đang chạy");

    await t1; // chờ t1 kết thúc
    // ......
    await t2; // chờ t2 kết thúc
}

public static async Task<string> Async1 (string thamso1, string thamso2) {
    // Khai báo delegate cho Task
    Func<object, string> myfunc = (object thamso) => {
        dynamic ts = thamso;
        for (int i = 1; i <= 10; i++) {
            WriteLine ($"{i,5} {Thread.CurrentThread.ManagedThreadId,3} Tham số {ts.x} {ts.y}",
                ConsoleColor.Green);
            Thread.Sleep (500);
        }
        return $"Kết thúc Async1! {ts.x}";
    };

    Task<string> task = new Task<string> (myfunc, new { x = thamso1, y = thamso2 }); // Create Task
    task.Start();  // Run Task

    await task;

    TestAsync01.WriteLine("Async1 - làm gì đó khi task chạy xong", ConsoleColor.Red);
    string ketqua = task.Result; // đọc kết quả mà không phải lo block thread gọi Async1

    Console.WriteLine(ketqua);  
    return ketqua;

}

public static async Task Async2 () {
    // Khai báo delegate cho Task
    Action myaction = () => {
        for (int i = 1; i <= 10; i++) {
            WriteLine ($"{i,5} {Thread.CurrentThread.ManagedThreadId,3}", ConsoleColor.Yellow);
            Thread.Sleep (2000);
        }
    };
    Task task = new Task (myaction);
    task.Start();

    await task;

    Console.WriteLine("Async2: Làm gì đó sau khi task kết thúc");
}
```

## Async Method return a Task
* -> khai báo **`async cho method`** đảm bảo trình biên dịch **`tạo ra đối tượng Task<T>`** để trả về mặc dù trong thân là return T
* -> khi khai báo hàm với async, nếu không có kiểu trả về thì ta phải **`dùng Task làm kiểu trả về thay vì void`** vì không await được
* -> do async method là 1 Task, nên ta có thể **`await ở một phương thức async khác`**

```c# - Download 1 file sử dụng "WebClient" với "async/await"
// Synchronous Downloading
public static void DownloadFile (string url) {
    using (var client = new WebClient ()) {
        Console.Write ("Starting download ..." + url);
        byte[] data = client.DownloadData(new Uri(url)); // mảng byte tải về

        string filename = System.IO.Path.GetFileName(url); // get file name
        System.IO.File.WriteAllBytes(filename, data); // save file at path "filename"
    }
    Console.WriteLine("Đã hoàn thành tải file");
}

// Asynchronous Dowloading
// -> đưa method body vào 1 Action
// -> rồi tạo Task từ delegate 
public class DownloadAsync {
    public static async Task DownloadFile (string url) {
        Action downloadaction = () => {
            using (var client = new WebClient ()) {
                Console.Write ("Starting download ..." + url);
                byte[] data = client.DownloadData(new Uri(url));

                string filename = System.IO.Path.GetFileName(url);
                System.IO.File.WriteAllBytes(filename, data);
            }
        };

        Task task = new Task(downloadaction);
        task.Start();

        await task;
        Console.WriteLine("Đã hoàn thành tải file");
    }
}

// Run:
static async Task Main(string[] args)
{
    string url = "https://github.com/microsoft/vscode/archive/1.48.0.tar.gz";
    var taskdonload = DownloadAsync.DownloadFile(url);
    
    //..

    await taskdonload;
    Console.WriteLine("Làm gì đó khi file tải xong");
}
```

=================================================================
# CancellationToken
* -> to signal to a thread or an operation that it should be canceled

```c#
static async Task Main(string[] args)
{
    var CancellationTokenSource = new CancellationTokenSource(); // Create a CancellationTokenSource
    CancellationToken cancellationToken = tokenSource.CancellationTokenSource; // Get token 

    Task task1 = new Task(() => {
        for (int i = 0; i < 10000; i++)
        {
            if (cancellationToken.IsCancellationRequested) // kiểm tra xem có yêu cầu dừng Task không
            {
                Console.WriteLine("TASK1 STOP");
                cancellationToken.ThrowIfCancellationRequested();
                return;
            }

            Console.WriteLine("TASK1 runing ... " + i);
            Task.Delay(300);
        }
    }, cancellationToken);

    Task task2 = new Task(() => {
        for (int i = 0; i < 10000; i++)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                Console.WriteLine("TASK1 STOP");
                cancellationToken.ThrowIfCancellationRequested();
                return;
            }
            Console.WriteLine("TASK2 runing ... " + i);
            Task.Delay(300);
        }
    }, cancellationToken);

    // Run all tasks:
    task1.Start();
    task2.Start();

    while (true)
    {
       var c = Console.ReadKey().KeyChar;

       if (c == 'e') // khi user bấm phím "e"
       {
           CancellationTokenSource.Cancel(); // phát yêu cầu dừng task
           break;
       }
    }

    try
    {
        await operationTask;
        Console.WriteLine("Operation completed successfully.");
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine("Operation was canceled.");
    }
    Console.ReadKey();
}
```

=================================================================
# Thread.Sleep() và Task.Delay()

* **Thread.Sleep()**
* -> is **`a blocking method`** that **pauses the execution of the current thread** for a specified period of time 
* -> it is a **`very bad`** idea to use Thread.Sleep **in asynchronous code**, because of the cause of a context switch
* => this can lead to **`inefficient resource utilization`**, especially in scenarios where you have **`multiple threads`**
* => because async code allow us to use 1 Thread **`run multiple tasks concurrently`**, instead of the need for **huge amounts of individual threads** 
* => this allows **`a threadpool`** to **`service many requests at once`**
* => however, given that async code usually runs on the threadpool; Thread.Sleep() consumes an entire thread **`that could otherwise be used elsewhere`**
* => việc nhiều task runs Thread.Sleep() có thể **exhausting all threadpool threads**

* **Task.Delay()** 
* -> is **`a non-blocking method`** that **creates and run a Task** that **`completes after a specified period of time`** 
* -> so it **`should be use in asynchronous code`**, allowing **`Thread`** to continue execute other tasks or operations **`while waiting for the specified delay`** 
* -> we can **await** or use in combination with other asynchronous operations

```C#
class Program
{
    static void Main(string[] args)
    {
        Task delay = asyncTask();
        syncCode();
        delay.Wait();
        Console.ReadLine();
    }

    static async Task asyncTask()
    {
        var sw = new Stopwatch();
        sw.Start();
        Console.WriteLine("async: Starting");
        Task delay = Task.Delay(5000);
        Console.WriteLine("async: Running for {0} seconds", sw.Elapsed.TotalSeconds);
        await delay;
        Console.WriteLine("async: Running for {0} seconds", sw.Elapsed.TotalSeconds);
        Console.WriteLine("async: Done");
    }

    static void syncCode()
    {
        var sw = new Stopwatch();
        sw.Start();
        Console.WriteLine("sync: Starting");
        Thread.Sleep(5000);
        Console.WriteLine("sync: Running for {0} seconds", sw.Elapsed.TotalSeconds);
        Console.WriteLine("sync: Done");
    }
}

// Output:
// async: Starting
// async: Running for 0.0070048 seconds
// sync: Starting
// async: Running for 5.0119008 seconds
// async: Done
// sync: Running for 5.0020168 seconds
// sync: Done
```