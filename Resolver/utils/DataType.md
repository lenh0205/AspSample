====================================================================
# Dictionary 

```cs
var students = new Dictionary<int, StudentName>()
{
    { 111, new StudentName { FirstName="Sachin", LastName="Karnik", ID=211 } },
    { 112, new StudentName { FirstName="Dina", LastName="Salimzianova", ID=317 } },
    { 113, new StudentName { FirstName="Andy", LastName="Ruth", ID=198 } }
};
// hoặc:
var students2 = new Dictionary<int, StudentName>()
{
    [111] = new StudentName { FirstName="Sachin", LastName="Karnik", ID=211 },
    [112] = new StudentName { FirstName="Dina", LastName="Salimzianova", ID=317 } ,
    [113] = new StudentName { FirstName="Andy", LastName="Ruth", ID=198 }
};
// hoặc:
var students = new Dictionary<int, StudentName>();
students.Add(111, new StudentName { FirstName="Sachin", LastName="Karnik", ID=211 });
students.Add(112, new StudentName { FirstName="Dina", LastName="Salimzianova", ID=317 });
students.Add(113, new StudentName { FirstName="Andy", LastName="Ruth", ID=198 });
```

====================================================================
# List<KeyValuePair> vs Dictionary<T Key, T Value>

* **List<KeyValuePar>**
* -> **`lighter`**
* -> **`insertion`** is faster in List
* -> **`searching`** is slower than Dictionary
* -> can only be assigned value during creation - **`changing the key,value is not possible`** (_to solve this - remove and add new item in same place_)

* **Dictionary<T Key, T Value>**
* -> **`heavy`**
* -> **`insertion`** is slower - has to compute Hash
* -> **`searching`** is faster because of Hash
* -> **`can change and update dictionary`**

====================================================================
# Hashtable vs Dictionary

## HashTable
* -> do not need to **`specify data types`** key/values when declaring Hashtable 
* -> can store key/value pair of **`same type or different types at run time`** 
* -> since there is no strong typing during compile time, every time we retrieve value it has to **perform boxing/unboxing**, hence there is **`performance impact`** and process becomes slow 
* -> if **`access a key that doesn't present`** in the given Hashtable, then it will give null values
* -> it is **`thread safe`** 
* -> it doesn’t maintain the **`order of stored values`**

## Dictionary
* -> at the time of declaration we have to **`specify types for key/value`** 
* -> we have to **`store key/value of same type as defined during declaration (compile time)`** - that means we will get type safety with Dictionary<TKey, TValue> 
* -> the **`data retrieval is faster`** than Hashtable due to no boxing/unboxing 
* -> **`access a key that doesn't present in the given Dictionary`** will give a error
* -> also **`thread safe but`** only for public static members 
* -> always **`maintain the order of stored values`** 