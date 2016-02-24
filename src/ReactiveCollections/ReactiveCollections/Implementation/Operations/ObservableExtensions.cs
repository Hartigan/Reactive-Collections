using System;
using System.Linq;
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
	}
}