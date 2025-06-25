namespace BlazorSortable.Internal;

internal interface ISortableList : ISortableListInfo
{
    int DraggedItemIndex { get; }

    object? GetItem(int index);

    bool SuppressNextRemove { get; set; }
}
