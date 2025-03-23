# EntityState: “detached”, “unchanged”, “added”, “modified”, and “deleted”.
// -> là 1 enum của Microsoft.EntityFrameworkCore

// Entry method can only be used with entities that are being tracked by the context 
using (var context = new MyDbContext())
{
    // Create a new entity
    var newEntity = new MyEntity { Name = "New Entity" }; // Status: EntityState.Detached
    // -> Detached state - not being tracked by the context
    //  -> khi ở status là "Detached" , dù ta update, add,... thì trạng thái nó vẫn là "Detached"

    // Add the new entity to the context
    context.MyEntities.Add(newEntity);
    Console.WriteLine(context.Entry(newEntity).State); // Output: Added (EntityState.Added)

    // Save changes to insert the new entity into the database
    context.SaveChanges();
    Console.WriteLine(context.Entry(newEntity).State); // Output: Unchanged (EntityState.Unchanged)

    // Modify a property of the new entity
    newEntity.Name = "Modified Entity";
    Console.WriteLine(context.Entry(newEntity).State); // Output: Modified (EntityState.Modified)

    // Save changes to update the new entity in the database
    context.SaveChanges();
    Console.WriteLine(context.Entry(newEntity).State); // Output: Unchanged (EntityState.Unchanged)
    // "Unchanged" tức là entity đang được tracked bởi Dbcontext

    // Mark the new entity for deletion
    context.MyEntities.Remove(newEntity);
    Console.WriteLine(context.Entry(newEntity).State); // Output: Deleted (EntityState.Deleted)

    // Save changes to delete the new entity from the database
    context.SaveChanges();

    // The newEntity is now detached from the context
}


# ChangingEnityState
using (var context = new BloggingContext())
{
    var blog = context.Blogs.Find(1);
    context.Entry(blog).State = EntityState.Detached;
}

# AsNoTracking in Entity Framework
// -> an extension method for the "IQueryable" interface
// By default, EntityFramework tracks changes on entities that are queried and saves changes when calling SaveChanges() on the context
// "AsNoTracking" make the entities returned by the query aren't being tracked by change Tracker,so any changes made to the entity will not be persisted to the database when call SaveChanges
// which means that they are in a "detached" state

//  useful to retrieve data from the database for read-only purposes ; 
// don’t need to update the entities or track changes 
// improve the performance 

// Usage:
var blogs = context.Blogs.AsNoTracking().ToList(); 

// change the default tracking behavior for all queries executed by a DbContext instance:
context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

// or:
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(connectionString);
    options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});


# Context

public class Program
{
    public static void Main()
    {
        // Retrieve the entity from the database using one context
        MyEntity myEntity;
        using (var context = new MyDbContext())
        {
            myEntity = context.MyEntities.First();
        }

        // Try to delete the entity using a different context
        using (var context = new MyDbContext())
        {
            if (context.Entry(myEntity).State == EntityState.Detached)
            {
                context.MyEntities.Attach(myEntity);
            }
            // if not checking: throw an InvalidOperationException because the entity is detached from the context
            context.MyEntities.Remove(myEntity);
            context.SaveChanges();
        }
    }
}
