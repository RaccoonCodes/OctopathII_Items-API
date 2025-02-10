var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();

/* Packages
 * Microsoft.EntityFrameworkCore version 8.0.11
 * Microsoft.EntityFrameworkCore.Tools version 8.0.11
 * Microsoft.EntityFrameworkCore.SqlServer version 8.0.11
 * Swashbuckle.AspNetCore --version 7.2.0
 * 
 */