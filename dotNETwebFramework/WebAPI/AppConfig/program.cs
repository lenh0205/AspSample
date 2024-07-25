var builder = WebApplication.CreateBuilder(args);

Console.OutputEncoding = System.Text.Encoding.UTF8;

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Razor Page
builder.Services.AddRazorPages();

/// controller
builder.Services.AddControllers()
               .AddJsonOptions(options =>
               {
                   options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
                   options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals;
               });
// v2:
builder.Services.AddControllers().AddNewtonsoftJson(options => {
    options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Utc;
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    options.SerializerSettings.Formatting = Formatting.Indented;
});
///

/// Enpoint Api
builder.Services.AddEndpointsApiExplorer();
///

/// HttpContext Accessor
builder.Services.AddHttpContextAccessor();
///

/// Swagger
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SER", Version = "v1" });
});
// v2:
builder.Services.AddSwaggerGen(c => c.SwaggerDoc("v0", new OpenApiInfo { Title = "Lâm sản Api", Version = "v0.1", Description = "Developer by Văn Toàn, Khang, Phát" }));

/// DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"));
});
// v2:
builder.Services.AddDbContext<ForestContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString(nameof(ForestContext)),
    sqlOptions => {
        sqlOptions.MigrationsAssembly(typeof(ForestContext).Assembly.GetName().Name);
        sqlOptions.EnableRetryOnFailure(3);
    }), ServiceLifetime.Transient);
///

/// Mapper
var config = new MapperConfiguration(cfg =>
{
    cfg.AllowNullCollections = true;
    cfg.AddProfile(new MappingProfile());
});
var mapper = config.CreateMapper();
builder.Services.AddSingleton(mapper);
// v2:
builder.Services.AddMapper();
///

/// Add Services:
builder.Services
.AddScoped(typeof(IGenericRepository<,>), typeof(GenericRepository<,>))
.AddScoped(typeof(IGenericService<,>), typeof(GenericService<,>))
.AddScoped(typeof(IUnitOfWork<,>), typeof(UnitOfWork<,>))
.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork))
.AddScoped(typeof(ISeedService), typeof(SeedService));
builder.Services.AddScoped<IExternalApiServices, ExternalApiServices>();
// v2:
builder.Services.AddService();
///


var app = builder.Build();
using (var serviceScope = app.Services.CreateScope())
{
    var services = serviceScope.ServiceProvider;

    var myDependency = services.GetRequiredService<IExternalApiServices>();

    myDependency.GetDataFromConfig();
    myDependency.ReceiveExternalData();

}

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionMiddleware>();
app.Services.UseJobs();

app.UseCors(config => config
    .AllowAnyMethod()
    .AllowAnyHeader()
    .SetIsOriginAllowed(origin => true)
    .WithExposedHeaders("*")
);

/// Swagger:    
app.UseSwagger();
app.UseSwaggerUI(c => {});
// v2:
app.UseSwagger().UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v0/swagger.json", "LamSan.Api v0"); c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None); });
///

app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseHttpsRedirection();
app.UseHttpLogging();
app.UseRouting();
app.MapControllers();
app.Run();

app.MapRazorPages();
app.MapControllers();
app.Run();

// Migration
// -> the process of keeping your "database schema" in sync with "entity data model"
// -> make changes to data model <=> create a migration - captures changes and applies them to the database schema













