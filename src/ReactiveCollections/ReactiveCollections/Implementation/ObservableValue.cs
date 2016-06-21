using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using JetBrains.Annotations;
using ReactiveCollections.Domain;

namespace ReactiveCollections.Implementation
{
	public class ObservableValue<T> : IObservableValue<T>
	{
		private T _value;

		[NotNull]
		private readonly Subject<ValueChangedArgs<T>> _subject;

		public ObservableValue(T value)
		{
			_subject = new Subject<ValueChangedArgs<T>>();
			_value = value;
		}

		public T Value
		{
			get { return _value; }
			set
			{
				if (!EqualityComparer<T>.Default.Equals(_value, value))
				{
					var args = new ValueChangedArgs<T>(_value, value);
					_value = value;
					_subject.OnNext(args);
				}
			}
		}

		[NotNull]
		public IObservable<ValueChangedArgs<T>> ValueChanged => _subject;

		public static implicit operator T(ObservableValue<T> observableValue)
		{
			return observableValue.Value;
		}
	}
}