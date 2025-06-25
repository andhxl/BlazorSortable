namespace BlazorSortable;

/// <summary>
/// Provides information about a sortable list component.
/// </summary>
/// <remarks>
/// This interface defines the basic properties that identify and categorize a sortable list component.
/// </remarks>
public interface ISortableListInfo
{
    /// <summary>
    /// Unique identifier of the component.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Group name for interaction with other Sortable components.
    /// </summary>
    public string Group { get; }
}
