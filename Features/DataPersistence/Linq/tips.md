====================================================================
# Performance
## Case: Filter trước khi join bảng tốt hơn ra join bảng ra kết quả rồi mới filter
// có vẻ là việc mình join các bảng trước thành 1 queryable trước; 
// rồi phần filter cho từng bảng mình đặt hết lên cùng 1 cái queryable đó
// -> thì sẽ dễ control cũng như preditable hơn

## Case: ta có 2 model list: 1 từ database, 1 từ memory; cần lặp 1 thằng để update từng entity
// -> Ta nên "ToList()" cái list từ database; sau đó lặp qua nó
// -> rồi update từng entity

## Case: many method cause database round trips

```cs
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
```

====================================================================
# AntiPattern: N + 1 Problem

```cs
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
```