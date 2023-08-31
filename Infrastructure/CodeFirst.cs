public partial class ApplicationDbContext : DbContext
{

    private readonly IConfiguration _configuration;
    public ApplicationDbContext(IConfiguration configuration) => _configuration = configuration;

    public ApplicationDbContext(IConfiguration configuration, DbContextOptions<ApplicationDbContext> options)
    : base(options) => _configuration = configuration;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"),
                x => x.UseNetTopologySuite()); 
        }

        // Thêm 1 dòng này thì "Add-Migration" sẽ tự động tạo bảng:
        public virtual DbSet<HoSoCongViec> HoSoCongViecs { get; set; }


        //  "configure the model" that Entity Framework uses to map your classes to the database
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { 
            new HoSoCongViecConfiguration().Configure(modelBuilder.Entity<HoSoCongViec>());
        } 
    }
}

public class HoSoCongViecConfiguration : IEntityTypeConfiguration<HoSoCongViec>
{
    public void Configure(EntityTypeBuilder<HoSoCongViec> builder)
    {
        builder.ToTable("LTHS_HOSOCONGVIEC");
        builder.HasKey(e => e.Id); // primary key
        builder.Property(e=>e.MaHoSo).HasMaxLength(13).HasDefaultValue(string.Empty);

        builder.HasQueryFilter(e => !e.IsDeleted); 
        // -> áp thêm .Where(x => !x.IsDeleted) cho mọi câu query thực hiện trên context này
        // -> thường dùng cho "soft delete"
    }
}

# RelationshipConfig
//Model:
public class Blog
{
    public int BlogId { get; set; }
    public List<Post> Posts { get; set; }
}
public class Post
{
    public int PostId { get; set; }
    public int BlogId { get; set; }
    public Blog Blog { get; set; }
}

## Nguyên tắc:
// just need to configure the relationship on either the "Blog" or "Post", 
// EF Core'll automatically config the other side of the relationship for us

## Case 1: Cả 2 đều có Navigation property
modelBuilder.Entity<Blog>()
    .HasMany(b => b.Posts) // "Blog" can have many "Posts"
    .WithOne(p => p.Blog) // "Post" belongs to only one "Blog"
    .HasForeignKey(p => p.BlogId) // "Post" has a foreign key "BlogId" references the "Blog" primary key
    .IsRequired(); // every Post must have a non-null value for its foreign key property
// hoặc là:
modelBuilder.Entity<Post>()
    .HasOne(p => p.Blog) // "Post" is belong to only one "Blog"
    .WithMany(b => b.Posts) // 1 "Blog" có nhiều "Posts"
    .HasForeignKey(p => p.BlogId);

## Case 2: Blog không có navigation property "public List<Post> Posts { get; set; }"
modelBuilder.Entity<Blog>()
    .HasMany<Post>()
    .WithOne(p => p.Blog)
    .HasForeignKey(p => p.BlogId);
// hoặc là:
modelBuilder.Entity<Post>()
    .HasOne(p => p.Blog)
    .WithMany()
    .HasForeignKey(p => p.BlogId);

## Case 3: cả hai đều không có navigation property
// Entity Framework Core still be able to map the relationship and generate the appropriate database schema
modelBuilder.Entity<Blog>()
    .HasMany<Post>()
    .WithOne()
    .HasForeignKey(p => p.BlogId);
// hoặc là:
modelBuilder.Entity<Post>()
    .HasOne<Blog>()
    .WithMany()
    .HasForeignKey(p => p.BlogId);

// Ngoài ra:
.HasPrincipalKey 
// -> By default, Entity Framework Core will use the primary key of the principal entity as the principal key for a relationship, 
// -> but you can use the HasPrincipalKey method to specify a different property as the principal key if needed

# NavigationProperty
// -> are not necessary for defining relationships between entities
// -> but provide a convenient way to navigate the relationship in your code
// -> EF Core sẽ nhận thức được Navigation Property do có type là reference/collection types thay vì primitive type
// -> và sẽ không map những NavigationProperty to a column in the database
var blog = context.Blogs.Find(blogId);
var posts = blog.Posts; // model have navigation property
// Instead of:
var blog = context.Blogs.Find(blogId);
var posts = context.Posts.Where(p => p.BlogId == blog.BlogId).ToList(); // model don't have navigation property

# [NotMapped] data annotation / "Ignore" fluent API  
// specify that a property or class should not be mapped to a database table or column
public class Product
{
    public int ProductId { get; set; }
    public decimal Price { get; set; }
    public decimal TaxRate { get; set; }

    [NotMapped]
    public decimal PriceWithTax => Price * (1 + TaxRate);
}

# LazyLoading
// Assumpt Blog have 2 navigation properties: Posts and Author

// Lazy Loading: 
var blog = context.Blogs.Find(blogId);
var posts = blog.Posts; // First database query to load the Posts
var author = blog.Author; // Second database query to load the Author

// Eager Loading:
var blog = context.Blogs.Include(b => b.Posts).Include(b => b.Author).Single(b => b.BlogId == blogId);
var posts = blog.Posts; // No database query, data is already loaded
var author = blog.Author; // No database query, data is already loaded


