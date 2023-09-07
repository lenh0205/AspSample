# ExtensionMethod
// -> a static method 
// -> allows you to add methods to existing types 
// => without creating a new derived type, recompiling, or otherwise modifying the original type
// Extension methods are called as if they were instance methods on the extended type.

# Define
// -> define a static class to contain the extension method
// -> implement the extension method as a static method with at least the same visibility as the containing class
// -> The first parameter "specifies the type" that the method operates on +  preceded with the "this" modifier
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

// Usage:
using ExtensionMethods;

string s = "Hello Extension Methods"; 
int i = s.WordCount();


# LINQ standard query operators 
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