using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ReactiveCollections.Abstract.Transactions;
using ReactiveCollections.Abstract.Transactions.Arguments;
using ReactiveCollections.Extensions;

namespace ReactiveCollections.Implementation.Transactions
{
	public abstract class UpdateSetQuery<T> : IUpdateSetQuery<T>
	{
		private sealed class OnClearArgs : UpdateSetQuery<T>, ISetOnClearArgs<T>
		{
			[NotNull]
			private readonly IReadOnlyList<T> _items;

			public OnClearArgs(IReadOnlyList<T> items)
			{
				items.ArgumentNotNull("items != null");
				_items = items;
			}

			public override TResult Match<TResult>(
				Func<ISetOnInsertArgs<T>, TResult> onInsert,
				Func<ISetOnRemoveArgs<T>, TResult> onRemove,
				Func<ISetOnReplaceArgs<T>, TResult> onReplace,
				Func<ISetOnClearArgs<T>, TResult> onClear,
				Func<ISetOnEmptyArgs, TResult> onEmpty)
			{
				onClear.ArgumentNotNull("onClear != null");
				return onClear(this);
			}

			public override void Match(
				Action<ISetOnInsertArgs<T>> onInsert,
				Action<ISetOnRemoveArgs<T>> onRemove,
				Action<ISetOnReplaceArgs<T>> onReplace,
				Action<ISetOnClearArgs<T>> onClear,
				Action<ISetOnEmptyArgs> onEmpty)
			{
				onClear.ArgumentNotNull("onClear != null");
				onClear(this);
			}

			public IReadOnlyList<T> Items
			{
				get { return _items; }
			}
		}

		private sealed class OnEmptyArgs : UpdateSetQuery<T>, ISetOnEmptyArgs
		{
			public override TResult Match<TResult>(
				Func<ISetOnInsertArgs<T>, TResult> onInsert,
				Func<ISetOnRemoveArgs<T>, TResult> onRemove,
				Func<ISetOnReplaceArgs<T>, TResult> onReplace,
				Func<ISetOnClearArgs<T>, TResult> onClear,
				Func<ISetOnEmptyArgs, TResult> onEmpty)
			{
				onEmpty.ArgumentNotNull("onEmpty != null");
				return onEmpty(this);
			}

			public override void Match(
				Action<ISetOnInsertArgs<T>> onInsert,
				Action<ISetOnRemoveArgs<T>> onRemove,
				Action<ISetOnReplaceArgs<T>> onReplace,
				Action<ISetOnClearArgs<T>> onClear,
				Action<ISetOnEmptyArgs> onEmpty)
			{
				onEmpty.ArgumentNotNull("onEmpty != null");
				onEmpty(this);
			}
		}

		private sealed class OnInsertArgs : UpdateSetQuery<T>, ISetOnInsertArgs<T>
		{
			private readonly T _item;

			public OnInsertArgs(T item)
			{
				_item = item;
			}

			public override TResult Match<TResult>(
				Func<ISetOnInsertArgs<T>, TResult> onInsert,
				Func<ISetOnRemoveArgs<T>, TResult> onRemove,
				Func<ISetOnReplaceArgs<T>, TResult> onReplace,
				Func<ISetOnClearArgs<T>, TResult> onClear,
				Func<ISetOnEmptyArgs, TResult> onEmpty)
			{
				onInsert.ArgumentNotNull("onInsert != null");
				return onInsert(this);
			}

			public override void Match(
				Action<ISetOnInsertArgs<T>> onInsert,
				Action<ISetOnRemoveArgs<T>> onRemove,
				Action<ISetOnReplaceArgs<T>> onReplace,
				Action<ISetOnClearArgs<T>> onClear,
				Action<ISetOnEmptyArgs> onEmpty)
			{
				onInsert.ArgumentNotNull("onInsert != null");
				onInsert(this);
			}

			public T Item
			{
				get { return _item; }
			}
		}

		private sealed class OnRemoveArgs : UpdateSetQuery<T>, ISetOnRemoveArgs<T>
		{
			private readonly T _item;

