using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using JetBrains.Annotations;
using ReactiveCollections.Abstract.Collections;
using ReactiveCollections.Abstract.Transactions;
using ReactiveCollections.Domain;
using ReactiveCollections.Extensions;
using ReactiveCollections.Implementation.Functions;
using ReactiveCollections.Implementation.Operations;
using ReactiveCollections.Implementation.Threading;
using ReactiveCollections.Implementation.Transactions;

namespace ReactiveCollections.Implementation
{
	public static class ObservableExtensions
	{
		[NotNull]
		public static IObservableReadOnlyCollection<TOut> SelectRc<TIn, TOut>(
			[NotNull] this IObservableReadOnlyCollection<TIn> source,
			[NotNull] Func<TIn, TOut> selector,
			[NotNull] Func<TIn, IObservable<TIn>> updaterSelector)
		{
			return new CollectionSelectOperation<TIn,TOut>(
				source.SourceWithInitialization(),
				selector,
				updaterSelector);
		}

		[NotNull]
		public static IObservableReadOnlyList<TOut> SelectRl<TIn, TOut>(
			[NotNull] this IObservableReadOnlyList<TIn> source,
			[NotNull] Func<TIn, TOut> selector,
			[NotNull] Func<TIn, IObservable<TIn>> updaterSelector)
		{
			return new ListSelectOperation<TIn, TOut>(
				source.SourceWithInitialization(),
				selector,
				updaterSelector);
		}

		[NotNull]
		public static IObservableReadOnlyCollection<T> WhereRc<T>(
			[NotNull] this IObservableReadOnlyCollection<T> source,
			[NotNull] Func<T, bool> filter,
			[NotNull] Func<T, IObservable<T>> observableExtractor)
		{
			return new CollectionWhereOperation<T>(
				source.SourceWithInitialization(),
				filter,
				observableExtractor);
		}

		[NotNull]
		public static IObservableReadOnlyList<T> WhereRl<T>(
			[NotNull] this IObservableReadOnlyList<T> source,
			[NotNull] Func<T, bool> filter,
			[NotNull] Func<T, IObservable<T>> observableExtractor)
		{
			return new ListWhereOperation<T>(
				source.SourceWithInitialization(),
				filter,
				observableExtractor);
		}

		[NotNull]
		public static IObservableReadOnlyCollection<TOut> SelectManyRc<TIn, TOut>(
			[NotNull] this IObservableReadOnlyCollection<TIn> source,
			[NotNull] Func<TIn, IObservableReadOnlyCollection<TOut>> selector)
		{
			return new CollectionSelectManyOperation<TIn,TOut>(
				source.SourceWithInitialization(),
				selector);
		}

		[NotNull]
		public static IObservableReadOnlyList<TValue> SortRc<TValue, TKey>(
			[NotNull] this IObservableReadOnlyCollection<TValue> source,
			[NotNull] Func<TValue, TKey> selector,
			[NotNull] IComparer<TKey> comparer,
			[NotNull] Func<TValue, IObservable<TValue>> keyUpdater)
		{
			return new CollectionSortOperation<TValue, TKey>(
				source.SourceWithInitialization(),
				selector,
				comparer,
				keyUpdater);
		}

		[NotNull]
		public static IObservableReadOnlyList<T> SkipAndTakeRl<T>(
			[NotNull] this IObservableReadOnlyList<T> source,
			[NotNull] ObservableValue<int> skip,
			[NotNull] ObservableValue<int> take)
		{
			return new ListSkipAndTakeOperation<T>(
				source.SourceWithInitialization(),
				skip,
				take);
		}

		[NotNull]
		public static IObservableLookup<TKey, TValue> GroupByRc<TKey, TValue>(
			[NotNull] this IObservableReadOnlyCollection<TValue> source,
			[NotNull] Func<TValue, TKey> keySelector,
			[NotNull] Func<TValue, IObservable<TValue>> keyUpdaterSelector)
		{
			return source.GroupByRc(
				keySelector,
				EqualityComparer<TKey>.Default,
				keyUpdaterSelector);
		}

