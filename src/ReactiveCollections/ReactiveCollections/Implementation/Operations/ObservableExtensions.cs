using System;
using JetBrains.Annotations;
using ReactiveCollections.Abstract.Collections;

namespace ReactiveCollections.Implementation.Operations
{
	public static class ObservableExtensions
	{
		[NotNull]
		public static IObservableReadOnlyCollection<TOut> SelectRc<TIn, TOut>(
			[NotNull] this IObservableReadOnlyCollection<TIn> source,
			[NotNull] Func<TIn, TOut> selector)
		{
			return new CollectionSelectOperation<TIn,TOut>(source, selector);
		}

		[NotNull]
		public static IObservableReadOnlyList<TOut> SelectRl<TIn, TOut>(
			[NotNull] this IObservableReadOnlyList<TIn> source,
			[NotNull] Func<TIn, TOut> selector)
		{
			return new ListSelectOperation<TIn,TOut>(source, selector);
		}
	}
}