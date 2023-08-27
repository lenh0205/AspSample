
# SaveChanges()
// -> persist all changes (added, modified, or deleted entities) made in the context to the database

context.Blogs.Add(blog);
context.SaveChanges();


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
// -> begins tracking the given entity in the "Modified" state 
// -> it will be updated in the database when SaveChanges() is called
using (var context = new BloggingContext())
{
    var blog = new Blog { BlogId = 1, Url = "http://example.com/blog" };
    context.Update(blog);
    context.SaveChanges();
}

# Update()
// -> giống "Update()" nhưng cho nhiều phần tử
var entity1 = new MyEntity { Id = 1, Name = "Updated Name 1" };
var entity2 = new MyEntity { Id = 2, Name = "Updated Name 2" };
var entity3 = new MyEntity { Id = 3, Name = "Updated Name 3" };

var entities = new List<MyEntity> { entity1, entity2, entity3 };
context.UpdateRange(entities);
// hoặc:
context.UpdateRange(entity1, entity2, entity3);


# Entry
// -> provides access to information and the ability to perform actions on the entity
// ->  change the state of an entity, get the current values of an entity, get the original values of an entity, ...

public async Task UpdateYourEntityAsync(YourEntity yourEntity)
{
    _context.Entry(yourEntity).State = EntityState.Modified;

    // Prevent the "Name" property from being updated
    _context.Entry(yourEntity).Property(e => e.Name).IsModified = false; 

    await _context.SaveChangesAsync();
}


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