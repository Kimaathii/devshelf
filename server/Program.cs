using Microsoft.EntityFrameworkCore;
using server.Data;
using server.Services;



var builder = WebApplication.CreateBuilder(args);



//Add Ef core plus SQLite
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=devshelf.db"));

builder.Services.AddScoped<JwtService>();
builder.Services.AddScoped<AuthService>();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}


app.UseHttpsRedirection();


app.Run();

