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
    [Parameter]
    public EventCallback<object> OnDrop { get; set; }

    /// <summary>
    /// Unique component identifier.
    /// </summary>
    [Parameter]
    [Obsolete("Id is now generated automatically only. Manual setting is no longer supported.")]
    public string? Id { get; set; }

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
    /// <param name="sourceSortableId">Source SortableList identifier.</param>
    /// <param name="index">Item index in the source SortableList.</param>
    [JSInvokable]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void OnAddJs(string sourceSortableId, int index)
    {
        var sourceSortable = SortableService.GetSortableList(sourceSortableId)!;
        var sourceObject = sourceSortable[index]!;

        OnDrop.InvokeAsync(sourceObject);
    }
}
