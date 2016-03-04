using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using JetBrains.Annotations;
using ReactiveCollections.Abstract.Collections;
using ReactiveCollections.Abstract.Transactions;
using ReactiveCollections.Abstract.Transactions.Arguments;
using ReactiveCollections.Extensions;
using ReactiveCollections.Implementation.Collections;
using ReactiveCollections.Implementation.Transactions;

namespace ReactiveCollections.Implementation.Operations
{
	public class CollectionSelectManyOperation<TIn, TOut> : CollectionToCollectionOperationBase<TIn, TOut>
	{
		private class Data
		{
			public Data(
				[NotNull] IDisposable subscription,
				[NotNull] IObservableReadOnlyCollection<TOut> items)
			{
				Subscription = subscription;
				Items = items;
			}

			[NotNull]
			public IDisposable Subscription { get; }

			[NotNull]
			public IObservableReadOnlyCollection<TOut> Items { get; }
		}

		[NotNull]
		private readonly IObservableCollection<TOut> _data = new ObservableCollection<TOut>();

		[NotNull]
		private readonly Func<TIn, IObservableReadOnlyCollection<TOut>> _selector;

		[NotNull]
		private Dictionary<TIn, List<Data>> _map = new Dictionary<TIn, List<Data>>();

		public CollectionSelectManyOperation(
			[NotNull] IObservable<IUpdateCollectionQuery<TIn>> source,
			[NotNull] Func<TIn, IObservableReadOnlyCollection<TOut>> selector)
		{
			_selector = selector;
			_data.CollectionChanged.Subscribe(RaiseCollectionChanged);

			Subscibe(source);
		}

		public override IEnumerator<TOut> GetEnumerator() => _data.GetEnumerator();

		public override int Count => _data.Count;

		protected override IEnumerable<IUpdateCollectionQuery<TOut>> OnEmpty(ICollectionOnEmptyArgs arg)
		{
			return Enumerable.Empty<IUpdateCollectionQuery<TOut>>();
		}

		protected override IEnumerable<IUpdateCollectionQuery<TOut>> OnInsert(ICollectionOnInsertArgs<TIn> arg)
		{
			var newItem = _selector(arg.Item);
			var sub = CreateSubscribe(newItem);

			List<Data> list;
			if (!_map.TryGetValue(arg.Item, out list))
			{
				list = new List<Data>(1);
				_map.Add(arg.Item, list);
			}

			list.Add(new Data(sub, newItem));

			return Enumerable.Empty<IUpdateCollectionQuery<TOut>>();
		}

		protected override IEnumerable<IUpdateCollectionQuery<TOut>> OnReset(ICollectionOnResetArgs<TIn> arg)
		{
			foreach (var oldDataItem in _map.SelectMany(x => x.Value))
			{
				oldDataItem.Subscription.Dispose();
			}
			_map.Clear();

			foreach (var newItem in arg.NewItems)
			{
				List<Data> list;
				if (!_map.TryGetValue(newItem, out list))
				{
					list = new List<Data>(1);
					_map.Add(newItem, list);
				}

				var newData = _selector(newItem);
				var sub = CreateSubscribe(newData);

				list.Add(new Data(sub, newData));
			}

			_data.Reset(_map.SelectMany(x => x.Value).SelectMany(x => x.Items).ToList());

			return Enumerable.Empty<IUpdateCollectionQuery<TOut>>();
		}

		protected override IEnumerable<IUpdateCollectionQuery<TOut>> OnReplace(ICollectionOnReplaceArgs<TIn> arg)
		{
			var list = _map[arg.OldItem];

			var oldData = list.First();
			list.Remove(oldData);
			if (!list.Any())
			{
				_map.Remove(arg.OldItem);
			}

			oldData.Subscription.Dispose();

			foreach (var item in oldData.Items)
			{
				_data.Remove(item);
			}

			var newItem = _selector(arg.NewItem);
			var sub = CreateSubscribe(newItem);

			List<Data> newList;
			if (!_map.TryGetValue(arg.NewItem, out newList))
			{
				newList = new List<Data>(1);
				_map.Add(arg.NewItem, newList);
			}
			newList.Add(new Data(sub, newItem));

			return Enumerable.Empty<IUpdateCollectionQuery<TOut>>();
		}

		protected override IEnumerable<IUpdateCollectionQuery<TOut>> OnRemove(ICollectionOnRemoveArgs<TIn> arg)
		{
			var list = _map[arg.Item];
			var oldData = list.First();
			list.Remove(oldData);
			if (!list.Any())
			{
				_map.Remove(arg.Item);
			}

			oldData.Subscription.Dispose();

			foreach (var item in oldData.Items)
			{
				_data.Remove(item);
			}

			return Enumerable.Empty<IUpdateCollectionQuery<TOut>>();
		}

		private void OnDataUpdated([NotNull] IUpdateCollectionQuery<TOut> arg)
		{
			arg.Match(
				onInsert: x => { _data.Add(x.Item); },
				onRemove: x => { _data.Remove(x.Item); },
				onReplace: x => { _data.Replace(x.OldItem, x.NewItem); },
				onReset: x =>
				{
					foreach (var oldItem in x.OldItems)
					{
						_data.Remove(oldItem);
					}

					foreach (var newItem in x.NewItems)
					{
						_data.Add(newItem);
					}
				},
				onEmpty: x => { });
		}

		[NotNull]
		private IDisposable CreateSubscribe([NotNull] IObservableReadOnlyCollection<TOut> collection)
		{
			return collection.CollectionChanged
					.StartWith(UpdateCollectionQuery<TOut>.OnReset(Array.Empty<TOut>(), collection.ToList()))
					.WeakSubscribe(OnDataUpdated);
		}
	}
}