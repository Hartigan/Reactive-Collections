using System;

namespace ReactiveCollections.Domain
{
	public interface IObservableValue<T>
	{
		T Value { get; }

		IObservable<ValueChangedArgs<T>> ValueChanged { get; }
	}
}