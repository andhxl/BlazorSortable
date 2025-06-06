using System.Collections.Concurrent;

namespace BlazorSortable.Internal;

internal class SortableService : ISortableService
{
    private readonly ConcurrentDictionary<string, ISortable> _sortables = [];

    public void Register(string id, ISortable sortable)
    {
        _sortables[id] = sortable;
    }

    public void Unregister(string id)
    {
        _sortables.TryRemove(id, out _);
    }

    public ISortable? Get(string id)
    {
        _sortables.TryGetValue(id, out var sortable);
        return sortable;
    }
}
