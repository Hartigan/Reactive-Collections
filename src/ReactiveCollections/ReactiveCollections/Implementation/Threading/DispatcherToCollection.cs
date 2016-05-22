using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using JetBrains.Annotations;
using ReactiveCollections.Abstract.Transactions;
using ReactiveCollections.Abstract.Transactions.Arguments;
using ReactiveCollections.Implementation.Functions;

namespace ReactiveCollections.Implementation.Threading
{
	public sealed class DispatcherToCollection<T> : CollectionFunctionBase<T>
	{
		[NotNull]
		private readonly ICollection<T> _target;

		[NotNull]
		private readonly IScheduler _sheduler;

		public DispatcherToCollection(
			[NotNull] IObservable<IUpdateCollectionQuery<T>> source,
			[NotNull] ICollection<T> target,
			[NotNull] IScheduler sheduler)
		{
			_target = target;
			_sheduler = sheduler;
			Subscibe(source);
		}

		protected override void OnEmpty(ICollectionOnEmptyArgs<T> arg)
		{
		}

		protected override void OnReset(ICollectionOnResetArgs<T> arg)
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

		protected override void OnReplace(ICollectionOnReplaceArgs<T> arg)
		{
			_sheduler.Schedule(() =>
			{
				_target.Remove(arg.OldItem);
				_target.Add(arg.NewItem);
			});
		}

		protected override void OnRemove(ICollectionOnRemoveArgs<T> arg)
		{
			_sheduler.Schedule(() =>
			{
				_target.Remove(arg.Item);
			});
		}

		protected override void OnInsert(ICollectionOnInsertArgs<T> arg)
		{
			_sheduler.Schedule(() =>
			{
				_target.Add(arg.Item);
			});
		}
	}
}