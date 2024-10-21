
## DI without interface
```cs
builder.Services.AddScoped<UserService>();
```

## DI with Generic 

```cs
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
```


## DI with Factory