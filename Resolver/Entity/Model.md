=========================================
# Data Tranfer Object - DTO
* are objects in patterns used to carries data in software applications for **Web Layer**
* iis on the `Service Abstractiton` (**contract**) or `Application Services`
```cs
// don't need whole information in response 
public class PersonDto 
{
    public int PersonId {get; set;}
    public string FullName {get; set;}
    public int Age {get; set;}
} 
```

## Problem
* **`directly exposing domain entites to the client`**
```cs
[HttpGet]
public async Task<IActionResult> GetCities()
{
    var cities = await uwo.CityRepository.GetCitiesAsync();
    return Ok(cities); // directly return "city" entity
}
[HttpPost("post")]
public async Task<IActionResult> AddCity(City city)
{
    uow.CityRepository.AddCity(entity); 
    await uow.SaveAsync(); // directly save "city" entity that receives from client
    return StatusCode(201)
}
// -> means API Client is able to send and receive the objects that directly map to database table
```

### Hide properties that clients are not supposed to view
```cs
// Giả sử: "city" model hiện tại có 2 property: Id và Name; nhưng ta muốn thêm 2 additinal fiels nữa : UpdatedAt và UpdatedBy
// -> ta sẽ cần migrate lại Database (add 2 new columns)
// -> nếu H ta GET method to fetch city list, API response sẽ xuất hiện thêm 2 trường mới
```
* Malicious users will be able to get ID of various users and this info may sensitive

### Avoid "over-posting" vulnerabilities
```cs
// Malicious user would be able to POST those additional fields to our API method and save successfully to database, that they are not supposed to POST
```

### Decouple client from internal implementation 
```cs
// Giả sử ta cần đổi property "Name" thành "CityName"
// -> all API Clients will have to make changes in their application
```
* **`Client should never depend on API internal implementation`**
* we should be able to `change internal implementation without changing client`

## Solution
* all 3 problems can be solved by **adding a service layer** around these domain entities
* -> this layer `exposes the different sets of classes`, which look similar to domain entities
* -> but can be **`changed and evolved independently`**

* These classes are called **DTO** -> our **Controller will take DTO as input and return DTO as output**
* -> DTOs are the public interface to the domain entities
* -> changes in domain entities do not impact the client until service layer is modified

```cs
// Implement DTO example:
// -> tạo thư mục Dtos và tạo class CityDto
public class CityDto 
{
    public int Id {get; set;}
    public string Name {get; set;}
}


// in CityController:
[HttpPost("post")]
public async Task<IActionResult> AddCity(CityDto city)
{
    // CityRepository cần "City" object -> cần map "CityDto" to "City" 
    var city = new City {
        Name = CityDto.Name,
        UpdatedBy = 1,
        UpdatedAt = DateTime.Now
    };
    uow.CityRepository.AddCity(entity); 
    await uow.SaveAsync(); 
    return StatusCode(201)
}

// -> H thì cho dù ta có POST với 1 object với 2 trường "UpdatedBy" và "UpdatedAt" thì JSON fomatter cũng sẽ ignore
```

## Web Layer
* communicate with `Service Layer` - ask the final data to the end users to reponse API Reponse 

============================================
# Model
* are objects in patterns used to carries data in software applications for **Service Layer**

## View Model
* a screen level state holder 
* -> exposes state to UI and enscapsulates related business logic (_means view/page doesn't deal with business_)
* -> represents the data we want to display on the `Presentation Layer`
```cs
// có thêm field mới perform by some "business logic" or for "business logic"
public class Person 
{
    public int PersonId {get; set;}
    public string FirstName {get; set;}
    public string LastName {get; set;}
    public string FullName {get; set;}
    public DateTime DateOfBirth {get; set;}
}
```

## Service Layer
* Deal with `Application business logic`
* fetch communicate with `Database Layer` and `Web Layer`

=============================================
# Entity
* are objects in patterns used to carries data in software applications for **Database Layer**
* **Entity and Database table is in sync**

```cs
public class Person 
{
    public int PersonId {get; set;}
    public string FirstName {get; set;}
    public string LastName {get; set;}
    public DateTime DateOfBirth {get; set;}
}
```
## Database Layer
* deal with Database: fetch data 
