======================================================================
# The Identity model

## Entity types and their default Common Language Runtime (CLR) types
* -> **`User`** - **`IdentityUser`** - represents the user
* -> **`Role`** - **`IdentityRole`** - represents a role
* -> **`UserRole`** - **`IdentityUserRole`** - a join entity that associates users and roles

* -> **`UserLogin`** - **`IdentityUserLogin`** - associates a user with a login
* -> **`UserToken`** - **`IdentityUserToken`** - represents an authentication token for a user
* -> **`UserClaim`** - **`IdentityUserClaim`** - represents a claim that a user possesses
* -> **`RoleClaim`** - **`IdentityRoleClaim`** - represents a claim that's granted to all users within a role

* => rather than using these types directly, the types can be used as **`base classes`** for the app's own types
* => **the DbContext classes defined by Identity** are **`generic`**, such that **`different CLR types can be used for one or more of the entity types in the model`**
* _these generic types also **allow the User primary key (PK) data type to be changed**_

## Entity type relationships
* -> each **User** can have many associated **Roles**, and each **Role** can be associated with many **Users**
* -> this is a **many-to-many relationship** that **`requires a join table`** in the database
* -> the join table is represented by the **UserRole** entity

* -> each **User** can have many **UserClaims**
* -> each **User** can have many **UserLogins**
* -> each **User** can have many **UserTokens**
* -> each **Role** can have many associated **RoleClaims**

## Default model configuration
* _default configuration of Identity using **`EF Core Code First Fluent API`**:_

```cs
builder.Entity<TUser>(b =>
{
    // Primary key
    b.HasKey(u => u.Id);

    // Indexes for "normalized" username and email, to allow efficient lookups
    b.HasIndex(u => u.NormalizedUserName).HasName("UserNameIndex").IsUnique();
    b.HasIndex(u => u.NormalizedEmail).HasName("EmailIndex");

    // Maps to the AspNetUsers table
    b.ToTable("AspNetUsers");

    // A concurrency token for use with the optimistic concurrency checking
    b.Property(u => u.ConcurrencyStamp).IsConcurrencyToken();

    // Limit the size of columns to use efficient database types
    b.Property(u => u.UserName).HasMaxLength(256);
    b.Property(u => u.NormalizedUserName).HasMaxLength(256);
    b.Property(u => u.Email).HasMaxLength(256);
    b.Property(u => u.NormalizedEmail).HasMaxLength(256);

    // The relationships between User and other entity types
    // Note that these relationships are configured with no navigation properties

    // Each User can have many UserClaims
    b.HasMany<TUserClaim>().WithOne().HasForeignKey(uc => uc.UserId).IsRequired();

    // Each User can have many UserLogins
    b.HasMany<TUserLogin>().WithOne().HasForeignKey(ul => ul.UserId).IsRequired();

    // Each User can have many UserTokens
    b.HasMany<TUserToken>().WithOne().HasForeignKey(ut => ut.UserId).IsRequired();

    // Each User can have many entries in the UserRole join table
    b.HasMany<TUserRole>().WithOne().HasForeignKey(ur => ur.UserId).IsRequired();
});

builder.Entity<TUserClaim>(b =>
{
    // Primary key
    b.HasKey(uc => uc.Id);

    // Maps to the AspNetUserClaims table
    b.ToTable("AspNetUserClaims");
});

builder.Entity<TUserLogin>(b =>
{
    // Composite primary key consisting of the LoginProvider and the key to use
    // with that provider
    b.HasKey(l => new { l.LoginProvider, l.ProviderKey });

    // Limit the size of the composite key columns due to common DB restrictions
    b.Property(l => l.LoginProvider).HasMaxLength(128);
    b.Property(l => l.ProviderKey).HasMaxLength(128);

    // Maps to the AspNetUserLogins table
    b.ToTable("AspNetUserLogins");
});

builder.Entity<TUserToken>(b =>
{
    // Composite primary key consisting of the UserId, LoginProvider and Name
    b.HasKey(t => new { t.UserId, t.LoginProvider, t.Name });

    // Limit the size of the composite key columns due to common DB restrictions
    b.Property(t => t.LoginProvider).HasMaxLength(maxKeyLength);
    b.Property(t => t.Name).HasMaxLength(maxKeyLength);

    // Maps to the AspNetUserTokens table
    b.ToTable("AspNetUserTokens");
});

builder.Entity<TRole>(b =>
{
    // Primary key
    b.HasKey(r => r.Id);

    // Index for "normalized" role name to allow efficient lookups
    b.HasIndex(r => r.NormalizedName).HasName("RoleNameIndex").IsUnique();

    // Maps to the AspNetRoles table
    b.ToTable("AspNetRoles");

    // A concurrency token for use with the optimistic concurrency checking
    b.Property(r => r.ConcurrencyStamp).IsConcurrencyToken();

    // Limit the size of columns to use efficient database types
    b.Property(u => u.Name).HasMaxLength(256);
    b.Property(u => u.NormalizedName).HasMaxLength(256);

    // The relationships between Role and other entity types
    // Note that these relationships are configured with no navigation properties

    // Each Role can have many entries in the UserRole join table
    b.HasMany<TUserRole>().WithOne().HasForeignKey(ur => ur.RoleId).IsRequired();

    // Each Role can have many associated RoleClaims
    b.HasMany<TRoleClaim>().WithOne().HasForeignKey(rc => rc.RoleId).IsRequired();
});

builder.Entity<TRoleClaim>(b =>
{
    // Primary key
    b.HasKey(rc => rc.Id);

    // Maps to the AspNetRoleClaims table
    b.ToTable("AspNetRoleClaims");
});

builder.Entity<TUserRole>(b =>
{
    // Primary key
    b.HasKey(r => new { r.UserId, r.RoleId });

    // Maps to the AspNetUserRoles table
    b.ToTable("AspNetUserRoles");
});
```

