using System.ComponentModel;
using System.Reactive.Linq;

namespace SharedControls.Helper;

/// <summary>
/// Helper class for creating observable sequences from INotifyPropertyChanged objects.
/// Provides AOT (Ahead-Of-Time) compilation friendly alternatives to ReactiveUI's WhenAnyValue.
/// This avoids using reflection and expression trees, making it work better with code trimming.
/// </summary>
/// <remarks>
/// Why use this instead of ReactiveUI's WhenAnyValue?
/// - More compatible with AOT compilation and code trimming
/// - No reflection overhead, better performance
/// - Simpler to debug and understand
/// - Works across all platforms without trimming issues
/// </remarks>
public static class ObservableHelper
{
    /// <summary>
    /// Creates an observable sequence that emits values whenever a specific property changes.
    /// This is a safer, AOT-friendly alternative to ReactiveUI's WhenAnyValue method.
    /// Perfect for watching changes to properties and reacting to them in real-time.
    /// </summary>
    /// <typeparam name="T">The type of the property value being observed</typeparam>
    /// <param name="source">The object that implements INotifyPropertyChanged (like ViewModels)</param>
    /// <param name="propertyName">
    /// The name of the property to watch. Always use nameof() for type safety.
    /// Example: nameof(MyViewModel.MyProperty)
    /// </param>
    /// <param name="getter">
    /// A function that returns the current value of the property.
    /// Example: () => myViewModel.MyProperty
    /// </param>
    /// <param name="emitInitial">
    /// If true, emits the current property value immediately when someone subscribes.
    /// If false, only emits values after the property changes.
    /// Default is true for convenience.
    /// </param>
    /// <returns>An observable sequence that emits property values as they change</returns>
    /// <example>
    /// <code>
    /// // Watch for changes to a filter string and react to them
    /// viewModel.ObserveValue(nameof(FilterString), () => FilterString)
    ///     .Throttle(TimeSpan.FromMilliseconds(300))
    ///     .Subscribe(newValue => DoSomething(newValue));
    /// </code>
    /// </example>
    public static IObservable<T> ObserveValue<T>(this INotifyPropertyChanged source,
        string propertyName,
        Func<T> getter,
        bool emitInitial = true)
    {
        // Create an observable from the PropertyChanged event
        var changed = Observable
            .FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                h => source.PropertyChanged += h,
                h => source.PropertyChanged -= h)
            // Only notify when the specific property changes (or when all properties change)
            .Where(e => string.IsNullOrEmpty(e.EventArgs.PropertyName) || e.EventArgs.PropertyName == propertyName)
            // Get the new value each time the property changes
            .Select(_ => getter());

        // Start with the current value if requested, otherwise wait for first change
        return emitInitial ? changed.StartWith(getter()) : changed;
    }
}
