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
	public class ObservableCollection<T> : IObservableCollection<T>
	{
		[NotNull]
		private readonly List<T> _set;

		[NotNull]
		private readonly Subject<IUpdateCollectionQuery<T>> _subject;

		[CanBeNull]
		private Transaction<IUpdateCollectionQuery<T>, T> _transaction;

		public ObservableCollection()
		{
			_subject = new Subject<IUpdateCollectionQuery<T>>();
			_set = new List<T>();
		}

		public ObservableCollection(IEnumerable<T> items)
		{
			_subject = new Subject<IUpdateCollectionQuery<T>>();
			_set = new List<T>(items);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _set.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public IObservable<IUpdateCollectionQuery<T>> CollectionChanged
		{
			get { return _subject; }
		}

		public int Count
		{
			get { return _set.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public IDisposable Transaction()
		{
			if (_transaction != null)
			{
				throw new InvalidOperationException();
			}

			return _transaction = new Transaction<IUpdateCollectionQuery<T>, T>(_subject, DeleteTransaction);
		}

		public void Add(T item)
		{
			_set.Add(item);
			ToTransaction(UpdateCollectionQuery<T>.OnInsert(item));
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			_set.CopyTo(array, arrayIndex);
		}

		public bool Remove(T item)
		{
			if (_set.Remove(item))
			{
				ToTransaction(UpdateCollectionQuery<T>.OnRemove(item));
				return true;
			}
			return false;
		}

		public void Clear()
		{
			var query = UpdateCollectionQuery<T>.OnClear(_set.ToList());
			_set.Clear();
			ToTransaction(query);
		}

		public bool Contains(T item)
		{
			return _set.Contains(item);
		}

		private void DeleteTransaction()
		{
			_transaction = null;
		}

		private void ToTransaction([NotNull] IUpdateCollectionQuery<T> update)
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
