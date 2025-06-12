using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDbContext>(options => options.UseInMemoryDatabase("Todos"));
var app = builder.Build();

app.MapPost("/todos", async ([FromBody] Todo input, AppDbContext context) =>
{
    var existingTodo = await context.Todos.FirstOrDefaultAsync(t => t.Name.Equals(input.Name));
    if (existingTodo is not null) return Results.Conflict();
    context.Add(input);
    await context.SaveChangesAsync();
    return Results.Created($"/todos/{input.Id}", input);
});

app.MapPut("/todos/{id}", async ([FromRoute] int id, [FromBody] Todo input, AppDbContext context) =>
{
    if (input.Id != default && id != input.Id) return Results.UnprocessableEntity();
    var savedTodo = await context.Todos.FindAsync(id);
    if (savedTodo is null) return Results.NotFound();
    if (!savedTodo.Name.Equals(input.Name))
    {
        var todoWithSameName = await context.Todos.FirstOrDefaultAsync(t => t.Name.Equals(input.Name));
        if (todoWithSameName is not null) return Results.Conflict();
    }
    savedTodo.Update(input);
    await context.SaveChangesAsync();
    return Results.NoContent();
});

app.MapPatch("/todos/{id}", async ([FromRoute] int id, [FromBody] UpdateTodoInput input, AppDbContext context) =>
{
    var savedTodo = await context.Todos.FindAsync(id);
    if (savedTodo is null) return Results.NotFound();
    savedTodo.Update(input);
    await context.SaveChangesAsync();
    return Results.NoContent();
});

app.MapPatch("/todos/{id}/completed", async ([FromRoute] int id, [FromBody] CompleteTodoInput input, AppDbContext context) =>
{
    var savedTodo = await context.Todos.FindAsync(id);
    if (savedTodo is null) return Results.NotFound();
    if (input.Completed) savedTodo.Complete(); else savedTodo.Uncomplete();
    await context.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/todos/{id}", async ([FromRoute] int id, AppDbContext context) =>
{
    var savedTodo = await context.Todos.FindAsync(id);
    if (savedTodo is null) return Results.NotFound();
    context.Todos.Remove(savedTodo);
    await context.SaveChangesAsync();
    return Results.NoContent();
});

app.MapGet("/todos/{id}", async ([FromRoute] int id, AppDbContext context) =>
{
    var savedTodo = await context.Todos.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
    return savedTodo is null ? Results.NotFound() : Results.Ok(savedTodo);
});

app.MapGet("/todos", async (AppDbContext context,
[FromQuery] bool? completed, [FromQuery] string? nameContains,
[FromQuery] int skip = 0, [FromQuery] int limit = 10,
[FromQuery] string orderBy = "id", [FromQuery] bool orderAsc = true) =>
{
    var query = context.Todos.AsNoTracking();
    if (completed is not null)
        query = query.Where(t => t.Completed == completed);
    if (nameContains is not null)
        query = query.Where(t => t.Name.Contains(nameContains));
    query = orderBy switch
    {
        "description" => orderAsc ? query.OrderBy(t => t.Description) : query.OrderByDescending(t => t.Description),
        "name" => orderAsc ? query.OrderBy(t => t.Name) : query.OrderByDescending(t => t.Name),
        _ => orderAsc ? query.OrderBy(t => t.Id) : query.OrderByDescending(t => t.Id)
    };
    query = query.Skip(skip).Take(limit);
    var todos = await query.ToListAsync();
    return Results.Ok(todos);
});

app.Run();

internal class UpdateTodoInput
{
    public string Description { get; set; } = "";
    public bool Completed { get; set; }
}

internal class CompleteTodoInput
{
    public bool Completed { get; set; }
}