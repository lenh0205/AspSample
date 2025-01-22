> https://learn.microsoft.com/en-us/aspnet/mvc/overview/older-versions/getting-started-with-ef-5-using-mvc-4/implementing-the-repository-and-unit-of-work-patterns-in-an-asp-net-mvc-application
> https://codewithshadman.com/repository-pattern-csharp/

=======================================================================
# Repository and Unit of Work Design Patterns
* -> to **`create an abstraction layer`** between the **Data Access layer** and the **Business Logic layer** of an application
* => help **`insulate`** our **application** from **changes in the data store** and can facilitate **`automated unit testing`** or **`test-driven development (TDD)`**

=======================================================================
# Repository
* -> encapsulate the data access operations, provide a consistent interface (or classes) define the operations for working with data entities (promote separation of concerns)

### Implementation
* -> we'll implement **`a repository class`** for **`each entity type`** (_we can also implement a single repository for all entity types_)
* -> _if we implement one for each type, we can use **`separate classes`**, **a generic base class and derived classes**, or **an abstract base class and derived classes**_

* -> we'll create a **repository interface** and a **repository class**
* -> we'll **`instantiate the repository in our controller`**, we'll use the **`interface`** so that the controller will accept a reference to **any object that implements the repository interface** (_suitable for DI_)
* -> the repository will **`works with the Entity Framework`**

* => when the controller runs under **`a unit test`**, it receives a repository that works with data stored like in-memory collection - so that we can easily manipulate for testing

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

### Generic Repository
* -> **creating a repository class for each entity type** could result in **`a lot of redundant code`**, and it could result in **`partial updates`**
* => one way to minimize redundant code is to use **`a generic repository`**
* => and one way to avoid partial updates inside one transaction is use a **`unit of work`**

=======================================================================
## Unit of Work
* -> a unit of work class serves one purpose **`ensure that multiple repositories share a single database context`**
* -> when we used multiple repositories and each one **uses a separate database context instance**; then when we update 2 different entity types as part of the same transaction, **`one might succeed and the other might fail`**
* -> that way, **when a unit of work is complete** we can call the **SaveChanges** method on **that instance of the context** and be **`assured that all related changes will be coordinated`**

* => this is really important when dealing with **`transaction`**

```r - Ex:
// creating a repository class for each entity type could result in "partial updates"
// suppose we have to update two different entity types as "part of the same transaction"
// -> if each uses a separate database context instance, one might succeed and the other might fail
// -> "unit of work" ensure that "all repositories" use the "same database context"
// => and thus coordinate all updates
```

## Implementation
* -> the **`unit of work class`** **coordinates the work of multiple repositories** by **`creating a single database context class shared by all of them`**
* -> all that the _UnitOfWork class_ needs is a **`Save method`** and **`a property for each repository`**
* -> _each repository property_ **returns a repository instance** that has been **`instantiated using the same database context instance`** as the other repository instances
* => the _Unit of Work object_ will responsible for initiating database operations, tracking changes, and committing or rolling back the transaction

=======================================================================
# Example: Implement Repository
* -> migrate from controller using "DbContext" directly to using Repository
* -> create **`an abstraction layer`** between the **controller** and the **Entity Framework database context**

## Creating the Student Repository class

```cs - define repository
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
```cs - Controller using repository
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

=======================================================================
# Example: Implement a Generic Repository

## Create a Generic Repository

