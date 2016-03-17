using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using ReactiveCollections.Abstract.Transactions;
using ReactiveCollections.Abstract.Transactions.Arguments;
using ReactiveCollections.Domain;
using ReactiveCollections.Extensions;
using ReactiveCollections.Implementation.Transactions;

namespace ReactiveCollections.Implementation.Operations
{
	public class ListSkipAndTakeOperation<T> : ListToListOperationBase<T, T>
	{
		[NotNull] private readonly List<T> _data = new List<T>();
		[NotNull] private readonly ObservableValue<int> _skip;
		[NotNull] private readonly ObservableValue<int> _take;
		[NotNull] private readonly IDisposable _skipSub;
		[NotNull] private readonly IDisposable _takeSub;

		public ListSkipAndTakeOperation(
			[NotNull] IObservable<IUpdateListQuery<T>> source,
			[NotNull] ObservableValue<int> skip,
			[NotNull] ObservableValue<int> take)
		{
			_skip = skip;
			_take = take;

			_skipSub = skip.ValueChanged.WeakSubscribe(OnSkipChanged);
			_takeSub = take.ValueChanged.WeakSubscribe(OnTakeChanged);

			Subscibe(source);
		}

		private int ConvertSourceToActualIndex(int sourceIndex)
		{
			return sourceIndex - _skip;
		}

		private void OnTakeChanged([NotNull] ValueChangedArgs<int> args)
		{
			if (args.NewValue > args.OldValue)
			{
				for (int i = 0; i < args.NewValue - args.OldValue && _skip.Value + args.OldValue + i < _data.Count; i++)
				{
					var sourceIndex = _skip.Value + args.OldValue + i;
					var actualIndex = ConvertSourceToActualIndex(sourceIndex);
					RaiseListChanged(UpdateListQuery<T>.OnInsert(_data[sourceIndex], actualIndex));
				}
			}
			else
			{
				for (int i = Math.Min(_skip + args.OldValue, _data.Count) - 1; i >= _skip + args.NewValue; i--)
				{
					var sourceIndex = i;
					var actualIndex = ConvertSourceToActualIndex(sourceIndex);
					RaiseListChanged(UpdateListQuery<T>.OnRemove(_data[sourceIndex], actualIndex));
				}
			}
		}

		private void OnSkipChanged([NotNull] ValueChangedArgs<int> args)
		{
			if (args.NewValue > args.OldValue)
			{
				for (int i = args.OldValue; i < args.NewValue && i < _data.Count; i++)
				{
					var sourceIndex = i;
					RaiseListChanged(UpdateListQuery<T>.OnRemove(_data[sourceIndex], 0));
				}

				for (int i = args.OldValue + _take; i < args.OldValue + _take && i < _data.Count; i++)
				{
					var sourceIndex = i;
					var actualIndex = ConvertSourceToActualIndex(sourceIndex);
					RaiseListChanged(UpdateListQuery<T>.OnInsert(_data[sourceIndex], actualIndex));
				}
			}
			else
			{
				for (int i = Math.Min(args.OldValue + _take, _data.Count) - 1; i >= args.NewValue + _take; i++)
				{
					var sourceIndex = i;
					var actualIndex = i - args.OldValue;
					RaiseListChanged(UpdateListQuery<T>.OnRemove(_data[sourceIndex], actualIndex));
				}

				for (int i = args.OldValue - 1; i >= args.NewValue; i++)
				{
					var sourceIndex = i;
					RaiseListChanged(UpdateListQuery<T>.OnInsert(_data[sourceIndex], 0));
				}
			}
		}

		protected override IEnumerable<IUpdateListQuery<T>> OnInsert(IListOnInsertArgs<T> arg)
		{
			List<IUpdateListQuery<T>> result = new List<IUpdateListQuery<T>>(2);

			_data.Insert(arg.Index, arg.Item);

			if (arg.Index < _skip)
			{
				var sourceIndexForRemove = _skip + _take;

				if (sourceIndexForRemove >= 0 && sourceIndexForRemove < _data.Count)
				{
					result.Add(UpdateListQuery<T>.OnRemove(_data[sourceIndexForRemove], ConvertSourceToActualIndex(sourceIndexForRemove) - 1));
				}

				if (_skip + 1 < _data.Count)
				{
					result.Add(UpdateListQuery<T>.OnInsert(_data[_skip + 1], 0));
				}
			}
			else if (arg.Index >= _skip + _take)
			{
			}
			else
			{
				var sourceIndexForRemove = _skip + _take;

				if (sourceIndexForRemove >= 0 && sourceIndexForRemove < _data.Count)
				{
					result.Add(UpdateListQuery<T>.OnRemove(_data[sourceIndexForRemove], ConvertSourceToActualIndex(sourceIndexForRemove) - 1));
				}

				result.Add(UpdateListQuery<T>.OnInsert(arg.Item, ConvertSourceToActualIndex(arg.Index)));
			}

			return result;
		}

		public override IEnumerator<T> GetEnumerator() => _data.Skip(_skip).Take(_take).GetEnumerator();

		public override int Count => Math.Min(_take, Math.Max(0, _data.Count - _skip));

		public override T this[int index] => _data[_skip + index];

