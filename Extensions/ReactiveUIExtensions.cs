using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia.ReactiveUI;

namespace MTM_WIP_Application_Avalonia.Extensions;

/// <summary>
/// Extension methods for ReactiveUI collections and observables
/// </summary>
public static class ReactiveUIExtensions
{
    /// <summary>
    /// Creates an observable that fires when the collection changes
    /// </summary>
    /// <typeparam name="T">The type of items in the collection</typeparam>
    /// <param name="collection">The ObservableCollection to observe</param>
    /// <returns>An observable that fires on collection changes</returns>
    public static IObservable<Unit> ObserveCollectionChanges<T>(this ObservableCollection<T> collection)
    {
        return Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
            handler => collection.CollectionChanged += handler,
            handler => collection.CollectionChanged -= handler)
            .Select(_ => Unit.Default);
    }

    /// <summary>
    /// Creates an observable that provides the current count when the collection changes
    /// </summary>
    /// <typeparam name="T">The type of items in the collection</typeparam>
    /// <param name="collection">The ObservableCollection to observe</param>
    /// <returns>An observable that provides the count on collection changes</returns>
    public static IObservable<int> ObserveCollectionCount<T>(this ObservableCollection<T> collection)
    {
        return collection.ObserveCollectionChanges()
            .Select(_ => collection.Count)
            .StartWith(collection.Count);
    }

    /// <summary>
    /// Creates an observable that provides collection change details
    /// </summary>
    /// <typeparam name="T">The type of items in the collection</typeparam>
    /// <param name="collection">The ObservableCollection to observe</param>
    /// <returns>An observable that provides collection change event args</returns>
    public static IObservable<NotifyCollectionChangedEventArgs> ObserveCollectionChangedEventArgs<T>(this ObservableCollection<T> collection)
    {
        return Observable.FromEventPattern<NotifyCollectionChangedEventHandler, NotifyCollectionChangedEventArgs>(
            handler => collection.CollectionChanged += handler,
            handler => collection.CollectionChanged -= handler)
            .Select(eventPattern => eventPattern.EventArgs);
    }
}