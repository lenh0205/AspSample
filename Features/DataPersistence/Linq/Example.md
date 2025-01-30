
# Basic Join for many-to-many relationship

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