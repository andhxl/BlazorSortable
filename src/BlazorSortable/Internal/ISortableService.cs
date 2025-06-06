namespace BlazorSortable.Internal;

internal interface ISortableService
{
    public void RegisterSortableList(string id, ISortableList sortableList);

    public void UnregisterSortableList(string id);

    public ISortableList? GetSortableList(string id);
}
