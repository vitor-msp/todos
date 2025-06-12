namespace Todos;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

[Table("todos")]
[Index("Name", IsUnique = true)]
public class Todo
{
    [Key, Column("id")]
    public int Id { get; set; }

    [Column("name")]
    public required string Name { get; set; }

    [Column("description")]
    public string Description { get; set; } = "";

    [Column("completed")]
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