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



