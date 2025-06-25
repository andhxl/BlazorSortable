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
    /// Uses Action instead of EventCallback to prevent automatic StateHasChanged
    /// in the parent component, which can cause conflicts with dynamic collections.
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
    /// <param name="sourceSortableId">Source SortableList identifier.</param>
    /// <param name="isClone">Flag indicating whether the item is a clone.</param>
    /// <param name="index">Item index in the source SortableList.</param>
    [JSInvokable]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void OnAddJs(string sourceSortableId, bool isClone, int index)
    {
        var sourceSortable = SortableService.GetSortableList(sourceSortableId)!;
        var item = sourceSortable.GetItem(index);

        // Skip processing if item is null (can happen in clone mode when cloning fails)
        if (item is null) return;

        var args = new SortableEventArgs<object>(item, sourceSortable, index) { IsClone = isClone };
        OnDrop?.Invoke(args);
    }
}
