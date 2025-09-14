namespace BlazorSortable;

/// <summary>
/// Defines the mode for pulling items from the list during drag and drop operations.
/// </summary>
public enum SortablePullMode
{
    /// <summary>
    /// Allows pulling items from the list.
    /// </summary>
    True,

    /// <summary>
    /// Prohibits pulling items from the list.
    /// </summary>
    False,

    /// <summary>
    /// Allows pulling items only from specified groups.
    /// Requires setting the <see cref="Sortable{TItem}.PullGroups"/> parameter.
    /// </summary>
    Groups,

    /// <summary>
    /// Creates a clone of the item when dragging.
    /// Requires setting the <see cref="Sortable{TItem}.CloneFunction"/> parameter.
    /// </summary>
    Clone,

    /// <summary>
    /// Uses a custom function to determine if an item can be pulled from the list.
    /// Requires setting the <see cref="Sortable{TItem}.PullFunction"/> parameter.
    /// </summary>
    Function
}
