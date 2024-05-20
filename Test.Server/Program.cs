using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Reflection;
using Test.Server;
using System.Linq;


var builder = WebApplication.CreateBuilder(args);

string connection = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationContext>(options => options.UseNpgsql(connection));
builder.Services.AddControllers();
#region Добавление Swagger
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "WEB API Persons",
        Version = "v1",
        Description = "API для работы с бэкэндом серевера"
    });

});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
        {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}
#endregion

#region Запросы
app.MapGet("api/persons", async (ApplicationContext d) => await d.Persons.ToListAsync());

app.MapGet("api/persons/{id}", async (int id, ApplicationContext d) =>
{
    Person? user = await d.Persons.FirstOrDefaultAsync(x => x.Id == id);
    if (user == null) return Results.NotFound(new {message = "Пользователь не найден"});

    return Results.Json(user);
});

app.MapGet("api/persons/name/{name}", async (string name, ApplicationContext d) =>
{
    //Person? user = await d.Persons.FirstOrDefaultAsync(x => x.Name == name);
    IQueryable<Person?> user = d.Persons.Where(x => x.Name == name);
    if (user == null) return Results.NotFound(new { message = "Пользователь не найден" });

    return Results.Json(user);
});

app.MapGet("api/persons/age/{age}", async (int age, ApplicationContext d) =>
{
    //Person? user = await d.Persons.FirstOrDefaultAsync(x => x.Name == name);
    IQueryable<Person?> user = d.Persons.Where(x => x.Age >= age);
    if (user == null) return Results.NotFound(new { message = "Пользователь не найден" });

    return Results.Json(user);
});

app.MapDelete("api/persons/{id}", async (int id, ApplicationContext d) =>
{
    Person? user = await d.Persons.FirstOrDefaultAsync(x => x.Id == id);
    if (user == null) return Results.NotFound(new { message = "Пользователь не найден" });

    d.Persons.Remove(user);
    await d.SaveChangesAsync();
    return Results.Json(user);
});

app.MapPost("api/persons", async (Person user, ApplicationContext d) =>
{
    //user.Id = id++;
    await d.Persons.AddAsync(user);
    await d.SaveChangesAsync();
    return user;
});

app.MapPut("api/persons", async (Person userData, ApplicationContext d) =>
{
    Person? user = await d.Persons.FirstOrDefaultAsync(x => x.Id == userData.Id);
    if (user == null) return Results.NotFound(new { message = "Пользователь не найден" });

    user.Age = userData.Age;
    user.Name = userData.Name;
    await d.SaveChangesAsync();
    return Results.Json(user);
});
#endregion

app.Run("http://10.10.69.56:5003");

