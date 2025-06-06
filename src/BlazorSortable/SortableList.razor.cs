using BlazorSortable.Internal;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorSortable;

/// <summary>
/// Blazor component for creating sortable lists with drag and drop functionality.
/// </summary>
/// <typeparam name="TItem">Type of items in the list.</typeparam>
/// <remarks>
/// The component uses the SortableJS library.
/// </remarks>
public partial class SortableList<TItem> : SortableBase, ISortable
{
    /// <summary>
    /// List of items to display and sort.
    /// </summary>
    [Parameter, EditorRequired]
    public IList<TItem> Items { get; set; } = default!;

    /// <summary>
    /// Template for displaying each list item.
    /// </summary>
    [Parameter]
    public RenderFragment<TItem>? ChildContent { get; set; }

    /// <summary>
    /// Event that occurs when an item is added to the list.
    /// </summary>
    [Parameter]
    public EventCallback<(TItem item, string sourceSortableId, bool isClone)> OnAdd { get; set; }

    /// <summary>
    /// Event that occurs when the order of items in the list is updated.
    /// </summary>
    [Parameter]
    public EventCallback<TItem> OnUpdate { get; set; }

    /// <summary>
    /// Event that occurs when an item is removed from the list.
    /// </summary>
    [Parameter]
    public EventCallback<TItem> OnRemove { get; set; }

    /// <summary>
    /// Event that occurs when an unhandled error occurs inside the component.
    /// </summary>
    [Parameter]
    public EventCallback<Exception> OnError { get; set; }

    /// <summary>
    /// Mode for pulling items from the list.
    /// </summary>
    [Parameter]
    public PullMode? Pull { get; set; }

    /// <summary>
    /// Groups from which items can be pulled.
    /// </summary>
    [Parameter]
    public string[]? PullGroups { get; set; }

    /// <summary>
    /// Enables or disables sorting of items within the list.
    /// </summary>
    [Parameter]
    public bool Sort { get; set; } = true;

    /// <summary>
    /// Animation duration in milliseconds.
    /// </summary>
    [Parameter]
    public int Animation { get; set; } = 150;

    /// <summary>
    /// CSS selector for elements that can be used for dragging.
    /// Example: ".my-handle" - dragging only by elements with class my-handle
    /// </summary>
    [Parameter]
    public string? Handle { get; set; }

    /// <summary>
    /// CSS selector for elements that cannot be dragged.
    /// Example: ".ignore-elements" - dragging disabled for elements with class ignore-elements
    /// </summary>
    [Parameter]
    public string? Filter { get; set; }

    /// <summary>
    /// CSS class for the chosen element.
    /// </summary>
    [Parameter]
    public string ChosenClass { get; set; } = "sortable-chosen";

    /// <summary>
    /// CSS class for the dragged element.
    /// </summary>
    [Parameter]
    public string DragClass { get; set; } = "sortable-drag";

    /// <summary>
    /// Forces the use of fallback mode for dragging.
    /// </summary>
    [Parameter]
    public bool ForceFallback { get; set; } = true;

    /// <summary>
    /// CSS class for the element in fallback mode.
    /// </summary>
    [Parameter]
    public string FallbackClass { get; set; } = "sortable-fallback";

    /// <summary>
    /// Factory for creating item clones.
    /// </summary>
    [Parameter]
    public Func<TItem, TItem>? CloneFactory { get; set; }

    /// <summary>
    /// Dictionary of converters for transforming items from other lists.
    /// </summary>
    [Parameter]
    public Dictionary<string, Func<object, TItem>>? Converters { get; set; }

    /// <summary>
    /// Function for selecting a key for an item.
    /// </summary>
    [Parameter]
    public Func<TItem, object>? KeySelector { get; set; }

    private protected override string InitMethodName => "init";

    private protected override Task OnAfterFirstRenderAsync()
    {
        SortableService.Register(Id, this);

        return Task.CompletedTask;
    }

