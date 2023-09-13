# Delegate
// -> đại diện cho references tới methods, function (with a particular parameter list and return type)
// -> nói chung nó như đại diện cho Method có cùng signature (cùng param, return type) mà ta gán vào delegate
// -> từ đó chỉ cần gọi cùng 1 "delegate()" là có thể thực hiện các logic khác nhau tuỳ theo Method truyền vào
// => có thể dùng "mutilcast delegate" để gọi 1 delegate mà thực hiện logic của nhiều Method khác nhau 

 // Khai báo delegate
delegate int Calculator(int a, int b);
delegate void Callback(string message);
public delegate void MyDelegate(string message);

internal class Program
{
    static void Main(string[] args)
    {

        // Tạo instance của delegate, sử dụng delegate để tính tổng của 2 số
        Calculator calculator = new Calculator(Add); // hoặc: Calculator calculator = Add;
        int result = calculator(10, 5); // Output: 15

        // Sử dụng delegate để tính hiệu của 2 số
        calculator = (a, b) => a - b; // lambda expression
        result = calculator(10, 5); // Output: 5

        // Sử dụng delegate để tạo ra một callback
        CallMethodWithCallback("Hello, world!", DisplayMessage);

        // Mutilcast Delegate:
        MyDelegate myDelegate = Method1;
        myDelegate += Method2;

        myDelegate("Hello World!"); // Output: Method1: Hello World!    Method2: Hello World!
        myDelegate -= Method2; // remove a method from the invocation list of a delegate
    }

    private static int Add(int a, int b)
    {
        return a + b;
    }

    private static void DisplayMessage(string message)
    {
        Console.WriteLine("Callback called with message: " + message);
    }
    private static void CallMethodWithCallback(string message, Callback callback)
    {
        callback(message);
    }

    public static void Method1(string message)
    {
        Console.WriteLine("Method1: " + message);
    }
    public static void Method2(string message)
    {
        Console.WriteLine("Method2: " + message);
    }
}

# Event
// -> cơ chế để 1 object này thông báo đến object khác về sự xuất hiện của 1 "Action"
// => "publisher" - The class that raises the event (1 publisher)
// => "subscribers" - classes that receive the notification  (nhiều subscriber)

// -> "Event" thực chất nó là tương tự như 1 "delegate"
// =>  trong .NET, các Event xây dựng với nền tảng chính là delegate

## Event trong .NET
// -> đều được build trên delegate "EventHandler":
public delegate void EventHandler(object sender?, EventArgs e); // generic
public delegate void EventHandler<TEventArgs>(object sender?, TEventArgs e);

// -> Ví dụ: KeyDown, GotFocus, Load,... của Form, Application.ApplicationExit, Application.Idle ...

// -> Build custom "Event" from delegate "EventHandler" for Publisher:
// => build class derived from "EventArgs" ; 
// => đảm bảo dữ liệu không bị sửa mà chỉ có thể đọc bởi các Subscriber

// Xây dựng lớp MyEventArgs kế thừa từ EventArgs
public class MyEventArgs : EventArgs {
    public MyEventArgs (string data) {
        this.data = data;
    }
    // Lưu dữ liệu gửi đi từ publisher
    private string data;

    public string Data {
        get { return data; }
    }
}

// Publisher:
public class Publisher {
    // Tạo Event với EventHandler
    public event EventHandler event_news;

    public void Send () {
        event_news?.Invoke (this, new MyEventArgs ("Có tin mới Abc ..."));
    }
}

// Subscriber:
public class SubscriberA {
    public void Sub (Publisher p)
    {
        p.event_news += ReceiverFromPublisher;
    }

    private void ReceiverFromPublisher (object sender, MyEventArgs e)
    {
        Console.WriteLine ("SubscriberA: " + e.Data);
    }
}


// Subscriber:
public class SubscriberB {
    public void Sub (Publisher p)
    {
        p.event_news += ReceiverFromPublisher;
    }

    private void ReceiverFromPublisher (object sender, MyEventArgs e)
    {
        Console.WriteLine ("SubscriberB: " + e.Data);
    }
}

// Running:
static void TestEventHandler ()
{
    Publisher p  = new Publisher();
    SubscriberA sa = new SubscriberA();
    SubscriberB sb = new SubscriberB();

    sa.Sub (p); // sa đăng ký nhận sự kiện từ p
    sb.Sub (p); // sb đăng ký nhận sự kiện từ p

    p.Send (); // SubscriberA: Có tin mới Abc ...  SubscriberB: Có tin mới Abc ...
}


## Delegate Problem
// -> việc đăng ký của SubcriberA lúc trước có thể bị loại bỏ bởi SubcriberB bằng việc gán delegate = null
// -> Điều này là phá hỏng nguyên tắc hoạt động của mô hình lập trình sự kiện - phá vỡ sự đóng gói
// -> biến "Delegate" thành "Event"
// => chỉ có thể "+=" để đăng ký nhận sự kiện; "-=" để hủy nhận sự kiện 
// => không thể thực hiện gán delegate = null

// Publisher:
public class Publisher {
    public delegate void NotifyNews (object data); // delegate

    public NotifyNews event_news;

    public void Send () {
        event_news?.Invoke ("Co tin moi");
    }
}

// SubscriberA lớp này đăng ký nhận sự kiện từ Publisher,
public class SubscriberA {
    public void Sub (Publisher p) {
        p.event_news += ReceiverFromPublisher;
    }

    void ReceiverFromPublisher (object data) {
        Console.WriteLine ("SubscriberA: " + data.ToString ());
    }
}

// SubscriberB lớp này đăng ký nhận sự kiện từ Publisher,
public class SubscriberB {
    public void Sub (Publisher p) {
        p.event_news = null;  // Hủy các đối tượng khác nhận sự kiện
        p.event_news += ReceiverFromPublisher;
    }

    void ReceiverFromPublisher (object data) {
        Console.WriteLine ("SubscriberB: " + data.ToString ());
    }
}

// Program.cs
static void TestDelegate()
{
    Publisher p = new Publisher();
    SubscriberA sa = new SubscriberA();
    SubscriberB sb = new SubscriberB();

    sa.Sub(p);
    sb.Sub(p);

    p.Send(); // SubscriberB: Co tin moi
}