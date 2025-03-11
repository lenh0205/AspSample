========================================================================
# Pagination
* -> pagination refers to **`retrieving results in pages, rather than all at once`**
* -> this is typically done for **large resultsets**, where **a user interface is shown that allows the user to navigate to the next or previous page** of the results

## Required: Unique Ordering
* -> regardless of the pagination method used, always make sure that our **`ordering is fully unique`** (_tức là result được sort, và các phần tử đều đúng 1 thứ tự như vậy mỗi lần query dù có giá trị giống nhau_)
* -> note that **relational databases do not apply any ordering by default**, even on the primary key.

* _For example, if results are ordered only by date, but there can be **multiple results with the same date**_
* _then **`results could be skipped`** when paginating as they're ordered differently across two paginating queries_
* _Ordering by both date and ID (or any other unique property or combination of properties) makes the ordering fully unique and avoids this problem_

```cs
+----+--------+----------------------+
| Id | Title  |      CreatedDate     |
+----+--------+----------------------+
| 1  | Post A	| 2024-03-10 09:00:00  |
| 2	 | Post B	| 2024-03-10 10:00:00  |
| 3	 | Post C	| 2024-03-10 10:00:00  |
| 4	 | Post D	| 2024-03-10 11:00:00  |
| 5	 | Post E	| 2024-03-10 12:00:00  |
+----+--------+----------------------+

// -> khi ta Skip(0).Take(2) thì kết quả có 2 trường hợp "Post A + Post B" hoặc "Post A + Post C" 
// -> khi ta Skip(2).Take(2) thì kết quả có 2 trường hợp "Post B + Post D" hoặc "Post C + Post D"
// => vậy nên sẽ có trường hợp Page 1 là "Post A + Post C" và Page 2 là "Post C + Post D"; tức Post B bị skip còn Post C bị duplicate
```

## Using 'Offset pagination' or 'Keyset pagination'
* -> **`Keyset pagination`** is appropriate for **`pagination interfaces where the user navigates forwards and backwards`**, but does **not support random access, where the user can jump to any specific page**
* -> **`random access pagination`** requires using **`offset pagination`** as explained above;
* => because of the shortcomings of offset pagination, carefully consider if **random access pagination really is required** for our use case, or if **next/previous page navigation is enough**
* => if random access pagination is necessary, **a robust implementation** could **`use keyset pagination when navigation to the next/previous page`**, and **`offset navigation when jumping to any other page`**

## Indexes
* -> as with any other query, proper indexing is vital for good performance: make sure to **`have 'indexes' in place which correspond to our pagination ordering`**
* -> if **ordering by more than one column**, an index over those multiple columns can be defined; this is called a **`composite index`**

========================================================================
# Offset pagination
* -> a common way to implement pagination with databases is to use the **`Skip`** and **`Take`** LINQ operators (**OFFSET** and **LIMIT** in SQL)
* -> unfortunately, while this technique is **very intuitive**, it also has **some severe shortcomings**

```cs
// given a page size of 10 results, the third page can be fetched with EF Core as follows:
var position = 20;
var nextPage = await context.Posts
    .OrderBy(b => b.PostId)
    .Skip(position)
    .Take(10)
    .ToListAsync();
```

## Drawbacks
* -> the database must **still process the first 20 entries**, **`even if they aren't returned to the application`**; this creates **`possibly significant computation load`** that **increases with the number of rows being skipped**
* -> if any **`updates occur concurrently`**, our pagination may end up **skipping certain entries** or **showing them twice** (_Ex: if an entry is removed as the user is moving from page 2 to 3, the whole resultset "shifts up", and one entry would be skipped_)

========================================================================
# Keyset pagination
* -> the _recommended alternative to offset-based pagination_ - sometimes called **keyset pagination** (or **seek-based pagination**)
* -> is to simply use a **`WHERE`** clause to skip rows, instead of an offset - this means **remember the relevant values from the last entry fetched** (instead of its offset), and to **ask for the next rows after that row**
* -> and assuming an **index** is defined on the filtered column, this query is **`very efficient`**, and also **`isn't sensitive to any concurrent changes happening in lower Id values`**

```cs
// For example, assuming the last entry in the last page we fetched had an ID value of 55
var lastId = 55;
var nextPage = await context.Posts
    .OrderBy(b => b.PostId)
    .Where(b => b.PostId > lastId)
    .Take(10)
    .ToListAsync();
```

## Multiple pagination keys - Ordering vs Keyset
* -> When using keyset pagination, it's frequently necessary to **order by more than one property**; as **`more ordering keys are added, additional clauses (in WHERE) can be added`**

```CS
// For example, the following query paginates by date and ID
// This ensures that the next page picks off exactly where the previous one ended  

var lastDate = new DateTime(2020, 1, 1);
var lastId = 55;
var nextPage = await context.Posts
    .OrderBy(b => b.Date)
    .ThenBy(b => b.PostId)
    .Where(b => b.Date > lastDate || (b.Date == lastDate && b.PostId > lastId))
    .Take(10)
    .ToListAsync();
```
