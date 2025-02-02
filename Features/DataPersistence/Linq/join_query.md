> https://learn.microsoft.com/en-us/dotnet/csharp/linq/standard-query-operators/join-operations
 
====================================================================
# Join
* -> the join methods provided in the LINQ framework are "Join" and "GroupJoin" 
* -> these methods perform "equijoins", or joins that match two data sources based on equality of their keys

## Choosing "Join" type: the join methods provided in the LINQ framework are "Join" and "GroupJoin"
* in relational database terms, "Join" implements an "inner join"
* "GroupJoin" method has no direct equivalent in relational database terms
* -> "GroupJoin" implements a superset of "inner joins" and "left outer joins" (tức là 2 thằng đó đều có thể viết dưới dạng Group Join)
* -> "left outer join" is a join that returns each element of the first (left) data source, even if it has no correlated elements in the other data source

## OOP
* -> in object-oriented programming, "joining" could mean a correlation between objects that isn't modeled
* -> such as the "backwards direction" of a "one-way relationship"
* => useful for associating elements from different source sequences that have no direct relationship in the object model
* the only requirement is that the elements in each source share some value that can be compared for equality

## one-way relationship
* -> for example, a 'Student' class that has a property of type 'Department' that represents the major,
* -> but the 'Department' class doesn't have a property that is a collection of 'Student' objects
* => so if we have a list of Department objects and we want to find all the students in each department, we could use a join operation to find them

* -> để cần biết lúc nào thì chọn giữa "Left Join" à "Inner Join" thì ta cần nhớ là ta đang muốn lấy ra cái gì
* -> VD: UI ta cần hiển thị grid tất cả "HoSo" thì ta sẽ s/d "Left Join", và không quan tâm 1 số cột thiếu giá trị
* -> còn "Inner Join" thì ta sẽ hiển thị grid danh sách "HoSo" mà các cột đều đầy đủ thông tin
* -> Note: đối với "Left Join" cũng không hẳn là thiếu giá trị, ta có thể cho những cột đó giá trị mặc định
* -> Note: join đòi hỏi phần "on" ta cần để đúng thứ tự bảng trái trước rồi mới "equals" bảng phải

## Inner Join 
* -> produces a "flat sequence"; only those objects that have a match in the other data set are returned

### Single key join

```cs
var query = from student in students
            join department in departments on student.DepartmentID equals department.ID
            select new { Name = $"{student.FirstName} {student.LastName}", DepartmentName = department.Name };
var query = students.Join(departments,
    student => student.DepartmentID, department => department.ID,
    (student, department) => new { Name = $"{student.FirstName} {student.LastName}", DepartmentName = department.Name });

foreach (var item in query)
{
    Console.WriteLine($"{item.Name} - {item.DepartmentName}");
}
```

### Composite key join

```cs
// determine which teachers are also students
IEnumerable<string> query =
    from teacher in teachers
    join student in students on new
    {
        FirstName = teacher.First,
        LastName = teacher.Last
    } equals new
    {
        student.FirstName,
        student.LastName
    }
    select teacher.First + " " + teacher.Last;
IEnumerable<string> query = teachers
    .Join(students,
        teacher => new { FirstName = teacher.First, LastName = teacher.Last },
        student => new { student.FirstName, student.LastName },
        (teacher, student) => $"{teacher.First} {teacher.Last}"
);
foreach (string name in query)
{
    Console.WriteLine(name);
}
```

### Multiple join

