# Transient
services.AddTransient<ICounter, Counter>();
services.AddTransient<IFirstCounter, FirstCounter>();
services.AddTransient<ISecondCounter, SecondCounter>();
// Every time call API 
// -> the "FirstCounter" and the "SecondCounter" classes ask for an "ICouner" instance
// -> it will always create a new instance of "Counter"
// -> calling the /api/get will always result in 1


# Scoped
services.AddScoped<ICounter, Counter>();
services.AddTransient<IFirstCounter, FirstCounter>();
services.AddTransient<ISecondCounter, SecondCounter>();
// for the call inside of the Get method of the CountController
// -> when the instance of FirstCounter class and SecondCounter asks for ICounter instance
// -> it will provide the same instance to both the classes
// -> But across two different HTTP requests, they are not the same
// -> calling the /api/get will always result in 2


# Singleton
services.AddSingleton<ICounter, Counter>();
services.AddTransient<IFirstCounter, FirstCounter>();
services.AddTransient<ISecondCounter, SecondCounter>();
// the ICounter instance is shared between the FirstCounter and the SecondCounter class for every single HTTP request
// no matter how many times execute the HTTP request from the browser, the same object of Counter will be returned to the FirstCounter and SecondCounter
// -> every time we make a call, the count is going to increment by 2
// -> he output of the call will be 2, but next time it will be 4, and then it will be 6,...



// Controller
[Route("api/[controller]")]
[ApiController]
public class CountController : ControllerBase
{
    private readonly IFirstCounter firstCounter;
    private readonly ISecondCounter secondCounter;

    public CountController(IFirstCounter firstCounter, ISecondCounter secondCounter)
    {
        this.firstCounter = firstCounter;
        this.secondCounter = secondCounter;
    }

    // GET: api/<CountController>
    [HttpGet]
    public int Get()
    {
        firstCounter.IncrementAndGet();
        return secondCounter.IncrementAndGet();
    }
}

// Service
public interface ICounter
{
    int Get();
    void Increment();
}
public class Counter : ICounter
{
    private int count;

    public void Increment() => count++;

    public int Get() => count;
}

// Service Dependency 1
public class FirstCounter : IFirstCounter
{
    private readonly ICounter counter;
    public FirstCounter(ICounter counter) // DI service
    {
        this.counter = counter;
    } 

    public int IncrementAndGet()
    {
        counter.Increment();
        return counter.Get();
    }
}

// Service Dependency 2
public class SecondCounter : ISecondCounter
{
    private readonly ICounter counter;
    public SecondCounter(ICounter counter) // DI service
    {
        this.counter = counter;
    }
    public int IncrementAndGet()
    {
        counter.Increment();
        return counter.Get();
    }
}