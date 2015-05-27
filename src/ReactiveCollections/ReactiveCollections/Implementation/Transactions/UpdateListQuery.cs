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
		private sealed class OnClearArgs : UpdateListQuery<T>, IListOnClearArgs<T>, ISetOnClearArgs<T>
		{
			[NotNull]
			private readonly IReadOnlyList<T> _items;

			public OnClearArgs([NotNull] IReadOnlyList<T> items)
			{
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

			public override TResult Match<TResult>(
				Func<IListOnInsertArgs<T>, TResult> onInsert,
				Func<IListOnRemoveArgs<T>, TResult> onRemove,
				Func<IListOnReplaceArgs<T>, TResult> onReplace,
				Func<IListOnMoveArgs<T>, TResult> onMove,
				Func<IListOnClearArgs<T>, TResult> onClear,
				Func<IListOnEmptyArgs, TResult> onEmpty)
			{
				onClear.ArgumentNotNull("onClear != null");
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
				onClear.ArgumentNotNull("onClear != null");
				onClear(this);
			}

			IReadOnlyList<T> IListOnClearArgs<T>.Items
			{
				get { return _items; }
			}

			IReadOnlyList<T> ISetOnClearArgs<T>.Items
			{
				get { return _items; }
			}
		}

		private sealed class OnEmptyArgs : UpdateListQuery<T>, IListOnEmptyArgs
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

			public override TResult Match<TResult>(
				Func<IListOnInsertArgs<T>, TResult> onInsert,
				Func<IListOnRemoveArgs<T>, TResult> onRemove,
				Func<IListOnReplaceArgs<T>, TResult> onReplace,
				Func<IListOnMoveArgs<T>, TResult> onMove,
				Func<IListOnClearArgs<T>, TResult> onClear,
				Func<IListOnEmptyArgs, TResult> onEmpty)
			{
				onEmpty.ArgumentNotNull("onEmpty != null");
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
				onEmpty.ArgumentNotNull("onEmpty != null");
				onEmpty(this);
			}
		}

		private sealed class OnInsertArgs : UpdateListQuery<T>, IListOnInsertArgs<T>, ISetOnInsertArgs<T>
		{
			private readonly int _index;
			private readonly T _item;

			public OnInsertArgs(int index, T item)
			{
				_index = index;
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

			public override TResult Match<TResult>(
				Func<IListOnInsertArgs<T>, TResult> onInsert,
				Func<IListOnRemoveArgs<T>, TResult> onRemove,
				Func<IListOnReplaceArgs<T>, TResult> onReplace,
				Func<IListOnMoveArgs<T>, TResult> onMove,
				Func<IListOnClearArgs<T>, TResult> onClear,
				Func<IListOnEmptyArgs, TResult> onEmpty)
			{
				onInsert.ArgumentNotNull("onInsert != null");
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
				onInsert.ArgumentNotNull("onInsert != null");
				onInsert(this);
			}

			T IListOnInsertArgs<T>.Item
			{
				get { return _item; }
			}

			public int Index
			{
				get { return _index; }
			}

			T ISetOnInsertArgs<T>.Item
			{
				get { return _item; }
			}
		}

		private sealed class OnMoveArgs : UpdateListQuery<T>, IListOnMoveArgs<T>, ISetOnEmptyArgs
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

			public override TResult Match<TResult>(
				Func<IListOnInsertArgs<T>, TResult> onInsert,
				Func<IListOnRemoveArgs<T>, TResult> onRemove,
				Func<IListOnReplaceArgs<T>, TResult> onReplace,
				Func<IListOnMoveArgs<T>, TResult> onMove,
				Func<IListOnClearArgs<T>, TResult> onClear,
				Func<IListOnEmptyArgs, TResult> onEmpty)
			{
				onMove.ArgumentNotNull("onMove != null");
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
				onMove.ArgumentNotNull("onMove != null");
				onMove(this);
			}

			public T Item
			{
				get { return _item; }
			}

			public int OldIndex
			{
				get { return _oldIndex; }
			}

			public int NewIndex
			{
				get { return _newIndex; }
			}
		}

		private sealed class OnRemoveArgs : UpdateListQuery<T>, IListOnRemoveArgs<T>, ISetOnRemoveArgs<T>
		{
			private readonly T _item;
			private readonly int _index;

			T IListOnRemoveArgs<T>.Item
			{
				get { return _item; }
			}

			public int Index
			{
				get { return _index; }
			}

			T ISetOnRemoveArgs<T>.Item
			{
				get { return _item; }
			}

			public OnRemoveArgs(T item, int index)
			{
				_item = item;
				_index = index;
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

			public override TResult Match<TResult>(
				Func<IListOnInsertArgs<T>, TResult> onInsert,
				Func<IListOnRemoveArgs<T>, TResult> onRemove,
				Func<IListOnReplaceArgs<T>, TResult> onReplace,
				Func<IListOnMoveArgs<T>, TResult> onMove,
				Func<IListOnClearArgs<T>, TResult> onClear,
				Func<IListOnEmptyArgs, TResult> onEmpty)
			{
				onRemove.ArgumentNotNull("onRemove != null");
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
				onRemove.ArgumentNotNull("onRemove != null");
				onRemove(this);
			}
		}

		private sealed class OnReplaceArgs : UpdateListQuery<T>, IListOnReplaceArgs<T>, ISetOnReplaceArgs<T>
		{
			private readonly T _oldItem;
			private readonly T _newItem;
			private readonly int _index;

			T IListOnReplaceArgs<T>.OldItem
			{
				get { return _oldItem; }
			}

			T IListOnReplaceArgs<T>.NewItem
			{
				get { return _newItem; }
			}

			public int Index
			{
				get { return _index; }
			}

			T ISetOnReplaceArgs<T>.OldItem
			{
				get { return _oldItem; }
			}

			T ISetOnReplaceArgs<T>.NewItem
			{
				get { return _newItem; }
			}

			public OnReplaceArgs(T oldItem, T newItem, int index)
			{
				_oldItem = oldItem;
				_newItem = newItem;
				_index = index;
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

			public override TResult Match<TResult>(
				Func<IListOnInsertArgs<T>, TResult> onInsert,
				Func<IListOnRemoveArgs<T>, TResult> onRemove,
				Func<IListOnReplaceArgs<T>, TResult> onReplace,
				Func<IListOnMoveArgs<T>, TResult> onMove,
				Func<IListOnClearArgs<T>, TResult> onClear,
				Func<IListOnEmptyArgs, TResult> onEmpty)
			{
				onReplace.ArgumentNotNull("onReplace != null");
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
				onReplace.ArgumentNotNull("onReplace != null");
				onReplace(this);
			}
		}

		[NotNull]
		public static IUpdateListQuery<T> OnClear([NotNull] IReadOnlyList<T> items)
		{
			items.ArgumentNotNull("items != null");
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
