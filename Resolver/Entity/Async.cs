# Asynchronous
// -> allows code to be executed concurrently without blocking the execution of the calling thread
// -> When an asynchronous operation is started
// => the program continues to execute other code while it waits for the operation to complete. 
// => The program is notified when the operation is complete and can continue with the following line of code.

// -> callbacks, events, promises, async/await
// -> Async and await in C# are the code markers that mark code positions from where the control should resume after completing a task

# async keyword
// -> marks a method asynchronous
// -> it can be run in the background while another code executes

# await keyword
// ->  indicate that the method should wait for the result of an asynchronous operation before continuing
// -> wait for the method to complete before moving on; but not block the thread
// -> Asynchronous method return "Task<T>"
// => we need "await" to wait for the operation to complete and then assign the result "T" to variable
Task<List<Customer>> customersAsync = _context.Customers.ToListAsync();
List<Customer> customers = await customersAsync;

## .ToList() - synchronous method
// -> program will wait for the method to complete before moving on to the next line of code
// -> thread that is executing the code is blocked and cannot do anything else until the method completes

// -> In UI Thread: application will become unresponsive until the data is loaded
// => user will not be able to interact with the application while the data is being loaded

// -> in Web API:  performance issues if many requests are being handled concurrently, as 
// => each request would tie up a thread for the duration of its database operation
// => This means that if you have 100 concurrent requests, you’ll need 100 threads to handle them. 
// => If the number of requests exceeds the number of available threads in the thread pool, incoming requests will have to wait until a thread becomes available. This can lead to performance issues and poor response times.

var customers = _context.Customers.ToList(); // execute (cost 2s)
var blogs = _context.Blogs.ToList(); // wait for previous line to done (cost 3s)
var posts = _context.Posts.ToList(); // wait for previous line to done (cost 2s)
// cost 7s in total

## .ToListAsync() - asynchronous method
// -> program will not wait for the method to complete and will move on to the next line of code immediately
// -> thread that is executing the code is freed up to do other work while waiting for the method to complete

// -> in UI Thread: application will remain responsive while data is being loaded

// -> in Web API: system can free up the thread to handle other requests while waiting for the database operation to complete
// => if you have 100 concurrent requests, could potentially be handled by a much smaller number of threads
// => As each request awaits the database operation, its thread could be used to start handling another request
// => Once the operation completes, the original request can resume processing on any available thread

var customers = await _context.Customers.ToListAsync(); // execute (cost 2s)
var blogs = await _context.Blogs.ToListAsync(); // wait for previous line to done to get executed (cost 3s)
var posts = await _context.Posts.ToListAsync(); // wait for previous line to done to get executed (cost 2s) 
// cost 7s in total
var ids = customers.Select(x => x.Id); // wait for the line "var customers = ..." to complete to get executed
Console.WriteLine("Hello"); // wait until previous line complete to get executed

## Task.WhenAll() - execute concurrently/at the same time
var customersTask = _context.Customers.ToListAsync(); // cost 2s
var blogsTask = _context.Blogs.ToListAsync(); // cost 3s
var postsTask = _context.Posts.ToListAsync(); // cost 2s

await Task.WhenAll(customersTask, blogsTask, postsTask); // cost 3s in total

var customers = await customersTask;
var blogs = await blogsTask;
var posts = await postsTask;

## Using Asynchrony Methods in Foreach Sentences

# Example:

// Ex 1:
class Program
{
    static void Main(string[] args)
    {
        Method1();
        Method2();
        Console.ReadKey();
    }

    public static async Task Method1()
    {
        await Task.Run(() =>
        {
            for (int i = 0; i < 8; i++)
            {
                Console.WriteLine(" Method 1");
                // Do something
                Task.Delay(100).Wait();
            }
        });
    }
    public static void Method2()
    {
        for (int i = 0; i < 4; i++)
        {
            Console.WriteLine(" Method 2");
            // Do something
            Task.Delay(100).Wait();
        }
    }
}
/* Output:
 Method 1
 Method 2
 Method 1
 Method 2
 Method 2
 Method 1
 Method 2
 Method 1
 Method 1
 Method 1
 Method 1
 Method 1  */

