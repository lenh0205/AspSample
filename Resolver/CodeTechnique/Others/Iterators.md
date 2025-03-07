> https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/iterators
> https://stackoverflow.blog/2022/06/15/c-ienumerable-yield-return-and-lazy-evaluation/

=================================================================================
# Iterators 
* -> enable us to **`maintain the simplicity of a 'foreach' loop`** when we **`need to use complex code to populate a list sequence`**

* _this can be useful when we want to do the following:_
* _modify the list sequence after the first foreach loop iteration_
* _avoid fully loading a large list before the first iteration of a foreach loop. An example is a paged fetch to load a batch of table rows. Another example is the EnumerateFiles method, which implements iterators in .NET_
* _encapsulate building the list in the iterator. In the iterator method, you can build the list and then yield each result in a loop_

## Usage
* -> **an iterator** can occur as **`a method`** or **`get accessor`** (_an iterator cannot occur in an event, instance constructor, static constructor, or static finalizer_)
* -> we **`consume an iterator from client code`** by using a **`foreach`** statement (_or by using a **`LINQ query`**_)
* -> we can use a **`yield break`** statement to **end the iteration**
* -> từ **yield** chỉ có thể được dùng trước từ **return** hoặc **break** mà thôi

## Mechanism
* -> An **iterator method** uses the **`yield return`** statement to return each element one at a time 
* -> the **return type of an "iterator method" or "get accessor"** can be **`IEnumerable, IEnumerable<T>, IEnumerator, or IEnumerator<T>`**
* -> when a "yield return" statement is reached, the **`current location in code is remembered`**; so the execution is **`restarted from that location the next time the iterator function is called`**
* => on each successive iteration of the foreach loop (or the direct call to **`IEnumerator.MoveNext`**), the next iterator code body resumes after the previous "yield return" statement
* => it then continues to the next yield return statement until **the end of the iterator body is reached**, or until **a 'yield break' statement is encountered**

```cs
static void Main()
{
    // khi foreach gọi SomeNumbers() nó sẽ trả về 1 IEnumerable, và foreach sẽ lặp qua IEnumerable này
    foreach (int number in SomeNumbers())
    {
        Console.Write(number.ToString() + " ");
    }
    // Output: 3 5 8
    Console.ReadKey();
}

// this is a iterator method
public static System.Collections.IEnumerable SomeNumbers()
{
    yield return 3;
    yield return 5;
    yield return 8;
}

// -> foreach lặp qua IEnumerable giống như lặp qua 1 collection bình thường, nhưng cách thức hơi khác 1 xíu
// -> the first iteration of the foreach loop causes execution to proceed in the SomeNumbers iterator method until the first yield return statement is reached (_foreach sẽ chạy code bên trong SomeNumber từ trên xuống dưới đến khi gặp "yield return"_)
// -> This iteration returns a value of 3, and the current location in the iterator method is retained
// -> On the next iteration of the loop, execution in the iterator method continues from where it left off, again stopping when it reaches a yield return statement
// -> This iteration returns a value of 5, and the current location in the iterator method is again retained
// -> The loop completes when the end of the iterator method is reached
```

=================================================================================
## Technical Implementation
* -> although we **write an iterator as a method**, the compiler translates it into a **`nested class`** that is, in effect, a state machine; this class **`keeps track of the position of the iterator`** as long the foreach loop in the client code continues

* -> to see what the compiler does, we can use the **`Ildasm.exe`** tool to **view the common intermediate language code** that's generated for an iterator method

* -> when we **create an iterator for a "class" or "struct"**, we **don't have to implement the whole IEnumerator interface**; when the compiler detects the iterator, it **`automatically generates the Current, MoveNext, and Dispose methods of the IEnumerator or IEnumerator<T> interface`**
* -> a **class** implements the **`IEnumerable`** interface, which requires a **`GetEnumerator`** method (implementation)
* -> the compiler implicitly calls the "GetEnumerator" method (when we create an instance from this class), which returns an **`IEnumerator`** (as an instance for this class)


