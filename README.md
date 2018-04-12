# EfCore.GenericServices

This library helps you quickly code Create, Read, Update and Delete (CRUD) accesses
for a web/mobile/desktop application. It acts as a adapter between a database accessed
by Entity Framework Core (EF Core) and the needs of the front-end system.

Typical web applications have hundreds of CRUD pages - display this, edit that, delete the other. 
And each CRUD access has to adapt the data in the database to show the user, and then
apply the changes to the database. This library extracts 
the pattern into a library, making the writing of the CRUD front-end code both quicker and
simpler to write.

The code for each of the four access type (Create, Read, Update and Delete) are identical,
apart from the ViewModel/DTO specific to the actual feature. So, you create one set of update
code for your specific applictation and then cut/paste + change one line for all the other 
versions. Here is the code from the example Razor Page application contained in this repo
for adding a review to a Book (the example site is a tiny Amazon-like site).

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
If you compare that with the 
[AddPromotion](https://github.com/JonPSmith/EfCore.GenericServices/blob/master/RazorPageApp/Pages/Home/AddPromotion.cshtml.cs) or
[ChangePubDate](https://github.com/JonPSmith/EfCore.GenericServices/blob/master/RazorPageApp/Pages/Home/ChangePubDate.cshtml.cs)
update you will see they are identical apart from the type of the `Data` property.

And the ViewModel/DTO isn't anything special (see the 
[AddReviewDto](https://github.com/JonPSmith/EfCore.GenericServices/blob/master/ServiceLayer/HomeController/Dtos/AddReviewDto.cs)). 
They just need to be marked with an empty 
`ILinkToEntity<TEntity>` interface, which tells GenericServices which EF Core entity 
class to map to. *For more security you can also mark any read-only properties with the 
`[ReadOnly(true)]` attribute - GenericServices will never try to update the 
database with any read-only marked property.*

## Technical features
The EfCore.GenericServices is an open-source (MIT licence) netcoreapp2.0 library that assumes
you use EF Core for your database accesses. 

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
on one project alone I think it saved 2 months out of 10.
This new version contains the learning from that library, and the new DDD-enabling feature of EF Core
to reimagine that library, but in a very different (hopefully simpler) way.*

## Library performance

I have compared the performance of the GenericService library using the excellent
[BenchmarkDotNet](https://github.com/dotnet/BenchmarkDotNet) library, as part of the GitHub repo.
The worst performance loss was 5% (25 us.), and that is for the simplest update on the fasted in-memory database.
See the [Performance figures](https://github.com/JonPSmith/EfCore.GenericServices/wiki/Performance-figures)
wiki page for full details.

## Documentation and examples
* The [GenericServices Wiki](https://github.com/JonPSmith/EfCore.GenericServices/wiki) has
lots of documentation.
* The public methods in the libraray are all commented for intellisense feedback.
* This repo contains a runnable example Razor Page application, with the database in the DataLayer.
* This general article provides a longer introduction ???? link to come.