using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorSortable.Internal;

/// <summary>
/// Base abstract class for Sortable components.
/// </summary>
public abstract class SortableBase : ComponentBase, IAsyncDisposable
{
    /// <summary>
    /// Unique component identifier.
    /// </summary>
    [Parameter]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Group name for interaction with other lists.
    /// </summary>
    [Parameter]
    public string Group { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Mode for adding items to the list.
    /// </summary>
    [Parameter]
    public PutMode? Put { get; set; }

    /// <summary>
    /// Array of group names from which items can be added.
    /// </summary>
    [Parameter]
    public string[]? PutGroups { get; set; }

    /// <summary>
    /// CSS class for the ghost element during dragging.
    /// </summary>
    [Parameter]
    public string GhostClass { get; set; } = "sortable-ghost";

    /// <summary>
    /// CSS class for the list container.
    /// </summary>
    [Parameter]
    public string? Class { get; set; }

    [Inject] private protected ISortableService SortableService { get; set; } = default!;
    [Inject] private protected IJSRuntime JS { get; set; } = default!;

    private protected abstract string InitMethodName { get; }

    private protected DotNetObjectReference<SortableBase>? selfReference;
    private protected IJSObjectReference? jsModule;

    /// <summary>
    /// Initializes the Sortable component after the first render.
    /// </summary>
    /// <param name="firstRender">Flag indicating whether this is the first render of the component.</param>
    protected sealed override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender) return;

        selfReference = DotNetObjectReference.Create(this);

        jsModule = await JS.InvokeAsync<IJSObjectReference>("import",
            "./_content/BlazorSortable/js/blazor-sortable.js");

        var options = BuildOptions();

        await jsModule.InvokeVoidAsync(InitMethodName, Id, options, selfReference);

        await OnAfterFirstRenderAsync();
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
            _ => null
        };
    }

    /// <summary>
    /// Disposes of component resources, including JavaScript module and .NET object references.
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        selfReference?.Dispose();

        if (jsModule is not null)
        {
            await jsModule.InvokeVoidAsync("destroy", Id);
            await jsModule.DisposeAsync();
        }

        await DisposeAsyncCore();
    }

    private protected virtual ValueTask DisposeAsyncCore() => ValueTask.CompletedTask;
}
