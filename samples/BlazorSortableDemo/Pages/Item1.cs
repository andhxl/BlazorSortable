namespace BlazorSortableDemo.Pages;

public class Item1
{
    public Guid Id { get; } = Guid.NewGuid();

    public required string Name { get; init; }

    public string Type { get; } = nameof(Item1);
}
