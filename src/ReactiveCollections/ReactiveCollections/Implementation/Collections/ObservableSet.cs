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
	public class ObservableSet<T> : IObservableSet<T>
	{
		[NotNull]
		private readonly List<T> _set;

		[NotNull]
		private readonly Subject<IEnumerable<IUpdateSetQuery<T>>> _subject;

		[CanBeNull]
		private Transaction<IUpdateSetQuery<T>, T> _transaction;

		public ObservableSet()
		{
			_subject = new Subject<IEnumerable<IUpdateSetQuery<T>>>();
			_set = new List<T>();
		}

		public ObservableSet(IEnumerable<T> items)
		{
			_subject = new Subject<IEnumerable<IUpdateSetQuery<T>>>();
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

		public IObservable<IEnumerable<IUpdateSetQuery<T>>> SetChanged
		{
			get { return _subject; }
		}

		public int Count
		{
			get { return _set.Count; }
		}

		public IDisposable Transaction()
		{
			if (_transaction != null)
			{
				throw new InvalidOperationException();
			}

			return _transaction = new Transaction<IUpdateSetQuery<T>, T>(_subject, DeleteTransaction);
		}

		public void Add(T item)
		{
			ToTransaction(UpdateSetQuery<T>.OnInsert(item));
			_set.Add(item);
		}

		public bool Remove(T item)
		{
			if (_set.Remove(item))
			{
				ToTransaction(UpdateSetQuery<T>.OnRemove(item));
				return true;
			}
			return false;
		}

		public void Clear()
		{
			ToTransaction(UpdateSetQuery<T>.OnClear(_set.ToList()));
			_set.Clear();
		}

		private void DeleteTransaction()
		{
			_transaction = null;
		}

		private void ToTransaction([NotNull] IUpdateSetQuery<T> update)
		{
			if (_transaction != null)
			{
				_transaction.AddQuery(update);
			}
		}
	}
}
