using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using ReactiveCollections.Abstract.Collections;
using ReactiveCollections.Abstract.Transactions;
using ReactiveCollections.Abstract.Transactions.Arguments;
using ReactiveCollections.Implementation.Transactions;

namespace ReactiveCollections.Implementation.Operations
{
	public class ListSelectOperation<TIn, TOut> : ListToListOperationBase<TIn, TOut>
	{
		[NotNull] private readonly Func<TIn, TOut> _selector;
		[NotNull] private readonly List<TOut> _data = new List<TOut>(); 

		public ListSelectOperation(
			[NotNull] INotifyListChanged<TIn> source,
			[NotNull] Func<TIn, TOut> selector) : base(source)
		{
			_selector = selector;
		}

		protected override IEnumerable<IUpdateListQuery<TOut>> OnInsert(IListOnInsertArgs<TIn> arg)
		{
			TOut newItem = _selector(arg.Item);
			_data.Insert(arg.Index, newItem);
			return new[] { UpdateListQuery<TOut>.OnInsert(newItem, arg.Index) };
		}

		public override IEnumerator<TOut> GetEnumerator()
		{
			return _data.GetEnumerator();
		}

		public override int Count => _data.Count;

		public override TOut this[int index] => _data[index];

		protected override IEnumerable<IUpdateListQuery<TOut>> OnEmpty(IListOnEmptyArgs arg)
		{
			return Enumerable.Empty<IUpdateListQuery<TOut>>();
		}

		protected override IEnumerable<IUpdateListQuery<TOut>> OnReset(IListOnResetArgs<TIn> arg)
		{
			var oldItems = _data.ToList();
			_data.Clear();

			for (int i = 0; i < arg.NewItems.Count; i++)
			{
				_data.Insert(i, _selector(arg.NewItems[i]));
			}

			var newItems = _data.ToList();
			var newArgs = UpdateListQuery<TOut>.OnReset(oldItems, newItems);
			
			return new[] { newArgs };
		}

		protected override IEnumerable<IUpdateListQuery<TOut>> OnMove(IListOnMoveArgs<TIn> arg)
		{
			var item = _data[arg.OldIndex];
			_data.RemoveAt(arg.OldIndex);
			_data.Insert(arg.NewIndex, item);
			return new[] { UpdateListQuery<TOut>.OnMove(item, arg.OldIndex, arg.NewIndex) };
		}

		protected override IEnumerable<IUpdateListQuery<TOut>> OnReplace(IListOnReplaceArgs<TIn> arg)
		{
			var newItem = _selector(arg.NewItem);
			var oldItem = _data[arg.Index];
			_data.RemoveAt(arg.Index);
			_data.Insert(arg.Index, newItem);
			return new[] { UpdateListQuery<TOut>.OnReplace(oldItem, newItem, arg.Index) };
		}

		protected override IEnumerable<IUpdateListQuery<TOut>> OnRemove(IListOnRemoveArgs<TIn> arg)
		{
			var item = _data[arg.Index];
			_data.RemoveAt(arg.Index);
			return new[] { UpdateListQuery<TOut>.OnRemove(item, arg.Index) };
		}
	}
}
