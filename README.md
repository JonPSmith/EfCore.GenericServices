[
![buymeacoffee](https://www.buymeacoffee.com/assets/img/custom_images/orange_img.png)
](https://www.buymeacoffee.com/l709oYtzp)

**Now supports JSON Patch updates.**

# EfCore.GenericServices

This library helps you quickly code Create, Read, Update and Delete (CRUD) accesses for a web/mobile/desktop application. 
It acts as a adapter and command pattern between a database accessed by Entity Framework Core (EF Core) and the needs of the front-end system. 

This library takes advantage of the fact that each of the four CRUD database accesses differ in what they do, but they have a common set of data part they all use, which are: 

a) What database class/table do you want to access?
b) What properties in that class/table do you want to access or change?

This library uses DTOs (data transfer objects, also known as ViewModels) plus a special interface to define the class/table and the properties to access.
That allows the library to implement a generic solution for each of the four CRUD accesses, where the only thing that changes is the DTO you use.

Typical web applications have hundreds of CRUD pages - display this, edit that, delete the other -
and each CRUD access has to adapt the data in the database to show the user, and then apply the changes to the database. 
So, you create one set of update code for your specific application and then cut/paste + change one line (the DTO name) for all the other versions. 

I personally work with ASP.NET Core, so my examples are from that, but it will work with any NET Core type of application
*(I do know one person have used this libary with WPF).*