    private protected override Dictionary<string, object> BuildOptions()
    {
        var group = new Dictionary<string, object>
        {
            ["name"] = Group
        };

        var pull = GetPull();
        if (pull is not null)
            group["pull"] = pull;

        var put = GetPut();
        if (put is not null)
            group["put"] = put;

        var options = new Dictionary<string, object>
        {
            ["group"] = group,
            ["sort"] = Sort,
            ["animation"] = Animation,
            ["ghostClass"] = GhostClass,
            ["chosenClass"] = ChosenClass,
            ["dragClass"] = DragClass,
            ["forceFallback"] = ForceFallback,
            ["fallbackClass"] = FallbackClass
        };

        if (!string.IsNullOrWhiteSpace(Handle))
            options["handle"] = Handle;

        if (!string.IsNullOrWhiteSpace(Filter))
            options["filter"] = Filter;

        return options;
    }

    private object? GetPull()
    {
        return Pull switch
        {
            PullMode.True => true,
            PullMode.False => false,
            PullMode.Groups => PullGroups?.Length > 0 ? PullGroups : null,
            PullMode.Clone => "clone",
            _ => null
        };
    }

    private object GetKey(TItem item) => KeySelector?.Invoke(item) ?? item!;

    /// <summary>
    /// Event handler for adding an item, called from JavaScript.
    /// </summary>
    /// <param name="sourceSortableId">Source list identifier.</param>
    /// <param name="oldIndex">Item index in the source list.</param>
    /// <param name="newIndex">Item index in the target list.</param>
    /// <param name="isClone">Flag indicating whether the item is a clone.</param>
    [JSInvokable]
    public void OnAddJs(string sourceSortableId, int oldIndex, int newIndex, bool isClone)
    {
        var sourceSortable = SortableService.Get(sourceSortableId)!;
        sourceSortable.SuppressNextRemove = !isClone;

        var sourceObject = sourceSortable[oldIndex]!;

        TItem? itemToAdd = default;

        if (Converters is not null && Converters.ContainsKey(sourceSortableId))
        {
            try
            {
                itemToAdd = Converters[sourceSortableId](sourceObject);
            }
            catch (Exception ex)
            {
                OnError.InvokeAsync(ex);
                return;
            }
        }
        else if (sourceObject is TItem sourceItem)
        {
            if (isClone)
            {
                itemToAdd = (TItem?)sourceSortable.TryCloneItem(sourceItem);
            }
            else
            {
                itemToAdd = sourceItem;
            }
        }

        if (itemToAdd is null) return;

        InsertItem(newIndex, itemToAdd);
        sourceSortable.SuppressNextRemove = false;

        StateHasChanged();

        OnAdd.InvokeAsync((itemToAdd, sourceSortableId, isClone));
    }

    /// <summary>
    /// Event handler for updating item order, called from JavaScript.
    /// </summary>
    /// <param name="oldIndex">Item index before moving.</param>
    /// <param name="newIndex">Item index after moving.</param>
    [JSInvokable]
    public void OnUpdateJs(int oldIndex, int newIndex)
    {
        var itemToMove = Items[oldIndex];
        Items.RemoveAt(oldIndex);
        InsertItem(newIndex, itemToMove);

        StateHasChanged();

        OnUpdate.InvokeAsync(itemToMove);
    }

    /// <summary>
    /// Event handler for removing an item, called from JavaScript.
    /// </summary>
    /// <param name="index">Index of the item to remove.</param>
    [JSInvokable]
    public void OnRemoveJs(int index)
    {
        var sortable = (ISortable)this;

        if (sortable.SuppressNextRemove)
        {
            sortable.SuppressNextRemove = false;
            return;
        }

        var itemToRemove = Items[index];
        Items.RemoveAt(index);

        StateHasChanged();

        OnRemove.InvokeAsync(itemToRemove);
    }

    private void InsertItem(int index, TItem item)
    {
        if (index < Items.Count)
        {
            Items.Insert(index, item);
        }
        else
        {
            Items.Add(item);
        }
    }

    object? ISortable.this[int index]
    {
        get => Items[index];
    }

    object? ISortable.TryCloneItem(object item)
    {
        if (CloneFactory is null) return null;

        try
        {
            return CloneFactory((TItem)item);
        }
        catch (Exception ex)
        {
            OnError.InvokeAsync(ex);
            return null;
        }
    }

    bool ISortable.SuppressNextRemove { get; set; }

    private protected override ValueTask DisposeAsyncCore()
    {
        SortableService.Unregister(Id);

        return ValueTask.CompletedTask;
    }
}
