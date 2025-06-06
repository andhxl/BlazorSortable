namespace BlazorSortable.Internal;

internal interface ISortableList
{
    object? this[int index] { get; }

    object? TryCloneItem(object item);

    bool SuppressNextRemove { get; set; }
}
