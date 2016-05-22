using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using JetBrains.Annotations;
using ReactiveCollections.Abstract.Transactions;
using ReactiveCollections.Abstract.Transactions.Arguments;
using ReactiveCollections.Implementation.Functions;

namespace ReactiveCollections.Implementation.Threading
{
	public sealed class DispatcherToList<T> : ListFunctionBase<T>
	{
		[NotNull]
		private readonly IList<T> _target;

		[NotNull]
		private readonly IScheduler _sheduler;

		public DispatcherToList(
			[NotNull] IObservable<IUpdateListQuery<T>> source,
			[NotNull] IList<T> target,
			[NotNull] IScheduler sheduler)
		{
			_target = target;
			_sheduler = sheduler;
			Subscibe(source);
		}

		protected override void OnEmpty(IListOnEmptyArgs<T> arg)
		{
		}

		protected override void OnReset(IListOnResetArgs<T> arg)
		{
			_sheduler.Schedule(() =>
			{
				_target.Clear();

				foreach (var newItem in arg.NewItems)
				{
					_target.Add(newItem);
				}
			});
		}

		protected override void OnMove(IListOnMoveArgs<T> arg)
		{
			_sheduler.Schedule(() =>
			{
				_target.RemoveAt(arg.OldIndex);
				_target.Insert(arg.NewIndex, arg.Item);
			});
		}

		protected override void OnReplace(IListOnReplaceArgs<T> arg)
		{
			_sheduler.Schedule(() =>
			{
				_target[arg.Index] = arg.NewItem;
			});
		}

		protected override void OnRemove(IListOnRemoveArgs<T> arg)
		{
			_sheduler.Schedule(() =>
			{
				_target.RemoveAt(arg.Index);
			});
		}

		protected override void OnInsert(IListOnInsertArgs<T> arg)
		{
			_sheduler.Schedule(() =>
			{
				_target.Insert(arg.Index, arg.Item);
			});
		}
	}
}