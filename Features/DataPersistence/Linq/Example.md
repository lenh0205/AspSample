=================================================================
# Basic Join for many-to-many relationship
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

=================================================================
# tìm max của 1 bảng rồi join với 1 bảng để lấy thông tin tương ứng cho kết quả cuối
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