			public OnRemoveArgs(T item)
			{
				_item = item;
			}

			public override TResult Match<TResult>(
				Func<ISetOnInsertArgs<T>, TResult> onInsert,
				Func<ISetOnRemoveArgs<T>, TResult> onRemove,
				Func<ISetOnReplaceArgs<T>, TResult> onReplace,
				Func<ISetOnClearArgs<T>, TResult> onClear,
				Func<ISetOnEmptyArgs, TResult> onEmpty)
			{
				onRemove.ArgumentNotNull("onRemove != null");
				return onRemove(this);
			}

			public override void Match(
				Action<ISetOnInsertArgs<T>> onInsert,
				Action<ISetOnRemoveArgs<T>> onRemove,
				Action<ISetOnReplaceArgs<T>> onReplace,
				Action<ISetOnClearArgs<T>> onClear,
				Action<ISetOnEmptyArgs> onEmpty)
			{
				onRemove.ArgumentNotNull("onRemove != null");
				onRemove(this);
			}

			public T Item
			{
				get { return _item; }
			}
		}

		private sealed class OnReplaceArgs : UpdateSetQuery<T>, ISetOnReplaceArgs<T>
		{
			private readonly T _oldItem;
			private readonly T _newItem;

			public T OldItem
			{
				get { return _oldItem; }
			}

			public T NewItem
			{
				get { return _newItem; }
			}

			public OnReplaceArgs(T oldItem, T newItem)
			{
				_oldItem = oldItem;
				_newItem = newItem;
			}

			public override TResult Match<TResult>(
				Func<ISetOnInsertArgs<T>, TResult> onInsert,
				Func<ISetOnRemoveArgs<T>, TResult> onRemove,
				Func<ISetOnReplaceArgs<T>, TResult> onReplace,
				Func<ISetOnClearArgs<T>, TResult> onClear,
				Func<ISetOnEmptyArgs, TResult> onEmpty)
			{
				onReplace.ArgumentNotNull("onReplace != null");
				return onReplace(this);
			}

			public override void Match(
				Action<ISetOnInsertArgs<T>> onInsert,
				Action<ISetOnRemoveArgs<T>> onRemove,
				Action<ISetOnReplaceArgs<T>> onReplace,
				Action<ISetOnClearArgs<T>> onClear,
				Action<ISetOnEmptyArgs> onEmpty)
			{
				onReplace.ArgumentNotNull("onReplace != null");
				onReplace(this);
			}
		}

		[NotNull]
		public static IUpdateSetQuery<T> OnClear([NotNull] IReadOnlyList<T> items)
		{
			items.ArgumentNotNull("items != null");
			return new OnClearArgs(items);
		}

		[NotNull]
		public static IUpdateSetQuery<T> OnEmpty()
		{
			return new OnEmptyArgs();
		}

		[NotNull]
		public static IUpdateSetQuery<T> OnInsert(T item)
		{
			return new OnInsertArgs(item);
		}

		[NotNull]
		public static IUpdateSetQuery<T> OnRemove(T item)
		{
			return new OnRemoveArgs(item);
		}

		[NotNull]
		public static IUpdateSetQuery<T> OnReplace(T oldItem, T newItem)
		{
			return new OnReplaceArgs(oldItem, newItem);
		} 

		public abstract TResult Match<TResult>(
			Func<ISetOnInsertArgs<T>, TResult> onInsert,
			Func<ISetOnRemoveArgs<T>, TResult> onRemove,
			Func<ISetOnReplaceArgs<T>, TResult> onReplace,
			Func<ISetOnClearArgs<T>, TResult> onClear,
			Func<ISetOnEmptyArgs, TResult> onEmpty);

		public abstract void Match(
			Action<ISetOnInsertArgs<T>> onInsert,
			Action<ISetOnRemoveArgs<T>> onRemove,
			Action<ISetOnReplaceArgs<T>> onReplace,
			Action<ISetOnClearArgs<T>> onClear,
			Action<ISetOnEmptyArgs> onEmpty);
	}
}
