using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using ReactiveCollections.Abstract.Collections;
using ReactiveCollections.Abstract.Transactions;
using ReactiveCollections.Abstract.Transactions.Arguments;
using ReactiveCollections.Extensions;
using ReactiveCollections.Implementation.Transactions;

namespace ReactiveCollections.Implementation.Operations
{
	public sealed class CollectionSelectOperation<TIn, TOut> : CollectionToCollectionOperationBase<TIn, TOut>
	{
		[NotNull]
		private readonly Func<TIn, TOut> _selector;

		[NotNull]
		private Dictionary<TIn, List<TOut>> _map;

		private int _count;

		public CollectionSelectOperation(
			[NotNull] INotifyCollectionChanged<TIn> source,
			[NotNull] Func<TIn, TOut> selector) : base(source)
		{
			selector.ArgumentNotNull(nameof(selector));

			_selector = selector;
			_map = new Dictionary<TIn, List<TOut>>();
		}

		protected override IEnumerable<IUpdateCollectionQuery<TOut>> OnInsert(ICollectionOnInsertArgs<TIn> arg)
		{
			TOut newItem = _selector(arg.Item);
			IUpdateCollectionQuery<TOut> newArg = UpdateCollectionQuery<TOut>.OnInsert(newItem);

			List<TOut> list;
			if (!_map.TryGetValue(arg.Item, out list))
			{
				list = new List<TOut>(1);
				_map.Add(arg.Item, list);
			}

			list.Add(newItem);
			_count++;

			return new[] { newArg };
		}

		public override IEnumerator<TOut> GetEnumerator()
		{
			return _map.Values.SelectMany(list => list).GetEnumerator();
		}

		public override int Count => _count;

		protected override IEnumerable<IUpdateCollectionQuery<TOut>> OnEmpty(ICollectionOnEmptyArgs arg)
		{
			return Enumerable.Empty<IUpdateCollectionQuery<TOut>>();
		}

		protected override IEnumerable<IUpdateCollectionQuery<TOut>> OnClear(ICollectionOnClearArgs<TIn> arg)
		{
			IUpdateCollectionQuery<TOut> newArg = UpdateCollectionQuery<TOut>.OnClear(_map.Values.SelectMany(list => list).ToList());
			_map.Clear();
			_count = 0;
			return new [] { newArg };
		}

		protected override IEnumerable<IUpdateCollectionQuery<TOut>> OnReplace(ICollectionOnReplaceArgs<TIn> arg)
		{
			var newItem = _selector(arg.NewItem);
			var list = _map[arg.OldItem];

			var oldItem = list.First();
			list.Remove(oldItem);
			if (!list.Any())
			{
				_map.Remove(arg.OldItem);
			}

			List<TOut> newList;
			if (!_map.TryGetValue(arg.NewItem, out newList))
			{
				newList = new List<TOut>(1);
				_map.Add(arg.NewItem, newList);
			}
			newList.Add(newItem);

			var newArg = UpdateCollectionQuery<TOut>.OnReplace(oldItem, newItem);
			return new[] { newArg };
		}

		protected override IEnumerable<IUpdateCollectionQuery<TOut>> OnRemove(ICollectionOnRemoveArgs<TIn> arg)
		{
			var list = _map[arg.Item];
			var item = list.First();
			list.Remove(item);
			if (!list.Any())
			{
				_map.Remove(arg.Item);
			}

			var newArg = UpdateCollectionQuery<TOut>.OnRemove(item);
			return new[] { newArg };
		}
	}
}