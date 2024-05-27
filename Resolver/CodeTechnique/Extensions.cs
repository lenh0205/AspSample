# ExtensionMethod
// -> a static method 
// -> allows you to add methods to existing types 
// => without creating a new derived type, recompiling, or otherwise modifying the original type
// Extension methods are called as if they were instance methods on the extended type.

# Define
// -> define a "static class" to contain the extension method (name doesn’t affect the extension method)
// -> defind a "static method" as a "static class"
// -> The 'first parameter' use "this" modifier followed by the 'type' you want to extend 
// -> add 'additional parameters' after the first parameter if needed
// -> Inside method, write the code that implements the functionality

namespace ExtensionMethods
{
    public static class MyExtensions
    {
        public static int WordCount(this string str) // this + the "type" need extension
        {
            return str.Split(new char[] { ' ', '.', '?' }, StringSplitOptions.RemoveEmptyEntries).Length;
        }
    }
}

# Usage:
// -> call 'Extension method' on 'instances' of the type it extends
using ExtensionMethods;

string s = "Hello Extension Methods"; 
int i = s.WordCount();

## IEnumarable custom Extension Method:
public static class IEnumExtension
{
    // useful for iterate over an IEnumerable<T> and have access to both the item and its index in the collection
    public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> self)
        => self.Select((item, index) => (item, index));
    
    public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> self, int StartFrom)
        => self.Select((item, index) => (item, index + StartFrom));

}

// Usage:
List<string> fruits = new List<string> { "apple", "banana", "cherry" };
foreach (var (item, index) in fruits.WithIndex())
{
    Console.WriteLine($"Item: {item}, Index: {index}");
}

## LINQ standard query operators: .Where(), .First(),...
// => are extension methods 
// => add query functionality to the existing System.Collections.IEnumerable and System.Collections.Generic.IEnumerable<T> types


## DbContext Custom Extension Method
public static async Task SaveCollection<TEntity>(ForestContext db, string keyName, string key, List<TEntity> models)
  where TEntity : BaseEntity {
      var dataInDB = await db.Set<TEntity>().AsNoTracking().WhereClause(keyName, key).ToListAsync();
      var dataInDBIDs = dataInDB.Select(o => o.ID).ToList();
      var dataInModelIDs = models.Select(o => o.ID).ToList();

      var deleteIds = dataInDBIDs.Where(o => !dataInModelIDs.Contains(o)).ToList();
      var addIds = dataInModelIDs.Where(o => !dataInDBIDs.Contains(o)).ToList();
      var updateIds = dataInModelIDs.Where(o => dataInDBIDs.Contains(o)).ToList();

      if (deleteIds.Count > 0) {
          db.Set<TEntity>().RemoveRange(dataInDB.Where(o => deleteIds.Contains(o.ID)));
      }

      if (addIds.Count > 0) {
          var adds = models.Where(o => addIds.Contains(o.ID)).ToList();
          await db.Set<TEntity>().AddRangeAsync(adds);
      }

      if (updateIds.Count > 0) {
          var updates = models.Where(o => updateIds.Contains(o.ID)).ToList();
          updates.ForEach(o => db.Entry(o).State = EntityState.Modified);
      }
  }
public static Expression<Func<TEntity, bool>> WhereClause(string keyName, string key)
{
    var parameter = Expression.Parameter(typeof(TEntity), "o");
    var property = Expression.Property(parameter, keyName);
    var value = Expression.Constant(key);
    var equal = Expression.Equal(property, value);
    var lambda = Expression.Lambda<Func<TEntity, bool>>(equal, parameter);
    return lambda;
}

// Usage:
public async Task SaveProducts(List<Product> products)
{
    using (var db = new ForestContext())
    {
        await db.SaveCollection("ProductID", products);
    }
}


public static IQueryable<TSource> WhereIf<TSource>(this IQueryable<TSource> source,
    [DoesNotReturnIf(true)] bool when,
    Expression<Func<TSource, bool>> predicateTrue,
    Expression<Func<TSource, bool>>? predicateFalse = null)
{
    if (when) {
        return source.Where(predicateTrue);
    }
    if (predicateFalse != null) {
        return source.Where(predicateFalse);
    }
    return source;
}
public static IQueryable<TSource> WhereFunc<TSource>(this IQueryable<TSource> source,
    [DoesNotReturnIf(true)] bool when,
    Func<IQueryable<TSource>, IQueryable<TSource>> funcTrue,
    Func<IQueryable<TSource>, IQueryable<TSource>>? funcFalse = null)
{
    if (when) {
        return funcTrue.Invoke(source);
    }
    if (funcFalse != null) {
        return funcFalse.Invoke(source);
    }
    return source;
}

## "string" custom Extension Method
public static string QuoteName(this string self)
{
    string pattern = $@"\bDROP\b|\bdrop\b|\bTABLE\b|\btable\b|\bDATABASE\b|\bdatabase\b|\bINSERT\b|\binsert\b|\bDELETE\b|\bdelete\b|\bUPDATE\b|\bupdate\b|\bALTER\b|\balter\b|;"; // Mẫu regex để tìm kiếm tất cả các ký tự nguy hiểm
    return string.IsNullOrWhiteSpace(self) ? string.Empty : Regex.Replace(self, pattern, "");// Thay thế tất cả các ký tự nguy hiểm bằng khoảng trắng
}
public static string ConvertIntArrayToString(this int[] array)
{
    return string.Join(",", array);
}

## Table custom Extension Method
public static class TableExtensions
{
    public static string DataTableToJSONWithStringBuilder(DataTable table)
    {
        var JSONString = new StringBuilder();
        if (table.Rows.Count > 0)
        {
            JSONString.Append("[");
            for (int i = 0; i < table.Rows.Count; i++)
            {
                JSONString.Append("{");
                for (int j = 0; j < table.Columns.Count; j++)
                {
                    if (j < table.Columns.Count - 1)
                    {
                        JSONString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString() + "\",");
                    }
                    else if (j == table.Columns.Count - 1)
                    {
                        JSONString.Append("\"" + table.Columns[j].ColumnName.ToString() + "\":" + "\"" + table.Rows[i][j].ToString() + "\"");
                    }
                }
                if (i == table.Rows.Count - 1)
                {
                    JSONString.Append("}");
                }
                else
                {
                    JSONString.Append("},");
                }
            }
            JSONString.Append("]");
        }
        return JSONString.ToString();
    }

    public static DataColumn CreateColumn(string nameColumn, string _type = "String", bool readOnly = false, bool autoIncrement = false)
    {
        DataColumn column = new DataColumn();
        column.DataType = Type.GetType("System." + _type);
        column.ColumnName = nameColumn;
        column.AutoIncrement = autoIncrement;
        column.Caption = nameColumn;
        column.ReadOnly = readOnly;
        column.Unique = false;
        return column;
    }

    public static string FormatNumber(string s)
    {
        s = s.Replace('.', '_');
        s = s.Replace(',', '.');
        s = s.Replace('_', ',');
        return s;
    }
}