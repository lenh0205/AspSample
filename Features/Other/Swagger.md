
* -> Install **Swashbuckle.AspNetCore 6.0.0** (_sử dụng version 5.0 đôi khi có một số lỗi_)

```cs
builder.Services.AddEndpointsApiExplorer(); // chỉ dùng dòng này nếu ta đang s/d minimal API
builder.Services.AddSwaggerGen();

app.UseSwagger();
app.UseSwaggerUI();
```