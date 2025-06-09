using BlazorSortable.Internal;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.ComponentModel;

namespace BlazorSortable;

/// <summary>
/// Component for creating sortable lists with drag and drop functionality.
/// </summary>
/// <typeparam name="TItem">Type of items in the list.</typeparam>
public partial class SortableList<TItem> : SortableBase, ISortableList
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
    /// Function used to generate a stable key for each item, used in the <c>@key</c> directive for rendering.
    /// If not provided, the item itself is used as the key.
    /// </summary>
    [Parameter]
    public Func<TItem, object>? KeySelector { get; set; }

    /// <summary>
    /// Unique identifier of the component. Must be globally unique across all SortableList instances.
    /// </summary>
    /// <remarks>
    /// If not set explicitly, a GUID will be generated automatically.
    /// This ID is required for internal coordination between Sortable components and for using converters.
    /// If two SortableList are registered with the same ID, an <see cref="InvalidOperationException"/> will be thrown during rendering.
    /// Set this manually only if you need to identify the component externally, e.g., to provide a converter for it.
    /// </remarks>
    [Parameter]
    public string Id
    {
        get => id;
        set => id = value;
    }

    /// <summary>
    /// Mode for pulling items from the list.
    /// </summary>
    [Parameter]
    public PullMode? Pull { get; set; }

    /// <summary>
    /// Array of group names from which items can be pulled.
    /// </summary>
    /// <remarks>
    /// Used only when <see cref="Pull"/> is set to <see cref="PullMode.Groups"/>.
    /// Ignored in other pull modes.
    /// </remarks>
    [Parameter]
    public string[]? PullGroups { get; set; }

    /// <summary>
    /// Factory function used to create a deep copy of an item when items are pulled in clone mode.
    /// </summary>
    /// <remarks>
    /// Used only when <see cref="Pull"/> is set to <see cref="PullMode.Clone"/>.
    /// If not provided in clone mode, item cloning will be skipped and null will be added instead.
    /// </remarks>
    [Parameter]
    public Func<TItem, TItem>? CloneFactory { get; set; }

    /// <summary>
    /// Called when an unhandled exception occurs during object cloning.
    /// </summary>
    /// <remarks>
    /// Uses Action instead of EventCallback to prevent automatic StateHasChanged 
    /// in the parent component, which can cause conflicts with dynamic collections.
    /// </remarks>
    [Parameter]
    public Action<Exception>? OnCloneException { get; set; }

    /// <summary>
    /// Dictionary of converters for transforming items from other SortableLists.
    /// </summary>
    /// <remarks>
    /// The key is the <c>Id</c> of another <see cref="SortableList{TItem}"/> that provides items,
    /// and the value is a function that converts an item from that list to the target <typeparamref name="TItem"/> type.
    /// This is used when items are dragged between lists with different data types.
    /// </remarks>
    [Parameter]
    public Dictionary<string, Func<object, TItem>>? Converters { get; set; }

    /// <summary>
    /// Called when an unhandled exception occurs during converter execution.
    /// </summary>
    /// <remarks>
    /// Uses Action instead of EventCallback to prevent automatic StateHasChanged 
    /// in the parent component, which can cause conflicts with dynamic collections.
    /// </remarks>
    [Parameter]
    public Action<Exception>? OnConvertException { get; set; }

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
    /// Event that occurs when an item is added to the list.
    /// </summary>
    /// <remarks>
    /// Uses Action instead of EventCallback to prevent automatic StateHasChanged
    /// in the parent component, which can cause conflicts with dynamic collections.
    /// </remarks>
    [Parameter]
    public Action<(TItem item, string sourceId, bool isClone, int oldIndex, int newIndex)>? OnAdd { get; set; }

    /// <summary>
    /// Event that occurs when the order of items in the list is updated.
    /// </summary>
    /// <remarks>
    /// Uses Action instead of EventCallback to prevent automatic StateHasChanged
    /// in the parent component, which can cause conflicts with dynamic collections.
    /// </remarks>
    [Parameter]
    public Action<(TItem item, int oldIndex, int newIndex)>? OnUpdate { get; set; }

    /// <summary>
    /// Event that occurs when an item is removed from the list.
    /// </summary>
    /// <remarks>
    /// Uses Action instead of EventCallback to prevent automatic StateHasChanged
    /// in the parent component, which can cause conflicts with dynamic collections.
    /// </remarks>
    [Parameter]
    public Action<(TItem item, int index)>? OnRemove { get; set; }

    /// <summary>
    /// Validates required parameters during component initialization.
    /// Throws <see cref="ArgumentNullException"/> if any required parameter is null.
    /// </summary>
    protected override void OnInitialized()
    {
        ArgumentNullException.ThrowIfNull(Items);
    }

    private protected override string InitMethodName => "init";

    private protected override Task OnAfterFirstRenderAsync()
    {
        SortableService.RegisterSortableList(id, this);

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
    /// <param name="sourceId">Source list identifier.</param>
    /// <param name="isClone">Flag indicating whether the item is a clone.</param>
    /// <param name="oldIndex">Item index in the source list.</param>
    /// <param name="newIndex">Item index in the target list.</param>
    [JSInvokable]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void OnAddJs(string sourceId, bool isClone, int oldIndex, int newIndex)
    {
        var sourceSortable = SortableService.GetSortableList(sourceId)!;
        sourceSortable.SuppressNextRemove = !isClone;

        var sourceObject = sourceSortable.GetItem(oldIndex);
        if (sourceObject is null) return;

        TItem? itemToAdd = default;

        if (Converters is not null && Converters.ContainsKey(sourceId))
        {
            itemToAdd = TryConvertItem(sourceId, sourceObject);
        }
        else if (sourceObject is TItem sourceItem)
        {
            itemToAdd = sourceItem;
        }

        if (itemToAdd is null) return;

        InsertItem(newIndex, itemToAdd);
        sourceSortable.SuppressNextRemove = false;

        StateHasChanged();

        OnAdd?.Invoke((itemToAdd, sourceId, isClone, oldIndex, newIndex));
    }

    /// <summary>
    /// Event handler for updating item order, called from JavaScript.
    /// </summary>
    /// <param name="oldIndex">Item index before moving.</param>
    /// <param name="newIndex">Item index after moving.</param>
    [JSInvokable]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void OnUpdateJs(int oldIndex, int newIndex)
    {
        var itemToMove = Items[oldIndex];
        Items.RemoveAt(oldIndex);
        InsertItem(newIndex, itemToMove);

        StateHasChanged();

        OnUpdate?.Invoke((itemToMove, oldIndex, newIndex));
    }

    /// <summary>
    /// Event handler for removing an item, called from JavaScript.
    /// </summary>
    /// <param name="index">Index of the item to remove.</param>
    [JSInvokable]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void OnRemoveJs(int index)
    {
        var sortable = (ISortableList)this;
        if (sortable.SuppressNextRemove)
        {
            sortable.SuppressNextRemove = false;
            return;
        }

        var itemToRemove = Items[index];
        Items.RemoveAt(index);

        StateHasChanged();

        OnRemove?.Invoke((itemToRemove, index));
    }

    object? ISortableList.GetItem(int index)
    {
        var item = Items[index];

        return Pull == PullMode.Clone ? TryCloneItem(item) : item;
    }

    bool ISortableList.SuppressNextRemove { get; set; }

    private TItem? TryCloneItem(TItem item)
    {
        if (CloneFactory is null) return default;

        try
        {
            return CloneFactory(item);
        }
        catch (Exception ex)
        {
            OnCloneException?.Invoke(ex);
            return default;
        }
    }

    private TItem? TryConvertItem(string sortableId, object item)
    {
        try
        {
            return Converters![sortableId](item);
        }
        catch (Exception ex)
        {
            OnConvertException?.Invoke(ex);
            return default;
        }
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

    private protected override ValueTask DisposeAsyncCore()
    {
        SortableService.UnregisterSortableList(id);

        return ValueTask.CompletedTask;
    }
}
