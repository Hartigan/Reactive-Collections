using System;
using ReactiveCollections.Abstract.Transactions.Arguments;

namespace ReactiveCollections.Abstract.Transactions
{
	public interface IUpdateCollectionQuery<out T>
	{
		TResult Match<TResult>(
			Func<ICollectionOnInsertArgs<T>, TResult> onInsert,
			Func<ICollectionOnRemoveArgs<T>, TResult> onRemove,
			Func<ICollectionOnReplaceArgs<T>, TResult> onReplace,
			Func<ICollectionOnClearArgs<T>, TResult> onClear,
			Func<ICollectionOnEmptyArgs, TResult> onEmpty);

		void Match(
			Action<ICollectionOnInsertArgs<T>> onInsert,
			Action<ICollectionOnRemoveArgs<T>> onRemove,
			Action<ICollectionOnReplaceArgs<T>> onReplace,
			Action<ICollectionOnClearArgs<T>> onClear,
			Action<ICollectionOnEmptyArgs> onEmpty);
	}
}
