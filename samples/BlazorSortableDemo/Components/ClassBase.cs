namespace BlazorSortableDemo.Components;

public abstract class ClassBase
{
    public Guid Id { get; } = Guid.NewGuid();

    public int Value { get; set; }

    public override string ToString() => $"{GetType().Name}: {Value}";
}
