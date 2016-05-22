using System;
using System.Reactive.Disposables;
using JetBrains.Annotations;
using ReactiveCollections.Abstract.Transactions;
using ReactiveCollections.Abstract.Transactions.Arguments;
using ReactiveCollections.Extensions;

namespace ReactiveCollections.Implementation.Functions
{
	public abstract class CollectionFunctionBase<T> : IDisposable
	{
		[NotNull]
		private IDisposable _sub;

		public CollectionFunctionBase()
		{
			_sub = Disposable.Empty;
		}

		protected void Subscibe([NotNull] IObservable<IUpdateCollectionQuery<T>> source)
		{
			_sub = source.WeakSubscribe(ProcessQuery);
		}

		private void ProcessQuery([NotNull] IUpdateCollectionQuery<T> query)
		{
			query.Match(
				onInsert: OnInsert,
				onRemove: OnRemove,
				onReplace: OnReplace,
				onReset: OnReset,
				onEmpty: OnEmpty);
		}

		protected abstract void OnEmpty([NotNull] ICollectionOnEmptyArgs<T> arg);

		protected abstract void OnReset([NotNull] ICollectionOnResetArgs<T> arg);

		protected abstract void OnReplace([NotNull] ICollectionOnReplaceArgs<T> arg);

		protected abstract void OnRemove([NotNull] ICollectionOnRemoveArgs<T> arg);

		protected abstract void OnInsert([NotNull] ICollectionOnInsertArgs<T> arg);

		public virtual void Dispose()
		{
			_sub.Dispose();
		}
	}
}
