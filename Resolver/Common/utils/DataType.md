
# Dictionary 

```cs
var students = new Dictionary<int, StudentName>()
{
    { 111, new StudentName { FirstName="Sachin", LastName="Karnik", ID=211 } },
    { 112, new StudentName { FirstName="Dina", LastName="Salimzianova", ID=317 } },
    { 113, new StudentName { FirstName="Andy", LastName="Ruth", ID=198 } }
};
// hoặc:
var students2 = new Dictionary<int, StudentName>()
{
    [111] = new StudentName { FirstName="Sachin", LastName="Karnik", ID=211 },
    [112] = new StudentName { FirstName="Dina", LastName="Salimzianova", ID=317 } ,
    [113] = new StudentName { FirstName="Andy", LastName="Ruth", ID=198 }
};
// hoặc:
var students = new Dictionary<int, StudentName>();
students.Add(111, new StudentName { FirstName="Sachin", LastName="Karnik", ID=211 });
students.Add(112, new StudentName { FirstName="Dina", LastName="Salimzianova", ID=317 });
students.Add(113, new StudentName { FirstName="Andy", LastName="Ruth", ID=198 });
```