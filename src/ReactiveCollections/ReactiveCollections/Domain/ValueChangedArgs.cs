using System;

namespace ReactiveCollections.Domain
{
	public class ValueChangedArgs<T> : EventArgs
	{
		public ValueChangedArgs(T oldValue, T newValue)
		{
			OldValue = oldValue;
			NewValue = newValue;
		}

		public T OldValue { get; }

		public T NewValue { get; }
	}
}