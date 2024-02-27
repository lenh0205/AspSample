# Error: Actions require unique method/path combination for Swagger
* -> trong controller thay [Route("api/[controller]")] bằng [Route("api/[controller]/[action]")] ???
* -> hoặc define explicit route for action: [Route("GetById")]

# 
```C#
[HttpGet("{id}")]
public async Task<DataStoreQuery> GetByIdAsync(int id)
```