* -> Iterators don't support the IEnumerator.Reset method. To reiterate from the start, we **`must obtain a new iterator`**. Calling Reset on the iterator returned by an iterator method throws a NotSupportedException.

## Simple Iterator
```cs
// -> a single 'yield return' statement that is inside a 'for' loop
// -> in Main, each iteration of the foreach statement body creates a call to the iterator function, which proceeds to the next "yield return" statement

static void Main()
{
    var myIter = EvenSequence(5, 18);
    foreach (int number in myIter)
    {
        Console.Write(number.ToString() + " ");
    }
    
    Console.ReadKey(); // Output: 6 8 10 12 14 16 18
}

public static IEnumerable<int> EvenSequence(int firstNumber, int lastNumber)
{
    // Yield even numbers in the range.
    for (int number = firstNumber; number <= lastNumber; number++)
    {
        if (number % 2 == 0)
        {
            yield return number;
        }
    }
}
```

## Creating a Collection Class

```cs
static void Main()
{
    DaysOfTheWeek days = new DaysOfTheWeek();

    foreach (string day in days)
    {
        Console.Write(day + " ");
    }
    
    Console.ReadKey(); // Output: Sun Mon Tue Wed Thu Fri Sat
}

public class DaysOfTheWeek : IEnumerable
{
    private string[] days = ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"];

    public IEnumerator GetEnumerator()
    {
        for (int index = 0; index < days.Length; index++)
        {
            // Yield each day of the week.
            yield return days[index];
        }
    }
}
```

```cs
static void Main()
{
    Zoo theZoo = new Zoo();

    theZoo.AddMammal("Whale");
    theZoo.AddMammal("Rhinoceros");
    theZoo.AddBird("Penguin");
    theZoo.AddBird("Warbler");

    // the "foreach" statement that refers to the class instance (theZoo) implicitly calls the "GetEnumerator" method
    foreach (string name in theZoo)
    {
        Console.Write(name + " ");
    }
    Console.WriteLine(); // Output: Whale Rhinoceros Penguin Warbler

    // the "foreach" statements that refer to the Birds and Mammals properties use the "AnimalsForType" named iterator method
    foreach (string name in theZoo.Birds)
    {
        Console.Write(name + " ");
    }
    Console.WriteLine(); // Output: Penguin Warbler

    foreach (string name in theZoo.Mammals)
    {
        Console.Write(name + " ");
    }
    Console.WriteLine(); // Output: Whale Rhinoceros

    Console.ReadKey();
}

public class Zoo : IEnumerable
{
    // Private members.
    private List<Animal> animals = new List<Animal>();

    // Public methods.
    public void AddMammal(string name)
    {
        animals.Add(new Animal { Name = name, Type = Animal.TypeEnum.Mammal });
    }

    public void AddBird(string name)
    {
        animals.Add(new Animal { Name = name, Type = Animal.TypeEnum.Bird });
    }

    public IEnumerator GetEnumerator()
    {
        foreach (Animal theAnimal in animals)
        {
            yield return theAnimal.Name;
        }
    }

    // Public members.
    public IEnumerable Mammals
    {
        get { return AnimalsForType(Animal.TypeEnum.Mammal); }
    }

    public IEnumerable Birds
    {
        get { return AnimalsForType(Animal.TypeEnum.Bird); }
    }

    // Private methods.
    private IEnumerable AnimalsForType(Animal.TypeEnum type)
    {
        foreach (Animal theAnimal in animals)
        {
            if (theAnimal.Type == type)
            {
                yield return theAnimal.Name;
            }
        }
    }

    // Private class.
    private class Animal
    {
        public enum TypeEnum { Bird, Mammal }

        public string Name { get; set; }
        public TypeEnum Type { get; set; }
    }
}
```

