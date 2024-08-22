# Performance
## Case: Filter trước khi join bảng tốt hơn ra join bảng ra kết quả rồi mới filter
// có vẻ là việc mình join các bảng trước thành 1 queryable trước; 
// rồi phần filter cho từng bảng mình đặt hết lên cùng 1 cái queryable đó
// -> thì sẽ dễ control cũng như preditable hơn

## Case: ta có 2 model list: 1 từ database, 1 từ memory; cần lặp 1 thằng để update từng entity
// -> Ta nên "ToList()" cái list từ database; sau đó lặp qua nó
// -> rồi update từng entity

## Case: many method cause database round trips
var query =  _context.DanhSachHoSos.Include(x => x.HoSoCongViec).AsQueryable();
var totalRow = query.Count();
var result = query.ToList();
// -> 2 database round trips
// -> để giảm xuống còn 1 database round trips:
var result = query.Select(x => new { TotalRow = query.Count(), Results = query.ToList() }).FirstOrDefault();
// hoặc:
var result = query.ToList();
var totalRow = result.Count();
// -> nhưng nếu trong trường hợp ta chỉ muốn count số record thôi thì chỉ nên "query.Count()"
// => vì "query.Count()" returns only a single value; 
// => còn "ToList()" phải retrieves all rows, load into memory, stored in a list (consume memory + take longer to execute)

## Case: Update but exclude some properties
db.Entry(model).State = EntityState.Modified;
db.Entry(model).Property(x => x.Token).IsModified = false;
db.SaveChanges();

//=======================================================================================================================

# AntiPattern: N + 1 Problem
// using wrong:
var students = context.Students.ToList();
foreach (var student in students)
{
    var courses = context.Courses.Where(c => c.StudentId == student.Id).ToList();
}
// -> better approach:
var query = from student in context.Students
            join course in context.Courses on student.Id equals course.StudentId
            select new { Student = student, Course = course };
var result = query.ToList();

//=======================================================================================================================

# Basic 
## Find()
// checks if the entity is already being tracked by the context
// if yes, returns the tracked entity
// if not, sends a query to the database to find the entity
// if the entity exist in database, it is attached to the context with state set to "Unchanged"
// if the entity not exist database, the method returns 'null'
TEntity entity = _dbSet.Find(id);

## Attach() 
// -> attach an existing entity to the context and mark it as "Unchanged"
// -> call SaveChanges() on the context, no changes will be made to the database for this entity
// -> attach an entity that is already being tracked by the context, the Attach() method will have no effect
// -> create a new entity with the same primary key value as an existing entity; then try to attach it to the context will encounter an exception
_dbSet.Attach(entity);

## Add()
// -> Add a new entity to the context and mark it as "Added"
// -> call SaveChanges() on the context, a new record will be inserted into the database for the entity
_dbSet.Add(entity);
_dbSet.AddRange(entitiesToInsert);
await _dbSet.AddAsync(entity);
await _dbSet.AddRangeAsync(entitiesToInsert);

## Remove()
// -> Trước tiên ta cần "Find(id)" để retrieve the entity from the database and attach it to the context
// -> "Remove()" to mark an existing entity as "Deleted"
// -> "call SaveChanges() on the context, the entity will be deleted from the database"
_dbSet.Remove(entityToDelete);
_dbSet.RemoveRange(entitiesToDelete);

## Modified
// -> modify the properties of an entity that is being tracked by the context, the context will detect these changes and mark the entity as "Modified"
var entity = _context.MyEntities.Find(id);
entity.MyProperty = newValue;
_context.SaveChanges();

//=======================================================================================================================
// https://learn.microsoft.com/en-us/dotnet/csharp/linq/standard-query-operators/join-operations

# Join
// -> the join methods provided in the LINQ framework are "Join" and "GroupJoin" 
// -> these methods perform "equijoins", or joins that match two data sources based on equality of their keys



## Choosing "Join" type: the join methods provided in the LINQ framework are "Join" and "GroupJoin"
// in relational database terms, "Join" implements an "inner join"
// "GroupJoin" method has no direct equivalent in relational database terms
// -> "GroupJoin" implements a superset of "inner joins" and "left outer joins" (tức là 2 thằng đó đều có thể viết dưới dạng Group Join)
// -> "left outer join" is a join that returns each element of the first (left) data source, even if it has no correlated elements in the other data source
## OOP
// -> in object-oriented programming, "joining" could mean a correlation between objects that isn't modeled
// -> such as the "backwards direction" of a "one-way relationship"
// => useful for associating elements from different source sequences that have no direct relationship in the object model
// the only requirement is that the elements in each source share some value that can be compared for equality

## one-way relationship
// -> for example, a 'Student' class that has a property of type 'Department' that represents the major,
// -> but the 'Department' class doesn't have a property that is a collection of 'Student' objects
// => so if we have a list of Department objects and we want to find all the students in each department, we could use a join operation to find them

