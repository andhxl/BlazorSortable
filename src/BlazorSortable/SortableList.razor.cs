using BlazorSortable.Internal;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.ComponentModel;

namespace BlazorSortable;

// TODO: MultiDrag

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

    /// <inheritdoc />
    /// <exception cref="InvalidOperationException">
    /// Thrown when multiple SortableList components are registered with the same ID during rendering.
    /// </exception>
    [Parameter]
    public override string Id { get; set; } = Guid.NewGuid().ToString();

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
    public Func<TItem, TItem>? CloneFunction { get; set; }

    /// <summary>
    /// Called when an unhandled exception occurs during object cloning.
    /// </summary>
    /// <remarks>
    /// Uses Action instead of EventCallback to prevent automatic StateHasChanged  in the parent component.
    /// </remarks>
    [Parameter]
    public Action<Exception>? OnCloneException { get; set; }

    /// <summary>
    /// Function used to determine if an item can be pulled from this list to another list.
    /// </summary>
    /// <remarks>
    /// Used only when <see cref="Pull"/> is set to <see cref="PullMode.Function"/>.
    /// The function receives the item being dragged and the target list info.
    /// Return <c>true</c> to allow pulling, <c>false</c> to deny.
    /// </remarks>
    [Parameter]
    public Func<SortableTransferContext<TItem>, bool>? PullFunction { get; set; }

    /// <summary>
    /// Dictionary of converters for transforming items from other SortableLists.
    /// </summary>
    /// <remarks>
    /// The key is the <c>Id</c> of another <see cref="SortableList{TItem}"/> that provides items,
    /// and the value is a function that converts an item from that list to the target <typeparamref name="TItem"/> type.
    /// This is used when items are dragged between lists with different data types.
    /// </remarks>
    [Parameter]
    public Func<SortableTransferContext<object>, TItem?>? ConvertFunction { get; set; }

    /// <summary>
    /// Called when an unhandled exception occurs during converter execution.
    /// </summary>
    /// <remarks>
    /// Uses Action instead of EventCallback to prevent automatic StateHasChanged  in the parent component.
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
    /// Function used to determine if an item can be dragged.
    /// </summary>
    /// <remarks>
    /// If provided, only items that return true from this function will be draggable.
    /// The draggable class will be applied to items that return true.
    /// </remarks>
    [Parameter]
    public Func<TItem, bool>? DraggableSelector { get; set; }

    /// <summary>
    /// CSS class applied to items that can be dragged.
    /// </summary>
    /// <remarks>
    /// Used in conjunction with <see cref="DraggableSelector"/> to style draggable items.
    /// </remarks>
    [Parameter]
    public string DraggableClass { get; set; } = "sortable-draggable";

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
    /// Threshold for swap detection during dragging.
    /// </summary>
    /// <remarks>
    /// Determines how much overlap is required before items are swapped.
    /// Value between 0 and 1.
    /// </remarks>
    [Parameter]
    public double SwapThreshold { get; set; } = 1;

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

    ///// <summary>
    ///// Enables multi-drag functionality.
    ///// </summary>
    //[Parameter]
    //public bool MultiDrag { get; set; }

    ///// <summary>
    ///// CSS class for selected items in multi-drag mode.
    ///// </summary>
    //[Parameter]
    //public string SelectedClass { get; set; } = "sortable-selected";

    ///// <summary>
    ///// Key used to enable multi-drag selection.
    ///// </summary>
    ///// <remarks>
    ///// Default is "Control" key. Users must hold this key while clicking to select multiple items.
    ///// </remarks>
    //[Parameter]
    //public string? MultiDragKey { get; set; } = "Control";

    ///// <summary>
    ///// Prevents automatic deselection when clicking on selected items.
    ///// </summary>
    ///// <remarks>
    ///// When true, clicking on a selected item will not deselect it.
    ///// Useful for maintaining selection state during complex interactions.
    ///// </remarks>
    //[Parameter]
    //public bool AvoidImplicitDeselect { get; set; }

    /// <summary>
    /// Enables swap mode for dragging.
    /// </summary>
    /// <remarks>
    /// When enabled, dragging an item over another item will swap their positions
    /// instead of inserting the dragged item at the new position.
    /// </remarks>
    [Parameter]
    public bool Swap { get; set; }

    /// <summary>
    /// CSS class applied to items during swap highlighting.
    /// </summary>
    /// <remarks>
    /// Applied to items that would be swapped when <see cref="Swap"/> is enabled.
    /// </remarks>
    [Parameter]
    public string SwapClass { get; set; } = "sortable-swap-highlight";

    /// <summary>
    /// Enables scrolling of the container during dragging.
    /// </summary>
    /// <remarks>
    /// When enabled, the container will scroll when dragging items near its edges.
    /// </remarks>
    [Parameter]
    public bool Scroll { get; set; } = true;

    /// <summary>
    /// Event that occurs when the order of items in the list is updated.
    /// </summary>
    /// <remarks>
    /// Uses Action instead of EventCallback to prevent automatic StateHasChanged in the parent component.
    /// </remarks>
    [Parameter]
    public Action<SortableEventArgs<TItem>>? OnUpdate { get; set; }

    /// <summary>
    /// Event that occurs when an item is added to the list.
    /// </summary>
    /// <remarks>
    /// Uses Action instead of EventCallback to prevent automatic StateHasChanged in the parent component.
    /// </remarks>
    [Parameter]
    public Action<SortableEventArgs<TItem>>? OnAdd { get; set; }

    /// <summary>
    /// Event that occurs when an item is removed from the list.
    /// </summary>
    /// <remarks>
    /// Uses Action instead of EventCallback to prevent automatic StateHasChanged in the parent component.
    /// </remarks>
    [Parameter]
    public Action<SortableEventArgs<TItem>>? OnRemove { get; set; }

    ///// <summary>
    ///// Event that occurs when an item is selected in multi-drag mode.
    ///// </summary>
    ///// <remarks>
    ///// Uses Action instead of EventCallback to prevent automatic StateHasChanged in the parent component.
    ///// </remarks>
    //[Parameter]
    //public Action<TItem>? OnSelect { get; set; }

    ///// <summary>
    ///// Event that occurs when an item is deselected in multi-drag mode.
    ///// </summary>
    ///// <remarks>
    ///// Uses Action instead of EventCallback to prevent automatic StateHasChanged in the parent component.
    ///// </remarks>
    //[Parameter]
    //public Action<TItem>? OnDeselect { get; set; }

    private int draggedItemIndex = -1;
    private bool suppressNextRemove;

    /// <summary>
    /// Validates required parameters during component initialization.
    /// Throws <see cref="ArgumentNullException"/> if any required parameter is null.
    /// </summary>
    protected override void OnInitialized()
    {
        ArgumentNullException.ThrowIfNull(Items);
    }

    private protected override string InitMethodName => "initSortableList";

    /// <summary>
    /// Called by the Blazor framework after the component has rendered.
    /// </summary>
    /// <param name="firstRender">True if this is the first time the component is rendered.</param>
    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender)
        {
            SortableService.RegisterSortableList(Id, this);
        }
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
            ["disabled"] = Disabled,
            ["animation"] = Animation,
            ["ghostClass"] = GhostClass,
            ["chosenClass"] = ChosenClass,
            ["dragClass"] = DragClass,
            ["swapThreshold"] = SwapThreshold,
            ["forceFallback"] = ForceFallback,
            ["fallbackClass"] = FallbackClass,
        };

        if (!string.IsNullOrWhiteSpace(Handle))
            options["handle"] = Handle;

        if (!string.IsNullOrWhiteSpace(Filter))
            options["filter"] = Filter;

        if (DraggableSelector is not null)
            options["draggable"] = "." + DraggableClass;

        //if (MultiDrag)
        //{
        //    options["multiDrag"] = true;
        //    options["selectedClass"] = SelectedClass;
        //    options["avoidImplicitDeselect"] = AvoidImplicitDeselect;

        //    if (!string.IsNullOrWhiteSpace(MultiDragKey))
        //        options["multiDragKey"] = MultiDragKey;
        //}

        if (Swap)
        {
            options["swap"] = true;
            options["swapClass"] = SwapClass;
        }

        options["scroll"] = Scroll;

        // Possible bug in OnSpill: item might be removed from the list even if removeOnSpill is false and revertOnSpill is true

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
            PullMode.Function => "function",
            _ => null
        };
    }

    /// <summary>
    /// Called from JavaScript when the drag operation starts.
    /// </summary>
    /// <param name="index">The index of the item being dragged.</param>
    [JSInvokable]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void OnStartJs(int index)
    {
        draggedItemIndex = index;
    }

    /// <summary>
    /// Called from JavaScript when the drag operation ends.
    /// </summary>
    [JSInvokable]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void OnEndJs()
    {
        draggedItemIndex = -1;
    }

    /// <summary>
    /// Called from JavaScript to determine if an item can be pulled to the target sortable.
    /// </summary>
    /// <param name="toId">The ID of the target sortable.</param>
    /// <returns><c>true</c> if the item can be pulled; otherwise, <c>false</c>.</returns>
    [JSInvokable]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool OnPullJs(string toId)
    {
        var item = Items[draggedItemIndex];
        var to = SortableService.GetSortableList(toId)!;
        var ctx = new SortableTransferContext<TItem>(item, this, to);

        return PullFunction?.Invoke(ctx) ?? false;
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
        Items.Insert(newIndex, itemToMove);

        StateHasChanged();

        var args = new SortableEventArgs<TItem>(itemToMove, this, oldIndex, this, newIndex);

        OnUpdate?.Invoke(args);
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
        from.SuppressNextRemove = !isClone;

        var sourceObject = from.GetItem(oldIndex);
        if (sourceObject is null) return;

        TItem? itemToAdd = default;

        if (ConvertFunction is not null)
        {
            itemToAdd = TryConvertItem(sourceObject, from);
        }
        else if (sourceObject is TItem sourceItem)
        {
            itemToAdd = sourceItem;
        }

        if (itemToAdd is null) return;

        Items.Insert(newIndex, itemToAdd);
        from.SuppressNextRemove = false;

        StateHasChanged();

        var args = new SortableEventArgs<TItem>(itemToAdd, from, oldIndex, this, newIndex, isClone);

        OnAdd?.Invoke(args);
    }

    /// <summary>
    /// Event handler for removing an item, called from JavaScript.
    /// </summary>
    /// <param name="oldIndex">Item index in the source Sortable.</param>
    /// <param name="toId">Target Sortable identifier.</param>
    /// <param name="newIndex">Item index in the target Sortable.</param>
    [JSInvokable]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public void OnRemoveJs(int oldIndex, string toId, int newIndex)
    {
        if (suppressNextRemove)
        {
            suppressNextRemove = false;
            return;
        }

        var itemToRemove = Items[oldIndex];

        Items.RemoveAt(oldIndex);

        StateHasChanged();

        var to = SortableService.GetSortableList(toId)!;
        var args = new SortableEventArgs<TItem>(itemToRemove, this, oldIndex, to, newIndex);

        OnRemove?.Invoke(args);
    }

    ///// <summary>
    ///// Event handler for selecting an item, called from JavaScript.
    ///// </summary>
    ///// <param name="index">Index of the selected item.</param>
    //[JSInvokable]
    //[EditorBrowsable(EditorBrowsableState.Never)]
    //public void OnSelectJs(int index)
    //{
    //    OnSelect?.Invoke(Items[index]);
    //}

    ///// <summary>
    ///// Event handler for deselecting an item, called from JavaScript.
    ///// </summary>
    ///// <param name="index">Index of the deselected item.</param>
    //[JSInvokable]
    //[EditorBrowsable(EditorBrowsableState.Never)]
    //public void OnDeselectJs(int index)
    //{
    //    OnDeselect?.Invoke(Items[index]);
    //}

    int ISortableList.DraggedItemIndex => draggedItemIndex;

    object? ISortableList.GetItem(int index)
    {
        var item = Items[index];

        return Pull == PullMode.Clone ? TryCloneItem(item) : item;
    }

    bool ISortableList.SuppressNextRemove
    {
        get => suppressNextRemove;
        set => suppressNextRemove = value;
    }

    private TItem? TryCloneItem(TItem item)
    {
        if (CloneFunction is null) return default;

        try
        {
            return CloneFunction(item);
        }
        catch (Exception ex)
        {
            OnCloneException?.Invoke(ex);
            return default;
        }
    }

    private TItem? TryConvertItem(object item, ISortable from)
    {
        try
        {
            var ctx = new SortableTransferContext<object>(item, from, this);

            return ConvertFunction!(ctx);
        }
        catch (Exception ex)
        {
            OnConvertException?.Invoke(ex);
            return default;
        }
    }

    private protected override ValueTask DisposeAsyncCore()
    {
        SortableService.UnregisterSortableList(Id);

        return ValueTask.CompletedTask;
    }
}
