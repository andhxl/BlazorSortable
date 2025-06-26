namespace BlazorSortable.Internal;

internal interface ISortableService
{
    void RegisterSortableList(string id, ISortableList sortableList);

    void UnregisterSortableList(string id);

    ISortableList? GetSortableList(string id);
}
