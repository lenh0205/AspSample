
# Aggregate
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

# SelectMany
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