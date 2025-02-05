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
```

## Case: Update but exclude some properties
```cs
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

====================================================================
# Note

## Correct Execution Order (Simplified)
FROM (includes JOINs)
WHERE (filters rows)
GROUP BY (aggregates rows)
HAVING (filters aggregated groups)
SELECT (chooses columns)
ORDER BY (sorts results)
LIMIT/TOP (reduces output)

* => khi viết câu SQL ta cần viết **JOIN** trước khi **GROUP BY** hoặc **ORDER BY**, ta không cần phải groupby orderby, limit các kiểu rồi mới join SQL sẽ tự biết tối ưu phần join

=================================================================
# Example

## Basic Join for 'many-to-many' relationship
* -> cần **`inner join 2 lần`** giữa bảng gốc với bảng trung gian và bảng còn lại 

```cs - problem
Write a query to return the first name and last name of all actors in the film 'AFRICAN EGG'

// "actor" table
  col_name   | col_type
-------------+--------------------------
 actor_id    | integer
 first_name  | text
 last_name   | text

// "film" table
       col_name       |  col_type
----------------------+--------------------------
 film_id              | integer
 title                | text
 description          | text
 release_year         | integer
 language_id          | smallint
 original_language_id | smallint
 rental_duration      | smallint
 rental_rate          | numeric
 length               | smallint
 replacement_cost     | numeric
 rating               | text

// "film_actor" table
  col_name   | col_type
-------------+--------------------------
 actor_id    | smallint
 film_id     | smallint
``` 

```sql - solution
SELECT a.first_name, a.last_name
FROM actor a
JOIN film_actor fa ON a.actor_id = fa.actor_id
JOIN film f ON fa.film_id = f.film_id
WHERE f.title = 'AFRICAN EGG';
```

```cs - linq solution
var Actors = db.GetTable<Actor>();
var Films = db.GetTable<Film>();
var Film_actors = db.GetTable<FilmActor>();

var query = from a in Actors
     join fa in Film_actors on a.Actor_id equals fa.Actor_id
     join f in Films on fa.Film_id equals f.Film_id
     where f.Title == "AFRICAN EGG"
     select new 
     {
         a.First_name,
         a.Last_name
     }
```

## tìm max của 1 bảng rồi join với 1 bảng để lấy thông tin tương ứng cho kết quả cuối
* -> nếu chỉ tìm giới hạn 1 kết quả nhất định theo thứ tự thì khả năng cao là ta sẽ cần dùng **`orderby, select top, và aggregate func`**

```sql
SELECT c.name
FROM category c
JOIN film_category fc ON c.category_id = fc.category_id
GROUP BY c.category_id
ORDER BY COUNT(fc.film_id) DESC
LIMIT 1;

SELECT category.name
FROM (
  SELECT fc.category_id
	FROM film_category fc
	GROUP BY fc.category_id
	ORDER BY COUNT(fc.film_id) DESC
    LIMIT 1
  ) sub 
INNER JOIN category ON category.category_id = sub.category_id
```

```cs - linq
var query = db.Categories
    .Join(db.FilmCategories,
          c => c.CategoryId,
          fc => fc.CategoryId,
          (c, fc) => new { c.Name, fc.CategoryId })
    .GroupBy(x => new { x.CategoryId, x.Name })
    .OrderByDescending(g => g.Count())
    .Select(g => g.Key.Name)
    .FirstOrDefault();

var query = (
    from c in db.Categories
    join fc in db.FilmCategories on c.CategoryId equals fc.CategoryId
    group c by new { c.CategoryId, c.Name } into g
    orderby g.Count() descending 
    select g.Key.Name
).FirstOrDefault();
```

## Join để tìm 2 giá trị lớn nhất theo Time
* -> WHERE kết hợp với AND để tạo 2 điều kiện

```sql - create table
CREATE TABLE film (
    film_id              INT IDENTITY(1,1) PRIMARY KEY,
    title                NVARCHAR(255) NOT NULL,
    description          NVARCHAR(MAX),
    release_year         INT CHECK (release_year > 1800),
    language_id          SMALLINT NOT NULL,
    original_language_id SMALLINT,
    rental_duration      SMALLINT NOT NULL DEFAULT 3,
    rental_rate          DECIMAL(5,2) NOT NULL,
    length               SMALLINT,
    replacement_cost     DECIMAL(5,2) NOT NULL,
    rating               NVARCHAR(50)
);

CREATE TABLE inventory (
    inventory_id INT IDENTITY(1,1) PRIMARY KEY,
    film_id      INT NOT NULL,
    store_id     SMALLINT NOT NULL,
    CONSTRAINT FK_Film FOREIGN KEY (film_id) REFERENCES film(film_id) ON DELETE CASCADE
);

CREATE TABLE rental (
    rental_id    INT IDENTITY(1,1) PRIMARY KEY,
    rental_ts    DATETIMEOFFSET NOT NULL,
    inventory_id INT NOT NULL,
    customer_id  SMALLINT NOT NULL,
    return_ts    DATETIMEOFFSET,
    staff_id     SMALLINT NOT NULL,
    CONSTRAINT FK_Inventory FOREIGN KEY (inventory_id) REFERENCES inventory(inventory_id) ON DELETE CASCADE
);
```

```sql - join
SELECT f.film_id, f.title
FROM film f
JOIN inventory i ON f.film_id = i.film_id
JOIN rental r ON i.inventory_id = r.inventory_id
WHERE r.rental_ts >= '2020-06-01 00:00:00' 
  AND r.rental_ts < '2020-07-01 00:00:00'
GROUP BY f.film_id, f.title
ORDER BY COUNT(r.rental_id) DESC
LIMIT 2;
```

```cs - linq
(
	from f in Films
	join i in Inventories on f.Film_id equals i.Film_id
	join r in Rentals on i.Inventory_id equals r.Inventory_id
	where r.Rental_ts >= new DateTime(2020, 6, 1) && r.Rental_ts < new DateTime(2020, 7, 1)
	group r.Rental_id by new { f.Film_id, f.Title } into g
	orderby g.Count() descending
	select new { g.Key.Film_id, g.Key.Title }
).Take(2)

// or
Films
  .Join(Inventories, f => f.Film_id, i => i.Film_id, (f, i) => new { f, i })
  .Join(Rentals, fi => fi.i.Inventory_id, r => r.Inventory_id, (fi, r) => new { fi.f, r })
  .Where(x => x.r.Rental_ts >= new DateTime(2020, 6, 1) &&
              x.r.Rental_ts < new DateTime(2020, 7, 1))
  .GroupBy(x => new { x.f.Film_id, x.f.Title })
  .OrderByDescending(g => g.Count())
  .Select(g => new { g.Key.Film_id, g.Key.Title })
  .Take(2)
```

## Union with WHERE + LIKE

```sql
SELECT a.first_name, a.last_name FROM actor a
WHERE a.last_name LIKE 'A%'
UNION
SELECT c.first_name, c.last_name FROM customer c
WHERE c.last_name LIKE 'A%';
```
```cs
Actors
    .Where(a => a.Last_name.StartsWith("A"))
    .Select(a => new { a.First_name, a.Last_name })
    .Union(Customers
        .Where(c => c.Last_name.StartsWith("A"))
        .Select(c => new { c.First_name, c.Last_name }))
    .Distinct()
```