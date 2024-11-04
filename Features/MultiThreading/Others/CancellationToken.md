# CancellationToken
* -> sử dụng khi cần huỷ các tác vụ bất đồng bộ đang chạy, ngay cả các tác vụ không thể cancel

## .NET in the past
* -> ta sẽ sử dụng một đối tượng BackgroundWorker để chạy bất đồng bộ và các hành động chạy ngầm lâu dài và hủy các hành động này bằng cách gọi CancelAsync và cài đặt cờ CancellationPending là true
* -> mặc dù đây không phải là cách khuyến khích nhưng đây là những khái niệm được sử dụng đến khi có Task và CancellationToken  

```cs
private void BackgroundLongRunningTask(object sender, DoWorkEventArgs e)
{
    BackgroundWorker worker = (BackgroundWorker)sender;

    for (int i = 1; i <= 10000; i++)
    {
        if (worker.CancellationPending == true)
        {
            e.Cancel = true;
            break;
        }
        
        // Do something
    }
}
```

## Task
* -> **`Task`** đại diện cho một hành động bất đồng bộ

https://tedu.com.vn/lap-trinh-c/tim-hieu-task-cancellation-trong-c-277.html
https://learn.microsoft.com/en-us/dotnet/api/system.threading.cancellationtoken?view=net-8.0
https://hackernoon.com/why-do-you-need-a-cancellation-token-in-c-for-tasks
https://stackoverflow.com/questions/76582071/understanding-backgroundservice-and-cancellationtoken
https://dev.to/tkarropoulos/cancellation-tokens-in-c-cm0
https://medium.com/@mitesh_shah/a-deep-dive-into-c-s-cancellationtoken-44bc7664555f
https://learn.microsoft.com/en-us/dotnet/standard/threading/cancellation-in-managed-threads
