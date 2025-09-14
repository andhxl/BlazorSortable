namespace BlazorSortable.Internal;

internal interface ISortableList : ISortableInfo
{
    object this[int index] { get; }

    int DraggedItemIndex { get; }
}
