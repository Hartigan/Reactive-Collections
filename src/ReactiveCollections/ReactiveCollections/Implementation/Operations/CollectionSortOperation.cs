using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Runtime.InteropServices;
using JetBrains.Annotations;
using ReactiveCollections.Abstract.Transactions;
using ReactiveCollections.Abstract.Transactions.Arguments;
using ReactiveCollections.Implementation.Operations;
using ReactiveCollections.Implementation.Transactions;

namespace ReactiveCollections.Implementation.Operations
{
	public sealed class CollectionSortOperation<TValue, TKey> : CollectionToListOperationBase<TValue, TValue>
	{
		private sealed class ItemContainer : IDisposable
		{
			[NotNull]
			private readonly IDisposable _subscription;

			public ItemContainer(TValue value, TKey key)
			{
				Value = value;
				Key = key;
				_subscription = Disposable.Empty;
			}

			public ItemContainer(
				TValue value,
				TKey key,
				Action<ItemContainer> onItemChanged,
				IObservable<TValue> itemChanged) : this(value, key)
			{
				_subscription = itemChanged.Subscribe(_ => onItemChanged(this));
			}

			public TValue Value { get; }

			public TKey Key { get; }

			public override int GetHashCode()
			{
				return EqualityComparer<TValue>.Default.GetHashCode(Value);
			}

			private bool Equals(ItemContainer other)
			{
				return EqualityComparer<TValue>.Default.Equals(Value, other.Value);
			}

			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj)) return false;
				if (ReferenceEquals(this, obj)) return true;
				return obj is ItemContainer && Equals((ItemContainer) obj);
			}

			public void Dispose()
			{
				_subscription.Dispose();
			}
		}

		[NotNull] private readonly Func<TValue, TKey> _selector;
		[NotNull] private readonly IComparer<TKey> _comparer;
		[NotNull] private readonly IComparer<ItemContainer> _containerComparer;
		[NotNull] private readonly Func<TValue, IObservable<TValue>> _keyUpdater;
		[NotNull] private readonly List<ItemContainer> _list;

		public CollectionSortOperation(
			[NotNull] IObservable<IUpdateCollectionQuery<TValue>> source,
			[NotNull] Func<TValue, TKey> selector,
			[NotNull] IComparer<TKey> comparer,
			[NotNull] Func<TValue, IObservable<TValue>> keyUpdater)
		{
			_selector = selector;
			_comparer = comparer;
			_keyUpdater = keyUpdater;
			_list = new List<ItemContainer>();
			_containerComparer = Comparer<ItemContainer>.Create((x, y) => comparer.Compare(x.Key, y.Key));

			Subscibe(source);
		}

		private int AddItem(TValue value)
		{
			TKey key = _selector(value);
			var item = new ItemContainer(value, key, OnItemChanged, _keyUpdater(value));
			var index = _list.BinarySearch(item, _containerComparer);
			if (index < 0) index = ~index;
			_list.Insert(index, item);
			return index;
		}

		private int RemoveItem(TValue value)
		{
			TKey key = _selector(value);
			var item = new ItemContainer(value, key);
			var index = _list.IndexOf(item);
			_list[index].Dispose();
			_list.RemoveAt(index);
			return index;
		}

		private void OnItemChanged(ItemContainer itemContainer)
		{
			var newKey = _selector(itemContainer.Value);

			if (_comparer.Compare(itemContainer.Key, newKey) == 0) return;

			var oldIndex = RemoveItem(itemContainer.Value);
			var newIndex = AddItem(itemContainer.Value);

			if (oldIndex != newIndex)
			{
				RaiseListChanged(UpdateListQuery<TValue>.OnMove(itemContainer.Value, oldIndex, newIndex));
			}
		}


		protected override IEnumerable<IUpdateListQuery<TValue>> OnInsert(ICollectionOnInsertArgs<TValue> arg)
		{
			var index = AddItem(arg.Item);
			return new[] { UpdateListQuery<TValue>.OnInsert(arg.Item, index) };
		}

		public override IEnumerator<TValue> GetEnumerator() => _list.Select(x => x.Value).GetEnumerator();

		public override int Count => _list.Count;

		public override TValue this[int index] => _list[index].Value;

		protected override IEnumerable<IUpdateListQuery<TValue>> OnEmpty(ICollectionOnEmptyArgs<TValue> arg)
		{
			return Enumerable.Empty<IUpdateListQuery<TValue>>();
		}

		protected override IEnumerable<IUpdateListQuery<TValue>> OnReset(ICollectionOnResetArgs<TValue> arg)
		{
			var oldItems = new TValue[_list.Count];

			for (int i = 0; i < _list.Count; i++)
			{
				oldItems[i] = _list[i].Value;
				_list[i].Dispose();
			}

			_list.Clear();

			foreach (TValue newItem in arg.NewItems)
			{
				AddItem(newItem);
			}

			var newItems = new TValue[_list.Count];

			for (int i = 0; i < _list.Count; i++)
			{
				newItems[i] = _list[i].Value;
			}

			return new[] { UpdateListQuery<TValue>.OnReset(oldItems, newItems)};
		}

		protected override IEnumerable<IUpdateListQuery<TValue>> OnReplace(ICollectionOnReplaceArgs<TValue> arg)
		{
			var oldItemIndex = RemoveItem(arg.OldItem);
			var newItemIndex = AddItem(arg.NewItem);

			if (oldItemIndex != newItemIndex)
			{
				return new[]
				{
					UpdateListQuery<TValue>.OnRemove(arg.OldItem, oldItemIndex),
					UpdateListQuery<TValue>.OnInsert(arg.NewItem, newItemIndex)
				};
			}
			else
			{
				return new[] { UpdateListQuery<TValue>.OnReplace(arg.OldItem, arg.NewItem, newItemIndex)};
			}
		}

		protected override IEnumerable<IUpdateListQuery<TValue>> OnRemove(ICollectionOnRemoveArgs<TValue> arg)
		{
			var index = RemoveItem(arg.Item);
			return new[] { UpdateListQuery<TValue>.OnRemove(arg.Item, index) };
		}
	}
}