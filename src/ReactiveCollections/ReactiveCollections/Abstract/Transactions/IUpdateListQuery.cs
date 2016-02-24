using System;
using JetBrains.Annotations;
using ReactiveCollections.Abstract.Transactions.Arguments;

namespace ReactiveCollections.Abstract.Transactions
{
	public interface IUpdateListQuery<out T> : IUpdateCollectionQuery<T>
	{
		TResult Match<TResult>(
			[NotNull] Func<IListOnInsertArgs<T>, TResult> onInsert,
			[NotNull] Func<IListOnRemoveArgs<T>, TResult> onRemove,
			[NotNull] Func<IListOnReplaceArgs<T>, TResult> onReplace,
			[NotNull] Func<IListOnMoveArgs<T>, TResult> onMove,
			[NotNull] Func<IListOnResetArgs<T>, TResult> onReset,
			[NotNull] Func<IListOnEmptyArgs, TResult> onEmpty);

		void Match(
			[NotNull] Action<IListOnInsertArgs<T>> onInsert,
			[NotNull] Action<IListOnRemoveArgs<T>> onRemove,
			[NotNull] Action<IListOnReplaceArgs<T>> onReplace,
			[NotNull] Action<IListOnMoveArgs<T>> onMove,
			[NotNull] Action<IListOnResetArgs<T>> onReset,
			[NotNull] Action<IListOnEmptyArgs> onEmpty);
	}
}
