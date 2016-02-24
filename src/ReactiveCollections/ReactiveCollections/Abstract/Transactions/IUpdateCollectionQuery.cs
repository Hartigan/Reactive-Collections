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
			[NotNull] Func<ICollectionOnResetArgs<T>, TResult> onReset,
			[NotNull] Func<ICollectionOnEmptyArgs, TResult> onEmpty);

		void Match(
			[NotNull] Action<ICollectionOnInsertArgs<T>> onInsert,
			[NotNull] Action<ICollectionOnRemoveArgs<T>> onRemove,
			[NotNull] Action<ICollectionOnReplaceArgs<T>> onReplace,
			[NotNull] Action<ICollectionOnResetArgs<T>> onReset,
			[NotNull] Action<ICollectionOnEmptyArgs> onEmpty);
	}
}
