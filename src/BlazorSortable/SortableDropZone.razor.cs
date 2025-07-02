using BlazorSortable.Internal;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.ComponentModel;

namespace BlazorSortable;

// TODO: MultiDrag

/// <summary>
/// Component for creating a drop zone for elements.
/// </summary>
public partial class SortableDropZone : SortableBase
{
    /// <summary>
    /// Event that occurs when an element is dropped into the zone.
    /// </summary>
    /// <remarks>
    /// Uses Action instead of EventCallback to prevent automatic StateHasChanged in the parent component.
    /// </remarks>
    [Parameter]
    public Action<SortableEventArgs<object>>? OnDrop { get; set; }

    private protected override string InitMethodName => "initSortableDropZone";

    private protected override Dictionary<string, object> BuildOptions()
    {
        var group = new Dictionary<string, object>
        {
            ["name"] = Group
        };

        var put = GetPut();
        if (put is not null)
            group["put"] = put;

        var options = new Dictionary<string, object>
        {
            ["disabled"] = Disabled,
            ["group"] = group,
            ["ghostClass"] = GhostClass
        };

        return options;
    }

    /// <summary>
    /// Event handler for adding an item, called from JavaScript.
    /// </summary>
    /// <param name="fromId">Source Sortable identifier.</param>
    /// <param name="oldIndex">Item index in the source Sortable.</param>
    /// <param name="newIndex">Item index in the target Sortable.</param>
    /// <param name="isClone">Flag indicating whether the item is a clone.</param>
    [JSInvokable]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void OnAddJs(string fromId, int oldIndex, int newIndex, bool isClone)
    {
        var from = SortableService.GetSortableList(fromId)!;
        var item = from.GetItem(oldIndex);

        // Skip processing if item is null (can happen in clone mode when cloning fails)
        if (item is null) return;

        var args = new SortableEventArgs<object>(item, from, oldIndex, this, newIndex, isClone);
        OnDrop?.Invoke(args);
    }
}
