
# Pagination
* -> pagination refers to **`retrieving results in pages, rather than all at once`**
* -> this is typically done for **large resultsets**, where **a user interface is shown that allows the user to navigate to the next or previous page** of the results

## Note
* -> regardless of the pagination method used, always make sure that our **`ordering is fully unique`** (_tức là result được sort, và các phần tử đều đúng 1 thứ tự như vậy mỗi lần query dù có giá trị giống nhau_)
* -> ote that **relational databases do not apply any ordering by default**, even on the primary key.

* _For example, if results are ordered only by date, but there can be **multiple results with the same date**_
* _then **`results could be skipped`** when paginating as they're ordered differently across two paginating queries_
* _Ordering by both date and ID (or any other unique property or combination of properties) makes the ordering fully unique and avoids this problem_

```cs
+----+--------+----------------------+
| Id | Title  |      CreatedDate     |
+----+--------+----------------------+
| 1	 | Post A	| 2024-03-10 09:00:00  |
| 2	 | Post B	| 2024-03-10 10:00:00  |
| 3	 | Post C	| 2024-03-10 10:00:00  |
| 4	 | Post D	| 2024-03-10 11:00:00  |
| 5	 | Post E	| 2024-03-10 12:00:00  |
+----+--------+----------------------+

// -> khi ta Skip(0).Take(2) thì kết quả có 2 trường hợp "Post A + Post B" hoặc "Post A + Post C" 
// -> khi ta Skip(2).Take(2) thì kết quả có 2 trường hợp "Post B + Post D" hoặc "Post C + Post D"
// => vậy nên sẽ có trường hợp Page 1 là "Post A + Post C" và Page 2 là "Post C + Post D"; tức Post B bị skip còn Post C bị duplicate
```
