namespace BlazorSortable.Internal;

internal interface ISortableList
{
    object? GetItem(int index);

    bool SuppressNextRemove { get; set; }
}
