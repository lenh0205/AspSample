====================================================================
# OrderyBy()

```cs
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
```