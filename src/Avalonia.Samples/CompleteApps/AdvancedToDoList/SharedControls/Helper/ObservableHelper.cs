using System.ComponentModel;
using System.Reactive.Linq;

namespace SharedControls.Helper;

internal static class ObservableHelper
{
    /// <summary>
    /// AOT-friendly observable sequence for a single property value of an <see cref="INotifyPropertyChanged"/> source.
    /// Avoids expression trees and dynamic code used by ReactiveUI.WhenAnyValue.
    /// </summary>
    /// <typeparam name="T">The property type.</typeparam>
    /// <param name="source">The source object implementing INotifyPropertyChanged.</param>
    /// <param name="propertyName">The property name. Use nameof(...).</param>
    /// <param name="getter">A getter function returning the current value.</param>
    /// <param name="emitInitial">Emit the current value immediately when subscribed.</param>
    public static IObservable<T> ObserveValue<T>(this INotifyPropertyChanged source,
        string propertyName,
        Func<T> getter,
        bool emitInitial = true)
    {
        var changed = Observable
            .FromEventPattern<PropertyChangedEventHandler, PropertyChangedEventArgs>(
                h => source.PropertyChanged += h,
                h => source.PropertyChanged -= h)
            .Where(e => string.IsNullOrEmpty(e.EventArgs.PropertyName) || e.EventArgs.PropertyName == propertyName)
            .Select(_ => getter());

        return emitInitial ? changed.StartWith(getter()) : changed;
    }
}
