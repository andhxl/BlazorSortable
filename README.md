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
    <PackageReference Include="BlazorSortable" Version="1.*" />
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

### SortableList

```razor
<SortableList TItem="Person"
              Items="Persons"
              Class="my-sortable-list"
              Group="group1">
    <PersonComponent Person="context" />
</SortableList>
```

### SortableDropZone

```razor
<SortableDropZone Class="my-sortable-drop-zone"
                  Group="group1"
                  OnDrop="HandleDrop" />
```

### ConvertersBuilder

For convenient creation of converters dictionary, the `ConvertersBuilder` class is used. It allows defining transformations for different element types in a method chain:

```csharp
// Creating a converters dictionary
var converters = new ConvertersBuilder<Employee>()
    .Add<Person>("personList", p => new Employee { Name = p.FullName })
    .Add<Student>("studentList", s => new Employee { Name = s.Name });

// Using in component
<SortableList TItem="Employee"
              Items="Employees"
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
| `Items` | `IList<TItem>` | — | **Required.** List of items to display and sort |
| `Class` | `string` | `null` | CSS class for the container |
| `ChildContent` | `RenderFragment<TItem>` | — | Template for displaying each list item |
| `KeySelector` | `Func<TItem, object>` | `null` | Function for generating the key used in `@key`. If not set, the item itself is used |
| `OnAdd` | `EventCallback<(TItem item, string sourceSortableId, bool isClone)>` | — | Event when adding an item to the list |
| `OnUpdate` | `EventCallback<TItem>` | — | Event when updating the order of items |
| `OnRemove` | `EventCallback<TItem>` | — | Event when removing an item from the list |
| `OnException` | `EventCallback<Exception>` | — | Event for unhandled exceptions that occur inside the component during converter execution or object cloning |
| `Id` | `string` | `Guid.NewGuid().ToString()` | Unique identifier of the component. Used internally for coordination between Sortable components and for identifying lists in converters. Must be globally unique |
| `Group` | `string` | `Guid.NewGuid().ToString()` | Name of the group for interacting with other sortable instances |
| `Pull` | `PullMode?` | `null` | Mode for pulling items from the list |
| `PullGroups` | `string[]` | `null` | **Required when `Pull="PullMode.Groups"`.** Specifies the groups into which items from this list can be dragged |
| `CloneFactory` | `Func<TItem, TItem>` | `null` | **Required when `Pull="PullMode.Clone"`.** A factory method used to create a clone of the dragged item |
| `Put` | `PutMode?` | `null` | Mode for adding items to the list |
| `PutGroups` | `string[]` | `null` | **Required when `Put="PutMode.Groups"`.** Specifies the groups from which items are allowed to be added |
| `Converters` | `Dictionary<string, Func<object, TItem>>` | `null` | Dictionary of converters: the key is the `Id` of another `SortableList`, and the value is a function that converts an item from that list to `TItem` |
| `Sort` | `bool` | `true` | Enables or disables sorting of items within the list |
| `Animation` | `int` | `150` | Animation duration in milliseconds |
| `Handle` | `string` | `null` | CSS selector for elements that can be dragged (e.g. ".my-handle") |
| `Filter` | `string` | `null` | CSS selector for elements that cannot be dragged (e.g. ".ignore-elements") |
| `GhostClass` | `string` | `"sortable-ghost"` | CSS class for the placeholder during drag |
| `ChosenClass` | `string` | `"sortable-chosen"` | CSS class for the chosen element |
| `DragClass` | `string` | `"sortable-drag"` | CSS class for the dragged element |
| `ForceFallback` | `bool` | `true` | Forces fallback mode for dragging |
| `FallbackClass` | `string` | `"sortable-fallback"` | CSS class for the element in fallback mode |

### SortableDropZone

| Parameter | Type | Default Value | Description |
|----------|------|----------------------|-------------|
| `Class` | `string` | `null` | CSS class for the container |
| `OnDrop` | `EventCallback<object>` | — | Event when dropping an item in the zone |
| `Group` | `string` | `Guid.NewGuid().ToString()` | Name of the group for interacting with other sortable instances |
| `Put` | `PutMode?` | `null` | Mode for adding items to the zone |
| `PutGroups` | `string[]` | `null` | **Required when `Put="PutMode.Groups"`.** Specifies the groups from which items are allowed to be added |
| `GhostClass` | `string` | `"sortable-ghost"` | CSS class for the placeholder during drag |

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
