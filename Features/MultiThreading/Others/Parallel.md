
# System.Threading.Tasks.Parallel - lập trình đa luồng
* provides support for **parallel** **`loops and regions`**

* -> trừu tượng hóa các thread nhờ đó có thể chạy nhiều thread, trên Thread chạy nhiều Task
* -> 2 **`static method`** quan trọng là **Parallel.For**, **Parallel.ForEach** để chạy vòng lặp for và foreach giúp thực hiện đa tác vụ, đa tiến trình
* -> **Parallel.Invoke** có khả năng thực hiện một Action có khả năng chạy song song

==============================================================================
## Parallel.For
* executes a **for** loop in which iterations **`may run in parallel`**

* -> overloading method **`ParallelLoopResult result = Parallel.For(i1, i2, action);`** - tạo 1 vòng lặp chạy  từ số nguyên i1 đến i2, mỗi lần lặp nó sẽ thực hiện 1 **Action**
* -> **`ParallelLoopResult.IsCompleted`** cho biết vòng lặp đã được duyệt qua hết 

* **Notice**: ta nên sử dụng async Action và await đối với 1 Task.Wait() trong Action
* -> nó giúp sau khi vòng For chạy tất cả Action (Start() tất cả Task bên trong tất cả Actions), Thread có thể tiếp xử lý chính (thực thi những code bên dưới Parallel.For())
* -> Nếu không làm thế những Action vẫn được khởi chạy đa luồng đa tác vụ bình thường, nhưng bản thân  Parallel.For() sẽ block Thread ở xử lý chính (chính xác là xử lý gọi nó)
* -> và chỉ mở lại khi tất cả các Action hoàn thành (những Task bên trong Action completed)

```c#
class Program
{
    static void Main(string[] args)
    {
        ParallelLoopResult result = Parallel.For(1, 20, RunTask);  
        Console.WriteLine($"All tasks are started: {result.IsCompleted}");
    }

    public static async void RunTask(int i)  {
        PintInfo($"Start {i,3}");
        await Task.Delay(100).Wait();          
        PintInfo($"Finish {i,3}");
    }

    // in ra: lần lặp hiện tại + Id của Task hiện tại + Id của Thread hiện tại
    public static void PintInfo(string info) => Console.WriteLine(
        $"{info, 10} task:{Task.CurrentId,3}" + $"thread: {Thread.CurrentThread.ManagedThreadId}"
    ); 
}

// Output:
// -> các Task chạy song song, bắt đầu không theo thứ tự nhất định, có Task đã kết thúc thì Task khác mới bắt đầu chạy
// -> có nhiều ThreadId tức có nhiều Thread; có những Task khác TaskId nhưng cùng ThreadId do chúng được xử lý bởi chung 1 Thread 
// -> dòng "All tasks are started: True" sẽ được in ra ngay sau khi Thread Start() alls the Task (lưu ý vẫn có thể có những Task đã finish trước khi log được in ra), không đợi all Task finished
```

=======================================================
## Parallel.ForEach
* executes a foreach (For Each in Visual Basic) operation in which iterations may run in parallel

* -> tạo vòng lặp foreach qua 1 Collection (Array, List) để chạy song song đa tác vụ
* -> over loading method **`ParallelLoopResult result = Parallel.ForEach(source, action);`**
* -> action là 1 Action nhận đối số là giá trị phần tử mỗi lần lặp

```c#
static void Main(string[] args)
{
    ParallelLoopResult result = Parallel.ForEach(source, RunTask);

    Console.WriteLine($"All task started: {result.IsCompleted}");
    Console.WriteLine("Press any key ...");
    Console.ReadKey();
}

public string[] source = new string[] {
        "time 1","time 2","time 3",
        "time 4","time 5","time 6",
        "time 7","time 8","time 9"
};

public static async void RunTask(string s)  {
    PintInfo($"Start {s,10}");
    await Task.Delay(1);                 // Task.Delay là một async nên có thể await, RunTask chuyển điểm gọi nó tại đây
    PintInfo($"Finish {s,10}");
}
```

==================================================================================
## Parallel.Invoke 
* -> chạy song song nhiều loại Action (phương thức) một lúc thì dùng Paralell.Invoke
* -> Parallel.Invoke(action1, action2, action3);

```c#
static void Main(string[] args)
{
    Parallel.Invoke(taskAction, actionA, actionB);

    Console.WriteLine("Press any key ...");
    Console.ReadKey();
}

Action taskAction  = () => {
    RunTask("Action1");
};
public static async void taskAction(string s)  {
    PintInfo($"Start {s,10}");
    await Task.Delay(1);
    PintInfo($"Finish {s,10}");
}

public static void actionA() {
    PintInfo($"Finish {"ActionA",10}");
}

public static void actionB() {
    PintInfo($"Finish {"ActionB",10}");
}

public static void PintInfo(string info) =>
    Console.WriteLine($"{info, 10}    task:{Task.CurrentId,3}    "
    + $"thread: {Thread.CurrentThread.ManagedThreadId}");
```