// -> để cần biết lúc nào thì chọn giữa "Left Join" à "Inner Join" thì ta cần nhớ là ta đang muốn lấy ra cái gì
// -> VD: UI ta cần hiển thị grid tất cả "HoSo" thì ta sẽ s/d "Left Join", và không quan tâm 1 số cột thiếu giá trị
// -> còn "Inner Join" thì ta sẽ hiển thị grid danh sách "HoSo" mà các cột đều đầy đủ thông tin
// -> Note: đối với "Left Join" cũng không hẳn là thiếu giá trị, ta có thể cho những cột đó giá trị mặc định
// -> Note: join đòi hỏi phần "on" ta cần để đúng thứ tự bảng trái trước rồi mới "equals" bảng phải

## Inner Join 
// -> produces a "flat sequence"; only those objects that have a match in the other data set are returned

### Single key join
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

### Composite key join

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

### Multiple join
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

### Implement "Inner join" by using "Grouped join"

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

## Group Join - a "join" clause with an "into" expression
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

## Left Outer Join (Left Join):  calling the "DefaultIfEmpty() method" on the results of "a group join"
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

## Join relationship Table:
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

### DefaultIfEmpty()
// trả về 1 collection mới với 1 phần tử có giá trị mặc định nếu collection gốc rỗng; còn không thì trả về collection gốc
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

//=======================================================================================================================
# GroupBy
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

## with Aggregate function
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

// ----->

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

//=======================================================================================================================

# OrderyBy()
// -> trả về 1 IOrderedEnumerable

// thường thì ta sẽ sort theo 1 trường trong bảng
var orderByDescendingResult = from s in studentList orderby s.StudentName descending select s; 
var studentsInDescOrder = studentList.OrderByDescending(s => s.StudentName); 

// Custom Logic for Sort
// -> ta cần hiểu lamba mà ta provide cho .OrderBy(x => y)
// -> ta cần hiều là "y" từ giờ sẽ đại diện cho item "x"
// -> order sẽ so sánh giá trị của các "y" đại diện cho các item "x" 
// VD: sort lại danh sách học sinh theo đúng thứ tự danh sách tên trường học
var school = new List<string> { "School1", "School2", "School3" };
var studentInfos = await query.ToListAsync();
studentInfos = studentInfos.OrderBy(x => school.IndexOf(x.SchoolName)).ToList();

// Multiple Sorting
var orderByResult = from s in studentList orderby s.StudentName, s.Age select new { s.StudentName, s.Age };
var movies = _db.Movies.OrderBy(c => c.Category).ThenByDescending(n => n.Name)

//=======================================================================================================================

# SubExpression
var result = from donVi in hoSo_donVi.DefaultIfEmpty()
            let danhSachHoSo = qDanhSachHoSo.Where(x => x.HoSoCongViecID == hoSoCongViec.Id).FirstOrDefault()
            where danhSachHoSo != null // s/d FirstOrDefault() ở đây có thể trả về null, ta sẽ check nếu null thì trả về list rỗng
            select new DanhSachHoSoResponse
            {
            //.....
            }


// "let" is used to create a new range variable "danhSachHoSo"
// valid keyword within a LINQ query expression (can't use var)
// Mặc dù trong dòng "let" có "FirstOrDefault()" nhưng nó sẽ không execute
// Nó sẽ chỉ execute đến khi ta gọi "await result.ToListAsync()"

//=======================================================================================================================

# CombineListEntity
## Concat:
// -> returns a "new IEnumerable<T> object" that contains all the elements from both lists
List<int> list1 = new List<int> { 1, 2, 3 };
List<int> list2 = new List<int> { 4, 5, 6 };
IEnumerable<int> result = list1.Concat(list2);

## Union:
// -> create a "new IEnumerable<T> object" that contains the "unique elements" from both lists
List<int> list1 = new List<int> { 1, 2, 3 };
List<int> list2 = new List<int> { 3, 4, 5 };
IEnumerable<int> result = list1.Union(list2);

## AddRange:
// -> "modifies the first list" in place by adding add all the elements from the second list to the first list
// -> does not return a new object
List<int> list1 = new List<int> { 1, 2, 3 };
List<int> list2 = new List<int> { 4, 5, 6 };
list1.AddRange(list2);

# CombineListEntity with different type

## Use base type or interface
// -> create a new list of base type or interface of that 2 types
// -> add the elements from both lists to it
List<Animal> animals = new List<Animal>();
List<Dog> dogs = new List<Dog> { new Dog(), new Dog() };
List<Cat> cats = new List<Cat> { new Cat(), new Cat() };
animals.AddRange(dogs);
animals.AddRange(cats);

## Create a new list of the "dynamic" or "object" type
// -> create a new list of the dynamic or object type
// -> add the elements from both lists to it.
List<dynamic> items = new List<dynamic>();
List<int> numbers = new List<int> { 1, 2, 3 };
List<string> strings = new List<string> { "a", "b", "c" };
items.AddRange(numbers);
items.AddRange(strings);