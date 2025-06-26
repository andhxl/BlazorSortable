namespace BlazorSortable.Internal;

internal interface ISortableList : ISortable
{
    int DraggedItemIndex { get; }

    object? GetItem(int index);

    bool SuppressNextRemove { get; set; }
}
