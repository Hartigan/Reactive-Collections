using System;
using ReactiveCollections.Abstract.Transactions.Arguments;

namespace ReactiveCollections.Abstract.Transactions
{
	public interface IUpdateListQuery<out T> : IUpdateSetQuery<T>
	{
		TResult Match<TResult>(
			Func<IListOnInsertArgs<T>, TResult> onInsert,
			Func<IListOnRemoveArgs<T>, TResult> onRemove,
			Func<IListOnReplaceArgs<T>, TResult> onReplace,
			Func<IListOnMoveArgs<T>, TResult> onMove,
			Func<IListOnClearArgs<T>, TResult> onClear,
			Func<IListOnEmptyArgs, TResult> onEmpty);

		void Match(
			Action<IListOnInsertArgs<T>> onInsert,
			Action<IListOnRemoveArgs<T>> onRemove,
			Action<IListOnReplaceArgs<T>> onReplace,
			Action<IListOnMoveArgs<T>> onMove,
			Action<IListOnClearArgs<T>> onClear,
			Action<IListOnEmptyArgs> onEmpty);
	}
}