```cs
// -> each "join" clause in C# correlates a specified data source with the results of the previous join

// the first join returns a sequence of "anonymous types" that contain the "Student object" and "Department object"
// the second join correlates the "anonymous types" returned by the first join with Teacher objects 
// It returns a sequence of anonymous types that contain the student's name, the department name, and the teacher's name
var query = from student in students
    join department in departments on student.DepartmentID equals department.ID
    join teacher in teachers on department.TeacherID equals teacher.ID
    select new {
        StudentName = $"{student.FirstName} {student.LastName}",
        DepartmentName = department.Name,
        TeacherName = $"{teacher.First} {teacher.Last}"
    };
var query = students
    .Join(departments, student => student.DepartmentID, department => department.ID,
        (student, department) => new { student, department })
    .Join(teachers, commonDepartment => commonDepartment.department.TeacherID, teacher => teacher.ID,
        (commonDepartment, teacher) => new
        {
            StudentName = $"{commonDepartment.student.FirstName} {commonDepartment.student.LastName}",
            DepartmentName = commonDepartment.department.Name,
            TeacherName = $"{teacher.First} {teacher.Last}"
        });
```

### Implement "Inner join" by using "Grouped join"

```cs
// The group join creates a collection of intermediate groups, where each group consists of "a Department object" and a sequence of "matching Student objects"
// The second "from" clause combines (or flattens) this sequence of sequences into one longer sequence
// The "select" clause specifies the type of elements in the final sequence
var query1 =
    from department in departments
    join student in students on department.ID equals student.DepartmentID into gj
    from subStudent in gj
    select new
    {
        DepartmentName = department.Name,
        StudentName = $"{subStudent.FirstName} {subStudent.LastName}"
    };
var query1 = departments
    .GroupJoin(students, department => department.ID, student => student.DepartmentID,
        (department, gj) => new { department, gj })
    .SelectMany(departmentAndStudent => departmentAndStudent.gj,
        (departmentAndStudent, subStudent) => new
        {
            DepartmentName = departmentAndStudent.department.Name,
            StudentName = $"{subStudent.FirstName} {subStudent.LastName}"
        });
var query1 = from department in departments
    join student in students on department.ID equals student.DepartmentID
    select new
    {
        DepartmentName = department.Name,
        StudentName = $"{student.FirstName} {student.LastName}"
    };
var query1 = departments.Join(students, departments => departments.ID, student => student.DepartmentID,
    (department, student) => new
    {
        DepartmentName = department.Name,
        StudentName = $"{student.FirstName} {student.LastName}"
    });

Console.WriteLine("Inner join using GroupJoin():");
foreach (var v in query1)
{
    Console.WriteLine($"{v.DepartmentName} - {v.StudentName}");
}
```

## Group Join - a "join" clause with an "into" expression

```cs
// -> useful for produces a "hierarchical result sequence"
// -> "pairs" "each element from the first collection" with "a set of correlated elements from the second collection"
// => each element of the "first collection appears in the result set" of a group join regardless of whether correlated elements are found in the second collection (the sequence of correlated elements for that element is empty)
// => the "result selector" therefore has access to "every element of the first collection"
// => differs from the "result selector" in a non-group join, which cannot access elements from the first collection that have no match in the second collection

// unlike a non-group join, which produces a pair of elements for each match, 
// the group join produces only one resulting object for each element of the first collection, which is "Department" object
// the corresponding elements from the second collection - "Student" objects, are grouped into a collection
IEnumerable<IEnumerable<Student>> studentGroups = from department in departments
                    join student in students on department.ID equals student.DepartmentID into studentGroup
                    select new
                    {
                        DepartmentName = department.Name,
                        Students = studentGroup
                    };
// Output: 
// -> the "result selector function" creates an "a list" where each element is an "anonymous type" 
// -> that contains "the department's name" and "a collection of students in that department"

IEnumerable<IEnumerable<Student>> studentGroups = departments.GroupJoin(
    students,
    department => department.ID, 
    student => student.DepartmentID,
    (department, studentGroup) => studentGroup
);

foreach (IEnumerable<Student> studentGroup in studentGroups)
{
    Console.WriteLine($"{studentGroup.DepartmentName}:");
    foreach (Student student in studentGroup)
    {
        Console.WriteLine($"  - {student.FirstName}, {student.LastName}");
    }
}
```

## Left Outer Join (Left Join):  calling the "DefaultIfEmpty() method" on the results of "a group join"

