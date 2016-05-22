﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using JetBrains.Annotations;
using ReactiveCollections.Abstract.Collections;
using ReactiveCollections.Abstract.Transactions;
using ReactiveCollections.Domain;
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
				SourceWithInitialization(source),
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
				SourceWithInitialization(source),
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
				SourceWithInitialization(source),
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
				SourceWithInitialization(source),
				filter,
				observableExtractor);
		}

		[NotNull]
		public static IObservableReadOnlyCollection<TOut> SelectManyRc<TIn, TOut>(
			[NotNull] this IObservableReadOnlyCollection<TIn> source,
			[NotNull] Func<TIn, IObservableReadOnlyCollection<TOut>> selector)
		{
			return new CollectionSelectManyOperation<TIn,TOut>(
				SourceWithInitialization(source),
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
				SourceWithInitialization(source),
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
				SourceWithInitialization(source),
				skip,
				take);
		}

		[NotNull]
		public static IObservableLookup<TKey, TValue> GroupByRc<TKey, TValue>(
			[NotNull] this IObservableReadOnlyCollection<TValue> source,
			[NotNull] Func<TValue, TKey> keySelector,
			[NotNull] Func<TValue, IObservable<TValue>> keyUpdaterSelector)
		{
			return new CollectionGroupByOperation<TKey, TValue>(
				SourceWithInitialization(source),
				keySelector,
				keyUpdaterSelector);
		}

		[NotNull]
		private static IObservable<IUpdateListQuery<T>> SourceWithInitialization<T>(
			[NotNull] IObservableReadOnlyList<T> source)
		{
			return source.ListChanged.StartWith(UpdateListQuery<T>.OnReset(
				oldItems: Array.Empty<T>(),
				newItems: source.ToList()));
		}

		[NotNull]
		private static IObservable<IUpdateCollectionQuery<T>> SourceWithInitialization<T>(
			[NotNull] IObservableReadOnlyCollection<T> source)
		{
			return source.CollectionChanged.StartWith(UpdateListQuery<T>.OnReset(
				oldItems: Array.Empty<T>(),
				newItems: source.ToList()));
		}

		[NotNull]
		private static IDisposable BindToList<T>(
			[NotNull] IObservableReadOnlyList<T> source,
			[NotNull] IList<T> target,
			[NotNull] IScheduler scheduler)
		{
			return new DispatcherToList<T>(
				SourceWithInitialization(source),
				target,
				scheduler);
		}

		[NotNull]
		private static IDisposable BindToCollection<T>(
			[NotNull] IObservableReadOnlyCollection<T> source,
			[NotNull] ICollection<T> target,
			[NotNull] IScheduler scheduler)
		{
			return new DispatcherToCollection<T>(
				SourceWithInitialization(source),
				target,
				scheduler);
		}
	}
}