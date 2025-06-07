using System.Collections.Concurrent;

namespace BlazorSortable.Internal;

internal class SortableService : ISortableService
{
    private readonly ConcurrentDictionary<string, ISortableList> _sortableLists = [];

    public void RegisterSortableList(string id, ISortableList sortableList)
    {
        if (!_sortableLists.TryAdd(id, sortableList))
        {
            throw new InvalidOperationException($"A SortableList with ID '{id}' is already registered.");
        }
    }

    public void UnregisterSortableList(string id)
    {
        _sortableLists.TryRemove(id, out _);
    }

    public ISortableList? GetSortableList(string id)
    {
        _sortableLists.TryGetValue(id, out var sortableList);
        return sortableList;
    }
}
