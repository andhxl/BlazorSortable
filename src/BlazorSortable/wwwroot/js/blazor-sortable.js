export function init(id, options, component) {
    const el = document.getElementById(id);

    el._sortable = new Sortable(el, {
        ...options,
        onAdd: (event) => {
            component.invokeMethodAsync('OnAddJs', event.from.id, event.pullMode === 'clone', event.oldIndex, event.newIndex);
        },
        onUpdate: (event) => {
            // Revert the DOM to match the .NET state
            event.item.remove();
            event.from.insertBefore(event.item, event.from.children[event.oldIndex]);

            // Notify .NET to update its model and re-render
            component.invokeMethodAsync('OnUpdateJs', event.oldIndex, event.newIndex);
        },
        onRemove: (event) => {
            event.item.remove();
            event.from.insertBefore(event.item, event.from.children[event.oldIndex]);

            if (event.pullMode === 'clone') {
                event.clone.remove();
            }
            else {
                component.invokeMethodAsync('OnRemoveJs', event.oldIndex);
            }
        }
    });
}

export function initDropZone(id, options, component) {
    const el = document.getElementById(id);

    el._sortable = new Sortable(el, {
        ...options,
        onAdd: (event) => {
            component.invokeMethodAsync('OnAddJs', event.from.id, event.pullMode === 'clone', event.oldIndex);
        }
    });
}

export function destroy(id) {
    const el = document.getElementById(id);

    el._sortable.destroy();
    delete el._sortable;
}