## Model generic types

```cs - using Identity with support for roles
// Uses all the built-in Identity types
// Uses `string` as the key type
public class IdentityDbContext
    : IdentityDbContext<IdentityUser, IdentityRole, string>
{
}

// Uses the built-in Identity types except with a custom User type
// Uses `string` as the key type
public class IdentityDbContext<TUser>
    : IdentityDbContext<TUser, IdentityRole, string>
        where TUser : IdentityUser
{
}

// Uses the built-in Identity types except with custom User and Role types
// The key type is defined by TKey
public class IdentityDbContext<TUser, TRole, TKey> : IdentityDbContext<
    TUser, TRole, TKey, IdentityUserClaim<TKey>, IdentityUserRole<TKey>,
    IdentityUserLogin<TKey>, IdentityRoleClaim<TKey>, IdentityUserToken<TKey>>
        where TUser : IdentityUser<TKey>
        where TRole : IdentityRole<TKey>
        where TKey : IEquatable<TKey>
{
}

// No built-in Identity types are used; all are specified by generic arguments
// The key type is defined by TKey
public abstract class IdentityDbContext<
    TUser, TRole, TKey, TUserClaim, TUserRole, TUserLogin, TRoleClaim, TUserToken>
    : IdentityUserContext<TUser, TKey, TUserClaim, TUserLogin, TUserToken>
         where TUser : IdentityUser<TKey>
         where TRole : IdentityRole<TKey>
         where TKey : IEquatable<TKey>
         where TUserClaim : IdentityUserClaim<TKey>
         where TUserRole : IdentityUserRole<TKey>
         where TUserLogin : IdentityUserLogin<TKey>
         where TRoleClaim : IdentityRoleClaim<TKey>
         where TUserToken : IdentityUserToken<TKey>
```

```cs - use Identity without roles (only claims)
// Uses the built-in non-role Identity types except with a custom User type
// Uses `string` as the key type
public class IdentityUserContext<TUser> : IdentityUserContext<TUser, string> where TUser : IdentityUser
{
}

// Uses the built-in non-role Identity types except with a custom User type
// The key type is defined by TKey
public class IdentityUserContext<TUser, TKey> 
    : IdentityUserContext<
        TUser, 
        TKey, 
        IdentityUserClaim<TKey>, IdentityUserLogin<TKey>,
        IdentityUserToken<TKey>
    > where TUser : IdentityUser<TKey> where TKey : IEquatable<TKey>
{}

// No built-in Identity types are used; all are specified by generic arguments, with no roles
// The key type is defined by TKey
public abstract class IdentityUserContext<TUser, TKey, TUserClaim, TUserLogin, TUserToken> 
    : DbContext
        where TUser : IdentityUser<TKey>
        where TKey : IEquatable<TKey>
        where TUserClaim : IdentityUserClaim<TKey>
        where TUserLogin : IdentityUserLogin<TKey>
        where TUserToken : IdentityUserToken<TKey>
{
}
```

======================================================================
# Customize the model
* _the starting point for model customization is to **derive from the appropriate context type**_

## Custom Identity DbContext
* _https://learn.microsoft.com/en-us/aspnet/core/security/authentication/customize-identity-model?view=aspnetcore-6.0#model-generic-types_

