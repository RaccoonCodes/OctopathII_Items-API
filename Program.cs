using Microsoft.EntityFrameworkCore;
using OctopathII_Items.Data;
using OctopathII_Items.Models.Implementation;
using OctopathII_Items.Models.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DbConnection")
    )
);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IItemsService,ItemService>();
builder.Services.AddScoped<IEquipmentService,EquipmentService>();

var app = builder.Build();

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.Map("/", () => "Add \"/swagger/index.html\" to the end of the URL to access Swagger Testing ");
}

app.MapControllers();

app.Run();

/* Packages
 * Microsoft.EntityFrameworkCore 8.0.11
 * Microsoft.EntityFrameworkCore.Tools 8.0.11
 * Microsoft.EntityFrameworkCore.SqlServer 8.0.11
 * Swashbuckle.AspNetCore 7.2.0
 * CsvHelper 33.0.1
 * System.Linq.Dynamic.Core 1.6.0.2
 * 
 */