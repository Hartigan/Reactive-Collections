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
		private sealed class OnClearArgs : UpdateCollectionQuery<T>, ICollectionOnClearArgs<T>
		{
			[NotNull]
			private readonly IReadOnlyList<T> _items;

			public OnClearArgs(IReadOnlyList<T> items)
			{
				items.ArgumentNotNull(nameof(items));
				_items = items;
			}

			public override TResult Match<TResult>(
				Func<ICollectionOnInsertArgs<T>, TResult> onInsert,
				Func<ICollectionOnRemoveArgs<T>, TResult> onRemove,
				Func<ICollectionOnReplaceArgs<T>, TResult> onReplace,
				Func<ICollectionOnClearArgs<T>, TResult> onClear,
				Func<ICollectionOnEmptyArgs, TResult> onEmpty)
			{
				onClear.ArgumentNotNull(nameof(onClear));
				return onClear(this);
			}

			public override void Match(
				Action<ICollectionOnInsertArgs<T>> onInsert,
				Action<ICollectionOnRemoveArgs<T>> onRemove,
				Action<ICollectionOnReplaceArgs<T>> onReplace,
				Action<ICollectionOnClearArgs<T>> onClear,
				Action<ICollectionOnEmptyArgs> onEmpty)
			{
				onClear.ArgumentNotNull(nameof(onClear));
				onClear(this);
			}

			public IReadOnlyList<T> Items => _items;
		}

		private sealed class OnEmptyArgs : UpdateCollectionQuery<T>, ICollectionOnEmptyArgs
		{
			public override TResult Match<TResult>(
				Func<ICollectionOnInsertArgs<T>, TResult> onInsert,
				Func<ICollectionOnRemoveArgs<T>, TResult> onRemove,
				Func<ICollectionOnReplaceArgs<T>, TResult> onReplace,
				Func<ICollectionOnClearArgs<T>, TResult> onClear,
				Func<ICollectionOnEmptyArgs, TResult> onEmpty)
			{
				onEmpty.ArgumentNotNull(nameof(onEmpty));
				return onEmpty(this);
			}

			public override void Match(
				Action<ICollectionOnInsertArgs<T>> onInsert,
				Action<ICollectionOnRemoveArgs<T>> onRemove,
				Action<ICollectionOnReplaceArgs<T>> onReplace,
				Action<ICollectionOnClearArgs<T>> onClear,
				Action<ICollectionOnEmptyArgs> onEmpty)
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
				Func<ICollectionOnClearArgs<T>, TResult> onClear,
				Func<ICollectionOnEmptyArgs, TResult> onEmpty)
			{
				onInsert.ArgumentNotNull(nameof(onInsert));
				return onInsert(this);
			}

			public override void Match(
				Action<ICollectionOnInsertArgs<T>> onInsert,
				Action<ICollectionOnRemoveArgs<T>> onRemove,
				Action<ICollectionOnReplaceArgs<T>> onReplace,
				Action<ICollectionOnClearArgs<T>> onClear,
				Action<ICollectionOnEmptyArgs> onEmpty)
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
				Func<ICollectionOnClearArgs<T>, TResult> onClear,
				Func<ICollectionOnEmptyArgs, TResult> onEmpty)
			{
				onRemove.ArgumentNotNull(nameof(onRemove));
				return onRemove(this);
			}

			public override void Match(
				Action<ICollectionOnInsertArgs<T>> onInsert,
				Action<ICollectionOnRemoveArgs<T>> onRemove,
				Action<ICollectionOnReplaceArgs<T>> onReplace,
				Action<ICollectionOnClearArgs<T>> onClear,
				Action<ICollectionOnEmptyArgs> onEmpty)
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
				Func<ICollectionOnClearArgs<T>, TResult> onClear,
				Func<ICollectionOnEmptyArgs, TResult> onEmpty)
			{
				onReplace.ArgumentNotNull(nameof(onReplace));
				return onReplace(this);
			}

			public override void Match(
				Action<ICollectionOnInsertArgs<T>> onInsert,
				Action<ICollectionOnRemoveArgs<T>> onRemove,
				Action<ICollectionOnReplaceArgs<T>> onReplace,
				Action<ICollectionOnClearArgs<T>> onClear,
				Action<ICollectionOnEmptyArgs> onEmpty)
			{
				onReplace.ArgumentNotNull(nameof(onReplace));
				onReplace(this);
			}
		}

		[NotNull]
		public static IUpdateCollectionQuery<T> OnClear([NotNull] IReadOnlyList<T> items)
		{
			items.ArgumentNotNull(nameof(items));
			return new OnClearArgs(items);
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
			Func<ICollectionOnClearArgs<T>, TResult> onClear,
			Func<ICollectionOnEmptyArgs, TResult> onEmpty);

		public abstract void Match(
			Action<ICollectionOnInsertArgs<T>> onInsert,
			Action<ICollectionOnRemoveArgs<T>> onRemove,
			Action<ICollectionOnReplaceArgs<T>> onReplace,
			Action<ICollectionOnClearArgs<T>> onClear,
			Action<ICollectionOnEmptyArgs> onEmpty);
	}
}
