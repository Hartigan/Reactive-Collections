using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
		private readonly Collection<T> _collection;

		[NotNull]
		private readonly Subject<IUpdateCollectionQuery<T>> _subject;

		[CanBeNull]
		private Transaction<IUpdateCollectionQuery<T>, T> _transaction;

		public ObservableCollection()
		{
			_subject = new Subject<IUpdateCollectionQuery<T>>();
			_collection = new Collection<T>();
		}

		public ObservableCollection(IEnumerable<T> items)
		{
			_subject = new Subject<IUpdateCollectionQuery<T>>();
			_collection = new Collection<T>(items.ToList());
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _collection.GetEnumerator();
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
			get { return _collection.Count; }
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
			_collection.Add(item);
			ToTransaction(UpdateCollectionQuery<T>.OnInsert(item));
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			_collection.CopyTo(array, arrayIndex);
		}

		public bool Remove(T item)
		{
			if (_collection.Remove(item))
			{
				ToTransaction(UpdateCollectionQuery<T>.OnRemove(item));
				return true;
			}
			return false;
		}

		public void Clear()
		{
			var query = UpdateCollectionQuery<T>.OnClear(_collection.ToList());
			_collection.Clear();
			ToTransaction(query);
		}

		public bool Contains(T item)
		{
			return _collection.Contains(item);
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
