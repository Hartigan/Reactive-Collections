using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Disposables;
using ReactiveCollections.Abstract.Collections;
using ReactiveCollections.Abstract.Transactions;
using ReactiveCollections.Abstract.Transactions.Arguments;
using JetBrains.Annotations;
using ReactiveCollections.Extensions;

namespace ReactiveCollections.Implementation.Operations
{
	public class CollectionGroupByOperation<TKey, TValue> : CollectionToCollectionOperationBase<TValue, IObservableGrouping<TKey, TValue>>,
		IObservableLookup<TKey, TValue>
	{
		private sealed class ObservableCollectionWithKey : Collections.ObservableCollection<TValue>, IObservableGrouping<TKey, TValue>
		{
			public ObservableCollectionWithKey(TKey key)
			{
				Key = key;
			}

			public TKey Key { get; }
		}

		private sealed class ItemContainer : IDisposable
		{
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

			public override int GetHashCode()
			{
				return EqualityComparer<TValue>.Default.GetHashCode(Value);
			}

			[NotNull]
			private readonly IDisposable _subscription;

			public ItemContainer(TValue value)
			{
				Value = value;
				_subscription = Disposable.Empty;
			}

			public ItemContainer(
				TValue value,
				TKey key,
				ObservableCollectionWithKey collection,
				Action<ItemContainer> onItemChanged,
				IObservable<TValue> itemChanged) : this(value)
			{
				Key = key;
				Collection = collection;
				_subscription = itemChanged.Subscribe(_ => onItemChanged(this));
			}

			public TValue Value { get; }

			public TKey Key { get; set; }

			public ObservableCollectionWithKey Collection { get; set; }

			public void Dispose()
			{
				_subscription.Dispose();
			}
		}

		[NotNull] private readonly Func<TValue, TKey> _keySelector;
		[NotNull] private readonly Func<TValue, IObservable<TValue>> _keyUpdaterSelector;
		[NotNull] private readonly Collection<ItemContainer> _containers = new Collection<ItemContainer>();
		[NotNull] private Dictionary<TKey, ObservableCollectionWithKey> _map = new Dictionary<TKey, ObservableCollectionWithKey>();
		[NotNull] private IObservableCollection<IObservableGrouping<TKey, TValue>> _data = new Collections.ObservableCollection<IObservableGrouping<TKey, TValue>>();
		[NotNull] private readonly IDisposable _sub;

		public CollectionGroupByOperation(
			[NotNull] IObservable<IUpdateCollectionQuery<TValue>> source,
			[NotNull] Func<TValue, TKey> keySelector,
			[NotNull] Func<TValue, IObservable<TValue>> keyUpdaterSelector)
		{
			_keySelector = keySelector;
			_keyUpdaterSelector = keyUpdaterSelector;
			_sub = _data.CollectionChanged.WeakSubscribe(RaiseCollectionChanged);

			Subscibe(source);
		}

		private void OnItemChanged([NotNull] ItemContainer itemContainer)
		{
			var newKey = _keySelector(itemContainer.Value);
			if (EqualityComparer<TKey>.Default.Equals(newKey, itemContainer.Key)) return;

			var oldKey = itemContainer.Key;

			itemContainer.Collection.Remove(itemContainer.Value);
			if (itemContainer.Collection.Count == 0)
			{
				_map.Remove(oldKey);
				_data.Remove(itemContainer.Collection);
			}

			ObservableCollectionWithKey newCollection;
			if (!_map.TryGetValue(newKey, out newCollection))
			{
				newCollection = new ObservableCollectionWithKey(newKey);
				_map.Add(newKey, newCollection);
				_data.Add(newCollection);
			}

			newCollection.Add(itemContainer.Value);
			itemContainer.Key = newKey;
			itemContainer.Collection = newCollection;
		}

		public override IEnumerator<IObservableGrouping<TKey, TValue>> GetEnumerator() => _data.GetEnumerator();

		public override int Count => _data.Count;

		protected override IEnumerable<IUpdateCollectionQuery<IObservableGrouping<TKey, TValue>>> OnEmpty(ICollectionOnEmptyArgs<TValue> arg)
		{
			return Enumerable.Empty<IUpdateCollectionQuery<IObservableGrouping<TKey, TValue>>>();
		}

		protected override IEnumerable<IUpdateCollectionQuery<IObservableGrouping<TKey, TValue>>> OnReset(ICollectionOnResetArgs<TValue> arg)
		{
			_map.Clear();
			foreach (var itemContainer in _containers)
			{
				itemContainer.Dispose();
			}
			_containers.Clear();

			foreach (var newItem in arg.NewItems)
			{
				var newKey = _keySelector(newItem);
				var newKeyUpdater = _keyUpdaterSelector(newItem);
				ObservableCollectionWithKey newCollection;
				if (!_map.TryGetValue(newKey, out newCollection))
				{
					newCollection = new ObservableCollectionWithKey(newKey);
					_map.Add(newKey, newCollection);
				}
				newCollection.Add(newItem);

				var newContaiter = new ItemContainer(newItem, newKey, newCollection, OnItemChanged, newKeyUpdater);
				_containers.Add(newContaiter);
			}

			_data.Reset(_map.Values.ToList());
			return Enumerable.Empty<IUpdateCollectionQuery<IObservableGrouping<TKey, TValue>>>();
		}

		protected override IEnumerable<IUpdateCollectionQuery<IObservableGrouping<TKey, TValue>>> OnReplace(ICollectionOnReplaceArgs<TValue> arg)
		{
			var fakeContainer = new ItemContainer(arg.OldItem);
			var oldContainer = _containers.First(x => x.Equals(fakeContainer));
			_containers.Remove(oldContainer);

			var newKey = _keySelector(arg.NewItem);
			var newKeyUpdater = _keyUpdaterSelector(arg.NewItem);
			ObservableCollectionWithKey newCollection;
			if (!_map.TryGetValue(newKey, out newCollection))
			{
				newCollection = new ObservableCollectionWithKey(newKey);
				_map.Add(newKey, newCollection);
				_data.Add(newCollection);
			}

			var newContaiter = new ItemContainer(arg.NewItem, newKey, newCollection, OnItemChanged, newKeyUpdater);
			_containers.Add(newContaiter);

			
			if (EqualityComparer<TKey>.Default.Equals(newContaiter.Key, oldContainer.Key))
			{
				newCollection.Replace(arg.OldItem, arg.NewItem);
			}
			else
			{
				oldContainer.Collection.Remove(arg.OldItem);
				if (oldContainer.Collection.Count == 0)
				{
					_map.Remove(oldContainer.Key);
					_data.Remove(oldContainer.Collection);
				}
				newCollection.Add(arg.NewItem);
			}

			oldContainer.Dispose();
			return Enumerable.Empty<IUpdateCollectionQuery<IObservableGrouping<TKey, TValue>>>();
		}

		protected override IEnumerable<IUpdateCollectionQuery<IObservableGrouping<TKey, TValue>>> OnRemove(ICollectionOnRemoveArgs<TValue> arg)
		{
			var fakeContainer = new ItemContainer(arg.Item);
			var oldContainer = _containers.First(x => x.Equals(fakeContainer));
			_containers.Remove(oldContainer);

			oldContainer.Collection.Remove(arg.Item);
			if (oldContainer.Collection.Count == 0)
			{
				_map.Remove(oldContainer.Key);
				_data.Remove(oldContainer.Collection);
			}

			oldContainer.Dispose();
			return Enumerable.Empty<IUpdateCollectionQuery<IObservableGrouping<TKey, TValue>>>();
		}

		protected override IEnumerable<IUpdateCollectionQuery<IObservableGrouping<TKey, TValue>>> OnInsert(ICollectionOnInsertArgs<TValue> arg)
		{
			var newKey = _keySelector(arg.Item);
			var newKeyUpdater = _keyUpdaterSelector(arg.Item);
			ObservableCollectionWithKey newCollection;
			if (!_map.TryGetValue(newKey, out newCollection))
			{
				newCollection = new ObservableCollectionWithKey(newKey);
				_map.Add(newKey, newCollection);
				_data.Add(newCollection);
			}

			var newContaiter = new ItemContainer(arg.Item, newKey, newCollection, OnItemChanged, newKeyUpdater);
			_containers.Add(newContaiter);
			newCollection.Add(arg.Item);

			return Enumerable.Empty<IUpdateCollectionQuery<IObservableGrouping<TKey, TValue>>>();
		}

		public override void Dispose()
		{
			base.Dispose();
			_sub.Dispose();
		}

		public bool Contains(TKey key) => _map.ContainsKey(key);

		public IObservableReadOnlyCollection<TValue> this[TKey key] => _map[key];
	}
}