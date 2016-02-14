using System;
using System.Collections.Generic;
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
	public class ListWhereOperation<T> : ListToListOperationBase<T, T>
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
				[NotNull] IObservable<Unit> criteriaChanged,
				[NotNull] Action<Criteria> onItemUpdated)
			{
				_item = item;
				_condition = condition;
				_onItemUpdated = onItemUpdated;

				_value = condition(item);
				_sub = criteriaChanged.WeakSubscribe(UpdateValue);
			}

			private void UpdateValue(Unit arg)
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

			public int ActualIndex { get; set; }
		}

		private readonly List<Criteria> _data;
		private readonly Func<T, bool> _condition;
		private readonly Func<T, IObservable<Unit>> _getObservable;

		public ListWhereOperation(
			[NotNull] INotifyListChanged<T> source,
			[NotNull] Func<T, bool> condition,
			[NotNull] Func<T, IObservable<Unit>> getObservable) : base(source)
		{
			_condition = condition;
			_getObservable = getObservable;
			_data = new List<Criteria>();
		}

		protected override IEnumerable<IUpdateListQuery<T>> OnInsert(IListOnInsertArgs<T> arg)
		{
			var newItem = new Criteria(arg.Item, _condition, _getObservable(arg.Item), OnItemUpdated);
			_data.Insert(arg.Index, newItem);

			newItem.ActualIndex = arg.Index == 0 ? -1 : _data[arg.Index - 1].ActualIndex;

			IUpdateListQuery<T> newArg;

			if (newItem.Value)
			{
				AddOneFromIndex(arg.Index);
				newArg = UpdateListQuery<T>.OnInsert(newItem.Item, newItem.ActualIndex);
			}
			else
			{
				newArg = UpdateListQuery<T>.OnEmpty();
			}

			return new[] {newArg};
		}

		private void AddOneFromIndex(int index)
		{
			for (int i = index; i < _data.Count; i++)
			{
				_data[i].ActualIndex++;
			}
		}

		private void SubOneFromIndex(int index)
		{
			for (int i = index; i < _data.Count; i++)
			{
				_data[i].ActualIndex--;
			}
		}

		private void OnItemUpdated([NotNull] Criteria criteria)
		{
			var sourceIndex = _data.IndexOf(criteria);
			IUpdateListQuery<T> arg;
			if (criteria.Value)
			{
				AddOneFromIndex(sourceIndex);
				arg = UpdateListQuery<T>.OnInsert(criteria.Item, criteria.ActualIndex);
			}
			else
			{
				arg = UpdateListQuery<T>.OnRemove(criteria.Item, criteria.ActualIndex);
				SubOneFromIndex(sourceIndex);
			}

			RaiseListChanged(arg);
		}

		public override IEnumerator<T> GetEnumerator()
		{
			return _data.Where(x => x.Value).Select(x => x.Item).GetEnumerator();
		}

		public override int Count => _data.Count == 0 ? 0 : (_data[_data.Count - 1].ActualIndex + 1);

		public override T this[int index]
		{
			get
			{
				if (index >= Count || index < 0)
				{
					throw new ArgumentOutOfRangeException(nameof(index));
				}

				int left = 0;
				int right = _data.Count - 1;

				while (right > left)
				{
					var candidateIndex = (left + right)/2;
					var item = _data[candidateIndex];

					if (item.ActualIndex > index)
					{
						right = candidateIndex - 1;
					}
					else if (item.ActualIndex < index)
					{
						left = candidateIndex + 1;
					}
					else if (!item.Value)
					{
						right = candidateIndex - 1;
					}
					else
					{
						return _data[candidateIndex].Item;
					}
				}

				throw new InvalidOperationException(nameof(index));
			}
		}

		protected override IEnumerable<IUpdateListQuery<T>> OnEmpty(IListOnEmptyArgs arg)
		{
			return Enumerable.Empty<IUpdateListQuery<T>>();
		}

		protected override IEnumerable<IUpdateListQuery<T>> OnClear(IListOnClearArgs<T> arg)
		{
			var newArg = UpdateListQuery<T>.OnClear(_data.Where(x => x.Value).Select(x => x.Item).ToList());

			foreach (var criteria in _data)
			{
				criteria.Dispose();
			}

			_data.Clear();

			return new[] { newArg };
		}

		protected override IEnumerable<IUpdateListQuery<T>> OnMove(IListOnMoveArgs<T> arg)
		{
			var item = _data[arg.OldIndex];
			IUpdateListQuery<T> newArg;

			if (item.Value)
			{
				SubOneFromIndex(arg.OldIndex);
				_data.RemoveAt(arg.OldIndex);
				_data.Insert(arg.NewIndex, item);
				int oldActualIndex = item.ActualIndex;
				item.ActualIndex = arg.NewIndex == 0 ? -1 : (_data[arg.NewIndex - 1].ActualIndex);
				AddOneFromIndex(arg.NewIndex);
				newArg = UpdateListQuery<T>.OnMove(arg.Item, oldActualIndex, item.ActualIndex);
			}
			else
			{
				_data.RemoveAt(arg.OldIndex);
				_data.Insert(arg.NewIndex, item);
				item.ActualIndex = arg.NewIndex == 0 ? -1 : (_data[arg.NewIndex - 1].ActualIndex);
				newArg = UpdateListQuery<T>.OnEmpty();
			}

			return new[] {newArg};
		}

		protected override IEnumerable<IUpdateListQuery<T>> OnReplace(IListOnReplaceArgs<T> arg)
		{
			var srcItem = _data[arg.Index];
			var newItem = new Criteria(
				item: arg.NewItem,
				condition: _condition,
				criteriaChanged: _getObservable(arg.NewItem),
				onItemUpdated: OnItemUpdated);
			newItem.ActualIndex = srcItem.ActualIndex;

			_data.Remove(srcItem);
			_data.Add(newItem);

			IUpdateListQuery<T> newArg;
			if (srcItem.Value && newItem.Value)
			{
				newArg = arg;
			}
			else if (srcItem.Value && !newItem.Value)
			{
				newArg = UpdateListQuery<T>.OnRemove(srcItem.Item, srcItem.ActualIndex);
				SubOneFromIndex(arg.Index);
			}
			else if (!srcItem.Value && newItem.Value)
			{
				AddOneFromIndex(arg.Index);
				newArg = UpdateListQuery<T>.OnInsert(newItem.Item, newItem.ActualIndex);
			}
			else
			{
				newArg = UpdateListQuery<T>.OnEmpty();
			}

			srcItem.Dispose();

			return new[] { newArg };
		}

		protected override IEnumerable<IUpdateListQuery<T>> OnRemove(IListOnRemoveArgs<T> arg)
		{
			var item = _data[arg.Index];

			IUpdateListQuery<T> newArg;
			if (item.Value)
			{
				newArg = UpdateListQuery<T>.OnRemove(item.Item, item.ActualIndex);
				SubOneFromIndex(arg.Index);
			}
			else
			{
				newArg = UpdateListQuery<T>.OnEmpty();
			}

			_data.Remove(item);
			return new[] {newArg};
		}
	}
}