====================================================================

# Find()
* -> checks if the entity is already being tracked by the context
* -> if yes, returns the tracked entity
* -> if not, sends a query to the database to find the entity

* -> if the entity exist in database, it is attached to the context with state set to "Unchanged"
* -> if the entity not exist database, the method returns 'null'
TEntity entity = _dbSet.Find(id);

# Attach() 
* -> attach an existing entity to the context and mark it as **`Unchanged`**, call **SaveChanges()** on the context no changes will be made to the database for this entity
* -> attach an entity that is already being tracked by the context, the Attach() method will have no effect
* -> create a new entity with the same primary key value as an existing entity; then try to attach it to the context will encounter an exception

```cs
_dbSet.Attach(entity);
```

# Add()
* -> add a new entity to the context and mark it as **`Added`**
* -> call **SaveChanges()** on the context, a new record will be inserted into the database for the entity

```cs
_dbSet.Add(entity);
_dbSet.AddRange(entitiesToInsert);
await _dbSet.AddAsync(entity);
await _dbSet.AddRangeAsync(entitiesToInsert);
```

# Remove()
* -> trước tiên ta cần **Find(id)** để retrieve the entity from the database and attach it to the context
* -> **Remove()** to mark an existing entity as **`Deleted`**
* -> call **SaveChanges()** on the context, the entity will be deleted from the database

```cs
_dbSet.Remove(entityToDelete);
_dbSet.RemoveRange(entitiesToDelete);
```

# Modified
* -> modify the properties of an entity that is being tracked by the context, the context will detect these changes and mark the entity as **`Modified`**

```cs
var entity = _context.MyEntities.Find(id);
entity.MyProperty = newValue;
_context.SaveChanges();
```

====================================================================
# SubExpression

```cs
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
```

====================================================================
# CombineListEntity

## Concat:
* -> returns a "new IEnumerable<T> object" that contains all the elements from both lists

```cs
List<int> list1 = new List<int> { 1, 2, 3 };
List<int> list2 = new List<int> { 4, 5, 6 };
IEnumerable<int> result = list1.Concat(list2);
```

## Union:
* -> create a "new IEnumerable<T> object" that contains the "unique elements" from both lists

```cs
List<int> list1 = new List<int> { 1, 2, 3 };
List<int> list2 = new List<int> { 3, 4, 5 };
IEnumerable<int> result = list1.Union(list2);
```

## AddRange:
* -> "modifies the first list" in place by adding add all the elements from the second list to the first list
* -> does not return a new object

```cs
List<int> list1 = new List<int> { 1, 2, 3 };
List<int> list2 = new List<int> { 4, 5, 6 };
list1.AddRange(list2);
```

# CombineListEntity with different type

## Use base type or interface
// -> create a new list of base type or interface of that 2 types
// -> add the elements from both lists to it

```cs
List<Animal> animals = new List<Animal>();
List<Dog> dogs = new List<Dog> { new Dog(), new Dog() };
List<Cat> cats = new List<Cat> { new Cat(), new Cat() };
animals.AddRange(dogs);
animals.AddRange(cats);
```

## Create a new list of the "dynamic" or "object" type
* -> create a new list of the dynamic or object type
* -> add the elements from both lists to it

```cs
List<dynamic> items = new List<dynamic>();
List<int> numbers = new List<int> { 1, 2, 3 };
List<string> strings = new List<string> { "a", "b", "c" };
items.AddRange(numbers);
items.AddRange(strings);
```