﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using JetBrains.Annotations;
using ReactiveCollections.Abstract.Collections;
using ReactiveCollections.Abstract.Transactions;
using ReactiveCollections.Abstract.Transactions.Arguments;
using ReactiveCollections.Extensions;
using ReactiveCollections.Implementation.Transactions;

namespace ReactiveCollections.Implementation.Operations
{
	public class CollectionWhereOperation<T> : CollectionToCollectionOperationBase<T, T>
	{
		private sealed class Criteria : IDisposable
		{
			private readonly T _item;

			[NotNull]
			private readonly Func<T, bool> _condition;

			private readonly Action<Criteria> _onItemUpdated;

			[NotNull]
			private readonly IDisposable _sub;

			private bool _value;
			
			public Criteria(
				T item,
				Func<T, bool> condition,
				[NotNull] IObservable<T> criteriaChanged,
				[NotNull] Action<Criteria> onItemUpdated)
			{
				_item = item;
				_condition = condition;
				_onItemUpdated = onItemUpdated;

				_value = condition(item);
				_sub = criteriaChanged.WeakSubscribe(UpdateValue);
			}

			private void UpdateValue(T arg)
			{
				Value = _condition(_item);
			}

			public bool Value
			{
				get { return _value; }
				set
				{
					if (_value != value)
					{
						_value = value;
						_onItemUpdated(this);
					}
				}
			}

			public T Item => _item;

			public void Dispose()
			{
				_sub.Dispose();
			}
		}

		[NotNull] private readonly Collection<Criteria> _data = new Collection<Criteria>();
		[NotNull] private readonly Func<T, bool> _condition;
		[NotNull] private readonly Func<T, IObservable<T>> _getObservable;

		public CollectionWhereOperation(
			[NotNull] IObservable<IUpdateCollectionQuery<T>> source,
			[NotNull] Func<T, bool> condition,
			[NotNull] Func<T, IObservable<T>> getObservable)
		{
			_condition = condition;
			_getObservable = getObservable;

			Subscibe(source);
		}

		protected override IEnumerable<IUpdateCollectionQuery<T>> OnInsert(ICollectionOnInsertArgs<T> arg)
		{
			var criteria = new Criteria(
				item: arg.Item,
				condition: _condition,
				criteriaChanged: _getObservable(arg.Item),
				onItemUpdated: OnItemUpdated);

			_data.Add(criteria);

			if (criteria.Value)
			{
				return new[] {UpdateCollectionQuery<T>.OnInsert(criteria.Item)};
			}
			else
			{
				return Enumerable.Empty<IUpdateCollectionQuery<T>>();
			}
		}

		private void OnItemUpdated([NotNull] Criteria criteria)
		{
			IUpdateCollectionQuery<T> arg = criteria.Value
				? UpdateCollectionQuery<T>.OnInsert(criteria.Item)
				: UpdateCollectionQuery<T>.OnRemove(criteria.Item);

			RaiseCollectionChanged(arg);
		}

		public override IEnumerator<T> GetEnumerator()
		{
			return _data.Where(x => x.Value).Select(x => x.Item).GetEnumerator();
		}

		public override int Count => _data.Count(x => x.Value);

		protected override IEnumerable<IUpdateCollectionQuery<T>> OnEmpty(ICollectionOnEmptyArgs<T> arg)
		{
			return Enumerable.Empty<IUpdateCollectionQuery<T>>();
		}

		protected override IEnumerable<IUpdateCollectionQuery<T>> OnReset(ICollectionOnResetArgs<T> arg)
		{
			var oldItems = _data.Where(x => x.Value).Select(x => x.Item).ToList();

			foreach (var criteria in _data)
			{
				criteria.Dispose();
			}
			_data.Clear();

			foreach (var newItem in arg.NewItems)
			{
				var criteria = new Criteria(
					item: newItem,
					condition: _condition,
					criteriaChanged: _getObservable(newItem),
					onItemUpdated: OnItemUpdated);

				_data.Add(criteria);
			}

			var newItems = _data.Where(x => x.Value).Select(x => x.Item).ToList();

			var newArg = UpdateCollectionQuery<T>.OnReset(oldItems, newItems);

			return new[] {newArg};
		}

		protected override IEnumerable<IUpdateCollectionQuery<T>> OnReplace(ICollectionOnReplaceArgs<T> arg)
		{
			var srcItem = _data.Single(x => EqualityComparer<T>.Default.Equals(x.Item, arg.OldItem));
			var newItem = new Criteria(
				item: arg.NewItem,
				condition: _condition,
				criteriaChanged: _getObservable(arg.NewItem),
				onItemUpdated: OnItemUpdated);

			_data.Remove(srcItem);
			_data.Add(newItem);

			IUpdateCollectionQuery<T> newArg;
			if (srcItem.Value && newItem.Value)
			{
				newArg = arg;
			}
			else if (srcItem.Value && !newItem.Value)
			{
				newArg = UpdateCollectionQuery<T>.OnRemove(srcItem.Item);
			}
			else if (!srcItem.Value && newItem.Value)
			{
				newArg = UpdateCollectionQuery<T>.OnInsert(newItem.Item);
			}
			else
			{
				newArg = UpdateCollectionQuery<T>.OnEmpty();
			}

			srcItem.Dispose();

			return new[] {newArg};
		}

		protected override IEnumerable<IUpdateCollectionQuery<T>> OnRemove(ICollectionOnRemoveArgs<T> arg)
		{
			var srcItem = _data.Single(x => EqualityComparer<T>.Default.Equals(x.Item, arg.Item));
			_data.Remove(srcItem);

			IUpdateCollectionQuery<T> newArg;

			if (srcItem.Value)
			{
				newArg = UpdateCollectionQuery<T>.OnRemove(arg.Item);
			}
			else
			{
				newArg = UpdateCollectionQuery<T>.OnEmpty();
			}

			srcItem.Dispose();
			return new[] {newArg};
		}
	}
}