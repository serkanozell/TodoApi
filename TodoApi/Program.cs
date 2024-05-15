using Microsoft.EntityFrameworkCore;
using TodoApi;

var builder = WebApplication.CreateBuilder(args);

// Add DI - AddService

builder.Services.AddDbContext<TodoDbContext>(opt => opt.UseInMemoryDatabase("TodoDb"));

var app = builder.Build();

app.MapGet("/todoitems", async (TodoDbContext context) => await context.Todos.ToListAsync());

app.MapGet("/todoitems/completed", async (TodoDbContext context) => await context.Todos.Where(t => t.IsComplete).ToListAsync());

app.MapGet("/todoitems/{id}", async (int id, TodoDbContext context) => await context.Todos.FindAsync(id));

app.MapPost("/todoitems", async (TodoItem todoItem, TodoDbContext context) =>
{
    context.Todos.Add(todoItem);
    await context.SaveChangesAsync();
    return Results.Created($"/todoitems/{todoItem.Id}", todoItem);
});

app.MapPut("/todoitems/{id}", async (int id, TodoItem todoItem, TodoDbContext context) =>
{
    var todo = await context.Todos.FindAsync(id);
    if (todo == null) return Results.NotFound();
    todo.Name = todoItem.Name;
    todo.IsComplete = todoItem.IsComplete;
    await context.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/todoitems/{id}", async (int id, TodoDbContext context) =>
{
    if (await context.Todos.FindAsync(id) is TodoItem todo)
    {
        context.Todos.Remove(todo);
        await context.SaveChangesAsync();
        return Results.NoContent();
    }
    return Results.NotFound();
});

// Configure pipeline - UseMethod

app.Run();
