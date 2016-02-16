using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using JetBrains.Annotations;
using ReactiveCollections.Abstract.Collections;
using ReactiveCollections.Abstract.Transactions;
using ReactiveCollections.Implementation.Transactions;

namespace ReactiveCollections.Implementation.Collections
{
	public class ObservableList<T> : IObservableList<T>
	{
		[NotNull]
		private readonly List<T> _list;

		[NotNull]
		private readonly Subject<IUpdateListQuery<T>> _subject;

		[CanBeNull]
		private Transaction<IUpdateListQuery<T>, T> _transaction;

		public ObservableList()
		{
			_subject = new Subject<IUpdateListQuery<T>>();
			_list = new List<T>();
		}

		public ObservableList(IList<T> items)
		{
			_subject = new Subject<IUpdateListQuery<T>>();
			_list = new List<T>(items.ToList());
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _list.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IObservable<IUpdateCollectionQuery<T>> CollectionChanged => _subject;

		public void Add(T item)
		{
			_list.Add(item);
			ToTransaction(UpdateListQuery<T>.OnInsert(item, _list.Count - 1));
		}

		public void Clear()
		{
			var query = UpdateListQuery<T>.OnClear(_list.ToList());
			_list.Clear();
			ToTransaction(query);
		}

		public bool Contains(T item)
		{
			return _list.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			_list.CopyTo(array, arrayIndex);
		}

		public bool Remove(T item)
		{
			int index = _list.IndexOf(item);

			if (index >= 0)
			{
				_list.RemoveAt(index);
				ToTransaction(UpdateListQuery<T>.OnRemove(item, index));
				return true;
			}
			return false;
		}

		int IObservableCollection<T>.Count => _list.Count;

		public bool Replace(T oldItem, T newItem)
		{
			int index = _list.IndexOf(oldItem);

			if (index >= 0)
			{
				_list.RemoveAt(index);
				_list.Insert(index, newItem);
				ToTransaction(UpdateListQuery<T>.OnReplace(oldItem, newItem, index));
				return true;
			}
			return false;
		}

		int ICollection<T>.Count => _list.Count;

		public bool IsReadOnly => false;

		public int Count => _list.Count;

		public IDisposable Transaction()
		{
			if (_transaction != null)
			{
				throw new InvalidOperationException();
			}

			return _transaction = new Transaction<IUpdateListQuery<T>, T>(_subject, DeleteTransaction);
		}

		int IReadOnlyCollection<T>.Count => _list.Count;

		public void Move(int oldIndex, int newIndex)
		{
			if (oldIndex < 0 || oldIndex >= _list.Count || newIndex < 0 || newIndex >= _list.Count)
			{
				throw new InvalidOperationException();
			}

			T item = _list[oldIndex];
			_list.RemoveAt(oldIndex);
			_list.Insert(newIndex, item);
			ToTransaction(UpdateListQuery<T>.OnMove(item, oldIndex, newIndex));
		}

		public T this[int index]
		{
			get { return _list[index]; }
			set
			{
				T oldItem = _list[index];
				_list[index] = value;
				ToTransaction(UpdateListQuery<T>.OnReplace(oldItem, value, index));
			}
		}

		public int IndexOf(T item)
		{
			return _list.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			_list.Insert(index, item);
			ToTransaction(UpdateListQuery<T>.OnInsert(item, index));
		}

		public void RemoveAt(int index)
		{
			T item = _list[index];
			_list.RemoveAt(index);
			ToTransaction(UpdateListQuery<T>.OnRemove(item, index));
		}

		T IReadOnlyList<T>.this[int index] => _list[index];

		public IObservable<IUpdateListQuery<T>> ListChanged => _subject;

		int IObservableReadOnlyList<T>.Count => _list.Count;

		private void DeleteTransaction()
		{
			_transaction = null;
		}

		private void ToTransaction([NotNull] IUpdateListQuery<T> update)
		{
			if (_transaction != null)
			{
				_transaction.AddQuery(update);
			}
			else
			{
				_subject.OnNext(update);
			}
		}
	}
}
