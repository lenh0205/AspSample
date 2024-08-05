=======================================================================
# Repository and Unit of Work Design Patterns
* -> to **`create an abstraction layer`** between the **Data Access layer** and the **Business Logic layer** of an application
* => help **`insulate`** our **application** from **changes in the data store** and can facilitate **`automated unit testing`** or **`test-driven development (TDD)`**

=======================================================================
# Repository
encapsulate the data access operations, provide a consistent interface (or classes) define the operations for working with data entities (promote separation of concerns)

interfaces provide a contract for the application to interact with the database without exposing the underlying implementation details (querying the database, updating records)

### Implementation
* -> we'll implement **`a repository class`** for **`each entity type`** (_we can also implement a single repository for all entity types_)
* -> _if we implement one for each type, we can use **`separate classes`**, **a generic base class and derived classes**, or **an abstract base class and derived classes**_

* -> we'll create a **repository interface** and a **repository class**
* -> we'll **`instantiate the repository in our controller`**, we'll use the **`interface`** so that the controller will accept a reference to **any object that implements the repository interface** (_suitable for DI_)
* -> the repository will **`works with the Entity Framework`**

* => when the controller runs under **`a unit test`**, it receives a repository that works with data stored like in-memory collection - so that we can easily manipulate for testing

### Generic Repository
* -> **creating a repository class for each entity type** could result in **`a lot of redundant code`**, and it could result in **`partial updates`**

### add UnitOfWork
* -> we could instantiate a new context in the repository, but then if we used multiple repositories in one controller, **`each would end up with a separate context`**
* -> a unit of work class can **`ensure that all repositories use the same context`**

=======================================================================
## Unit of Work
* -> manage multiple operations within a single transaction to ensure consistency and atomicity of changes made to a database
* -> by grouping related operations into a single unit of work, treat them as a logical transaction

Add a middle layer between the Controller and the Repository -> This class will centralize storage for all related Repository to share 1 instance of DbContext.

The Unit of Work object is responsible for initiating database operations, tracking changes, and committing or rolling back the transaction

## Implementation
* -> the **`unit of work class`** **coordinates the work of multiple repositories** by **`creating a single database context class shared by all of them`**

=======================================================================
# Example: Implement Repository
* -> migrate from controller using "DbContext" directly to using Repository
* -> create **`an abstraction layer`** between the **controller** and the **Entity Framework database context**

## Creating the Student Repository class

```cs
public interface IStudentRepository : IDisposable
{
    IEnumerable<Student> GetStudents();
    Student GetStudentByID(int studentId);
    void InsertStudent(Student student);
    void DeleteStudent(int studentID);
    void UpdateStudent(Student student);
    void Save();
}

public class StudentRepository : IStudentRepository, IDisposable
{
    private SchoolContext context;

    public StudentRepository(SchoolContext context)
    {
        this.context = context;
    }

    public IEnumerable<Student> GetStudents()
    {
        return context.Students.ToList();
    }

    public Student GetStudentByID(int id)
    {
        return context.Students.Find(id);
    }

    public void InsertStudent(Student student)
    {
        context.Students.Add(student);
    }

    public void DeleteStudent(int studentID)
    {
        Student student = context.Students.Find(studentID);
        context.Students.Remove(student);
    }

    public void UpdateStudent(Student student)
    {
        context.Entry(student).State = EntityState.Modified;
    }

    public void Save()
    {
        context.SaveChanges();
    }

    private bool disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                context.Dispose();
            }
        }
        this.disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
```

## Change the Student Controller to Use the Repository
```cs - Controller
public class StudentController : Controller
{
    private IStudentRepository studentRepository;

    public StudentController()
    {
        this.studentRepository = new StudentRepository(new SchoolContext());
    }

    public StudentController(IStudentRepository studentRepository)
    {
        this.studentRepository = studentRepository;
    }

    // GET: /Student/
    public ViewResult Index(string sortOrder, string currentFilter, string searchString, int? page)
    {
        ViewBag.CurrentSort = sortOrder;
        ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
        ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";

        if (searchString != null)
        {
            page = 1;
        }
        else
        {
            searchString = currentFilter;
        }
        ViewBag.CurrentFilter = searchString;

        var students = from s in studentRepository.GetStudents()
                    select s;
        if (!String.IsNullOrEmpty(searchString))
        {
            students = students.Where(s => s.LastName.ToUpper().Contains(searchString.ToUpper())
                                || s.FirstMidName.ToUpper().Contains(searchString.ToUpper()));
        }
        switch (sortOrder)
        {
            case "name_desc":
                students = students.OrderByDescending(s => s.LastName);
                break;
            case "Date":
                students = students.OrderBy(s => s.EnrollmentDate);
                break;
            case "date_desc":
                students = students.OrderByDescending(s => s.EnrollmentDate);
                break;
            default:  // Name ascending 
                students = students.OrderBy(s => s.LastName);
                break;
        }

        int pageSize = 3;
        int pageNumber = (page ?? 1);
        return View(students.ToPagedList(pageNumber, pageSize));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Delete(int id)
    {
        try
        {
            Student student = studentRepository.GetStudentByID(id);
            studentRepository.DeleteStudent(id);
            studentRepository.Save();
        }
        catch (DataException)
        {
            //Log the error (uncomment dex variable name after DataException and add a line here to write a log.
            return RedirectToAction("Delete", new { id = id, saveChangesError = true });
        }
        return RedirectToAction("Index");
    }

    protected override void Dispose(bool disposing)
    {
        studentRepository.Dispose();
        base.Dispose(disposing);
    }
}
```

### Note: IQueryable vs. IEnumerable

```cs - "Index" action - before controller using repository, it use "DbContext"
// ......
var students = from s in context.Students select s;

if (!String.IsNullOrEmpty(searchString))
{
    students = students.Where(s => s.LastName.ToUpper().Contains(searchString.ToUpper())
        || s.FirstMidName.ToUpper().Contains(searchString.ToUpper()));
}
// ......
```

* -> the **students** is typed as an **`IQueryable`** object; **.Where()** method in the original code above becomes **a WHERE clause** in the SQL query that is sent to the database
* => until the "Index view" accesses the student model and use **.ToList()** to convert it to collection, **`the query isn't sent to the database`**
* => and **`only the selected entities are returned by the database`**

* -> however, after controller using repository, as a result of changing _context.Students_ to _studentRepository.GetStudents()_, the students variable after this statement is an **`IEnumerable`** collection 
* -> this includes all students in the database
* -> the end result of applying the **.Where** method is the same, **`but now the work is done in memory`** on the web server and not by the database
* => for queries that return large volumes of data, this can be **`inefficient`**

=======================================================================
# Example: Implement a Generic Repository

https://learn.microsoft.com/en-us/aspnet/mvc/overview/older-versions/getting-started-with-ef-5-using-mvc-4/implementing-the-repository-and-unit-of-work-patterns-in-an-asp-net-mvc-application