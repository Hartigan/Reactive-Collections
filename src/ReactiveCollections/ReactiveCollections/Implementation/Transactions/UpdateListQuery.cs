using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using ReactiveCollections.Abstract.Transactions;
using ReactiveCollections.Abstract.Transactions.Arguments;
using ReactiveCollections.Extensions;

namespace ReactiveCollections.Implementation.Transactions
{
	public abstract class UpdateListQuery<T> : IUpdateListQuery<T>
	{
		private sealed class OnClearArgs : UpdateListQuery<T>, IListOnClearArgs<T>, ICollectionOnClearArgs<T>
		{
			[NotNull]
			private readonly IReadOnlyList<T> _items;

			public OnClearArgs([NotNull] IReadOnlyList<T> items)
			{
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

			public override TResult Match<TResult>(
				Func<IListOnInsertArgs<T>, TResult> onInsert,
				Func<IListOnRemoveArgs<T>, TResult> onRemove,
				Func<IListOnReplaceArgs<T>, TResult> onReplace,
				Func<IListOnMoveArgs<T>, TResult> onMove,
				Func<IListOnClearArgs<T>, TResult> onClear,
				Func<IListOnEmptyArgs, TResult> onEmpty)
			{
				onClear.ArgumentNotNull(nameof(onClear));
				return onClear(this);
			}

			public override void Match(
				Action<IListOnInsertArgs<T>> onInsert,
				Action<IListOnRemoveArgs<T>> onRemove,
				Action<IListOnReplaceArgs<T>> onReplace,
				Action<IListOnMoveArgs<T>> onMove,
				Action<IListOnClearArgs<T>> onClear,
				Action<IListOnEmptyArgs> onEmpty)
			{
				onClear.ArgumentNotNull(nameof(onClear));
				onClear(this);
			}

			IReadOnlyList<T> IListOnClearArgs<T>.Items => _items;

			IReadOnlyList<T> ICollectionOnClearArgs<T>.Items => _items;
		}

		private sealed class OnEmptyArgs : UpdateListQuery<T>, IListOnEmptyArgs
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

			public override TResult Match<TResult>(
				Func<IListOnInsertArgs<T>, TResult> onInsert,
				Func<IListOnRemoveArgs<T>, TResult> onRemove,
				Func<IListOnReplaceArgs<T>, TResult> onReplace,
				Func<IListOnMoveArgs<T>, TResult> onMove,
				Func<IListOnClearArgs<T>, TResult> onClear,
				Func<IListOnEmptyArgs, TResult> onEmpty)
			{
				onEmpty.ArgumentNotNull(nameof(onEmpty));
				return onEmpty(this);
			}

			public override void Match(
				Action<IListOnInsertArgs<T>> onInsert,
				Action<IListOnRemoveArgs<T>> onRemove,
				Action<IListOnReplaceArgs<T>> onReplace,
				Action<IListOnMoveArgs<T>> onMove,
				Action<IListOnClearArgs<T>> onClear,
				Action<IListOnEmptyArgs> onEmpty)
			{
				onEmpty.ArgumentNotNull(nameof(onEmpty));
				onEmpty(this);
			}
		}

		private sealed class OnInsertArgs : UpdateListQuery<T>, IListOnInsertArgs<T>, ICollectionOnInsertArgs<T>
		{
			private readonly int _index;
			private readonly T _item;

			public OnInsertArgs(int index, T item)
			{
				_index = index;
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

			public override TResult Match<TResult>(
				Func<IListOnInsertArgs<T>, TResult> onInsert,
				Func<IListOnRemoveArgs<T>, TResult> onRemove,
				Func<IListOnReplaceArgs<T>, TResult> onReplace,
				Func<IListOnMoveArgs<T>, TResult> onMove,
				Func<IListOnClearArgs<T>, TResult> onClear,
				Func<IListOnEmptyArgs, TResult> onEmpty)
			{
				onInsert.ArgumentNotNull(nameof(onInsert));
				return onInsert(this);
			}

			public override void Match(
				Action<IListOnInsertArgs<T>> onInsert,
				Action<IListOnRemoveArgs<T>> onRemove,
				Action<IListOnReplaceArgs<T>> onReplace,
				Action<IListOnMoveArgs<T>> onMove,
				Action<IListOnClearArgs<T>> onClear,
				Action<IListOnEmptyArgs> onEmpty)
			{
				onInsert.ArgumentNotNull(nameof(onInsert));
				onInsert(this);
			}

			T IListOnInsertArgs<T>.Item => _item;

			public int Index => _index;

			T ICollectionOnInsertArgs<T>.Item => _item;
		}

		private sealed class OnMoveArgs : UpdateListQuery<T>, IListOnMoveArgs<T>, ICollectionOnEmptyArgs
		{
			private readonly int _oldIndex;
			private readonly int _newIndex;
			private readonly T _item;

			public OnMoveArgs(int oldIndex, int newIndex, T item)
			{
				_oldIndex = oldIndex;
				_newIndex = newIndex;
				_item = item;
			}

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

			public override TResult Match<TResult>(
				Func<IListOnInsertArgs<T>, TResult> onInsert,
				Func<IListOnRemoveArgs<T>, TResult> onRemove,
				Func<IListOnReplaceArgs<T>, TResult> onReplace,
				Func<IListOnMoveArgs<T>, TResult> onMove,
				Func<IListOnClearArgs<T>, TResult> onClear,
				Func<IListOnEmptyArgs, TResult> onEmpty)
			{
				onMove.ArgumentNotNull(nameof(onMove));
				return onMove(this);
			}