		[NotNull]
		public static IObservableLookup<TKey, TValue> GroupByRc<TKey, TValue>(
			[NotNull] this IObservableReadOnlyCollection<TValue> source,
			[NotNull] Func<TValue, TKey> keySelector,
			[NotNull] IEqualityComparer<TKey> keyComparer,
			[NotNull] Func<TValue, IObservable<TValue>> keyUpdaterSelector)
		{
			return new CollectionGroupByOperation<TKey, TValue>(
				source.SourceWithInitialization(),
				keySelector,
				keyComparer,
				keyUpdaterSelector);
		}

		[NotNull]
		public static IObservableReadOnlyCollection<TValue> DistinctRc<TKey, TValue>(
			[NotNull] this IObservableReadOnlyCollection<TValue> source,
			[NotNull] Func<TValue, TKey> keySelector,
			[NotNull] Func<TValue, IObservable<TValue>> keyUpdaterSelector)
		{
			return source.DistinctRc(
				keySelector,
				EqualityComparer<TKey>.Default,
				keyUpdaterSelector);
		}

		[NotNull]
		public static IObservableReadOnlyCollection<TValue> DistinctRc<TKey, TValue>(
			[NotNull] this IObservableReadOnlyCollection<TValue> source,
			[NotNull] Func<TValue, TKey> keySelector,
			[NotNull] IEqualityComparer<TKey> keyComparer,
			[NotNull] Func<TValue, IObservable<TValue>> keyUpdaterSelector)
		{
			return new CollectionDistinctOperation<TValue,TKey>(
				source.SourceWithInitialization(),
				keySelector,
				keyComparer,
				keyUpdaterSelector);
		}

		[NotNull]
		public static IObservableReadOnlyCollection<TValue> UnionRc<TKey, TValue>(
			[NotNull] this IObservableReadOnlyCollection<TValue> first,
			[NotNull] IObservableReadOnlyCollection<TValue> second,
			[NotNull] Func<TValue, TKey> keySelector,
			[NotNull] Func<TValue, IObservable<TValue>> keyUpdaterSelector)
		{
			return first.UnionRc(
				second,
				keySelector,
				EqualityComparer<TKey>.Default,
				keyUpdaterSelector);
		}

		[NotNull]
		public static IObservableReadOnlyCollection<TValue> UnionRc<TKey, TValue>(
			[NotNull] this IObservableReadOnlyCollection<TValue> first,
			[NotNull] IObservableReadOnlyCollection<TValue> second,
			[NotNull] Func<TValue, TKey> keySelector,
			[NotNull] IEqualityComparer<TKey> keyComparer,
			[NotNull] Func<TValue, IObservable<TValue>> keyUpdaterSelector)
		{
			return new CollectionUnionOperation<TValue,TKey>(
				first,
				second,
				keySelector,
				keyComparer,
				keyUpdaterSelector);
		}

		[NotNull]
		public static IDisposable BindToList<T>(
			[NotNull] IObservableReadOnlyList<T> source,
			[NotNull] IList<T> target,
			[NotNull] IScheduler scheduler)
		{
			return new DispatcherToList<T>(
				source.SourceWithInitialization(),
				target,
				scheduler);
		}

		[NotNull]
		public static IDisposable BindToCollection<T>(
			[NotNull] IObservableReadOnlyCollection<T> source,
			[NotNull] ICollection<T> target,
			[NotNull] IScheduler scheduler)
		{
			return new DispatcherToCollection<T>(
				source.SourceWithInitialization(),
				target,
				scheduler);
		}

		[NotNull]
		public static IObservableValue<T> SomeItemOrDefault<T>(
			[NotNull] this IObservableReadOnlyCollection<T> source)
		{
			return new SomeItemOrDefaultFunction<T>(
				source.SourceWithInitialization());
		}

		[NotNull]
		public static IObservableValue<int> CountRc<T>(
			[NotNull] this IObservableReadOnlyCollection<T> source)
		{
			return new CollectionCountFunction<T>(
				source.SourceWithInitialization());
		}
	}
}