```cs - GenericRepository.cs
// generic repository will handle typical CRUD requirements
// for special requirements (complex filtering or ordering) of a particular entity type,
// we can create a derived class that has additional methods 
public class GenericRepository<TEntity> where TEntity : class
{
    // for repository to be instantiated
    internal SchoolContext context; // database context
    internal DbSet<TEntity> dbSet; // the entity set

    public GenericRepository(SchoolContext context)
    {
        this.context = context;
        this.dbSet = context.Set<TEntity>();
    }

    // ensure that the work is done by the database rather than in memory of web server
    // this generic method avoid a large number of derived classes that have specialized methods do the same thing
    // which could more work to maintain in a complex app
    public virtual IEnumerable<TEntity> Get(
        Expression<Func<TEntity, bool>> filter = null, // Ex: student => student.LastName == "Smith"
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null, // Ex: q => q.OrderBy(s => s.LastName)
        string includeProperties = "" // provide a comma-delimited list of navigation properties for "eager loading"
    )
    {
        IQueryable<TEntity> query = dbSet;

        if (filter != null)
        {
            query = query.Where(filter); //  applies the filter expression
        }

        foreach (var includeProperty in includeProperties.Split(
            new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
        {
            // applies the "eager-loading expressions" after parsing the comma-delimited list
            query = query.Include(includeProperty);
        }

        if (orderBy != null)
        {
            return orderBy(query).ToList(); // applies the orderBy expression
        }
        else
        {
            return query.ToList();
        }
    }

    // the "GetByID", "Insert", "Update" methods is similar to what we saw in the non-generic repository
    public virtual TEntity GetByID(object id)
    {
        return dbSet.Find(id);
    }

    public virtual void Insert(TEntity entity)
    {
        dbSet.Add(entity);
    }

    public virtual void Delete(object id)
    {
        TEntity entityToDelete = dbSet.Find(id);
        Delete(entityToDelete);
    }

    // for concurrency handling you need a Delete method that takes an entity instance 
    // that includes the original value of a tracking property
    public virtual void Delete(TEntity entityToDelete)
    {
        if (context.Entry(entityToDelete).State == EntityState.Detached)
        {
            dbSet.Attach(entityToDelete);
        }
        dbSet.Remove(entityToDelete);
    }

    public virtual void Update(TEntity entityToUpdate)
    {
        dbSet.Attach(entityToUpdate);
        context.Entry(entityToUpdate).State = EntityState.Modified;
    }
}
```

## Creating the Unit of Work Class

```cs - UnitOfWork.cs
public class UnitOfWork : IDisposable // for disposes the database context
{
    private SchoolContext context = new SchoolContext();
    private GenericRepository<Department> departmentRepository;
    private GenericRepository<Course> courseRepository;

    // each repository property checks whether the repository already exists
    // if not, it instantiates the repository, passing in the context instance
    // as a result, all repositories share the same context instance
    public GenericRepository<Department> DepartmentRepository
    {
        get
        {

            if (this.departmentRepository == null)
            {
                this.departmentRepository = new GenericRepository<Department>(context);
            }
            return departmentRepository;
        }
    }

    public GenericRepository<Course> CourseRepository
    {
        get
        {

            if (this.courseRepository == null)
            {
                this.courseRepository = new GenericRepository<Course>(context);
            }
            return courseRepository;
        }
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

## Changing the Course Controller to use the UnitOfWork Class and Repositories

```cs - 
// CourseController to access both the "Department" and the "Course" entity sets using GenericRepository and UnitOfWork
public class CourseController : Controller
{
    private UnitOfWork unitOfWork = new UnitOfWork();

    // all references to the "database context" are replaced by references to the appropriate "repository"
    // using "UnitOfWork" properties to access the "repository"
    // the "Dispose" method disposes the "UnitOfWork instance"

    var courses = unitOfWork.CourseRepository.Get(includeProperties: "Department");
    // ...
    Course course = unitOfWork.CourseRepository.GetByID(id);
    // ...
    unitOfWork.CourseRepository.Insert(course);
    unitOfWork.Save();
    // ...
    Course course = unitOfWork.CourseRepository.GetByID(id);
    // ...
    unitOfWork.CourseRepository.Update(course);
    unitOfWork.Save();
    // ...
    var departmentsQuery = unitOfWork.DepartmentRepository.Get(
        orderBy: q => q.OrderBy(d => d.Name));
    // ...
    Course course = unitOfWork.CourseRepository.GetByID(id);
    // ...
    unitOfWork.CourseRepository.Delete(id);
    unitOfWork.Save();
    // ...

    protected override void Dispose(bool disposing)
    {
        unitOfWork.Dispose();
        base.Dispose(disposing);
    }
}
```