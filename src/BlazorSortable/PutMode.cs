namespace BlazorSortable;

/// <summary>
/// Defines the mode for adding items to the list during drag and drop operations.
/// </summary>
public enum PutMode
{
    /// <summary>
    /// Allows adding items to the list.
    /// </summary>
    True,

    /// <summary>
    /// Prohibits adding items to the list.
    /// </summary>
    False,

    /// <summary>
    /// Allows adding items only from specified groups.
    /// Requires setting the <see cref="Internal.SortableBase.PutGroups"/> parameter.
    /// </summary>
    Groups
}
