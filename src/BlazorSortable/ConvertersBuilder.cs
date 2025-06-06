namespace BlazorSortable;

/// <summary>
/// Builder for creating a converter dictionary.
/// </summary>
/// <typeparam name="TTarget">Target type for all converters.</typeparam>
/// <example>
/// <code>
/// Converters = new ConvertersBuilder()
///     .Add{Person}("personList", p => new Employee { Name = p.FullName })
///     .Add{Student}("studentList", s => new Employee { Name = s.Name });
/// </code>
/// </example>
public class ConvertersBuilder<TTarget>
{
    private readonly Dictionary<string, Func<object, TTarget>> _converters = [];

    /// <summary>
    /// Adds a converter from TSource to TTarget.
    /// </summary>
    /// <typeparam name="TSource">Source type for conversion.</typeparam>
    /// <param name="key">Unique identifier for the converter (usually the source component's Id).</param>
    /// <param name="converter">Function that converts from TSource to TTarget.</param>
    /// <returns>Builder instance for method chaining.</returns>
    public ConvertersBuilder<TTarget> Add<TSource>(string key, Func<TSource, TTarget> converter)
    {
        _converters[key] = obj => converter((TSource)obj);
        return this;
    }

    /// <summary>
    /// Creates a converter dictionary.
    /// </summary>
    /// <returns>Dictionary ready to be assigned to the Sortable.Converters property.</returns>
    public Dictionary<string, Func<object, TTarget>> Build() => _converters;

    /// <summary>
    /// Implicit conversion to Dictionary for convenience.
    /// </summary>
    public static implicit operator Dictionary<string, Func<object, TTarget>>(ConvertersBuilder<TTarget> builder) =>
        builder.Build();
}
