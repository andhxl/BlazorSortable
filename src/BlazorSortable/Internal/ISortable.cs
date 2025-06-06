namespace BlazorSortable.Internal;

internal interface ISortable
{
    object? this[int index] { get; }

    object? TryCloneItem(object item);

    bool SuppressNextRemove { get; set; }
}
