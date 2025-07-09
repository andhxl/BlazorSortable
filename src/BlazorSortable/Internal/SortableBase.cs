using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.ComponentModel;

namespace BlazorSortable.Internal;

// TODO: MultiDrag

/// <summary>
/// Base abstract class for Sortable components.
/// </summary>
public abstract class SortableBase : ComponentBase, IAsyncDisposable, IDisposable, ISortable
{
    /// <summary>
    /// CSS class applied to the root container of the Sortable component.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    /// <summary>
    /// Inline CSS styles applied to the root container of the Sortable component.
    /// </summary>
    [Parameter]
    public string? Style { get; set; }

    /// <summary>
    /// Specifies additional custom attributes that will be rendered by the component.
    /// </summary>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? Attributes { get; set; }

    /// <summary>
    /// Unique identifier of the component. Must be globally unique across all Sortable instances.
    /// </summary>
    /// <remarks>
    /// If not set explicitly, a GUID will be generated automatically.
    /// This ID is required for internal coordination between Sortable components.
    /// Set this manually only if you need to identify the component externally.
    /// </remarks>
    [Parameter]
    public virtual string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Group name for interaction with other Sortable components.
    /// </summary>
    [Parameter]
    public string Group { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Mode for adding items to this Sortable component.
    /// </summary>
    [Parameter]
    public PutMode? Put { get; set; }

    /// <summary>
    /// Array of group names from which items can be added.
    /// </summary>
    /// <remarks>
    /// Used only when <see cref="Put"/> is set to <see cref="PutMode.Groups"/>.
    /// Ignored in other put modes.
    /// </remarks>
    [Parameter]
    public string[]? PutGroups { get; set; }

    /// <summary>
    /// Custom function to determine whether an item can be added to this Sortable component.
    /// </summary>
    /// <remarks>
    /// Used only when <see cref="Put"/> is set to <see cref="PutMode.Function"/>.
    /// The function receives the item being dragged and the target list info as parameters.
    /// Should return <c>true</c> if the item can be added, <c>false</c> otherwise.
    /// </remarks>
    [Parameter]
    public Func<SortableTransferContext<object>, bool>? PutFunction { get; set; }

    /// <summary>
    /// Disables the Sortable component when set to true.
    /// When disabled, drag and drop operations are not allowed.
    /// </summary>
    [Parameter]
    public bool Disabled { get; set; }

    /// <summary>
    /// CSS class for the ghost element during dragging.
    /// </summary>
    [Parameter]
    public string GhostClass { get; set; } = "sortable-ghost";

    [Inject] private protected ISortableService SortableService { get; set; } = default!;
    [Inject] private protected IJSRuntime JS { get; set; } = default!;

    private protected IJSObjectReference? jsModule;
    private protected DotNetObjectReference<SortableBase>? selfReference;

    private protected abstract string InitMethodName { get; }

    /// <summary>
    /// Initializes the Sortable component after the first render.
    /// </summary>
    /// <param name="firstRender">Flag indicating whether this is the first render of the component.</param>
    protected sealed override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            jsModule = await JS.InvokeAsync<IJSObjectReference>("import",
                "./_content/BlazorSortable/js/blazor-sortable.js");
            selfReference = DotNetObjectReference.Create(this);
            await jsModule.InvokeVoidAsync(InitMethodName, Id, BuildOptions(), selfReference);

            await OnAfterFirstRenderAsync();
        }
    }

    private protected virtual Task OnAfterFirstRenderAsync() => Task.CompletedTask;

    private protected abstract Dictionary<string, object> BuildOptions();

    private protected object? GetPut()
    {
        return Put switch
        {
            PutMode.True => true,
            PutMode.False => false,
            PutMode.Groups => PutGroups?.Length > 0 ? PutGroups : null,
            PutMode.Function => "function",
            _ => null
        };
    }

    /// <summary>
    /// Called from JavaScript to determine if an item can be put into this sortable from the source sortable.
    /// </summary>
    /// <param name="fromId">The ID of the source sortable.</param>
    /// <returns><c>true</c> if the item can be placed; otherwise, <c>false</c>.</returns>
    [JSInvokable]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool OnPutJs(string fromId)
    {
        var from = SortableService.GetSortableList(fromId)!;
        var item = from.GetItem(from.DraggedItemIndex);

        // Skip processing if item is null (can happen in clone mode when cloning fails)
        if (item is null) return false;

        var ctx = new SortableTransferContext<object>(item, from, this);

        return PutFunction?.Invoke(ctx) ?? false;
    }

    /// <summary>
    /// Asynchronously disposes of resources.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (jsModule is not null)
        {
            await jsModule.InvokeVoidAsync("destroySortable", Id);
            await jsModule.DisposeAsync();
        }

        // Dispose selfReference after JavaScript module to prevent ObjectDisposedException
        // when JS tries to serialize already disposed DotNetObjectReference
        selfReference?.Dispose();

        await DisposeAsyncCore();
        Dispose();
    }

    private protected virtual ValueTask DisposeAsyncCore() => ValueTask.CompletedTask;

    /// <summary>
    /// Synchronously disposes of resources.
    /// </summary>
    public virtual void Dispose() { }
}