## Using Iterators with a Generic List
* _the example uses **named iterators** to support various ways (**`iterator method`** or **`get accessor`**) of iterating through the same collection of data_
* _these named iterators are the "TopToBottom" and "BottomToTop" properties, and the "TopN" method_
* _Ex: the "BottomToTop" property uses an iterator in a **get accessor**_

```cs
// -> the "Stack<T>" generic class implements the "IEnumerable<T>" generic interface
// -> the generic "GetEnumerator" method returns the array values by using the "yield return" statement
// -> in addition to the "generic GetEnumerator method"; the "non-generic GetEnumerator method" must also be implemented because IEnumerable<T> inherits from IEnumerable
// -> the non-generic implementation defers to the generic implementation

static void Main()
{
    Stack<int> theStack = new Stack<int>();

    //  Add items to the stack.
    for (int number = 0; number <= 9; number++)
    {
        theStack.Push(number);
    }

    // Retrieve items from the stack.
    // foreach is allowed because theStack implements IEnumerable<int>.
    foreach (int number in theStack)
    {
        Console.Write("{0} ", number);
    }
    Console.WriteLine();
    // Output: 9 8 7 6 5 4 3 2 1 0

    // foreach is allowed, because theStack.TopToBottom returns IEnumerable(Of Integer).
    foreach (int number in theStack.TopToBottom)
    {
        Console.Write("{0} ", number);
    }
    Console.WriteLine();
    // Output: 9 8 7 6 5 4 3 2 1 0

    foreach (int number in theStack.BottomToTop)
    {
        Console.Write("{0} ", number);
    }
    Console.WriteLine();
    // Output: 0 1 2 3 4 5 6 7 8 9

    foreach (int number in theStack.TopN(7))
    {
        Console.Write("{0} ", number);
    }
    Console.WriteLine();
    // Output: 9 8 7 6 5 4 3

    Console.ReadKey();
}

public class Stack<T> : IEnumerable<T>
{
    private T[] values = new T[100];
    private int top = 0;

    public void Push(T t)
    {
        values[top] = t;
        top++;
    }
    public T Pop()
    {
        top--;
        return values[top];
    }

    // This method implements the GetEnumerator method. It allows
    // an instance of the class to be used in a foreach statement.
    public IEnumerator<T> GetEnumerator()
    {
        for (int index = top - 1; index >= 0; index--)
        {
            yield return values[index];
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IEnumerable<T> TopToBottom
    {
        get { return this; }
    }

    public IEnumerable<T> BottomToTop
    {
        get
        {
            for (int index = 0; index <= top - 1; index++)
            {
                yield return values[index];
            }
        }
    }

    public IEnumerable<T> TopN(int itemsFromTop)
    {
        // Return less than itemsFromTop if necessary.
        int startIndex = itemsFromTop >= top ? 0 : top - itemsFromTop;

        for (int index = top - 1; index >= startIndex; index--)
        {
            yield return values[index];
        }
    }

}
```
=================================================================================
# Extension

## IEnumerable 
* -> enables **iterating over a collection** (_ta có thể nghĩ về nó như list hoặc array_)
* -> is the **return type from an iterator**
* => we can use a **`foreach`** statement to loop through it,
* => use **`LINQ`** to map or reduce it in a hundred different ways
* => can explicitly cast it to an array with **`.ToArray()`** and access elements by **`index`**

```cs
IEnumerable<int> GetOneTwoThree() {
  yield return 1;
  yield return 2;
  yield return 3;
  // We could put "yield break;" here but there's no need, the end of the function signals the same thing.
}
var numbers = GetOneTwoThree();

foreach (var number in numbers) {
  Console.WriteLine(number);
  // Output:
  // 1
  // 2
  // 3
}

var doubledNumbers = numbers.Select(num => num * 2);

foreach (var number in doubledNumbers) {
  Console.WriteLine(number);
  // Output:
  // 2
  // 4
  // 6
}

var numberArray = numbers.ToArray();
Console.WriteLine(numberArray[0]); // Output: 1
```

