namespace BlazorSortable;

/// <summary>
/// Represents event arguments for sorting operations in Blazor Sortable components.
/// </summary>
/// <typeparam name="TItem">The type of the item being sorted.</typeparam>
/// <param name="item">The item participating in the sorting operation.</param>
/// <param name="sourceSortable">Source sortable.</param>
/// <param name="oldIndex">The previous index of the item in the list. Default is -1.</param>
/// <param name="newIndex">The new index of the item in the list. Default is -1.</param>
public class SortableEventArgs<TItem>(TItem item, ISortableListInfo sourceSortable, int oldIndex = -1, int newIndex = -1)
{
    /// <summary>
    /// Gets the item participating in the sorting operation.
    /// </summary>
    public TItem Item { get; } = item;

    /// <summary>
    /// Source sortable.
    /// </summary>
    public ISortableListInfo SourceSortable { get; } = sourceSortable;

    /// <summary>
    /// Gets or sets a value indicating whether the item is a clone.
    /// Clones are created when dragging items between lists.
    /// </summary>
    public bool IsClone { get; init; }

    /// <summary>
    /// Gets the previous index of the item in the list.
    /// A value of -1 indicates that the item had no previous index.
    /// </summary>
    public int OldIndex { get; } = oldIndex;

    /// <summary>
    /// Gets the new index of the item in the list.
    /// A value of -1 indicates that the item has no new index.
    /// </summary>
    public int NewIndex { get; } = newIndex;
}
