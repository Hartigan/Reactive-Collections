using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using JetBrains.Annotations;
using ReactiveCollections.Abstract.Collections;
using ReactiveCollections.Implementation.Transactions;

namespace ReactiveCollections.Implementation.Operations
{
	public static class ObservableExtensions
	{
		[NotNull]
		public static IObservableReadOnlyCollection<TOut> SelectRc<TIn, TOut>(
			[NotNull] this IObservableReadOnlyCollection<TIn> source,
			[NotNull] Func<TIn, TOut> selector)
		{
			return new CollectionSelectOperation<TIn,TOut>(
				source.CollectionChanged.StartWith(UpdateCollectionQuery<TIn>.OnReset(
					oldItems: Array.Empty<TIn>(),
					newItems: source.ToList())),
				selector);
		}

		[NotNull]
		public static IObservableReadOnlyList<TOut> SelectRl<TIn, TOut>(
			[NotNull] this IObservableReadOnlyList<TIn> source,
			[NotNull] Func<TIn, TOut> selector)
		{
			return new ListSelectOperation<TIn, TOut>(
				source.ListChanged.StartWith(UpdateListQuery<TIn>.OnReset(
					oldItems: Array.Empty<TIn>(),
					newItems: source.ToList())),
				selector);
		}

		[NotNull]
		public static IObservableReadOnlyCollection<T> WhereRc<T>(
			[NotNull] this IObservableReadOnlyCollection<T> source,
			[NotNull] Func<T, bool> filter,
			[NotNull] Func<T, IObservable<Unit>> observableExtractor)
		{
			return new CollectionWhereOperation<T>(
				source.CollectionChanged.StartWith(UpdateCollectionQuery<T>.OnReset(
					oldItems: Array.Empty<T>(),
					newItems: source.ToList())),
				filter,
				observableExtractor);
		}

		[NotNull]
		public static IObservableReadOnlyList<T> WhereRl<T>(
			[NotNull] this IObservableReadOnlyList<T> source,
			[NotNull] Func<T, bool> filter,
			[NotNull] Func<T, IObservable<Unit>> observableExtractor)
		{
			return new ListWhereOperation<T>(
				source.ListChanged.StartWith(UpdateListQuery<T>.OnReset(
					oldItems: Array.Empty<T>(),
					newItems: source.ToList())),
				filter,
				observableExtractor);
		}

		[NotNull]
		public static IObservableReadOnlyCollection<TOut> SelectManyRc<TIn, TOut>(
			[NotNull] this IObservableReadOnlyCollection<TIn> source,
			[NotNull] Func<TIn, IObservableReadOnlyCollection<TOut>> selector)
		{
			return new CollectionSelectManyOperation<TIn,TOut>(
				source.CollectionChanged.StartWith(UpdateCollectionQuery<TIn>.OnReset(
					oldItems: Array.Empty<TIn>(),
					newItems: source.ToList())),
				selector);
		}
	}
}