## Lazy evaluation
* -> is when we **wait to execute a piece of code until we absolutely, positively have to**
* -> when we **call iterator method**, we'll **`get a return value despite the fact that none of the code in the function has actually been executed yet`**

```cs
// LinqPad
bool didTheCodeRun = false;

IEnumerable<bool> RunTheCode() {
  didTheCodeRun = true;
  yield return true;
}

void Main() {
  var results = RunTheCode();
  didTheCodeRun.Dump(); // false => None of the code in our iterator runs 
}
```

### benefit
* -> if we **never end up iterating through the IEnumerable** at all then **`a bunch of code the computer didn't have to execute`**
* -> if we're using a LINQ method like **.First()** to try to find a specific item in the collection; we **`don't need to run all the code in the iterator`**, once .First() finds a value that matches the predicate it will stop iterating
* => if we're working with an IEnumerable that potentially has thousands of values (or more), we can **`save a lot of CPU cycles`** by **`only iterating as far as we need to`**

```cs
// Here's a variable to track execution of code in an iterator
int lastYielded = -1;

// Here's an iterator for us to play with
IEnumerable<int> GetOneToTen() {
  for (var num = 1; num <= 10; num++) {
    lastYielded = num;
    yield return num;
  }
}

void Main() {
  var numbers = GetOneToTen();
  lastYielded.Dump(); // Output: -1

  // This gives us an 'instance' of the iteration
  var enumerator = numbers.GetEnumerator();

  // This executes the iterator until the first yield return is reached
  enumerator.MoveNext();

  // This gives us the current (most recently yielded) value of the iterator
  enumerator.Current.Dump(); // Output: 1
  lastYielded.Dump(); // Output: 1

  // This will iterate from 1 to 4, then stop
  foreach (var num in numbers) {
    if (num >= 4) {
      break;
    }
  }

  lastYielded.Dump(); // Output: 4

  // This will not execute any code in the iterator.
  //  LINQ methods are lazily evaluated as well
  var numbersTimesTwo = numbers.Select(num => num * 2);
  lastYielded.Dump(); // Output: 4

  // This will force the entire iterator to run, yielding all values
  var arr = numbers.ToArray();
  lastYielded.Dump(); // Output: 10
}
```

## Rules when implement IEnumerable 
* -> try to avoid **`side effects**` when writing an iterator method**`
* -> avoid **`iterating over the same IEnumerable multiple times`**
* _if we know we're going to access every value (for example, **iterating it using a foreach with no break or return**), we should **`enumerate it sooner rather than later by coercing the IEnumerable to a list or array type`**; then **use that list or array for all future operations**_

### Side Effect
 Anyone who uses the method should be able to treat it as though it synchronously returns an array. If an iterator changes or executes anything outside of itself, the caller may end up confused. This applies to functions passed to LINQ methods as well, since many of them return IEnumerable and are lazily evaluated.

### iterating over the same IEnumerable multiple times
* _it’s important to point out that many iterators are not as simple as the ones we've been using here_
* -> an iterator could **query a database**, for example - including the unfortunate possibility that it might **alter data**, or that iterating through it twice might **`yield completely different results!`**
* => some tools (like ReSharper) will warn us against multiple enumeration for this reason
* => an **iterator** is, from one perspective, nothing more than a synchronous method that may not execute its code right away (or at all)
* => however, to muddy the waters just a little, **not all iterators are synchronous**; there’s also an **`IAsyncEnumerable`** interface (_we can **loop through it with "await" foreach**_)

* -> most of the time none of this is a problem, almost all the time we can treat an IEnumerable like a list; but we shouldn't rely on this
* -> Ex: we may use .Select() to transform a collection while using an external variable to track a small piece of state on each iteration, then got very confused when the external variable wasn't updated later in the method
* => in the end, we will fix the problem by **`forcing the iteration to complete with .ToArray()`** (_there are multiple ways to approach something like this, depending on the expected size of the collection and what we're trying to track_)
