# Performance
// Filter trước khi join bảng tốt hơn ra join bảng ra kết quả rồi mới filter

# Basic
## Find()
// checks if the entity is already being tracked by the context
// if yes, returns the tracked entity
// if not, sends a query to the database to find the entity
// if the entity exist in database, it is attached to the context with state set to "Unchanged"
// if the entity not exist database, the method returns 'null'

## Attach() 
// -> attach an existing entity to the context and mark it as "Unchanged"
// -> call SaveChanges() on the context, no changes will be made to the database for this entity
// -> attach an entity that is already being tracked by the context, the Attach() method will have no effect
// -> create a new entity with the same primary key value as an existing entity; then try to attach it to the context will encounter an exception

## Add()
// -> Add a new entity to the context and mark it as "Added"
// -> call SaveChanges() on the context, a new record will be inserted into the database for the entity

## Remove()
// -> Trước tiên ta cần "Find(id)" để retrieve the entity from the database and attach it to the context
// -> "Remove()" to mark an existing entity as "Deleted"
// -> "call SaveChanges() on the context, the entity will be deleted from the database"

## Modified
// -> modify the properties of an entity that is being tracked by the context, the context will detect these changes and mark the entity as "Modified"
var entity = _context.MyEntities.Find(id);
entity.MyProperty = newValue;
_context.SaveChanges();

# join
// join đòi hỏi phần "on" ta cần để đúng thứ tự bảng trái trước rồi mới "equals" bảng phải

// Inner Join 
var data = from fd in FlightDetails
           join pd in PassengersDetails on fd.Flightno equals pd.FlightNo
           select new {
               nr = fd.Flightno,
               name = fd.FlightName,
               passengerId = pd.PassengerId,
               passengerType = pd.PassengerType
           };

// Left Join: có thêm "DefaultIfEmpty()"
var data = from fd in FlightDetails
           join pd in PassengersDetails on fd.Flightno equals pd.FlightNo into joinedT
           from pd in joinedT.DefaultIfEmpty()
           select new {
               nr = fd.Flightno,
               name = fd.FlightName,
               passengerId = pd == null ? String.Empty : pd.PassengerId,
               passengerType = pd == null ? String.Empty : pd.PassengerType
           };

// Nếu Table1 có columnA với 2 record có giá trị bằng 5; Table2 có columnB với 3 record có giá trị bằng 5
// Nếu ta join 2 bảng này với "Table1.columnA = Table2.columnB"
// Ta sẽ có 1 result với 6 record

# SubExpression
var result = from donVi in hoSo_donVi.DefaultIfEmpty()
            let danhSachHoSo = qDanhSachHoSo.Where(x => x.HoSoCongViecID == hoSoCongViec.Id).FirstOrDefault()
            select new DanhSachHoSoResponse
            {
            //.....
            }
// "let" is used to create a new range variable "danhSachHoSo"
// valid keyword within a LINQ query expression (can't use var)
// Mặc dù trong dòng "let" có "FirstOrDefault()" nhưng nó sẽ không execute
// Nó sẽ chỉ execute đến khi ta gọi "await result.ToListAsync()"


# CombineListEntity
## Concat:
// -> returns a "new IEnumerable<T> object" that contains all the elements from both lists
List<int> list1 = new List<int> { 1, 2, 3 };
List<int> list2 = new List<int> { 4, 5, 6 };
IEnumerable<int> result = list1.Concat(list2);

## Union:
// -> create a "new IEnumerable<T> object" that contains the "unique elements" from both lists
List<int> list1 = new List<int> { 1, 2, 3 };
List<int> list2 = new List<int> { 3, 4, 5 };
IEnumerable<int> result = list1.Union(list2);

## AddRange:
// -> "modifies the first list" in place by adding add all the elements from the second list to the first list
// -> does not return a new object
List<int> list1 = new List<int> { 1, 2, 3 };
List<int> list2 = new List<int> { 4, 5, 6 };
list1.AddRange(list2);

# CombineListEntity with different type

## Use base type or interface
// -> create a new list of base type or interface of that 2 types
// -> add the elements from both lists to it
List<Animal> animals = new List<Animal>();
List<Dog> dogs = new List<Dog> { new Dog(), new Dog() };
List<Cat> cats = new List<Cat> { new Cat(), new Cat() };
animals.AddRange(dogs);
animals.AddRange(cats);

## Create a new list of the "dynamic" or "object" type
// -> create a new list of the dynamic or object type
// -> add the elements from both lists to it.
List<dynamic> items = new List<dynamic>();
List<int> numbers = new List<int> { 1, 2, 3 };
List<string> strings = new List<string> { "a", "b", "c" };
items.AddRange(numbers);
items.AddRange(strings);