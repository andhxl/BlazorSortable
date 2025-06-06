namespace BlazorSortable.Internal;

internal interface ISortableService
{
    public void Register(string id, ISortable sortable);

    public void Unregister(string id);

    public ISortable? Get(string id);
}
