using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using JetBrains.Annotations;
using ReactiveCollections.Abstract.Collections;
using ReactiveCollections.Abstract.Transactions;
using ReactiveCollections.Implementation.Transactions;

namespace ReactiveCollections.Extensions
{
	internal static class InternalExtensions
	{
		[AssertionMethod]
		public static void ArgumentNotNull([AssertionCondition(AssertionConditionType.IS_NOT_NULL)] this object obj, string name)
		{
			if (obj == null)
			{
				throw new ArgumentNullException(name);
			}
		}

		[NotNull]
		public static IObservable<T> ToKeepAliveObservable<T>([NotNull] this IObservable<T> source, [NotNull] object target)
		{
			source.ArgumentNotNull(nameof(source));
			target.ArgumentNotNull(nameof(target));

			IObservable<T> result = Observable.Create<T>(observer =>
			{
				IDisposable subscription = source.Subscribe(observer);

				return Disposable.Create(() =>
				{
					subscription.Dispose();
					GC.KeepAlive(target);
				});
			});
			return result;
		}

		[NotNull]
		public static IObservable<IUpdateListQuery<T>> SourceWithInitialization<T>(
			[NotNull] this IObservableReadOnlyList<T> source)
		{
			return source.ListChanged.StartWith(UpdateListQuery<T>.OnReset(
				oldItems: Array.Empty<T>(),
				newItems: source.ToList()));
		}

		[NotNull]
		public static IObservable<IUpdateCollectionQuery<T>> SourceWithInitialization<T>(
			[NotNull] this IObservableReadOnlyCollection<T> source)
		{
			return source.CollectionChanged.StartWith(UpdateListQuery<T>.OnReset(
				oldItems: Array.Empty<T>(),
				newItems: source.ToList()));
		}
	}
}