[NuGet link](https://www.nuget.org/packages/EfCore.GenericServices/) and [link to documentation](https://github.com/JonPSmith/EfCore.GenericServices/wiki).

MIT license.

## Code examples of using EfCore.GenericServices

I personally work with ASP.NET Core, so my examples are all around ASP.NET Core, but EfCore.GenericServices will work with any NET Core type of application
*(I do know one person have used this libary with WPF).*

### ASP.NET Core MVC - razor pages

The classic way to produce HTML pages in ASP.NET is using the MVC approach, with razor pages.
Here a simple example to show you the basic way to inject and then call the `ICrudServices`, in this case a simple List.

```csharp
public class BookController

    private ICrudServices _service;

    public BookController(ICrudServices service)
    {
        _service = service;
    }

    public ActionResult Index()
    {
        var dataToDisplay = _service.ReadManyNoTracked<BookListDto>().ToList()
        return View(dataToDisplay);
    }
    //... etc.
```

### ASP.NET Core MVC - razor pages

Here is the code from the example Razor Page application contained in this repo for adding a review to a Book (the example site is a tiny Amazon-like site).
This example shows an more complex example where I am updating the Book class that uses a Domain-Driven Design (DDD) approach 
to add a new review to a book. The code shown is the complete code in the [AddReview.cshtml.cs](https://github.com/JonPSmith/EfCore.GenericServices/blob/master/RazorPageApp/Pages/Home/AddReview.cshtml.cs) class.

```csharp
public class AddReviewModel : PageModel
{
    private readonly ICrudServices _service;

    public AddReviewModel(ICrudServices service)
    {
        _service = service;
    }

    [BindProperty]
    public AddReviewDto Data { get; set; }

    public void OnGet(int id)
    {
        Data = _service.ReadSingle<AddReviewDto>(id);
        if (!_service.IsValid)
        {
            _service.CopyErrorsToModelState(ModelState, Data, nameof(Data));
        }
    }

    public IActionResult OnPost()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }
        _service.UpdateAndSave(Data);
        if (_service.IsValid)
            return RedirectToPage("BookUpdated", new { message = _service.Message});

        //Error state
        _service.CopyErrorsToModelState(ModelState, Data, nameof(Data));
        return Page();
    }
}
```

NOTE: If you compare the code above with the 
[AddPromotion](https://github.com/JonPSmith/EfCore.GenericServices/blob/master/RazorPageApp/Pages/Home/AddPromotion.cshtml.cs) or
[ChangePubDate](https://github.com/JonPSmith/EfCore.GenericServices/blob/master/RazorPageApp/Pages/Home/ChangePubDate.cshtml.cs)
updates in the same example then you will see they are identical apart from the type of the `Data` property.

And the ViewModel/DTO isn't anything special (see the 
[AddReviewDto](https://github.com/JonPSmith/EfCore.GenericServices/blob/master/ServiceLayer/HomeController/Dtos/AddReviewDto.cs)). 
They just need to be marked with an empty 
`ILinkToEntity<TEntity>` interface, which tells GenericServices which EF Core entity 
class to map to. *For more security you can also mark any read-only properties with the 
`[ReadOnly(true)]` attribute - GenericServices will never try to update the 
database with any read-only marked property.*

## ASP.NET Web API

When using ASP.NET Web API then another companion library called [EfCore.GenericServices.AspNetCore](https://github.com/JonPSmith/EfCore.GenericServices.AspNetCore)
provides extension methods to help return the data in the correct form (plus other methods to allow unit testing of Web API actions using EfCore.GenericServices).

The code below comes from the example [ToDoController](https://github.com/JonPSmith/EfCore.GenericServices.AspNetCore/blob/master/ExampleWebApi/Controllers/ToDoController.cs)
example in the EfCore.GenericServices.AspNetCore GitHub repo.

```csharp
public class ToDoController : ControllerBase
{

    [HttpGet]
    public async Task<ActionResult<WebApiMessageAndResult<List<TodoItem>>>> GetManyAsync([FromServices]ICrudServices service)
    {
        return service.Response(await service.ReadManyNoTracked<TodoItem>().ToListAsync());
    }

    [Route("name")]
    [HttpPatch()]
    public ActionResult<WebApiMessageOnly> Name(ChangeNameDto dto, [FromServices]ICrudServices service)
    {
        service.UpdateAndSave(dto);
        return service.Response();

    //... other action methods removed 
}
```


## Technical features
The EfCore.GenericServices ([NuGet, EfCore.GenericServices](https://www.nuget.org/packages/EfCore.GenericServices/)), 
is an open-source (MIT licence) netcoreapp2.0 library that assumes you use EF Core for your database accesses. 
It has good documentation in the [repo's Wiki](https://github.com/JonPSmith/EfCore.GenericServices/wiki).

It is designed to work with both standard-styled
entity classes (e.g. public setters on the properties and a public, paremeterless constructor),
or with a Domain-Driven Design (DDD) styled entity classes (e.g. where all updates are done through named 
methods in the the entity class) - see 
[this article](https://www.thereformedprogrammer.net/creating-domain-driven-design-entity-classes-with-entity-framework-core/)
for more on the difference between standard-styled entity classes and DDD styled entity classes.

It also works well with with dependancy injection (DI), such as ASP.NET Core's DI service.
But does also contain a simplified, non-DI based configuration system suitable for unit testing 
and/or serverless applications.

*NOTE: I created a [similar library](https://github.com/JonPSmith/GenericServices)
for EF6.x back in 2014, which has saved my many months of (boring) coding -
on one project alone I think it saved 2 months out of 12.
This new version contains the learning from that library, and the new DDD-enabling feature of EF Core
to reimagine that library, but in a very different (hopefully simpler) way.*

## Library performance

I have compared the performance of the GenericService library using the excellent
[BenchmarkDotNet](https://github.com/dotnet/BenchmarkDotNet) library, as part of the GitHub repo.
The performance loss was an extra 25 us. - worst case, 6% of total time, and that is for the 
simplest update on the fastest in-memory database.
See the [Performance figures](https://github.com/JonPSmith/EfCore.GenericServices/wiki/Performance-figures)
wiki page for full details.

## Documentation and examples
* The [GenericServices Wiki](https://github.com/JonPSmith/EfCore.GenericServices/wiki) has
lots of documentation.
* The public methods in the library are all commented for intellisense feedback.
* This repo contains a runnable example Razor Page application, with the database in the DataLayer.
* This [intoductory article](https://www.thereformedprogrammer.net/genericservices-a-library-to-provide-crud-front-end-services-from-a-ef-core-database/)
provides a longer introduction to EfCore.GenericServices.