using System.Collections.Generic;
using JetBrains.Annotations;
using ReactiveCollections.Abstract.Transactions;

namespace ReactiveCollections.Abstract.Collections
{
	public interface IObservableCollection<T> : IObservableReadOnlyCollection<T>, ICollection<T>, ITransactional
	{
		new int Count { get; }

		bool Replace(T oldItem, T newItem);

		void Reset([NotNull] IReadOnlyList<T> items);
	}
}
