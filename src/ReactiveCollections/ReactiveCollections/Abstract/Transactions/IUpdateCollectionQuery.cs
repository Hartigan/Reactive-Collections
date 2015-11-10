using System;
using JetBrains.Annotations;
using ReactiveCollections.Abstract.Transactions.Arguments;

namespace ReactiveCollections.Abstract.Transactions
{
	public interface IUpdateCollectionQuery<out T>
	{
		TResult Match<TResult>(
			[NotNull] Func<ICollectionOnInsertArgs<T>, TResult> onInsert,
			[NotNull] Func<ICollectionOnRemoveArgs<T>, TResult> onRemove,
			[NotNull] Func<ICollectionOnReplaceArgs<T>, TResult> onReplace,
			[NotNull] Func<ICollectionOnClearArgs<T>, TResult> onClear,
			[NotNull] Func<ICollectionOnEmptyArgs, TResult> onEmpty);

		void Match(
			[NotNull] Action<ICollectionOnInsertArgs<T>> onInsert,
			[NotNull] Action<ICollectionOnRemoveArgs<T>> onRemove,
			[NotNull] Action<ICollectionOnReplaceArgs<T>> onReplace,
			[NotNull] Action<ICollectionOnClearArgs<T>> onClear,
			[NotNull] Action<ICollectionOnEmptyArgs> onEmpty);
	}
}
