namespace BlazorSortable;

/// <summary>
/// Provides information about a sortable component.
/// </summary>
/// <remarks>
/// This interface defines the basic properties that identify and categorize a sortable component.
/// </remarks>
public interface ISortableInfo
{
    /// <summary>
    /// Unique identifier of the component.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Group name for interaction with other Sortable components.
    /// </summary>
    string Group { get; }
}
