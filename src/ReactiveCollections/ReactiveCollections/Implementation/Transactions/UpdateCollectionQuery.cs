using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ReactiveCollections.Abstract.Transactions;
using ReactiveCollections.Abstract.Transactions.Arguments;
using ReactiveCollections.Extensions;

namespace ReactiveCollections.Implementation.Transactions
{
	public abstract class UpdateCollectionQuery<T> : IUpdateCollectionQuery<T>
	{
		private sealed class OnResetArgs : UpdateCollectionQuery<T>, ICollectionOnResetArgs<T>
		{
			[NotNull]
			private readonly IReadOnlyList<T> _oldItems;

			[NotNull]
			private readonly IReadOnlyList<T> _newItems;

			public OnResetArgs(IReadOnlyList<T> oldItems, IReadOnlyList<T> newItems)
			{
				oldItems.ArgumentNotNull(nameof(oldItems));
				newItems.ArgumentNotNull(nameof(newItems));

				_oldItems = oldItems;
				_newItems = newItems;
			}

			public override TResult Match<TResult>(
				Func<ICollectionOnInsertArgs<T>, TResult> onInsert,
				Func<ICollectionOnRemoveArgs<T>, TResult> onRemove,
				Func<ICollectionOnReplaceArgs<T>, TResult> onReplace,
				Func<ICollectionOnResetArgs<T>, TResult> onReset,
				Func<ICollectionOnEmptyArgs<T>, TResult> onEmpty)
			{
				onReset.ArgumentNotNull(nameof(onReset));
				return onReset(this);
			}

			public override void Match(
				Action<ICollectionOnInsertArgs<T>> onInsert,
				Action<ICollectionOnRemoveArgs<T>> onRemove,
				Action<ICollectionOnReplaceArgs<T>> onReplace,
				Action<ICollectionOnResetArgs<T>> onReset,
				Action<ICollectionOnEmptyArgs<T>> onEmpty)
			{
				onReset.ArgumentNotNull(nameof(onReset));
				onReset(this);
			}

			public IReadOnlyList<T> OldItems => _oldItems;

			public IReadOnlyList<T> NewItems => _newItems;
		}

		private sealed class OnEmptyArgs : UpdateCollectionQuery<T>, ICollectionOnEmptyArgs<T>
		{
			public override TResult Match<TResult>(
				Func<ICollectionOnInsertArgs<T>, TResult> onInsert,
				Func<ICollectionOnRemoveArgs<T>, TResult> onRemove,
				Func<ICollectionOnReplaceArgs<T>, TResult> onReplace,
				Func<ICollectionOnResetArgs<T>, TResult> onReset,
				Func<ICollectionOnEmptyArgs<T>, TResult> onEmpty)
			{
				onEmpty.ArgumentNotNull(nameof(onEmpty));
				return onEmpty(this);
			}

			public override void Match(
				Action<ICollectionOnInsertArgs<T>> onInsert,
				Action<ICollectionOnRemoveArgs<T>> onRemove,
				Action<ICollectionOnReplaceArgs<T>> onReplace,
				Action<ICollectionOnResetArgs<T>> onReset,
				Action<ICollectionOnEmptyArgs<T>> onEmpty)
			{
				onEmpty.ArgumentNotNull(nameof(onEmpty));
				onEmpty(this);
			}
		}

		private sealed class OnInsertArgs : UpdateCollectionQuery<T>, ICollectionOnInsertArgs<T>
		{
			private readonly T _item;

			public OnInsertArgs(T item)
			{
				_item = item;
			}

			public override TResult Match<TResult>(
				Func<ICollectionOnInsertArgs<T>, TResult> onInsert,
				Func<ICollectionOnRemoveArgs<T>, TResult> onRemove,
				Func<ICollectionOnReplaceArgs<T>, TResult> onReplace,
				Func<ICollectionOnResetArgs<T>, TResult> onReset,
				Func<ICollectionOnEmptyArgs<T>, TResult> onEmpty)
			{
				onInsert.ArgumentNotNull(nameof(onInsert));
				return onInsert(this);
			}

			public override void Match(
				Action<ICollectionOnInsertArgs<T>> onInsert,
				Action<ICollectionOnRemoveArgs<T>> onRemove,
				Action<ICollectionOnReplaceArgs<T>> onReplace,
				Action<ICollectionOnResetArgs<T>> onReset,
				Action<ICollectionOnEmptyArgs<T>> onEmpty)
			{
				onInsert.ArgumentNotNull(nameof(onInsert));
				onInsert(this);
			}

