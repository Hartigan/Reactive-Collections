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
		private sealed class OnResetArgs : UpdateListQuery<T>, IListOnResetArgs<T>
		{
			public OnResetArgs(IReadOnlyList<T> oldItems, IReadOnlyList<T> newItems)
			{
				oldItems.ArgumentNotNull(nameof(oldItems));
				newItems.ArgumentNotNull(nameof(newItems));

				OldItems = oldItems;
				NewItems = newItems;
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

			public override TResult Match<TResult>(
				Func<IListOnInsertArgs<T>, TResult> onInsert,
				Func<IListOnRemoveArgs<T>, TResult> onRemove,
				Func<IListOnReplaceArgs<T>, TResult> onReplace,
				Func<IListOnMoveArgs<T>, TResult> onMove,
				Func<IListOnResetArgs<T>, TResult> onReset,
				Func<IListOnEmptyArgs<T>, TResult> onEmpty)
			{
				onReset.ArgumentNotNull(nameof(onReset));
				return onReset(this);
			}

			public override void Match(
				Action<IListOnInsertArgs<T>> onInsert,
				Action<IListOnRemoveArgs<T>> onRemove,
				Action<IListOnReplaceArgs<T>> onReplace,
				Action<IListOnMoveArgs<T>> onMove,
				Action<IListOnResetArgs<T>> onReset,
				Action<IListOnEmptyArgs<T>> onEmpty)
			{
				onReset.ArgumentNotNull(nameof(onReset));
				onReset(this);
			}

			public IReadOnlyList<T> OldItems { get; }

			public IReadOnlyList<T> NewItems { get; }
		}

		private sealed class OnEmptyArgs : UpdateListQuery<T>, IListOnEmptyArgs<T>
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

			public override TResult Match<TResult>(
				Func<IListOnInsertArgs<T>, TResult> onInsert,
				Func<IListOnRemoveArgs<T>, TResult> onRemove,
				Func<IListOnReplaceArgs<T>, TResult> onReplace,
				Func<IListOnMoveArgs<T>, TResult> onMove,
				Func<IListOnResetArgs<T>, TResult> onReset,
				Func<IListOnEmptyArgs<T>, TResult> onEmpty)
			{
				onEmpty.ArgumentNotNull(nameof(onEmpty));
				return onEmpty(this);
			}

			public override void Match(
				Action<IListOnInsertArgs<T>> onInsert,
				Action<IListOnRemoveArgs<T>> onRemove,
				Action<IListOnReplaceArgs<T>> onReplace,
				Action<IListOnMoveArgs<T>> onMove,
				Action<IListOnResetArgs<T>> onReset,
				Action<IListOnEmptyArgs<T>> onEmpty)
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

			public override TResult Match<TResult>(
				Func<IListOnInsertArgs<T>, TResult> onInsert,
				Func<IListOnRemoveArgs<T>, TResult> onRemove,
				Func<IListOnReplaceArgs<T>, TResult> onReplace,
				Func<IListOnMoveArgs<T>, TResult> onMove,
				Func<IListOnResetArgs<T>, TResult> onReset,
				Func<IListOnEmptyArgs<T>, TResult> onEmpty)
			{
				onInsert.ArgumentNotNull(nameof(onInsert));
				return onInsert(this);
			}

			public override void Match(
				Action<IListOnInsertArgs<T>> onInsert,
				Action<IListOnRemoveArgs<T>> onRemove,
				Action<IListOnReplaceArgs<T>> onReplace,
				Action<IListOnMoveArgs<T>> onMove,
				Action<IListOnResetArgs<T>> onReset,
				Action<IListOnEmptyArgs<T>> onEmpty)
			{
				onInsert.ArgumentNotNull(nameof(onInsert));
				onInsert(this);
			}

			T IListOnInsertArgs<T>.Item => _item;

			public int Index => _index;

			T ICollectionOnInsertArgs<T>.Item => _item;
		}

		private sealed class OnMoveArgs : UpdateListQuery<T>, IListOnMoveArgs<T>, ICollectionOnEmptyArgs<T>
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

			public override TResult Match<TResult>(
				Func<IListOnInsertArgs<T>, TResult> onInsert,
				Func<IListOnRemoveArgs<T>, TResult> onRemove,
				Func<IListOnReplaceArgs<T>, TResult> onReplace,
				Func<IListOnMoveArgs<T>, TResult> onMove,
				Func<IListOnResetArgs<T>, TResult> onReset,
				Func<IListOnEmptyArgs<T>, TResult> onEmpty)
			{
				onMove.ArgumentNotNull(nameof(onMove));
				return onMove(this);
			}

			public override void Match(
				Action<IListOnInsertArgs<T>> onInsert,
				Action<IListOnRemoveArgs<T>> onRemove,
				Action<IListOnReplaceArgs<T>> onReplace,
				Action<IListOnMoveArgs<T>> onMove,
				Action<IListOnResetArgs<T>> onReset,
				Action<IListOnEmptyArgs<T>> onEmpty)
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

			public override TResult Match<TResult>(
				Func<IListOnInsertArgs<T>, TResult> onInsert,
				Func<IListOnRemoveArgs<T>, TResult> onRemove,
				Func<IListOnReplaceArgs<T>, TResult> onReplace,
				Func<IListOnMoveArgs<T>, TResult> onMove,
				Func<IListOnResetArgs<T>, TResult> onReset,
				Func<IListOnEmptyArgs<T>, TResult> onEmpty)
			{
				onRemove.ArgumentNotNull(nameof(onRemove));
				return onRemove(this);
			}

			public override void Match(
				Action<IListOnInsertArgs<T>> onInsert,
				Action<IListOnRemoveArgs<T>> onRemove,
				Action<IListOnReplaceArgs<T>> onReplace,
				Action<IListOnMoveArgs<T>> onMove,
				Action<IListOnResetArgs<T>> onReset,
				Action<IListOnEmptyArgs<T>> onEmpty)
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

			public override TResult Match<TResult>(
				Func<IListOnInsertArgs<T>, TResult> onInsert,
				Func<IListOnRemoveArgs<T>, TResult> onRemove,
				Func<IListOnReplaceArgs<T>, TResult> onReplace,
				Func<IListOnMoveArgs<T>, TResult> onMove,
				Func<IListOnResetArgs<T>, TResult> onReset,
				Func<IListOnEmptyArgs<T>, TResult> onEmpty)
			{
				onReplace.ArgumentNotNull(nameof(onReplace));
				return onReplace(this);
			}

			public override void Match(
				Action<IListOnInsertArgs<T>> onInsert,
				Action<IListOnRemoveArgs<T>> onRemove,
				Action<IListOnReplaceArgs<T>> onReplace,
				Action<IListOnMoveArgs<T>> onMove,
				Action<IListOnResetArgs<T>> onReset,
				Action<IListOnEmptyArgs<T>> onEmpty)
			{
				onReplace.ArgumentNotNull(nameof(onReplace));
				onReplace(this);
			}
		}

		[NotNull]
		public static IUpdateListQuery<T> OnReset([NotNull] IReadOnlyList<T> oldItems, [NotNull] IReadOnlyList<T> newItems)
		{
			return new OnResetArgs(oldItems, newItems);
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
			Func<ICollectionOnResetArgs<T>, TResult> onReset,
			Func<ICollectionOnEmptyArgs<T>, TResult> onEmpty);

		public abstract void Match(
			Action<ICollectionOnInsertArgs<T>> onInsert,
			Action<ICollectionOnRemoveArgs<T>> onRemove,
			Action<ICollectionOnReplaceArgs<T>> onReplace,
			Action<ICollectionOnResetArgs<T>> onReset,
			Action<ICollectionOnEmptyArgs<T>> onEmpty);

		public abstract TResult Match<TResult>(
			Func<IListOnInsertArgs<T>, TResult> onInsert,
			Func<IListOnRemoveArgs<T>, TResult> onRemove,
			Func<IListOnReplaceArgs<T>, TResult> onReplace,
			Func<IListOnMoveArgs<T>, TResult> onMove,
			Func<IListOnResetArgs<T>, TResult> onReset,
			Func<IListOnEmptyArgs<T>, TResult> onEmpty);

		public abstract void Match(
			Action<IListOnInsertArgs<T>> onInsert,
			Action<IListOnRemoveArgs<T>> onRemove,
			Action<IListOnReplaceArgs<T>> onReplace,
			Action<IListOnMoveArgs<T>> onMove,
			Action<IListOnResetArgs<T>> onReset,
			Action<IListOnEmptyArgs<T>> onEmpty);
	}
}
