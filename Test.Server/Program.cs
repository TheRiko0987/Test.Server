using Microsoft.EntityFrameworkCore;
using System;
using Test.Server;

//int id = 1;

var builder = WebApplication.CreateBuilder(args);

string connection = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationContext>(options => options.UseNpgsql(connection));

var app = builder.Build();

app.MapGet("api/persons", async (ApplicationContext d) => await d.Persons.ToListAsync());

app.MapGet("api/persons/{id}", async (int id, ApplicationContext d) =>
{
    Person? user = await d.Persons.FirstOrDefaultAsync(x => x.Id == id);
    if (user == null) return Results.NotFound(new {message = "Пользователь не найден"});

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

app.Run();

