# OctopathII_Items
This is a two part project for one of my favorite JRPG games, Octopath II, items. The list will conatain Items and Equipment such as weapons from the game. This repository focuses on the API part of the project. The Front-End is yet to be determined in terms of either it will be in Razor views or in Angular.

# Table of Contents
1. [Quick Summary about Why I Chose Octopath](#Quick-Summary-about-Why-I-Chose-Octopath)
2. [Overview](#Overview)
3. [Packages and Framework](#Packages-and-Framework)
4. [ApplicationDB](#ApplicationDB)
5. [Migrations](#Migrations)
6. [Seed Controller](#Seed-Controller)
   - [ImportItems()](#ImportItems)
   - [ImportEquipment()](#ImportEquipment)
7. [PaginationHelper](#PaginationHelper)
8. [LinkDTO](#LinkDTO)
9. [Item Service](#Item-Service)
    - [GetItemsAsync](#GetItemsAsync)
    - [PutItemAsync](#PutItemAsync)
    - [GetInfoAsync](#GetInfoAsync)
10. [ItemController](#ItemController)
    - [GetInfo](#GetInfo)
    - [PutItem](#PutItem)
 11. [GetPaginatedDataAsync](#GetPaginatedDataAsync)
 12. [RequestDTO](#RequestDTO)
 13. [Attribute](#Attribute)
     - [SortOrderValidatorAttribute](#SortOrderValidatorAttribute)
 14. [Equipment Controller](#Equipment-Controller)
     - [GetEquipmentAsync](#GetEquipmentAsync)
 15. [EquipmentService](#EquipmentService)
 16. [Conclusion](#Conclusion)

## Quick Summary about Why I Chose Octopath
Octopath Traveler is a JRPG game that is a world exploration turn based game. It's art is `HD-2D` meaning that the character and scenery is pixelated but it is rendered in higher quality keeping both old and modern style combination. There are 3 games in its series : Octopath Traveler, Octopath Traveler: Champions of the Continent, and Octopath Traveler II. The reason why I pick on working this personal project is because I love this game. It is definetly one of my top 3 favorite games that I have played through out my life. From its story and music, it is amazing. So I decided to work on Full stack solo project about this game. As mentioned above, this will mainly focus on Item and Equipment of the game along with its meta data.  

## Overview
The Api Will retireve data information stored from the database. The database is populated by CSV files that are obtained by

`https://www.kaggle.com/datasets/mattop/octopath-traveler-ii-equipment`

`https://www.kaggle.com/datasets/mattop/octopath-traveler-ii-items`

These files last update was on 4/29/2024. There won't be future update on the list since these are set items and equipment in the game.

The following column and information will be used and focused on 

**Item**
- Name
- Price
- Description
- Item Type
- sell price
- buy price

**Equipment**
- everthing above
- hp,sp,effect,def stats

The database is hidden, so you will need to create your own database or use the local method for your editor. Once created, Implement the migrations, update database, and populate the values into database with the seed controller via Swagger.

## Packages and Framework
The following packages was used in this project:

`Microsoft.EntityFrameworkCore 8.0.11`

`Microsoft.EntityFrameworkCore.Tools 8.0.11`

`Microsoft.EntityFrameworkCore.SqlServer 8.0.11`

`Swashbuckle.AspNetCore 7.2.0`

`CsvHelper version 33.0.1`

The Version used in this project is `.NET 8` and its SDK version is `8.0.404`

## ApplicationDB

This class is the setup for creating database table for SQL. It contains two tables, `Items` and `Equipment`. 
```csharp
public DbSet<Item> Items => Set<Item>();
public DbSet<Equipment> Equipment => Set<Equipment>();
```

I have opted to use `IEntityTypeConfiguration` on two classes that will house their fluent API respectively: `ItemEntityTypeConfiguration` and `EquipmenmtEntityTypeConfiguration`

In `ItemEntityTypeConfiguration` it houses the properties and table needed to create for SQL database using `Item` class.
```csharp
public class ItemEntityTypeConfiguration : IEntityTypeConfiguration<Item>
{
    public void Configure(EntityTypeBuilder<Item> builder)
    {
        builder.ToTable("Items");
        builder.HasKey(x => x.Name);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(255);
        builder.Property(e => e.Description).HasMaxLength(500);
        builder.HasIndex(e => e.Item_Type);
        builder.HasIndex(e => e.Sell_Price);
    }
}
```

In `EquipmenmtEntityTypeConfiguration`, it follows the same procedure as `Item` just except of using `Item` class, it uses `Equipment` class.
```csharp
public class EquipmentEntityTypeConfiguration : IEntityTypeConfiguration<Equipment>
{
    public void Configure(EntityTypeBuilder<Equipment> builder)
    {
        builder.HasKey(e => e.Name);
        builder.Property(e => e.Name).IsRequired().HasMaxLength(255);
        builder.HasIndex(e => e.Equipment_Type);
        builder.HasIndex(e => e.Sell_Price);
        builder.HasIndex(e => e.Physical_Atk);
        builder.HasIndex(e => e.Elemental_Atk);
    }
}
```
Once created, we need to register them in ApplicationDBContext, in which I did by adding the following method:

```csharp
 protected override void OnModelCreating(ModelBuilder modelBuilder)
 {
     base.OnModelCreating(modelBuilder);
     modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
 }
```

Since I used `IEntityTypeConfiguration<T>` in the previous two classes. The `ApplyConfigurationsFromAssembly<T>` will automatically implement them from the same assembly as `ApplicationDbContext`. This way, it keeps the `ApplicationDbContext` clean.

## Migrations
Once the `ApplicationDbContext` is complete, we need to run the migrations so that the tables get created so I ran the following command 

`dotnet ef migrations add "Initial" -o "Data/Migrations"`

I used the flag `-o` to add migrations folder in a specific directory: `"Data/Migrations"`

After a successful build, run the following command to apply the tables into the database

`dotnet ef database update`

## Seed Controller
This controller populates the database with data from CSV files that are stored in `Data/Source`. Here are the two important methods in the `SeedController`

### ImportItems()

This method populates the Database with Items from the `octopath_items.csv` file.
```csharp
var config = new CsvConfiguration(CultureInfo.InvariantCulture)
{
    HasHeaderRecord = true,
    Delimiter = ","
};
using var reader = new StreamReader(
    System.IO.Path.Combine(_webHostEnvironment.ContentRootPath, "Data/Source/octopath_items.csv"));

using var csv = new CsvReader(reader, config);
```
This section sets up the configuartion on how the file is going to be read and letting the reader know where the file is located.

```csharp
 var existingItems = new HashSet<string>( await _context.Items.AsNoTracking().Select(x => x.Name)
     .ToListAsync(),StringComparer.OrdinalIgnoreCase);

  var skippedRows = 0;
  var newItems = new List<Item>();
  var localItems = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
```
This keep track of existing items in the database to prevent duplicates based on names and keeping track of local items being added to the database. I used `AsNoTracking()` since I am only retrieving data and I am not making any changes to the file. The `skippedRows` variable skips any row where the item is existing in the database.

```csharp
foreach (var record in csv.GetRecords<ItemRecord>())
{
    if (string.IsNullOrEmpty(record.Name) || existingItems.Contains(record.Name) 
        || localItems.Contains(record.Name))
    {
        skippedRows++;
        continue;
    }
    var item = new Item()
    {
        Name = record.Name,
        Description = record.Description,
        Buy_Price = record.Buy_Price,
        Sell_Price = record.Sell_Price,
        Item_Type = record.Item_Type
    };

    newItems.Add(item);
    localItems.Add(record.Name);
}
```
In this foreach loop, the data is loaded on call to reduce memory load and when there is no item in the database with the name that is selected, then it gets added along with the info in the `item` object. 

```csharp
if (newItems.Count != 0)
{
    using var transaction = _context.Database.BeginTransaction();
    await _context.Items.AddRangeAsync(newItems);
    await _context.SaveChangesAsync();
    await transaction.CommitAsync();

    return "Data successfully populated for Items";
}
```
Then it gets saved and imported into the database and return a string where, if successful, it response with a successful message.

Overall the Time Complexity of this Method is `O(N + M)` where N is the number of existing records in the database and where M is the number of records in the CSV file. Since I am using HashSet, the lookups time complexity is `O(1)`, meaning constant.

### ImportEquipment()
This method follows the same process as ImportItem, the only difference is the data type that it will be storing on and the CSV file that is being retrieve. 
```csharp
foreach (var record in csv.GetRecords<EquipmentRecord>())
{
    if (string.IsNullOrEmpty(record.Name) || existingItems.Contains(record.Name)
        || localEquip.Contains(record.Name))
    {
        skippedRows++;
        continue;
    }

    var equip = new Equipment()
    {
        Name = record.Name,
        Max_Hp = record.Max_Hp,
        Max_SP = record.Max_SP,
        Physical_Atk = record.Physical_Atk,
        Elemental_Atk = record.Elemental_Atk,
        Physical_Def = record.Physical_Def,
        Elemental_Def = record.Elemental_Def,
        Accuracy = record.Accuracy,
        Speed = record.Speed,
        Critical = record.Critical,
        Evasion = record.Evasion,
        Effect = record.Effect,
        Buy_Price = record.Buy_Price,
        Sell_Price = record.Sell_Price,
        Equipment_Type = record.Equipment_Type
    };

    newEquip.Add(equip);
    localEquip.Add(record.Name);
}
```
The data type that is used in the foreach loop is `Equipment` and, just like the previous method, it gets added to the database.

The time complexity for this method is the same as the previous method `O(N + M)` where N is the number of existing records in the database and where M is the number of records in the CSV file.

## PaginationHelper
The `GeneratePaginationLinks` method is a static function class that generates pagination links for API responses. It constructs a SELF, NEXT, and PREVIOUS link to navigate between paginated results.

```csharp
public static List<LinkDTO> GeneratePaginationLinks(string baseUrl, string rel, string action, int pageIndex,
    int pageSize, int totalPages, Dictionary<string, string>? additionalParams = null)
{
    var links = new List<LinkDTO>();

    string BuildUrl(int index)
    {
        var queryParams = new Dictionary<string, string>()
        {
            { "pageIndex", index.ToString() },
            { "pageSize", pageSize.ToString() }
        };
        if (additionalParams != null)
        {
            foreach (var param in additionalParams)
            {
                queryParams[param.Key] = param.Value;
            }
        }
        var queryString = string.Join("&", queryParams
            .Select(kvp => $"{HttpUtility.UrlEncode(kvp.Key)}={HttpUtility.UrlEncode(kvp.Value)}"));
        return $"{baseUrl}?{queryString}";
    }

    // Self link
    links.Add(new LinkDTO(BuildUrl(pageIndex), "self", action));

    // Next link (if not on the last page)
    if (pageIndex + 1 < totalPages)
    {
        links.Add(new LinkDTO(BuildUrl(pageIndex + 1), "next", action));
    }

    // Previous link (if not on the first page)
    if (pageIndex > 0)
    {
        links.Add(new LinkDTO(BuildUrl(pageIndex - 1), "previous", action));
    }

    return links;
}
```

In the parameters, it includes base Url, relationship, action, Index, page size, total pages, and a dictionary with any additional parameters that has been instantiated with null if not declared when called. The building URL process is in `BuildUrl`.

## LinkDTO 
This DTO class represents HATEOAS style links. It is the basis of constructing the link used in the `GeneratePaginationLinks`.
```charp
public class LinkDTO
{
    public string Href { get; private set; } //URLs
    public string Rel { get; private set; } //Relationship
    public string Type { get; private set; } //Type being send

    public LinkDTO(string href, string rel, string type)
    => (Href, Rel, Type) = (href, rel, type);
}
```
setter are private which enforces the user to call the constructor in order to set the object with an LinkDTO.

## Item Service
This Implementation class host the business logic that will be used in `ItemController`.

### GetItemsAsync
This method and the method for `Equipment` in `EquipmentService` share the same procedure. So instead of rewriting the same code but with different object types, I created a generic static class that allows both of these method to use it. It is called  `GetPaginatedDataAsync` in `Extension` folder. It will be mention below this section. The time Complexity for this method is mentioned in `GetPaginatedDataAsync`

### PutItemAsync
This method updates information already existing in the database. Its parameters takes object `Item`
```csharp
Item? updatedItem = await _context.Items.FirstOrDefaultAsync(x => x.Name == item.Name); 

if(updatedItem == null)
{
    return null;
}

updatedItem.Description = item.Description;
updatedItem.Buy_Price = item.Buy_Price;
updatedItem.Sell_Price = item.Sell_Price;
updatedItem.Item_Type = item.Item_Type;

```

It first, instantiate an object `Item` where it will hold the existing Name item in the database. If not found, then it set as null where it will be catched in the `if` statement. Otherwise, new updated data will bet set in the `updatedItem` variable.

```csharp
 try
 {
     await _context.SaveChangesAsync();
 }
 catch (Exception ex)
 {
     throw new Exception("An error occurred while updating the item.", ex);
 }

 return updatedItem;
```
It then tries to save updated info into the database and catches any exception cause by the save. Typically, there would be a concurrency catch but in this case, I will omit this and keep it all into one catch statement.

The Overall Time Complexity of this method is `O(lg n)`. This is due to the property `Name` being indexed. Otherwsise, without indexing, this would deteriorate the time complexity to `O(n)`. Hence why Indexing is important here.

### GetInfoAsync
Its parameters takes in a string value that holds the name of the item. Its purpose is to return information about the item based on the name. It will either return an Object `Item` that hold info about the item or return null where the `Item` Controller will handle the null return

```csharp
 return await _context.Items.AsNoTracking().Where(x => x.Name == name).Select(s => new Item
 {
     Name = s.Name,
     Description = s.Description,
     Buy_Price = s.Buy_Price,
     Sell_Price = s.Sell_Price,
     Item_Type = s.Item_Type

 }).FirstOrDefaultAsync();
```

The time complexity of this method is `O(lg n)`, again for the same reason in `PutItemAsync` the indexing plays a role for improving performance. As you can also see, I used `AsNoTracking` in this method as I don't need to do changes in the data. Improving performance further by reducing memory needed to complete the operation.

## ItemController
This controller host api interaction to the database it comes with READ and EDIT Operations since the CSV file that we used has all the data needed and it is a set number items in the Octopath Traveler II game. This controller focuses on the Items in the game. 

```csharp
private readonly IItemsService _itemsService;
public ItemsController(IItemsService itemsService)
    => _itemsService = itemsService;

```
The controller depends on `IItemService` and uses Dependency injection to register it in the controller. 

```csharp
public async Task<ActionResult<RestDTO<Item[]>>> GetItems([FromQuery] RequestDTO<Item> requestDTO)
{
    RestDTO<Item[]> results = await _itemsService.GetItemsAsync(requestDTO, Url.Action(
        null, "Items", null, Request.Scheme)!, "Self", "GET");

    if (!results.Data.Any())
    {
        return results.Message != null
            ? Ok(results.Message)
            : BadRequest("Invalid pagination parameters. Ensure 'pageIndex' >= 0 and 'pageSize' > 0.");
    }
    return Ok(results);
}

```
This method retrieves the data from the database from Item Table. As mention before, I've separated the business logic and HTTP response to maintain Separation of Concern Design Pattern. If everything is successful, it will return a JSON  response containing the data, links, messages, and other meta data such as filter, count, records,etc.

### PutItem
```csharp
 public async Task <ActionResult<Item>> PutItem([FromBody]Item item)
 {
     Item? result = await _itemsService.PutItemAsync(item);

     if(result == null)
     {
         return NotFound("Error: Name of the item is not found!");
     }
     return Ok(result);
 }
```
This method updates existing information in the database within the table `Item`. It maps the options in the object `Item`. All the business work is in the `ItemService` class. This is method will handle its HTTP response. 

### GetInfo
```csharp
public async Task<ActionResult<Item>> GetInfo(string name)
{
    Item? result = await _itemsService.GetInfoAsync(name);

    if (result == null)
    {
        return NotFound(new { Message = "Item not found!" });
    }
    return Ok(result);

}
```
This method retrieves information based on name provided. Same as previous method, this method focuses on HTTP response, the business work is done by ItemService 

## GetPaginatedDataAsync
This generic static class is used for the GET method mentioned in both Item and Equipment Services. It retrives paginated, filtered, and sorted information from the database based on the data type used for this method. It also includes Links used for pagination. 
```csharp
 public static async Task<RestDTO<T[]>> GetPaginatedDataAsync<T>(
     RequestDTO<T> requestDTO, IQueryable<T> query,
     string base_url, string rel, string action) where T : class
 {
     if (requestDTO.PageIndex < 0 || requestDTO.PageSize <= 0)
     {
         return new RestDTO<T[]>()
         {
             Data = []
         };
     }

     // Apply filtering
     if (!string.IsNullOrEmpty(requestDTO.FilterQuery))
     {
         if (!string.IsNullOrEmpty(requestDTO.SortColumn))
         {
             query = query.Where($"{requestDTO.SortColumn}.Contains(@0)", requestDTO.FilterQuery);
         }
         else if (typeof(T).GetProperty("Name") != null)
         {
             query = query.Where("Name.Contains(@0)", requestDTO.FilterQuery);
         }
     }
```
First, it checks the index and size that the user has input and validate. Afterwords the Filtering and a fallback is used when filtering and sorting is used. 
```csharp
var recordCount = await query.CountAsync();

if (recordCount == 0)
{
    return new RestDTO<T[]>
    {
        Data = Array.Empty<T>(),
        PageIndex = requestDTO.PageIndex,
        PageSize = requestDTO.PageSize,
        RecordCount = recordCount,
        Message = "No Records found for this input",
        Links = new List<LinkDTO>()
    };
}

var totalPages = (int)Math.Ceiling(recordCount / (double)requestDTO.PageSize);

T[]? result = await query.OrderBy($"{requestDTO.SortColumn} {requestDTO.SortOrder}")
                .Skip(requestDTO.PageIndex * requestDTO.PageSize)
                .Take(requestDTO.PageSize)
                .ToArrayAsync();

var links = PaginationHelper.GeneratePaginationLinks(base_url, rel, action,
    requestDTO.PageIndex, requestDTO.PageSize, totalPages,
    new Dictionary<string, string> {
        { "SortColumn", requestDTO.SortColumn ?? string.Empty },
        { "SortOrder",  requestDTO.SortOrder ?? string.Empty },
        { "FilterQuery", requestDTO.FilterQuery ?? string.Empty }
    }
);
```
Afterword, it counts the current container in `query` to count the records. If it is 0, then it return an empty `Rest<T>` type. Pagination is applied and since this method is a generic type, The array is of type `T` when query is called and set as an array. The links are generated via `GeneratePaginationLinks` helper which was already mentioned before. 
```csharp
 return new RestDTO<T[]>
 {
     Data = result,
     PageIndex = requestDTO.PageIndex,
     PageSize = requestDTO.PageSize,
     RecordCount = recordCount,
     TotalPages = totalPages,
     Message = "Successful retrieval",
     Links = links
 };
```
Finally, it return a `RestDTO<T>` object that can depend which data type is being used.

The Time Complexity of the method is `O(log n + k + p)` as indexes are applied appropriately which can be shown in my Fluent API classes. Also `log n` comes from index filtering operation, `k` is representing skip operation during pagination and `p` where it represents take operation where we return the number of records in the result set. The Dominant term is `log n` however if `k` or `p` become large they can definitely play a role in terms of performance. In our case though, since the database is relatively small, the overall Time Complexity is `O(log n)`

## RequestDTO
This class contains properties and values used for API querries. It is a generic Data Transfer Object, or DTO, used to paginate and filter request made to the database. 

```csharp
public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
{
    if (SortColumn != null)
    {
        var properties = typeof(T).GetProperties()
            .Select(p => p.Name)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        if (!properties.Contains(SortColumn))
        {
            yield return new ValidationResult(
                $"SortColumn '{SortColumn}' is not a valid property of {typeof(T).Name}.",
                new[] { nameof(SortColumn) });
        }
    }

}
```

It uses Validation Attribute that I will get into below and a manual validation method for sorting column to run against sort column properties. The Column will be based on the object type used with the `RequestDTO<T>`

## Attribute
This folder holds validation used for the RequestDTO class needed to ensure valid data is used

### SortOrderValidatorAttribute
`SortOrderValidatorAttribute` is a custom validation class used for validating sorting order values. It ensures that the value provided is either "ASC" (ascending) or "DESC" (descending).

```csharp
public string[] AllowedValues { get; set; } = new[] { "ASC", "DESC" };

public SortOrderValidatorAttribute() : base("Values must be one of the following {0}") { }

protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
{
    var strValue = value as string;
    if (!string.IsNullOrEmpty(strValue) && AllowedValues.Contains(strValue))
    {
        return ValidationResult.Success;
    }
    return new ValidationResult(FormatErrorMessage(string.Join(", ", AllowedValues)));
}
```
The class Extends `ValidationAttribute` to enable model property validation. The `AllowedValues` restricts validation option to two `ASC` and `DESC`. The base contructor sets an error message with a `{0}` placeholder and it gets replaced when an error occurs.

In the `IsValid()` method, it checks the input if it is non-empty and it is of the allowed values. It returns a `ValidationResult.Success` if valid. Otherwise return an error message.

## Equipment Controller
This controller houses manages equipment request to the database. Item and Equipment are separated since both have different properties in both database column and model.

### GetEquipmentAsync
Retrieves data from the database and returns an array of Equipment object. 
```csharp
public async Task<ActionResult<RestDTO<Equipment[]>>> GetEquipment([FromQuery]RequestDTO<Equipment> requestDTO)
{
    RestDTO<Equipment[]> results = await _equipmentService.GetEquipmentAsync(requestDTO, Url.Action(
        null, "Items", null, Request.Scheme)!, "Self", "GET");

    if (!results.Data.Any())
    {
        return results.Message != null
            ? Ok(results.Message)
            : BadRequest("Invalid pagination parameters. Ensure 'pageIndex' >= 0 and 'pageSize' > 0.");
    }

    return Ok(results);
}
```
Again, as mentioned before, this returns only HTTP Response. The business logic is in `EquipmentService`.

## EquipmentService
All the methods within this class follows similar to `ItemService` just the only differnce is that the return type is `Equipment` instead of `Item`. The Time Complexity is also similar to or the same as `ItemService`. The `EquipmentService` inherits `IEquipmentService`.
```csharp
public interface IEquipmentService
{
    Task<RestDTO<Equipment[]>> GetEquipmentAsync(RequestDTO<Equipment> restDTO, string base_url, string rel, string action);
 Task<Equipment?> PutEquipmentAsync(Equipment equipment);
 Task<Equipment?> GetInfoEquipment(string name);

}
```

## Conclusion
This backend project is the API part for retrieving data from the database. The front end framework will be determine later on, though most likely it will be in Angular since I, personally, want to get into working in Typescript as well. Any Updates or edit to this project will be posted at the top of this README Section!

Have a Nice Day!

