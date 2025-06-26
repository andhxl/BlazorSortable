# BlazorSortable

![BlazorSortable icon](https://raw.githubusercontent.com/andhxl/BlazorSortable/main/icon.png)

[![NuGet Version](https://img.shields.io/nuget/vpre/BlazorSortable?style=for-the-badge)](https://www.nuget.org/packages/BlazorSortable)
[![NuGet Downloads](https://img.shields.io/nuget/dt/BlazorSortable?style=for-the-badge)](https://www.nuget.org/packages/BlazorSortable)

A Blazor component that wraps the [SortableJS](https://github.com/SortableJS/Sortable) library, designed for creating interactive sortable lists with drag-and-drop support. Inspired by [BlazorSortable](https://github.com/the-urlist/BlazorSortable) and represents an improved and extended implementation.

## Installation

### Via dotnet CLI
```bash
dotnet add package BlazorSortable
```

### Via PackageReference
Add to your .csproj file:
```xml
<ItemGroup>
  <PackageReference Include="BlazorSortable" Version="3.*" />
</ItemGroup>
```

## Setup

1. Add the SortableJS library to:
    - `wwwroot/index.html` (for Blazor WebAssembly)
    - `Components/App.razor` (for Blazor Web App)
    - `Pages/_Host.cshtml` (for Blazor Server)

    Using one of the following methods:

    a) Via CDN:
    ```html
    <script src="https://cdn.jsdelivr.net/npm/sortablejs@latest/Sortable.min.js"></script>
    ```

    b) Locally:
    ```html
    <script src="lib/sortable/dist/js/Sortable.min.js"></script>
    ```

    For local installation:
    1. Download the latest version of SortableJS from [GitHub](https://github.com/SortableJS/Sortable/releases)
    2. Create the folder structure in `wwwroot`: `lib/sortable/dist/js/`
    3. Place the `Sortable.min.js` file in the created folder
    4. Ensure the path in the script tag matches the file location

2. (Optional) Add base styles to the same file where you added the script:
```html
<link rel="stylesheet" href="_content/BlazorSortable/css/blazor-sortable.css" />
```

3. Add services in `Program.cs`:
```csharp
using BlazorSortable;

// ...

builder.Services.AddSortableServices();
```

4. Add the using directive in `_Imports.razor`:
```razor
@using BlazorSortable
```

## Usage Examples

### SortableList

#### With a component
```razor
<SortableList Items="Persons"
              Class="my-sortable-list"
              Group="group1">
    <PersonComponent Person="context" />
</SortableList>

<SortableList TItem="Person"
              Items="Persons"
              Class="my-sortable-list"
              Group="group1"
              Context="person">
    <div class="person-card">
        <h4>@person.Name</h4>
        <p>@person.Email</p>
        <span class="badge">@person.Department</span>
    </div>
</SortableList>
```

### SortableDropZone

```razor
<SortableDropZone Class="my-sortable-drop-zone"
                  Group="group1"
                  OnDrop="OnDrop" />
```

## Events

Events use `Action<T>` instead of `EventCallback<T>`.
**Reason:** `EventCallback.InvokeAsync` automatically triggers `StateHasChanged()` in the parent component. For this component, this causes conflicts between DOM and data model.

All events receive a `SortableEventArgs<TItem>` parameter containing information about the operation.

### SortableEventArgs

The `SortableEventArgs<TItem>` class provides information about sorting operations:

| Property | Type | Description |
|----------|------|-------------|
| `TItem` | — | The type of the item |
| `Item` | `TItem` | The item participating in the operation |
| `From` | `ISortable` | Source sortable component |
| `OldIndex` | `int` | The previous index of the item in the source sortable |
| `To` | `ISortable` | Target sortable component |
| `NewIndex` | `int` | The new index of the item in the target sortable |
| `IsClone` | `bool` | Flag indicating whether the item is a clone |

### SortableTransferContext

The `SortableTransferContext<TItem>` class represents the context of transferring an item between sortable components:

| Property | Type | Description |
|----------|------|-------------|
| `TItem` | — | The type of the item being transferred |
| `Item` | `TItem` | The item being transferred between sortable components |
| `From` | `ISortable` | The source sortable component |
| `To` | `ISortable` | The target sortable component |

### ISortable

The `ISortable` interface provides information about a sortable component:

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `string` | Unique identifier of the component |
| `Group` | `string` | Group name for interaction with other Sortable components |

### Order & Notes

- **Order of events** when dragging between lists:
  1. `OnAdd` is triggered **first**.
  2. `OnRemove` is triggered **after**.
- During `OnAdd`, the item is **still present in the source list**.

## Component Parameters

> **Note:** MultiDrag plugin support will be added in future releases.

### SortableList

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `TItem` | — | — | The type of items in the sortable list |
| `Items` | `IList<TItem>` | **Required** | List of items to display and sort |
| `ChildContent` | `RenderFragment<TItem>` | `null` | Template for displaying each list item. Can be a component, HTML elements, or any Razor markup |
| `KeySelector` | `Func<TItem, object>` | `null` | Function for generating the key used in `@key`. If not set, the item itself is used |
| `Context` | `string` | `context` | Name of the parameter used in the child content template to refer to the current item |
| `Class` | `string` | `null` | CSS class for the container |
| `Style` | `string` | `null` | Inline CSS styles for the container |
| `Attributes` | `IReadOnlyDictionary<string, object>` | `null` | Additional custom attributes that will be rendered by the component |
| `Id` | `string` | `Random GUID` | Unique identifier of the component. Used internally for coordination between Sortable components. Must be globally unique |
| `Group` | `string` | `Random GUID` | Name of the group for interacting with other sortable instances |
| `Pull` | `PullMode` | `null` | Mode for pulling items from the list |
| `PullGroups` | `string[]` | `null` | **Required when `Pull="PullMode.Groups"`.** Specifies the groups into which items from this list can be dragged |
| `CloneFunction` | `Func<TItem, TItem>` | `null` | **Required when `Pull="PullMode.Clone"`.** A factory method used to create a clone of the dragged item |
| `OnCloneException` | `Action<Exception>` | `null` | Raised when an exception occurs during object cloning |
| `PullFunction` | `Func<SortableTransferContext<TItem>, bool>` | `null` | **Required when `Pull="PullMode.Function"`.** Function to determine if an item can be pulled to the target Sortable component |
| `Put` | `PutMode` | `null` | Mode for adding items to the list |
| `PutGroups` | `string[]` | `null` | **Required when `Put="PutMode.Groups"`.** Specifies the groups from which items are allowed to be added |
| `PutFunction` | `Func<SortableTransferContext<object>, bool>` | `null` | **Required when `Put="PutMode.Function"`.** Function to determine if an item can be put into this list. This function is invoked synchronously from JS using `invokeMethod` |
| `ConvertFunction` | `Func<SortableTransferContext<object>, TItem?>` | `null` | Function to convert items from other Sortable component to the target type |
| `OnConvertException` | `Action<Exception>` | `null` | Raised when an exception occurs during item conversion |
| `Sort` | `bool` | `true` | Enables or disables sorting of items within the list |
| `Disabled` | `bool` | `false` | Disables the Sortable component when set to true |
| `Animation` | `int` | `150` | Animation duration in milliseconds |
| `Handle` | `string` | `null` | CSS selector for elements that can be dragged (e.g. ".my-handle") |
| `Filter` | `string` | `null` | CSS selector for elements that cannot be dragged (e.g. ".ignore-elements") |
| `DraggableSelector` | `Func<TItem, bool>` | `null` | Function to determine if an item can be dragged |
| `DraggableClass` | `string` | `"sortable-draggable"` | CSS class applied to items that can be dragged |
| `GhostClass` | `string` | `"sortable-ghost"` | CSS class for the placeholder during drag |
| `ChosenClass` | `string` | `"sortable-chosen"` | CSS class for the chosen element |
| `DragClass` | `string` | `"sortable-drag"` | CSS class for the dragged element |
| `SwapThreshold` | `double` | `1` | Threshold for swap detection during dragging (0-1) |
| `ForceFallback` | `bool` | `true` | Forces fallback mode for dragging |
| `FallbackClass` | `string` | `"sortable-fallback"` | CSS class for the element in fallback mode |
| `Swap` | `bool` | `false` | Enables swap mode for dragging |
| `SwapClass` | `string` | `"sortable-swap-highlight"` | CSS class applied to items during swap highlighting |
| `Scroll` | `bool` | `true` | Enables scrolling of the container during dragging |
| `OnUpdate` | `Action<SortableEventArgs<TItem>>` | `null` | Raised when the order of items is changed |
| `OnAdd` | `Action<SortableEventArgs<TItem>>` | `null` | Raised when an item is added to the list |
| `OnRemove` | `Action<SortableEventArgs<TItem>>` | `null` | Raised when an item is removed from the list |

### SortableDropZone

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `Class` | `string` | `null` | CSS class for the container |
| `Style` | `string` | `null` | Inline CSS styles for the container |
| `Attributes` | `IReadOnlyDictionary<string, object>` | `null` | Additional custom attributes that will be rendered by the component |
| `Id` | `string` | `Random GUID` | Unique identifier of the component. Used internally for coordination between Sortable components. Must be globally unique |
| `Group` | `string` | `Random GUID` | Name of the group for interacting with other sortable instances |
| `Put` | `PutMode` | `null` | Mode for adding items to the zone |
| `PutGroups` | `string[]` | `null` | **Required when `Put="PutMode.Groups"`.** Specifies the groups from which items are allowed to be added |
| `PutFunction` | `Func<SortableTransferContext<object>, bool>` | `null` | **Required when `Put="PutMode.Function"`.** Function to determine if an item can be put into this zone. This function is invoked synchronously from JS using `invokeMethod` |
| `Disabled` | `bool` | `false` | Disables the Sortable component when set to true |
| `GhostClass` | `string` | `"sortable-ghost"` | CSS class for the placeholder during drag |
| `OnDrop` | `Action<SortableEventArgs<object>>` | `null` | Raised when an item is dropped in the zone |

### PullMode

| Value | Description |
|-------|-------------|
| `True` | Allows pulling items from the list |
| `False` | Prohibits pulling items from the list |
| `Groups` | Allows pulling items only from specified groups (requires `PullGroups` parameter) |
| `Clone` | Creates a clone of the item when dragging (requires `CloneFunction` parameter) |
| `Function` | Uses a custom function to determine if items can be pulled (requires `PullFunction` parameter) |

### PutMode

| Value | Description |
|-------|-------------|
| `True` | Allows adding items to the list |
| `False` | Prohibits adding items to the list |
| `Groups` | Allows adding items only from specified groups (requires `PutGroups` parameter) |
| `Function` | Uses a custom function to determine if items can be added (requires `PutFunction` parameter) |