```cs
// is a join in which each element of the first collection is returned, 
// regardless of whether it has any correlated elements in the second collection

// Process:
// the first step in producing a left outer join of two collections is to perform an "inner join" by using "a group join"
// the second step, include each element of the first (left) collection in the result set even if that element has no matches in the right collection
// -> accomplished by calling DefaultIfEmpty on each sequence of matching elements from the group join

// "DefaultIfEmpty" method returns a collection that contains a single, default value if the sequence of matching Student objects is empty for any Department object, 
// ensuring that each "Department" object is represented in the result collection
var query =
    from student in students
    join department in departments on student.DepartmentID equals department.ID into gj
    from subgroup in gj.DefaultIfEmpty()
    select new
    {
        student.FirstName,
        student.LastName,
        Department = subgroup?.Name ?? string.Empty //  checks for a "null" before accessing each element of each "Student" collection
    };
var query = students.GroupJoin(departments, student => student.DepartmentID, department => department.ID,
    (student, department) => new { student, subgroup = department.DefaultIfEmpty() })
    .Select(gj => new
    {
        gj.student.FirstName,
        gj.student.LastName,
        Department = gj.subgroup?.FirstOrDefault()?.Name ?? string.Empty
    });

foreach (var v in query)
{
    Console.WriteLine($"{v.FirstName:-15} {v.LastName:-15}: {v.Department}");
}

// Nếu Table1 có columnA với 2 record có giá trị bằng 5; Table2 có columnB với 3 record có giá trị bằng 5
// Nếu ta join 2 bảng này với "Table1.columnA = Table2.columnB"
// Ta sẽ có 1 result với 6 record
```

## Join relationship Table:

```cs
// Trong trường hợp Bảng 1 có Record thoã mãn "Where()", nhưng Bảng 2 không tồn tại Record link đến Bảng 1; 
// nhưng ta vẫn luôn lấy ra những dữ liệu thoã mãn, còn dữ liệu không thoả mãn thì ta cho giá trị mặc định

var qHoSoCongViec = _context.HoSoCongViecs.Where(x => !x.IsDeleted).AsQueryable();
var qDanhSachHoSo = _context.DanhSachHoSos.Include(x => x.HoSoCongViec).AsQueryable();

qDanhSachHoSo = qDanhSachHoSo.Where(_ => _.LoaiDanhSachHoSo == request.LoaiDanhSachHoSo);
if (request.NguoiLapId > 0) qHoSoCongViec = qHoSoCongViec.Where(_ => _.NguoiLapId == request.NguoiLapId);

var result = from hoSoCongViec in qHoSoCongViec
                join hoSo in qDanhSachHoSo on hoSoCongViec.Id equals hoSo.HoSoCongViecID into hoSoGroup
                from hoSo in hoSoGroup.DefaultIfEmpty()
                join user in _context.UserMasters on hoSo.HoSoCongViec.NguoiLapId equals user.UserMasterId into hoSo_user
                from user in hoSo_user.DefaultIfEmpty()
                select new
                {
                    hoSoCongViec.Id,
                    Status = hoSo.Status ?? HoSoStatus.KhoiTao,
                };

return await result.ToListAsync();
```

### DefaultIfEmpty()
* -> trả về 1 collection mới với 1 phần tử có giá trị mặc định nếu collection gốc rỗng; còn không thì trả về collection gốc

```cs
IList<string> emptyList = new List<string>();
var newList1 = emptyList.DefaultIfEmpty(); 
Console.WriteLine("Count: {0}" , newList1.Count()); // 1
Console.WriteLine("Value: {0}" , newList1.ElementAt(0)); // null

List<Pet> pets = new List<Pet>{ new Pet { Name="Barley", Age=8 }, new Pet { Name="Boots", Age=4 } };
foreach (Pet pet in pets.DefaultIfEmpty())
{
    Console.WriteLine(pet.Name);
}
/*
 Barley
 Boots
*/
```
