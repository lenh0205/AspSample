====================================================================
# GroupBy

```cs
// trả về IEnumerable<Grouping>

// khi ta query ra 1 list bao gồm nhiều Row mà có 1 hoặc nhiều field nào đó giống nhau 
// -> thì để tránh nó ta có 2 cách là "DistinceBy()" hoặc "GroupBy()"
// -> nhưng "DistinceBy(x => x.Field)" sẽ làm mất đi các Row có field chỉ định giống nhau và chỉ trả về 1 Row với đầy đủ field
// -> còn "GroupBy(x => new { x.Field1, x.Field2 })" cũng sẽ làm mất đi các Row có field chỉ định giống nhau và trả về 1 Row với chỉ field ta đã chỉ định
// => nhưng "GroupBy()" có thể giúp ta thực hiện những "Aggregate function" trong từng group

// khi ".Select()" ta sẽ cần truy cập các field thông qua ".Key."

var persons = new List<Person>
{
    new Person { PersonId = 1, Car = "abc" },
    new Person { PersonId = 3, Car = "def" },
    new Person { PersonId = 1, Car = "egh" },
    new Person { PersonId = 5, Car = "jkm" },
    new Person { PersonId = 1, Car = "erp" },
    new Person { PersonId = 3, Car = "uhk" },
};

IEnumerable results = from p in persons
              group p.Car by p.PersonId into g 
              select new { PersonId = g.Key, Cars = g.ToList() };
// -> nhóm dựa trên "PersonId" giống nhau để tạo thành thành các "Key" riêng biệt
// -> mỗi Key sẽ ứng với 1 list "p.Car" 

// hoặc
var results = persons.GroupBy(
    p => p.PersonId, // nếu muốn nhóm dựa trên 2 cột VD: p => new { p.PersonId, p.Car}
    p => p.car,
    (key, g) => new { PersonId = key, Cars = g.ToList() });

output = [
  {
    "personId": 1,
    "cars": ["abc", "egh", "erp"]
  },
  {
    "personId": 3,
    "cars": ["def", "uhk"]
  },
  {
    "personId": 5,
    "cars": ["jkm"]
  }
]
```

## with Aggregate function

```cs
foreach
(
    var line in data
        .GroupBy(info => info.metric)
        .Select(group => new { Metric = group.Key, Count = group.Count() })
        .OrderBy(x => x.Metric)
)
{
     Console.WriteLine("{0} {1}", line.Metric, line.Count);
}

var consolidatedChildren = from c in children
                            group c by new { c.School, c.Friend, c.FavoriteColor, } into gcs
                            select new ConsolidatedChild()
                            {
                                School = gcs.Key.School,
                                Friend = gcs.Key.Friend,
                                FavoriteColor = gcs.Key.FavoriteColor,
                                Children = gcs.ToList(),
                            };
var consolidatedChildren = children
                            .GroupBy(c => new{ c.School, c.Friend, c.FavoriteColor, })
                            .Select(gcs => new ConsolidatedChild()
                            {
                                School = gcs.Key.School,
                                Friend = gcs.Key.Friend,
                                FavoriteColor = gcs.Key.FavoriteColor,
                                Children = gcs.ToList(),
                            });

// -----> case phức tạp:  
// -> nhóm những record có trường "TenFile" giống nhau trong list "lstTepDinhKem", 
// -> lấy aggregate max value "PhienBan" của từng nhóm, 
// -> đồng thời lấy "DinhKemID" của record chứa max value đó
 var lstTepDinhKem1 = lstTepDinhKem.GroupBy(x => x.TenFile, (key, xs) => new { 
        TenFile = key,
        PhienBan = xs.Max(xs => xs.PhienBan),
        DinhKemID = xs.OrderByDescending(x => x.PhienBan).First().DinhKemID,
 }).ToList();

 // hoặc
 var maxes = list.GroupBy(x => x.id2, (id, xs) => xs.Max(x => x.value));
 // hoặc
var maxes = from x in list
            group x.value by x.id2 into values
            select new 
            {
                id = values.Key
                Max = values.Max();
            }
 // hoặc
 var maxes = list.GroupBy(x => x.id2, x => x.value).Select(values => values.Max());
 // hoặc
 var maxes = list.GroupBy(x => x.id2,     // Key selector
                         x => x.value,   // Element selector
                         (key, values) => values.Max()); // Result selector
// hoặc
var maxes = list.GroupBy(x => x.id2).Select(xs => xs.Select(x => x.value).Max())
```

====================================================================
# Note
* -> lưu ý nếu ta chỉ groupby bằng 1 trường thì không thể "SELECT *" - vì nó đỏi hỏi ta cần groupby tất cả các trường để có thể select tất cả trường 