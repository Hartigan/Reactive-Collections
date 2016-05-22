using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using JetBrains.Annotations;
using ReactiveCollections.Abstract.Transactions;
using ReactiveCollections.Abstract.Transactions.Arguments;
using ReactiveCollections.Implementation.Transactions;

namespace ReactiveCollections.Implementation.Operations
{
	public class ListSelectOperation<TIn, TOut> : ListToListOperationBase<TIn, TOut>
	{
		private sealed class ItemContainer : IDisposable
		{
			[NotNull]
			private readonly IDisposable _subscription;

			public ItemContainer(
				TOut value,
				TIn key,
				Action<ItemContainer> onItemChanged,
				IObservable<TIn> itemChanged)
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

		[NotNull] private readonly Func<TIn, TOut> _selector;
		[NotNull] private readonly Func<TIn, IObservable<TIn>> _updaterSelector;
		[NotNull] private readonly List<ItemContainer> _data = new List<ItemContainer>(); 

		public ListSelectOperation(
			[NotNull] IObservable<IUpdateListQuery<TIn>> source,
			[NotNull] Func<TIn, TOut> selector,
			[NotNull] Func<TIn, IObservable<TIn>> updaterSelector)
		{
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
			var index = _data.IndexOf(itemContainer);

			RaiseListChanged(UpdateListQuery<TOut>.OnReplace(oldItem, newItem, index));
		}

		protected override IEnumerable<IUpdateListQuery<TOut>> OnInsert(IListOnInsertArgs<TIn> arg)
		{
			TOut newItem = _selector(arg.Item);
			_data.Insert(arg.Index, new ItemContainer(newItem, arg.Item, OnItemChanged, _updaterSelector(arg.Item)));
			return new[] { UpdateListQuery<TOut>.OnInsert(newItem, arg.Index) };
		}

		public override IEnumerator<TOut> GetEnumerator()
		{
			return _data.Select(x => x.Value).GetEnumerator();
		}

		public override int Count => _data.Count;

		public override TOut this[int index] => _data[index].Value;

		protected override IEnumerable<IUpdateListQuery<TOut>> OnEmpty(IListOnEmptyArgs<TIn> arg)
		{
			return Enumerable.Empty<IUpdateListQuery<TOut>>();
		}

		protected override IEnumerable<IUpdateListQuery<TOut>> OnReset(IListOnResetArgs<TIn> arg)
		{
			var oldItems = _data.Select(x => x.Value).ToList();

			foreach (var itemContainer in _data)
			{
				itemContainer.Dispose();
			}

			_data.Clear();

			for (int i = 0; i < arg.NewItems.Count; i++)
			{
				_data.Insert(i, new ItemContainer(_selector(arg.NewItems[i]), arg.NewItems[i], OnItemChanged, _updaterSelector(arg.NewItems[i])));
			}

			var newItems = _data.Select(x => x.Value).ToList();
			var newArgs = UpdateListQuery<TOut>.OnReset(oldItems, newItems);
			
			return new[] { newArgs };
		}

		protected override IEnumerable<IUpdateListQuery<TOut>> OnMove(IListOnMoveArgs<TIn> arg)
		{
			var item = _data[arg.OldIndex];
			_data.RemoveAt(arg.OldIndex);
			_data.Insert(arg.NewIndex, item);
			return new[] { UpdateListQuery<TOut>.OnMove(item.Value, arg.OldIndex, arg.NewIndex) };
		}

		protected override IEnumerable<IUpdateListQuery<TOut>> OnReplace(IListOnReplaceArgs<TIn> arg)
		{
			var newItem = _selector(arg.NewItem);
			var oldItem = _data[arg.Index];
			oldItem.Dispose();
			_data.RemoveAt(arg.Index);
			_data.Insert(arg.Index, new ItemContainer(newItem, arg.NewItem, OnItemChanged, _updaterSelector(arg.NewItem)));
			return new[] { UpdateListQuery<TOut>.OnReplace(oldItem.Value, newItem, arg.Index) };
		}

		protected override IEnumerable<IUpdateListQuery<TOut>> OnRemove(IListOnRemoveArgs<TIn> arg)
		{
			var item = _data[arg.Index];
			_data.RemoveAt(arg.Index);
			item.Dispose();
			return new[] { UpdateListQuery<TOut>.OnRemove(item.Value, arg.Index) };
		}
	}
}