// Ex 2:
class Program
{
    static void Main(string[] args)
    {
        callMethod();
        Console.ReadKey();
    }

    public static async void callMethod()
    {
        Task<int> task = Method1();
        Method2();
        int count = await task;
        Method3(count);
    }

    public static async Task<int> Method1()
    {
        int count = 0;
        await Task.Run(() =>
        {
            for (int i = 0; i < 8; i++)
            {
                Console.WriteLine(" Method 1");
                count += 1;
            }
        });
        return count;
    }

    public static void Method2()
    {
        for (int i = 0; i < 4; i++)
        {
            Console.WriteLine(" Method 2");
        }
    }

    public static void Method3(int count)
    {
        Console.WriteLine("Total count is " + count);
    }
}
/* Output:
 Method 1
 Method 1
 Method 1
 Method 1
 Method 1
 Method 1
 Method 1
 Method 1
 Method 2
 Method 2
 Method 2
 Method 2
Total count is 8  */

// Ex 3:
class Program
{
    static async Task Main(string[] args)
    {
        await callMethod();
        Console.ReadKey();
    }

    public static async Task callMethod()
    {
        Method2();
        var count = await Method1();
        Method3(count);
    }

    public static async Task<int> Method1()
    {
        int count = 0;
        await Task.Run(() =>
        {
            for (int i = 0; i < 8; i++)
            {
                Console.WriteLine(" Method 1");
                count += 1;
            }
        });
        return count;
    }

    public static void Method2()
    {
        for (int i = 0; i < 2; i++)
        {
            Console.WriteLine(" Method 2");
        }
    }

    public static void Method3(int count)
    {
        Console.WriteLine("Total count is " + count);
    }
}
/* Output:
 Method 2
 Method 2
 Method 1
 Method 1
 Method 1
 Method 1
 Method 1
 Method 1
 Method 1
 Method 1
Total count is 8 */

# Realtime
Real-time example

Some support APIs from the .NET Framework 4.5, and the Windows runtime contains methods that support async programming.

We can use all of these in the real-time project with the help of async and await keywords for the faster execution of the task.

Some APIs that contain async methods are HttpClient, SyndicationClient, StorageFile, StreamWriter, StreamReader, XmlReader, MediaCapture, BitmapEncoder, BitmapDecoder, etc.

In this example, we will read all the characters from a large text file asynchronously and get the total length of all the characters.

Sample code

class Program
{  ke
    static void Main()
    {
        Task task = new Task(CallMethod);
        task.Start();
        task.Wait();
        Console.ReadLine();
    }

    static async void CallMethod()
    {
        string filePath = "E:\\sampleFile.txt";
        Task<int> task = ReadFile(filePath);

        Console.WriteLine(" Other Work 1");
        Console.WriteLine(" Other Work 2");
        Console.WriteLine(" Other Work 3");

        int length = await task;
        Console.WriteLine(" Total length: " + length);

        Console.WriteLine(" After work 1");
        Console.WriteLine(" After work 2");
    }

    static async Task<int> ReadFile(string file)
    {
        int length = 0;

        Console.WriteLine(" File reading is stating");
        using (StreamReader reader = new StreamReader(file))
        {
            // Reads all characters from the current position to the end of the stream asynchronously
            // and returns them as one string.
            string s = await reader.ReadToEndAsync();

            length = s.Length;
        }
        Console.WriteLine(" File reading is completed");
        return length;
    }
}
C#
In the code given above, we are calling a ReadFile method to read the contents of a text file and get the length of the total characters present in the text file.

In our sampleText.txt, the file contains too many characters, so It will take a long time to read all the characters.

Here, we are using async programming to read all the contents from the file, so it will not wait to get a return value from this method and execute the other lines of code. , However, it still has to wait for the line of code given below because we are using await keywords, and we will use the return value for the line of code below.

int length = await task;
Console.WriteLine(" Total length: " + length);
C#
Subsequently, other lines of code will be executed sequentially. 

Console.WriteLine(" After work 1");
Console.WriteLine(" After work 2");