		protected override IEnumerable<IUpdateListQuery<T>> OnEmpty(IListOnEmptyArgs arg)
		{
			return Enumerable.Empty<IUpdateListQuery<T>>();
		}

		protected override IEnumerable<IUpdateListQuery<T>> OnReset(IListOnResetArgs<T> arg)
		{
			T[] oldItems = new T[Count];
			for (int i = 0; i < Count; i++)
			{
				oldItems[i] = _data[_skip + i];
			}
			_data.Clear();

			_data.AddRange(arg.NewItems);
			T[] newItems = new T[Count];
			for (int i = 0; i < Count; i++)
			{
				newItems[i] = _data[_skip + i];
			}

			return new[] { UpdateListQuery<T>.OnReset(oldItems, newItems) };
		}

		protected override IEnumerable<IUpdateListQuery<T>> OnMove(IListOnMoveArgs<T> arg)
		{
			List<IUpdateListQuery<T>> result = new List<IUpdateListQuery<T>>(2);
			var skipAndTake = _skip + _take;
			_data.RemoveAt(arg.OldIndex);
			_data.Insert(arg.NewIndex, arg.Item);

			if (arg.OldIndex < _skip && arg.NewIndex < _skip)
			{
			}
			else if (arg.OldIndex >= skipAndTake && arg.NewIndex >= skipAndTake)
			{
			}
			else if (arg.OldIndex >= _skip && arg.NewIndex >= _skip && arg.OldIndex < skipAndTake && arg.NewIndex < skipAndTake)
			{
				result.Add(UpdateListQuery<T>.OnMove(
					arg.Item,
					ConvertSourceToActualIndex(arg.OldIndex),
					ConvertSourceToActualIndex(arg.NewIndex)));
			}
			else if (arg.OldIndex < _skip && arg.NewIndex < skipAndTake)
			{
				result.Add(UpdateListQuery<T>.OnRemove(_data[_skip - 1], 0));
				result.Add(UpdateListQuery<T>.OnInsert(arg.Item, ConvertSourceToActualIndex(arg.NewIndex)));
			}
			else if (arg.OldIndex < _skip && arg.NewIndex >= skipAndTake)
			{
				result.Add(UpdateListQuery<T>.OnRemove(_data[_skip - 1], 0));
				result.Add(UpdateListQuery<T>.OnInsert(_data[skipAndTake - 1], ConvertSourceToActualIndex(skipAndTake - 1)));
			}
			else if (arg.OldIndex < skipAndTake && arg.NewIndex < _skip)
			{
				result.Add(UpdateListQuery<T>.OnRemove(arg.Item, ConvertSourceToActualIndex(arg.OldIndex)));
				result.Add(UpdateListQuery<T>.OnInsert(_data[_skip], 0));
			}
			else if (arg.OldIndex >= skipAndTake && arg.NewIndex < _skip)
			{
				result.Add(UpdateListQuery<T>.OnRemove(_data[skipAndTake], ConvertSourceToActualIndex(skipAndTake - 1)));
				result.Add(UpdateListQuery<T>.OnInsert(_data[_skip], 0));
			}

			return result;
		}

		protected override IEnumerable<IUpdateListQuery<T>> OnReplace(IListOnReplaceArgs<T> arg)
		{
			_data[arg.Index] = arg.NewItem;

			if (arg.Index < _skip)
			{
			}
			else if (arg.Index >= _skip + _take)
			{
			}
			else
			{
				return new[] { UpdateListQuery<T>.OnReplace(arg.OldItem, arg.NewItem, ConvertSourceToActualIndex(arg.Index)) };
			}

			return Enumerable.Empty<IUpdateListQuery<T>>();
		}

		protected override IEnumerable<IUpdateListQuery<T>> OnRemove(IListOnRemoveArgs<T> arg)
		{
			List<IUpdateListQuery<T>> result = new List<IUpdateListQuery<T>>(2);

			_data.RemoveAt(arg.Index);

			if (arg.Index < _skip)
			{
				var sourceIndexForRemove = _skip - 1;

				if (sourceIndexForRemove < _data.Count && sourceIndexForRemove >= 0)
				{
					result.Add(UpdateListQuery<T>.OnInsert(_data[sourceIndexForRemove], 0));
				}

				var sourceIndexForInsert = _skip + _take - 1;

				if (sourceIndexForInsert >= 0 && sourceIndexForInsert < _data.Count)
				{
					result.Add(UpdateListQuery<T>.OnInsert(_data[sourceIndexForInsert], ConvertSourceToActualIndex(sourceIndexForInsert)));
				}
			}
			else if (arg.Index >= _skip + _take)
			{
			}
			else
			{
				result.Add(UpdateListQuery<T>.OnRemove(arg.Item, ConvertSourceToActualIndex(arg.Index)));

				var sourceIndexForInsert = _skip + _take - 1;

				if (sourceIndexForInsert >= 0 && sourceIndexForInsert < _data.Count)
				{
					result.Add(UpdateListQuery<T>.OnInsert(_data[sourceIndexForInsert], ConvertSourceToActualIndex(sourceIndexForInsert)));
				}
			}

			return result;
		}

		public override void Dispose()
		{
			base.Dispose();
			_skipSub.Dispose();
			_takeSub.Dispose();
		}
	}
}