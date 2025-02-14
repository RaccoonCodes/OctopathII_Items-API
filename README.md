# OctopathII_Items
This is a two part project for one of my favorite JRPG games, Octopath II, items. The list will conatain Items and Equipment such as weapons from the game. This repository focuses on the API part of the project. The Front-End is yet to be determined in terms of either it will be in Razor views or in Angular.

## Overview
The Api Will retireve data information stored from the database. The database is populated by CSV files that are obtained by

`https://www.kaggle.com/datasets/mattop/octopath-traveler-ii-equipment`

`https://www.kaggle.com/datasets/mattop/octopath-traveler-ii-items`

These files last update was on 4/29/2024. There won't be future update on the list since these are set items and equipment in the game.

The following column and information will be used and focused on 
- Name
- Price
- Description
- Item Type
- sell price

The database is hidden, so you will need to create your own database or use the local method for your editor. Once created, Implement the migrations, update database, and populate the values into database with the seed controller via Swagger.

## Packages
The following packages was used in this project:

`Microsoft.EntityFrameworkCore 8.0.11`

`Microsoft.EntityFrameworkCore.Tools 8.0.11`

`Microsoft.EntityFrameworkCore.SqlServer 8.0.11`

`Swashbuckle.AspNetCore 7.2.0`

`CsvHelper version 33.0.1`

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
This method retrieves data from the database. 
```csharp
 if(restDTO.PageIndex < 0 || restDTO.PageSize <= 0)
 {
     return new RestDTO<Item[]>()
     {
         Data = Array.Empty<Item>()
     };
 }
 var query = _context.Items.AsNoTracking().AsQueryable();
 
 if (!string.IsNullOrEmpty(restDTO.FilterQuery))
 {
     if (!string.IsNullOrEmpty(restDTO.SortColumn))
     {
         query = query.Where($"{restDTO.SortColumn}.Contains(@0)", restDTO.FilterQuery);
     }
     else
     {
         query = query.Where(q => q.Name.Contains(restDTO.FilterQuery));
     }
 }
```

Since we are only reading data, `AsNoTracking` is used to reduce memory consumption.  Afterwords, if filtering is applied then it will check if there is sorting. If there is, then it will apply the appropiate LINQ to the queryable before the execution. Otherwise, only add the filter to `Name` as a fallback. 

```csharp
 var recordCount = await query.CountAsync();

 if(recordCount == 0)
 {
     return new RestDTO<Item[]>
     {
         Data = Array.Empty<Item>(),
         PageIndex = restDTO.PageIndex,
         PageSize = restDTO.PageSize,
         RecordCount = recordCount,
         Message = "No Records found for this input",
         Links = new List<LinkDTO>()
     };
 }

 var totalPages = (int)Math.Ceiling(recordCount / (double)restDTO.PageSize);

 Item[]? result = await query.OrderBy($"{restDTO.SortColumn} {restDTO.SortOrder}")
                 .Skip(restDTO.PageIndex * restDTO.PageSize)
                 .Take(restDTO.PageSize)
                 .ToArrayAsync();

 var links = PaginationHelper.GeneratePaginationLinks(base_url, rel, action,
    restDTO.PageIndex, restDTO.PageSize, totalPages, 
    new Dictionary<string, string> {
        { "SortColumn", restDTO.SortColumn ?? string.Empty },
        { "SortOrder", restDTO.SortOrder ?? string.Empty },
        { "FilterQuery", restDTO.FilterQuery ?? string.Empty }
           }
 );

 return new RestDTO<Item[]>
 {
     Data = result,
     PageIndex = restDTO.PageIndex,
     PageSize = restDTO.PageSize,
     RecordCount = recordCount,
     TotalPages = totalPages,
     Message = "Successful retrieval",
     Links = links
 };
```

After the record is counted from the `query` variable. If it is zero then it returns an empty data with appropiate information, otherwise, it will continue on the pagination and result of the query built and executed the list as an array. The return is an array of `RestDTO<Item[]>`

The Time Complexity of the method is `O(log n + k + p)` as indexes are applied appropriately which can be shown in my Fluent API classes. Also `log n` comes from index filtering operation, `k` is representing skip operation during pagination and `p` where it represents take operation where we return the number of records in the result set. The Dominant term is `log n` however if `k` or `p` become large they can definitely play a role in terms of performance. In our case though, since the database is relatively small, the overall Time Complexity is `O(log n)`


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



