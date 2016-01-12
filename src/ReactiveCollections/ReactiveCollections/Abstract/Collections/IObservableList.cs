using System.Collections.Generic;

namespace ReactiveCollections.Abstract.Collections
{
	public interface IObservableList<T> : IObservableCollection<T>, IObservableReadOnlyList<T>, IList<T>
	{
		void Move(int oldIndex, int newIndex);

		new T this[int index] { get; set; }

		new int Count { get; }
	}
}