			public T Item => _item;
		}

		private sealed class OnRemoveArgs : UpdateCollectionQuery<T>, ICollectionOnRemoveArgs<T>
		{
			private readonly T _item;

			public OnRemoveArgs(T item)
			{
				_item = item;
			}

			public override TResult Match<TResult>(
				Func<ICollectionOnInsertArgs<T>, TResult> onInsert,
				Func<ICollectionOnRemoveArgs<T>, TResult> onRemove,
				Func<ICollectionOnReplaceArgs<T>, TResult> onReplace,
				Func<ICollectionOnResetArgs<T>, TResult> onReset,
				Func<ICollectionOnEmptyArgs<T>, TResult> onEmpty)
			{
				onRemove.ArgumentNotNull(nameof(onRemove));
				return onRemove(this);
			}

			public override void Match(
				Action<ICollectionOnInsertArgs<T>> onInsert,
				Action<ICollectionOnRemoveArgs<T>> onRemove,
				Action<ICollectionOnReplaceArgs<T>> onReplace,
				Action<ICollectionOnResetArgs<T>> onReset,
				Action<ICollectionOnEmptyArgs<T>> onEmpty)
			{
				onRemove.ArgumentNotNull(nameof(onRemove));
				onRemove(this);
			}

			public T Item => _item;
		}

		private sealed class OnReplaceArgs : UpdateCollectionQuery<T>, ICollectionOnReplaceArgs<T>
		{
			private readonly T _oldItem;
			private readonly T _newItem;

			public T OldItem => _oldItem;

			public T NewItem => _newItem;

			public OnReplaceArgs(T oldItem, T newItem)
			{
				_oldItem = oldItem;
				_newItem = newItem;
			}

			public override TResult Match<TResult>(
				Func<ICollectionOnInsertArgs<T>, TResult> onInsert,
				Func<ICollectionOnRemoveArgs<T>, TResult> onRemove,
				Func<ICollectionOnReplaceArgs<T>, TResult> onReplace,
				Func<ICollectionOnResetArgs<T>, TResult> onReset,
				Func<ICollectionOnEmptyArgs<T>, TResult> onEmpty)
			{
				onReplace.ArgumentNotNull(nameof(onReplace));
				return onReplace(this);
			}

			public override void Match(
				Action<ICollectionOnInsertArgs<T>> onInsert,
				Action<ICollectionOnRemoveArgs<T>> onRemove,
				Action<ICollectionOnReplaceArgs<T>> onReplace,
				Action<ICollectionOnResetArgs<T>> onReset,
				Action<ICollectionOnEmptyArgs<T>> onEmpty)
			{
				onReplace.ArgumentNotNull(nameof(onReplace));
				onReplace(this);
			}
		}

		[NotNull]
		public static IUpdateCollectionQuery<T> OnReset([NotNull] IReadOnlyList<T> oldItems, [NotNull] IReadOnlyList<T> newItems)
		{
			return new OnResetArgs(oldItems, newItems);
		}

		[NotNull]
		public static IUpdateCollectionQuery<T> OnEmpty()
		{
			return new OnEmptyArgs();
		}

		[NotNull]
		public static IUpdateCollectionQuery<T> OnInsert(T item)
		{
			return new OnInsertArgs(item);
		}

		[NotNull]
		public static IUpdateCollectionQuery<T> OnRemove(T item)
		{
			return new OnRemoveArgs(item);
		}

		[NotNull]
		public static IUpdateCollectionQuery<T> OnReplace(T oldItem, T newItem)
		{
			return new OnReplaceArgs(oldItem, newItem);
		} 

		public abstract TResult Match<TResult>(
			Func<ICollectionOnInsertArgs<T>, TResult> onInsert,
			Func<ICollectionOnRemoveArgs<T>, TResult> onRemove,
			Func<ICollectionOnReplaceArgs<T>, TResult> onReplace,
			Func<ICollectionOnResetArgs<T>, TResult> onReset,
			Func<ICollectionOnEmptyArgs<T>, TResult> onEmpty);

		public abstract void Match(
			Action<ICollectionOnInsertArgs<T>> onInsert,
			Action<ICollectionOnRemoveArgs<T>> onRemove,
			Action<ICollectionOnReplaceArgs<T>> onReplace,
			Action<ICollectionOnResetArgs<T>> onReset,
			Action<ICollectionOnEmptyArgs<T>> onEmpty);
	}
}
