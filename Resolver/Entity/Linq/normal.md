=========================================================================
# ,Aggregate()
* -> nó cho phép ta tích luỹ qua từng lần lặp (_giống reduce trong javascript_)

```cs
// input: x=3, y=3 // output: 0,0 0,1 0,2 1,0 1,1 1,2 2,0 2,1 2,2
public IActionResult LayTatCaToaDo(int x, int y)
{
    var xRange = Enumerable.Range(0, x);
    var yRange = Enumerable.Range(0, y);
    var result = xRange.Aggregate(" ", (acc, curr) =>
    {
        var item = curr + "," + string.Join(" " + curr.ToString() + ",", yRange);
        return acc + item + " ";
    });
    return Ok(result);
} 
```

=========================================================================
# .SelectMany()
* -> **`flattens queries that return lists of lists`**
```cs
public class PhoneNumber
{
    public string Number { get; set; }
}

public class Person
{
    public IEnumerable<PhoneNumber> PhoneNumbers { get; set; }
    public string Name { get; set; }
}

IEnumerable<Person> people = new List<Person>()
{
    new Person() { 
        Name = "A", 
        PhoneNumbers = new List<PhoneNumber> () { new PhoneNumber() { Number = "123" }, new PhoneNumber() { Number = "234" }} 
    },
    new Person() { 
        Name = "B", 
        PhoneNumbers = new List<PhoneNumber> () { new PhoneNumber() { Number = "345" }, new PhoneNumber() { Number = "456" }}
    },
    new Person() { 
        Name = "C", 
        PhoneNumbers = new List<PhoneNumber> () { new PhoneNumber() { Number = "567" }, new PhoneNumber() { Number = "678" }}
    }
};

IEnumerable<IEnumerable<PhoneNumber>> phoneLists = people.Select(p => p.PhoneNumbers);
IEnumerable<PhoneNumber> phoneNumbers = people.SelectMany(p => p.PhoneNumbers);
var directory = people.SelectMany(p => p.PhoneNumbers, (parent, child) => new { parent.Name, child.Number });

return Ok(new
{
    phoneLists,
    phoneNumbers,
    directory
});

// Output:
{
  "phoneLists": [
    [{ "number": "123" }, { "number": "234" }],
    [{ "number": "345" }, { "number": "456" }],
    [{ "number": "567" }, { "number": "678" }]
  ],
  "phoneNumbers": 
    [{"number": "123"}, {"number": "234"}, {"number": "345"}, {"number": "456"}, {"number": "567"}, {"number": "678"}],
  "directory": [
    {"name": "A", "number": "123"}, 
    {"name": "A", "number": "234"},
    {"name": "B", "number": "345"},
    {"name": "B", "number": "456"},
    {"name": "C", "number": "567"},
    {"name": "C", "number": "678"}
  ]
}
```

* -> we can also use it like **`SQL cross join`** 
* _tức là ta có thể tạo **all the possible combinations** from the elements of 2 data set_
* _Ví dụ: với "Set A={a,b,c}" và "Set B={x,y}" thì `.SelectMany()` can be used to get the following set: { (x,a) , (x,b) , (x,c) , (y,a) , (y,b) , (y,c) }_

```cs
List<string> animals = new List<string>() { "cat", "dog", "donkey" };
List<int> number = new List<int>() { 10, 20 };

var mix = number.SelectMany(num => animals, (n, a) => new { n, a });
return Ok(mix);

// Output:
response = [
  {"n": 10, "a": "cat" },
  { "n": 10, "a": "dog" },
  { "n": 10, "a": "donkey" },
  { "n": 20, "a": "cat" },
  { "n": 20, "a": "dog" },
  { "n": 20, "a": "donkey" }
]
```

=========================================================================
# Enumerable.Zip()
* -> **`merges`** each element of the **first sequence** with an element that has the **`same index`** in the **second sequence**

* -> this method is implemented by using **`deferred execution`** - the **query represented by this method is not executed until the object is enumerated** (_either by calling its **GetEnumerator** method directly or by using **foreach**_)
* -> if the sequences do not have the same number of elements, the method merges sequences until it reaches the end of one of them (`tức là 1 thằng có 3 phần tử, 1 thằng có 4 phần tử thì kết quả sẽ có 3 phần tử`)

```cs
int[] numbers = { 1, 2, 3, 4 };
string[] words = { "one", "two", "three" };

IEnumerable<string> numbersAndWords = numbers.Zip(words, (first, second) => first + " " + second);
// Output: ["1 one", "2 two", "3 three"]

IEnumerable<string> wordsAndNumbers = words.Zip(numbers, (first, second) => first + " " + second);
// Output: ["one 1", "two 2", "three 3"]
```

=========================================================================
# .ToLookup()
* -> similar to a Dictionary but the one advantage that it can contain **duplicate keys**
* -> receive 2 parameters: first is lambda to specify the field as **`Key for lookup`**; second lambda to specify the field we want to take as **`final values base on lookup key`**

```cs
var persons = new List<Person>
{
    new Person { PersonId = 1, Car = "abc" },
    new Person { PersonId = 3, Car = "def" },
    new Person { PersonId = 1, Car = "egh" },
    new Person { PersonId = 5, Car = "jkm" },
    new Person { PersonId = 1, Car = "erp" },
    new Person { PersonId = 3, Car = "uhk" },
};
ILookup<int,string> carsByPersonId = persons.ToLookup(p => p.PersonId, p => p.Car);
IEnumerable<string>? result = carsByPersonId[1]; // ["abc", "egh", "erp"]

public class Person
{
    public int PersonId { get; set; }
    public string Car { get; set; } = string.Empty;
}
```

```cs - loop
public static void ToLookupEx1()
{
  List<Package> packages = new List<Package>
  { 
    new Package { Company = "Coho Vineyard", Weight = 25.2, TrackingNumber = 89453312L },
    new Package { Company = "Lucerne Publishing", Weight = 18.7, TrackingNumber = 89112755L },
    new Package { Company = "Wingtip Toys", Weight = 6.0, TrackingNumber = 299456122L },
    new Package { Company = "Contoso Pharmaceuticals", Weight = 9.3, TrackingNumber = 670053128L },
    new Package { Company = "Wide World Importers", Weight = 33.8, TrackingNumber = 4665518773L } 
  };

  ILookup<char, string> lookup = packages.ToLookup(
                                  p => p.Company[0], // use the first character of Company as the key value
                                  p => p.Company + " " + p.TrackingNumber); // element values

  // Iterate through each IGrouping in the Lookup.
  foreach (IGrouping<char, string> packageGroup in lookup)
  {
      Console.WriteLine(packageGroup.Key); // key value

      // Iterate through each value in the IGrouping 
      foreach (string str in packageGroup)
          Console.WriteLine("    {0}", str);
    }
}

/* -----------> Output:
 C
     Coho Vineyard 89453312
     Contoso Pharmaceuticals 670053128
 L
     Lucerne Publishing 89112755
 W
     Wingtip Toys 299456122
     Wide World Importers 4665518773
*/
```