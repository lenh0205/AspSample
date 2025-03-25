> suy đoán: đầu tiên sử dụng **Command** pattern để tạo 1 layer giữa logic điều hướng (Controller) và logic business (các Handler)
> nhưng khi 1 Controller cần giao tiếp với quá nhiều Command thì ta sẽ sử dụng Mediator để tập trung các Command, Controller chỉ cần giao tiếp với Mediator thông qua Notify(Command)

# MediatR
* -> phần logic sẽ để trong Handler **`IRequestHandler<in TRequest, TResponse>`**
* -> **TRequest** ở đây là "Query" hoặc "Command" **`IRequest<out TResponse>`**
* -> để gọi đến Handler ta sẽ dùng **`_mediator.Send(IRequest request);`**

* -> install MediatR
```bash
$ dotnet add package MediatR.Extensions.Microsoft.DependencyInjection
```

* -> config
```cs
// do trường hợp này ta define query, command, handler trong "DemoLibraryMediatREntrypoint" class library
services.AddMediatR(typeof(DemoLibraryMediatREntrypoint).Assembly);
```

* -> Controllers
```cs
[Route("api/[controller]")]
[ApiController]
public class PersonController : ControllerBase
{
    private readonly IMediator _mediator;

    public PersonController(IMediator mediator)
    {
        _mediator = mediator;
    }

    // GET: api/<PersonController>
    [HttpGet]
    public async Task<List<PersonModel>> Get()
    {
        return await _mediator.Send(new GetPersonListQuery());
    }

    // GET api/<PersonController>/5
    [HttpGet("{id}")]
    public async Task<PersonModel> Get(int id)
    {
        return await _mediator.Send(new GetPersonByIdQuery(id));
    }

    // POST api/<PersonController>
    [HttpPost]
    public async Task<PersonModel> Post([FromBody] PersonModel value)
    {
        var model = new InsertPersonCommand(value.FirstName, value.LastName);
        return await _mediator.Send(model);
    }
}
```

```cs
// GetPersonListQuery.cs
public record GetPersonListQuery() : IRequest<List<PersonModel>>;

// GetPersonListHandler.cs
public class GetPersonListHandler : IRequestHandler<GetPersonListQuery, List<PersonModel>>
{
    private readonly IDataAccess _data;

    public GetPersonListHandler(IDataAccess data)
    {
        _data = data;
    }

    public Task<List<PersonModel>> Handle(GetPersonListQuery request, CancellationToken cancellationToken)
    {
        return Task.FromResult(_data.GetPeople());
    }
}
```

```cs
// GetPersonByIdQuery.cs
public record GetPersonByIdQuery(int Id) : IRequest<PersonModel>;

// GetPersonByIdHandler.cs
public class GetPersonByIdHandler : IRequestHandler<GetPersonByIdQuery, PersonModel>
{
    private readonly IMediator _mediator;

    public GetPersonByIdHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<PersonModel> Handle(GetPersonByIdQuery request, CancellationToken cancellationToken)
    {
        var results = await _mediator.Send(new GetPersonListQuery());

        var output = results.FirstOrDefault(x => x.Id == request.Id);

        return output;
    }
}
```

```cs
// InsertPersonCommand.cs
public record InsertPersonCommand(string FirstName, string LastName) : IRequest<PersonModel>;

// GetPersonByIdHandler.cs
public class GetPersonByIdHandler : IRequestHandler<GetPersonByIdQuery, PersonModel>
{
    private readonly IMediator _mediator;

    public GetPersonByIdHandler(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<PersonModel> Handle(GetPersonByIdQuery request, CancellationToken cancellationToken)
    {
        var results = await _mediator.Send(new GetPersonListQuery());

        var output = results.FirstOrDefault(x => x.Id == request.Id);

        return output;
    }
}
```