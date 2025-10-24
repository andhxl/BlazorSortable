namespace BlazorSortable.Internal;

internal static class SortableAssemblyInfo
{
    public static string VersionQuery { get; } = GetVersionQuery();

    private static string GetVersionQuery()
    {
        var v = typeof(Sortable<>).Assembly.GetName().Version;
        return v is null ? "" : $"?v={v}";
    }
}
