
# SaveChanges()
// -> persist all changes (added, modified, or deleted entities) made in the context to the database
// -> SaveChanges() applying the changes to the database
// -> for all entity is being tracked by the Entity Framework’s change tracker
// -> các entity có được SaveChange() cần có Id đồng nhất với database nêu không sẽ lỗi

// Example: dù enity được lấy và update ở scope khác, SaveChanges() vẫn bắt được
foreach(var mucluc in lstMucLucForUpdate)
{
    var trackedEntity = _context.MucLucs.FirstOrDefault(_ => _.Id == mucluc.Id);
    trackedEntity.MaDV = mucluc.MaDV ?? string.Empty;
}
await _context.SaveChangesAsync();


# Dispose()
// -> releases all resources used by the context
// -> do not need to call Dispose() directly in ASP.NET Core, as DI library will handle disposal for us
// -> "using" block also auto call "Dispose()" when exit the block
// ->  can use creating and managing the lifetime of the context manually 
// -> call when finished using the context

var context = new MyContext();
try
{
    // Perform data access using the context
}
finally
{
    context.Dispose();
}


# Update()
// -> begins "tracking" the given entity in the "Modified" state if entity’s primary key value is set
// -> If the entity’s primary key value is not set (empty, null, or the default value for the specified data type), 
// => the "Update" method considers it a new entity and sets its "EntityState" to "Added"
// -> it will be updated in the database when SaveChanges() is called

// -> update multiple entities of different types to the context ("Update()" method của DbSet chỉ có thể s/d vs các entity có type của DbSet)
using (var context = new BloggingContext())
{
    var blog = new Blog { BlogId = 1, Url = "http://example.com/blog" };
    context.Update(blog);
    context.SaveChanges();
}

# UpdateRange()
// -> giống "Update()" nhưng cho nhiều phần tử
var entity1 = new MyEntity { Id = 1, Name = "Updated Name 1" };
var entity2 = new MyEntity { Id = 2, Name = "Updated Name 2" };
var entity3 = new MyEntity { Id = 3, Name = "Updated Name 3" };

var entities = new List<MyEntity> { entity1, entity2, entity3 };
context.UpdateRange(entities);
// hoặc:
context.UpdateRange(entity1, entity2, entity3);


# Entry(entity)
// -> returns an EntityEntry object
// -> provides access to information and the ability to perform actions on the entity
// ->  change the state of an entity, get the current values of an entity, get the original values of an entity, ...

## ".State" property
// -> to get or set the state of an entity
// => change the state of an entity
// -> ta cũng có thể đổi trạng thái của entity từ "Detached" thành "Modified" (phải khác primary key)
public async Task UpdateYourEntityAsync(YourEntity yourEntity)
{
    _context.Entry(yourEntity).State = EntityState.Modified;

    // Prevent the "Name" property from being updated
    _context.Entry(yourEntity).Property(e => e.Name).IsModified = false; 

    await _context.SaveChangesAsync();
}

## ".CurrentValues" property: 
// -> get or set the current values of an entity
// => to update an entity with new values:
var entity = context.Entities.Find(1);
var currentValues = context.Entry(entity).CurrentValues;
currentValues.SetValues(newValues);

## ".OriginalValues" property: 
// -> get or set the original values of an entity. 
// => revert changes made to an entity:
var entity = context.Entities.Find(1);
var entry = context.Entry(entity);
entry.CurrentValues.SetValues(entry.OriginalValues);
entry.State = EntityState.Unchanged;

## ".GetDatabaseValues" property: 
// -> get the values of an entity as they currently exist in the database. 
// => to refresh an entity with values from the database:
var entity = context.Entities.Find(1);
var databaseValues = context.Entry(entity).GetDatabaseValues();
context.Entry(entity).CurrentValues.SetValues(databaseValues);

## ".Reload()" method: 
// -> reload an entity from the database, 
// -> overwriting any property values with values from the database
var entity = context.Entities.Find(1);
context.Entry(entity).Reload();


# Set<TEntity>()
// ->  returns a DbSet<TEntity> instance for access to entities of the given type in the context

using (var context = new BloggingContext())
{
    var blogs = context.Set<Blog>();
    var blog = new Blog { Url = "http://example.com" };
    blogs.Add(blog);
    context.SaveChanges();
}


# Database
// -> gets a "DatabaseFacade" instance; allows to perform database related operations on the context

using (var context = new BloggingContext())
{
    var connection = context.Database.GetDbConnection();
}