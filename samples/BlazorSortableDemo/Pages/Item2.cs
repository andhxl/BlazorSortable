namespace BlazorSortableDemo.Pages;

public class Item2
{
    public Guid Id { get; } = Guid.NewGuid();

    public required string Name { get; init; }

    public int Value { get; set; }

    public string Type { get; } = nameof(Item2);
}