## Custom user data
* -> custom user data is supported by inheriting from **`IdentityUser`**

* -> if we add a property to our **Identity user class**
* -> there's **no need to `override 'OnModelCreating'` in the ApplicationDbContext class**, EF Core **`maps the property by convention`**
* -> however, the **`database needs to be updated to create a new custom column`** by adding a migration, and then update the database 

```cs - custom user data
public class ApplicationUser : IdentityUser
{
    public string CustomTag { get; set; }
}

// generic argument for the context
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}
```

```cs - update in "Program.cs" config
services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
        .AddEntityFrameworkStores<ApplicationDbContext>();

// this equivalent to:
services.AddAuthentication(o =>
{
    o.DefaultScheme = IdentityConstants.ApplicationScheme;
    o.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
.AddIdentityCookies(o => { });

services.AddIdentityCore<TUser>(o =>
{
    o.Stores.MaxLengthForKeys = 128;
    o.SignIn.RequireConfirmedAccount = true;
})
.AddDefaultUI()
.AddDefaultTokenProviders();     
```

## Change the primary key type
* -> a **change to the PK column's data type after the database has been created** is **`problematic on many database systems`**
* -> **changing the PK** typically involves **`dropping`** and **`re-creating`** the table
* -> therefore, **key types** should be **`specified in the initial migration`** when the database is created

* => steps to change the PK type:

### Drop Database
* -> if the **database was created before the PK change**, run **`Drop-Database (PMC)`** or **`dotnet ef database drop (.NET CLI)`** to delete it

### Remove Migration
* -> after confirming deletion of the database, **remove the initial migration** with **`Remove-Migration (PMC)`** or **`dotnet ef migrations remove (.NET CLI)`**

### Update DbContext
* -> **update the ApplicationDbContext class** to **`derive from IdentityDbContext<TUser,TRole,TKey>`**
* -> **`specify the new key type for TKey`**

```cs - For example: to use a "Guid" key type

// the generic classes IdentityUser<TKey> and IdentityRole<TKey> must be specified to use the new key type
public class ApplicationDbContext : IdentityDbContext<IdentityUser<Guid>, IdentityRole<Guid>, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
}
```

```cs - Startup.ConfigureServices must be updated to use the generic user:
services.AddDefaultIdentity<IdentityUser<Guid>>(options => options.SignIn.RequireConfirmedAccount = true)
        .AddEntityFrameworkStores<ApplicationDbContext>();
```

### Update custom Identity User class (if exist)
* -> if a custom **ApplicationUser** class is being used, **`update the class to inherit from IdentityUser`**

```cs
public class ApplicationUser : IdentityUser<Guid>
{
    public string CustomTag { get; set; }
}
```

```cs - update "ApplicationDbContext" to reference the custom "ApplicationUser" class
public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
}
```

```cs - Startup.ConfigureServices
// the primary key's data type is inferred by analyzing the DbContext object
services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
        .AddEntityFrameworkStores<ApplicationDbContext>();
```

### Update custom Identity Role class (if exist)
* -> if a custom **ApplicationRole** class is being used, **`update the class to inherit from IdentityRole<TKey>`**

```cs
public class ApplicationRole : IdentityRole<Guid>
{
    public string Description { get; set; }
}
```

```cs - update "ApplicationDbContext" to reference the custom "ApplicationRole" class
// references a custom "ApplicationUser" and a custom "ApplicationRole"
public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
}
```

```cs - Startup.ConfigureServices
public void ConfigureServices(IServiceCollection services)
{
    services.Configure<CookiePolicyOptions>(options =>
    {
        options.CheckConsentNeeded = context => true;
        options.MinimumSameSitePolicy = SameSiteMode.None;
    });

    services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(
            Configuration.GetConnectionString("DefaultConnection")));

    services.AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultUI()
            .AddDefaultTokenProviders();

    services.AddMvc()
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
}
```

### Add navigation properties
* -> changing the model configuration for **`relationships can be more difficult`** than making other changes
* -> care must be taken to **`replace the existing relationships rather than create new, additional relationships`**
* -> in particular, the changed relationship must **`specify the same foreign key (FK) property as the existing relationship`**

```cs - Example: the relationship between "Users" and "UserClaims" by default is:
builder.Entity<TUser>(b =>
{
    // Each User can have many UserClaims
    b.HasMany<TUserClaim>()
     .WithOne()
     .HasForeignKey(uc => uc.UserId)
     .IsRequired();
});
```