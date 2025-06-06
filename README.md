# BlazorSortable

![BlazorSortable icon](icon.png)

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
    <PackageReference Include="BlazorSortable" Version="1.0.0" />
</ItemGroup>
```

## Setup

1. Add the SortableJS library to `index.html` (for Blazor WebAssembly) or `_host.cshtml` (for Blazor Server) using one of the following methods:

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

2. (Optional) Add base styles to `index.html` or `_host.cshtml`:
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

### Sortable List

```razor
<SortableList TItem="Item"
              Items="@Items"
              Group="group1"
              Class="sortable-list">
    <ItemComponent Item="context" />
</SortableList>
```

### Drop Zone

```razor
<SortableDropZone Group="group1"
                  Class="drop-zone"
                  OnDrop="HandleDrop" />
```

### Using Converters

For convenient creation of converter dictionaries, the `ConvertersBuilder` class is used. It allows defining transformations for different element types in a method chain:

```csharp
// Creating a converter dictionary
var converters = new ConvertersBuilder<Employee>()
    .Add<Person>("personList", p => new Employee { Name = p.FullName })
    .Add<Student>("studentList", s => new Employee { Name = s.Name });

// Using in component
<SortableList TItem="Employee"
              Items="@Employees"
              Converters="converters"
              Group="group1">
    <ItemComponent Item="context" />
</SortableList>
```

## Component Parameters

### SortableList

| Parameter | Type | Default Value | Description |
|----------|------|----------------------|-------------|
| `TItem` | *(type parameter)* | — | The type of item in the list |
| `Items` | `IList<TItem>` | — | **Required parameter.** List of items to display and sort |
| `ChildContent` | `RenderFragment<TItem>` | — | Template for displaying each list item |
| `OnAdd` | `EventCallback<(TItem item, string sourceSortableId, bool isClone)>` | — | Event when adding an item to the list |
| `OnUpdate` | `EventCallback<TItem>` | — | Event when updating the order of items |
| `OnRemove` | `EventCallback<TItem>` | — | Event when removing an item from the list |
| `OnError` | `EventCallback<Exception>` | — | Event for unhandled errors inside the component when calling converter or CloneFactory |
| `Id` | `string` | `Guid.NewGuid().ToString()` | Unique component identifier |
| `Group` | `string` | `Guid.NewGuid().ToString()` | Group name for interaction with other lists |
| `Pull` | `PullMode?` | `null` | Mode for pulling items from the list |
| `PullGroups` | `string[]` | `null` | Groups from which items can be pulled |
| `Put` | `PutMode?` | `null` | Mode for adding items to the list |
| `PutGroups` | `string[]` | `null` | Array of group names from which items can be added |
| `Sort` | `bool` | `true` | Enables or disables sorting of items within the list |
| `Animation` | `int` | `150` | Animation duration in milliseconds |
| `Handle` | `string` | `null` | CSS selector for elements that can be used for dragging |
| `Filter` | `string` | `null` | CSS selector for elements that cannot be dragged |
| `GhostClass` | `string` | `"sortable-ghost"` | CSS class for the ghost element during dragging |
| `ChosenClass` | `string` | `"sortable-chosen"` | CSS class for the chosen element |
| `DragClass` | `string` | `"sortable-drag"` | CSS class for the dragged element |
| `ForceFallback` | `bool` | `true` | Forces fallback mode for dragging |
| `FallbackClass` | `string` | `"sortable-fallback"` | CSS class for the element in fallback mode |
| `Class` | `string` | `null` | CSS class for the list container |
| `CloneFactory` | `Func<TItem, TItem>` | `null` | Factory for creating item clones |
| `Converters` | `Dictionary<string, Func<object, TItem>>` | `null` | Dictionary of converters for transforming items from other lists |
| `KeySelector` | `Func<TItem, object>` | `null` | Function for selecting a key for an item |

### SortableDropZone

| Parameter | Type | Default Value | Description |
|----------|------|----------------------|-------------|
| `OnDrop` | `EventCallback<object>` | — | Event when dropping an item in the zone |
| `Id` | `string` | `Guid.NewGuid().ToString()` | Unique component identifier |
| `Group` | `string` | `Guid.NewGuid().ToString()` | Group name for interaction with other lists |
| `Put` | `PutMode?` | `null` | Mode for adding items to the zone |
| `PutGroups` | `string[]` | `null` | Array of group names from which items can be added |
| `GhostClass` | `string` | `"sortable-ghost"` | CSS class for the ghost element during dragging |
| `Class` | `string` | `null` | CSS class for the drop zone container |

### PullMode

| Value | Description |
|----------|-----------|
| `True` | Allows pulling items from the list |
| `False` | Prohibits pulling items from the list |
| `Groups` | Allows pulling items only from specified groups (requires `PullGroups` parameter) |
| `Clone` | Creates a clone of the item when dragging (requires `CloneFactory` parameter) |

### PutMode

| Value | Description |
|----------|-----------|
| `True` | Allows adding items to the list |
| `False` | Prohibits adding items to the list |
| `Groups` | Allows adding items only from specified groups (requires `PutGroups` parameter) |
