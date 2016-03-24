using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using JetBrains.Annotations;
using ReactiveCollections.Abstract.Transactions;
using ReactiveCollections.Abstract.Transactions.Arguments;
using ReactiveCollections.Extensions;

namespace ReactiveCollections.Implementation.Threading
{
	public sealed class DispatcherToList<T> : IDisposable
	{
		[NotNull]
		private readonly IList<T> _target;

		[NotNull]
		private readonly IScheduler _sheduler;

		[NotNull]
		private readonly IDisposable _sub;

		public DispatcherToList(
			[NotNull] IObservable<IUpdateListQuery<T>> source,
			[NotNull] IList<T> target,
			[NotNull] IScheduler sheduler)
		{
			_target = target;
			_sheduler = sheduler;
			_sub = source.WeakSubscribe(ProcessQuery);
		}

		private void ProcessQuery([NotNull] IUpdateListQuery<T> query)
		{
			_sheduler.Schedule(() =>
			{
				query.Match(
					onInsert: OnInsert,
					onRemove: OnRemove,
					onReplace: OnReplace,
					onMove: OnMove,
					onReset: OnReset,
					onEmpty: OnEmpty);
			});
		}

		private void OnEmpty([NotNull] IListOnEmptyArgs arg)
		{
		}

		private void OnReset([NotNull] IListOnResetArgs<T> arg)
		{
			_target.Clear();

			foreach (var newItem in arg.NewItems)
			{
				_target.Add(newItem);
			}
		}

		private void OnMove([NotNull] IListOnMoveArgs<T> arg)
		{
			_target.RemoveAt(arg.OldIndex);
			_target.Insert(arg.NewIndex, arg.Item);
		}

		private void OnReplace([NotNull] IListOnReplaceArgs<T> arg)
		{
			_target[arg.Index] = arg.NewItem;
		}

		private void OnRemove([NotNull] IListOnRemoveArgs<T> arg)
		{
			_target.RemoveAt(arg.Index);
		}

		private void OnInsert([NotNull] IListOnInsertArgs<T> arg)
		{
			_target.Insert(arg.Index, arg.Item);
		}

		public void Dispose()
		{
			_sub.Dispose();
		}
	}
}