			public override void Match(
				Action<IListOnInsertArgs<T>> onInsert,
				Action<IListOnRemoveArgs<T>> onRemove,
				Action<IListOnReplaceArgs<T>> onReplace,
				Action<IListOnMoveArgs<T>> onMove,
				Action<IListOnClearArgs<T>> onClear,
				Action<IListOnEmptyArgs> onEmpty)
			{
				onMove.ArgumentNotNull(nameof(onMove));
				onMove(this);
			}

			public T Item => _item;

			public int OldIndex => _oldIndex;

			public int NewIndex => _newIndex;
		}

		private sealed class OnRemoveArgs : UpdateListQuery<T>, IListOnRemoveArgs<T>, ICollectionOnRemoveArgs<T>
		{
			private readonly T _item;
			private readonly int _index;

			T IListOnRemoveArgs<T>.Item => _item;

			public int Index => _index;

			T ICollectionOnRemoveArgs<T>.Item => _item;

			public OnRemoveArgs(T item, int index)
			{
				_item = item;
				_index = index;
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

			public override TResult Match<TResult>(
				Func<IListOnInsertArgs<T>, TResult> onInsert,
				Func<IListOnRemoveArgs<T>, TResult> onRemove,
				Func<IListOnReplaceArgs<T>, TResult> onReplace,
				Func<IListOnMoveArgs<T>, TResult> onMove,
				Func<IListOnClearArgs<T>, TResult> onClear,
				Func<IListOnEmptyArgs, TResult> onEmpty)
			{
				onRemove.ArgumentNotNull(nameof(onRemove));
				return onRemove(this);
			}

			public override void Match(
				Action<IListOnInsertArgs<T>> onInsert,
				Action<IListOnRemoveArgs<T>> onRemove,
				Action<IListOnReplaceArgs<T>> onReplace,
				Action<IListOnMoveArgs<T>> onMove,
				Action<IListOnClearArgs<T>> onClear,
				Action<IListOnEmptyArgs> onEmpty)
			{
				onRemove.ArgumentNotNull(nameof(onRemove));
				onRemove(this);
			}
		}

		private sealed class OnReplaceArgs : UpdateListQuery<T>, IListOnReplaceArgs<T>, ICollectionOnReplaceArgs<T>
		{
			private readonly T _oldItem;
			private readonly T _newItem;
			private readonly int _index;

			T IListOnReplaceArgs<T>.OldItem => _oldItem;

			T IListOnReplaceArgs<T>.NewItem => _newItem;

			public int Index => _index;

			T ICollectionOnReplaceArgs<T>.OldItem => _oldItem;

			T ICollectionOnReplaceArgs<T>.NewItem => _newItem;

			public OnReplaceArgs(T oldItem, T newItem, int index)
			{
				_oldItem = oldItem;
				_newItem = newItem;
				_index = index;
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

			public override TResult Match<TResult>(
				Func<IListOnInsertArgs<T>, TResult> onInsert,
				Func<IListOnRemoveArgs<T>, TResult> onRemove,
				Func<IListOnReplaceArgs<T>, TResult> onReplace,
				Func<IListOnMoveArgs<T>, TResult> onMove,
				Func<IListOnClearArgs<T>, TResult> onClear,
				Func<IListOnEmptyArgs, TResult> onEmpty)
			{
				onReplace.ArgumentNotNull(nameof(onReplace));
				return onReplace(this);
			}

			public override void Match(
				Action<IListOnInsertArgs<T>> onInsert,
				Action<IListOnRemoveArgs<T>> onRemove,
				Action<IListOnReplaceArgs<T>> onReplace,
				Action<IListOnMoveArgs<T>> onMove,
				Action<IListOnClearArgs<T>> onClear,
				Action<IListOnEmptyArgs> onEmpty)
			{
				onReplace.ArgumentNotNull(nameof(onReplace));
				onReplace(this);
			}
		}

		[NotNull]
		public static IUpdateListQuery<T> OnClear([NotNull] IReadOnlyList<T> items)
		{
			items.ArgumentNotNull(nameof(items));
			return new OnClearArgs(items);
		}

		[NotNull]
		public static IUpdateListQuery<T> OnEmpty()
		{
			return new OnEmptyArgs();
		}

		[NotNull]
		public static IUpdateListQuery<T> OnInsert(T item, int index)
		{
			return new OnInsertArgs(index, item);
		}

		[NotNull]
		public static IUpdateListQuery<T> OnMove(T item, int oldIndex, int newIndex)
		{
			return new OnMoveArgs(oldIndex, newIndex, item);
		}

		[NotNull]
		public static IUpdateListQuery<T> OnRemove(T item, int index)
		{
			return new OnRemoveArgs(item, index);
		}

		[NotNull]
		public static IUpdateListQuery<T> OnReplace(T oldItem, T newItem, int index)
		{
			return new OnReplaceArgs(oldItem, newItem, index);
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

		public abstract TResult Match<TResult>(
			Func<IListOnInsertArgs<T>, TResult> onInsert,
			Func<IListOnRemoveArgs<T>, TResult> onRemove,
			Func<IListOnReplaceArgs<T>, TResult> onReplace,
			Func<IListOnMoveArgs<T>, TResult> onMove,
			Func<IListOnClearArgs<T>, TResult> onClear,
			Func<IListOnEmptyArgs, TResult> onEmpty);

		public abstract void Match(
			Action<IListOnInsertArgs<T>> onInsert,
			Action<IListOnRemoveArgs<T>> onRemove,
			Action<IListOnReplaceArgs<T>> onReplace,
			Action<IListOnMoveArgs<T>> onMove,
			Action<IListOnClearArgs<T>> onClear,
			Action<IListOnEmptyArgs> onEmpty);
	}
}
