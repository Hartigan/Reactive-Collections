using System;
using System.Reactive.Disposables;
using JetBrains.Annotations;
using ReactiveCollections.Abstract.Transactions;
using ReactiveCollections.Abstract.Transactions.Arguments;
using ReactiveCollections.Extensions;

namespace ReactiveCollections.Implementation.Functions
{
	public abstract class ListFunctionBase<T> : IDisposable
	{
		[NotNull]
		private IDisposable _sub;

		public ListFunctionBase()
		{
			_sub = Disposable.Empty;
		}

		protected void Subscibe([NotNull] IObservable<IUpdateListQuery<T>> source)
		{
			_sub = source.WeakSubscribe(ProcessQuery);
		}

		private void ProcessQuery([NotNull] IUpdateListQuery<T> query)
		{
			query.Match(
				onInsert: OnInsert,
				onRemove: OnRemove,
				onReplace: OnReplace,
				onMove: OnMove,
				onReset: OnReset,
				onEmpty: OnEmpty);
		}

		protected abstract void OnEmpty([NotNull] IListOnEmptyArgs<T> arg);

		protected abstract void OnReset([NotNull] IListOnResetArgs<T> arg);

		protected abstract void OnMove([NotNull] IListOnMoveArgs<T> arg);

		protected abstract void OnReplace([NotNull] IListOnReplaceArgs<T> arg);

		protected abstract void OnRemove([NotNull] IListOnRemoveArgs<T> arg);

		protected abstract void OnInsert([NotNull] IListOnInsertArgs<T> arg);

		public virtual void Dispose()
		{
			_sub.Dispose();
		}
	}
}