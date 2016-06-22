using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using ReactiveCollections.Abstract.Transactions;
using ReactiveCollections.Abstract.Transactions.Arguments;
using ReactiveCollections.Domain;

namespace ReactiveCollections.Implementation.Functions
{
	public sealed class ListFirstOrDefaultFunction<T> : ListFunctionBase<T>, IObservableValue<T>
	{
		[NotNull]
		private readonly ObservableValue<T> _first = new ObservableValue<T>(default(T));

		[NotNull]
		private readonly List<T> _data = new List<T>(); 

		public ListFirstOrDefaultFunction(
			[NotNull] IObservable<IUpdateListQuery<T>> source)
		{
			Subscibe(source);
		}

		protected override void OnEmpty(IListOnEmptyArgs<T> arg)
		{
		}

		protected override void OnReset(IListOnResetArgs<T> arg)
		{
			_data.Clear();
			_data.AddRange(arg.NewItems);
			_first.Value = _data.FirstOrDefault();
		}

		protected override void OnMove(IListOnMoveArgs<T> arg)
		{
			_data.RemoveAt(arg.OldIndex);
			_data.Insert(arg.NewIndex, arg.Item);

			if (arg.OldIndex == 0 || arg.NewIndex == 0)
			{
				_first.Value = _data.FirstOrDefault();
			}
		}

		protected override void OnReplace(IListOnReplaceArgs<T> arg)
		{
			_data[arg.Index] = arg.NewItem;
			if (arg.Index == 0)
			{
				_first.Value = arg.NewItem;
			}
		}

		protected override void OnRemove(IListOnRemoveArgs<T> arg)
		{
			_data.RemoveAt(arg.Index);
			if (arg.Index == 0)
			{
				_first.Value = _data.FirstOrDefault();
			}
		}

		protected override void OnInsert(IListOnInsertArgs<T> arg)
		{
			_data.Insert(arg.Index, arg.Item);
			if (arg.Index == 0)
			{
				_first.Value = _data.FirstOrDefault();
			}
		}

		public T Value => _first.Value;

		public IObservable<ValueChangedArgs<T>> ValueChanged => _first.ValueChanged;
	}
}
