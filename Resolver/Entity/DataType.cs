# DataTable type
// Để Tranfer data có dạng "DataTable" trong "request-response" ta sẽ cần Serialize nó
// -> nếu Client-Server:
var dt = new DataTable();
var dtForTransfer = JsonConvert.SerializeObject(dt);
var result = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(dt); // nếu ta cần trả về 1 Array gồm nhiều object
// -> nếu Server-Server:
return new {Dt = JsonConvert.SerializeObject(dt), DtReplace = JsonConvert.SerializeObject(dtreplace)}; // response
var resultDataApi = commonApi.CallAPI_common(url, "POST", requestData);
var dataTableApi = JsonConvert.DeserializeObject<JObject>(resultDataApi.DataResult.ToString());
var dt = JsonConvert.DeserializeObject<DataTable>(dataTableApi["dt"].ToString());
var dtReplace = JsonConvert.DeserializeObject<DataTable>(dataTableApi["dtReplace"].ToString());

// ta cũng có thể viết 1 hàm để convert từ "DataTable" thành "Dictionary" :
public List<Dictionary<string, object>> ConvertDataTableToList(DataTable dt)
{
    var rows = new List<Dictionary<string, object>>();
    Dictionary<string, object> row;
    foreach (DataRow dr in dt.Rows)
    {
        row = new Dictionary<string, object>();
        foreach (DataColumn col in dt.Columns)
        {
            row.Add(col.ColumnName, dr[col]);
        }
        rows.Add(row);
    }
    return rows;
}

# object type
// -> All types in C# inherit directly or indirectly from "object" type (alias for System.Object);
// => include reference types and value types; reference types and value types 
// =>  can assign values of any type to variables of type "object"
// ->  require boxing and unboxing for conversion (it makes it slow)
// -> the C# compiler checks types during compile-time
// -> has only a few operation/members (Equals, GetHashCode, ToString) and doesn’t provide much flexibility.
object test = "hello"; // Boxing no error
test = 10; // Boxing no error
test = test + 10; // Compile time error; "object" not contain "+" operation
var myInt = (int) test // Unboxing


# dynamic type
// -> functions like "object" type in most cases
// -> compiler assumes a "dynamic" element supports any operation (whether the object gets its value from a COM API, from a dynamic language such as IronPython, from the HTML Document Object Model (DOM), from reflection, or from somewhere else in the program,...)
// -> is a static type, but an object of type dynamic bypasses static type checking
// -> not require boxing and unboxing
// -> checks types during runtime (All errors on dynamic variables are discovered at runtime only)
dynamic dyn = "Hello, World!";
int len = dyn.Length; // No error at compile time, runtime  
dyn = 123;
int len2 = dyn.Length; // not an error at compile time, but runtime error 

# var keyword
// -> compiler infers the type of the variable from its initialization expression
var test = 10; // "test" integer type
test = test + 10; // No error
test = "hello"; // Compile time error as test is an integer type

# BoxingvsvsvsUnBoxing
## Boxing 
// -> process of converting a "value type" to the "type object" or to any interface type implemented by this value type. 
// -> When CLR boxes a value type, it wraps the value inside a "System.Object" instance and stores it on the managed heap. 
// -> Boxing is implicit

## Unboxing
// -> extracts the value type from the object
// -> Unboxing is explicit