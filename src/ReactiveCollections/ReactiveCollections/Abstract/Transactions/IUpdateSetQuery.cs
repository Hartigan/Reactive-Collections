﻿using System;
using ReactiveCollections.Abstract.Transactions.Arguments;

namespace ReactiveCollections.Abstract.Transactions
{
	public interface IUpdateSetQuery<out T>
	{
		TResult Match<TResult>(
			Func<ISetOnInsertArgs<T>, TResult> onInsert,
			Func<ISetOnRemoveArgs<T>, TResult> onRemove,
			Func<ISetOnClearArgs<T>, TResult> onClear,
			Func<ISetOnEmptyArgs, TResult> onEmpty);

		void Match(
			Action<ISetOnInsertArgs<T>> onInsert,
			Action<ISetOnRemoveArgs<T>> onRemove,
			Action<ISetOnClearArgs<T>> onClear,
			Action<ISetOnEmptyArgs> onEmpty);
	}
}