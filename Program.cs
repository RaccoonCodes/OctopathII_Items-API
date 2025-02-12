using Microsoft.EntityFrameworkCore;
using OctopathII_Items.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DbConnection")
    )
);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.Map("/", () => "Add \"/swagger/index.html\" to the end of the URL to access Swagger Testing ");
}

app.MapControllers();

app.Run();

/* Packages
 * Microsoft.EntityFrameworkCore version 8.0.11
 * Microsoft.EntityFrameworkCore.Tools version 8.0.11
 * Microsoft.EntityFrameworkCore.SqlServer version 8.0.11
 * Swashbuckle.AspNetCore --version 7.2.0
 * CsvHelper version 33.0.1
 * 
 * 
 * To do:
 * Start Seed Controller and populate
 * Start on Retrieval process such as DTO, Model, and controller.
 * 
 */