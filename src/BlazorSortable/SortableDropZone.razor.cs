using BlazorSortable.Internal;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.ComponentModel;

namespace BlazorSortable;

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
    public Action<(object item, string sourceId, bool isClone, int index)>? OnDrop { get; set; }

    private protected override string InitMethodName => "initDropZone";

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
            ["group"] = group,
            ["ghostClass"] = GhostClass
        };

        return options;
    }

    /// <summary>
    /// Event handler for adding an item, called from JavaScript.
    /// </summary>
    /// <param name="sourceId">Source SortableList identifier.</param>
    /// <param name="isClone">Flag indicating whether the item is a clone.</param>
    /// <param name="index">Item index in the source SortableList.</param>
    [JSInvokable]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void OnAddJs(string sourceId, bool isClone, int index)
    {
        var sortable = SortableService.GetSortableList(sourceId)!;
        var item = sortable.GetItem(index);
        if (item is null) return;

        OnDrop?.Invoke((item, sourceId, isClone, index));
    }
}
