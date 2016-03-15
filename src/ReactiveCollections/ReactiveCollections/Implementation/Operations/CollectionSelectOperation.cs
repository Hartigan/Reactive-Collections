using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using JetBrains.Annotations;
using ReactiveCollections.Abstract.Transactions;
using ReactiveCollections.Abstract.Transactions.Arguments;
using ReactiveCollections.Extensions;
using ReactiveCollections.Implementation.Transactions;

namespace ReactiveCollections.Implementation.Operations
{
	public sealed class CollectionSelectOperation<TIn, TOut> : CollectionToCollectionOperationBase<TIn, TOut>
	{
		private sealed class ItemContainer : IDisposable
		{
			[NotNull]
			private readonly IDisposable _subscription;

			public ItemContainer(
				TOut value,
				TIn key,
				Action<ItemContainer> onItemChanged,
				IObservable<Unit> itemChanged)
			{
				Value = value;
				Key = key;
				_subscription = itemChanged.Subscribe(_ => onItemChanged(this));
			}

			public TOut Value { get; set; }

			public TIn Key { get; }

			public void Dispose()
			{
				_subscription.Dispose();
			}
		}

		[NotNull]
		private readonly Func<TIn, TOut> _selector;

		[NotNull]
		private readonly Func<TIn, IObservable<Unit>> _updaterSelector;

		[NotNull]
		private Dictionary<TIn, LinkedList<ItemContainer>> _map = new Dictionary<TIn, LinkedList<ItemContainer>>();

		private int _count;

		public CollectionSelectOperation(
			[NotNull] IObservable<IUpdateCollectionQuery<TIn>> source,
			[NotNull] Func<TIn, TOut> selector,
			[NotNull] Func<TIn, IObservable<Unit>> updaterSelector)
		{
			selector.ArgumentNotNull(nameof(selector));
			_selector = selector;
			_updaterSelector = updaterSelector;

			Subscibe(source);
		}

		private void OnItemChanged([NotNull] ItemContainer itemContainer)
		{
			var oldItem = itemContainer.Value;
			var newItem = _selector(itemContainer.Key);

			if (EqualityComparer<TOut>.Default.Equals(oldItem, newItem)) return;

			itemContainer.Value = newItem;
			RaiseCollectionChanged(UpdateCollectionQuery<TOut>.OnReplace(oldItem, newItem));
		}

		protected override IEnumerable<IUpdateCollectionQuery<TOut>> OnInsert(ICollectionOnInsertArgs<TIn> arg)
		{
			TOut newItem = _selector(arg.Item);
			IUpdateCollectionQuery<TOut> newArg = UpdateCollectionQuery<TOut>.OnInsert(newItem);

			LinkedList<ItemContainer> list;
			if (!_map.TryGetValue(arg.Item, out list))
			{
				list = new LinkedList<ItemContainer>();
				_map.Add(arg.Item, list);
			}

			list.AddLast(new ItemContainer(newItem, arg.Item, OnItemChanged, _updaterSelector(arg.Item)));
			_count++;

			return new[] { newArg };
		}

		public override IEnumerator<TOut> GetEnumerator()
		{
			return _map.Values.SelectMany(list => list).Select(x => x.Value).GetEnumerator();
		}

		public override int Count => _count;

		protected override IEnumerable<IUpdateCollectionQuery<TOut>> OnEmpty(ICollectionOnEmptyArgs arg)
		{
			return Enumerable.Empty<IUpdateCollectionQuery<TOut>>();
		}

		protected override IEnumerable<IUpdateCollectionQuery<TOut>> OnReset(ICollectionOnResetArgs<TIn> arg)
		{
			var oldItems = _map.Values.SelectMany(x => x).Select(x => x.Value).ToList();

			foreach (var itemContainer in _map.Values.SelectMany(x => x))
			{
				itemContainer.Dispose();
			}

			_map.Clear();
			_count = 0;

			foreach (var newItem in arg.NewItems)
			{
				LinkedList<ItemContainer> list;
				if (!_map.TryGetValue(newItem, out list))
				{
					list = new LinkedList<ItemContainer>();
					_map.Add(newItem, list);
				}

				list.AddLast(new ItemContainer(_selector(newItem), newItem, OnItemChanged, _updaterSelector(newItem)));
				_count++;
			}

			var newItems = _map.Values.SelectMany(x => x).Select(x => x.Value).ToList();

			IUpdateCollectionQuery<TOut> newArg = UpdateCollectionQuery<TOut>.OnReset(oldItems, newItems);

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
			oldItem.Dispose();

			LinkedList<ItemContainer> newList;
			if (!_map.TryGetValue(arg.NewItem, out newList))
			{
				newList = new LinkedList<ItemContainer>();
				_map.Add(arg.NewItem, newList);
			}
			newList.AddLast(new ItemContainer(newItem, arg.NewItem, OnItemChanged, _updaterSelector(arg.NewItem)));

			var newArg = UpdateCollectionQuery<TOut>.OnReplace(oldItem.Value, newItem);
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
			item.Dispose();

			var newArg = UpdateCollectionQuery<TOut>.OnRemove(item.Value);
			return new[] { newArg };
		}
	}
}