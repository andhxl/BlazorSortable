export function initSortable(id, options, component) {
    const el = document.getElementById(id);
    if (!el) return;

    if (options.group.pull === "function") {
        options.group.pull = function (to, from, dragEl, evt) {
            return component.invokeMethod('OnPullJs', to.el.id);
        };
    }
    if (options.group.put === "function") {
        options.group.put = function (to, from, dragEl, evt) {
            return component.invokeMethod('OnPutJs', from.el.id);
        };
    }

    // TODO: MultiDrag
    el._sortable = new Sortable(el, {
        ...options,
        onStart: (evt) => {
            component.invokeMethodAsync('OnStartJs', evt.oldIndex);
        },
        onEnd: (evt) => {
            component.invokeMethodAsync('OnEndJs');
        },
        onUpdate: (evt) => {
            //const oldIndexes = extractIndexes(evt.oldIndicies);
            //const newIndexes = extractIndexes(evt.newIndicies);

            // Revert the DOM to match the .NET state
            evt.item.remove();
            evt.from.insertBefore(evt.item, evt.from.children[evt.oldIndex]);

            // Notify .NET to update its model and re-render
            component.invokeMethodAsync('OnUpdateJs', evt.oldIndex, evt.newIndex);
        },
        onAdd: (evt) => {
            //const oldIndexes = extractIndexes(evt.oldIndicies);
            //const newIndexes = extractIndexes(evt.newIndicies);

            component.invokeMethodAsync('OnAddJs', evt.from.id, evt.oldIndex, evt.newIndex, evt.pullMode === 'clone');
        },
        onRemove: (evt) => {
            //const oldIndexes = extractIndexes(evt.oldIndicies);
            //const newIndexes = extractIndexes(evt.newIndicies);

            evt.item.remove();
            evt.from.insertBefore(evt.item, evt.from.children[evt.oldIndex]);

            if (evt.pullMode === 'clone') {
                evt.clone?.remove(); // Handle MultiDrag null case
            }
            else {
                component.invokeMethodAsync('OnRemoveJs', evt.oldIndex, evt.to.id, evt.newIndex);
            }
        },
        //onSelect: (evt) => {
        //    component.invokeMethodAsync('OnSelectJs', evt.oldIndex);
        //},
        //onDeselect: (evt) => {
        //    component.invokeMethodAsync('OnDeselectJs', evt.oldIndex);
        //}
    });
}

export function destroySortable(id) {
    const el = document.getElementById(id);
    if (!el) return;

    if (el._sortable) {
        el._sortable.destroy();
        delete el._sortable;
    }
}

//function extractIndexes(indicies) {
//    return indicies.map(i => i.index);
//}
