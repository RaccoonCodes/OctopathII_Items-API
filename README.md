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
