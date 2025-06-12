
using Microsoft.EntityFrameworkCore;

[Index("Name", IsUnique = true)]
public class Todo
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string Description { get; set; } = "";
    public bool Completed { get; set; } = false;

    internal void Complete() => Completed = true;
    internal void Uncomplete() => Completed = false;

    internal void Update(Todo input)
    {
        Name = input.Name;
        Description = input.Description;
        Completed = input.Completed;
    }

    internal void Update(UpdateTodoInput input)
    {
        Description = input.Description;
        Completed = input.Completed;
    }
}