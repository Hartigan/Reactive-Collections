using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using JetBrains.Annotations;
using ReactiveCollections.Abstract.Transactions;
using ReactiveCollections.Abstract.Transactions.Arguments;
using ReactiveCollections.Extensions;

namespace ReactiveCollections.Implementation.Threading
{
	public class DispatcherToCollection<T> : IDisposable
	{
		[NotNull]
		private readonly ICollection<T> _target;

		[NotNull]
		private readonly IScheduler _sheduler;

		[NotNull]
		private readonly IDisposable _sub;

		public DispatcherToCollection(
			[NotNull] IObservable<IUpdateCollectionQuery<T>> source,
			[NotNull] ICollection<T> target,
			[NotNull] IScheduler sheduler)
		{
			_target = target;
			_sheduler = sheduler;
			_sub = source.WeakSubscribe(ProcessQuery);
		}

		private void ProcessQuery([NotNull] IUpdateCollectionQuery<T> query)
		{
			_sheduler.Schedule(() =>
			{
				query.Match(
					onInsert: OnInsert,
					onRemove: OnRemove,
					onReplace: OnReplace,
					onReset: OnReset,
					onEmpty: OnEmpty);
			});
		}

		private void OnEmpty([NotNull] ICollectionOnEmptyArgs arg)
		{
		}

		private void OnReset([NotNull] ICollectionOnResetArgs<T> arg)
		{
			_target.Clear();

			foreach (var newItem in arg.NewItems)
			{
				_target.Add(newItem);
			}
		}

		private void OnReplace([NotNull] ICollectionOnReplaceArgs<T> arg)
		{
			_target.Remove(arg.OldItem);
			_target.Add(arg.NewItem);
		}

		private void OnRemove([NotNull] ICollectionOnRemoveArgs<T> arg)
		{
			_target.Remove(arg.Item);
		}

		private void OnInsert([NotNull] ICollectionOnInsertArgs<T> arg)
		{
			_target.Add(arg.Item);
		}

		public void Dispose()
		{
			_sub.Dispose();
		}
	}
}