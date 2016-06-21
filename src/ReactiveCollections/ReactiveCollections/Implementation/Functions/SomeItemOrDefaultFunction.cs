using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ReactiveCollections.Abstract.Transactions.Arguments;
using ReactiveCollections.Domain;
using JetBrains.Annotations;
using ReactiveCollections.Abstract.Transactions;

namespace ReactiveCollections.Implementation.Functions
{
	public sealed class SomeItemOrDefaultFunction<T> : CollectionFunctionBase<T>, IObservableValue<T>
	{
		[NotNull]
		private readonly ObservableValue<T> _value = new ObservableValue<T>(default(T)); 

		[NotNull]
		private readonly Collection<T> _data = new Collection<T>();

		public SomeItemOrDefaultFunction([NotNull] IObservable<IUpdateCollectionQuery<T>> source)
		{
			Subscibe(source);
		}

		protected override void OnEmpty(ICollectionOnEmptyArgs<T> arg)
		{
		}

		protected override void OnReset(ICollectionOnResetArgs<T> arg)
		{
			_data.Clear();
			foreach (var item in arg.NewItems)
			{
				_data.Add(item);
			}
			_value.Value = _data.FirstOrDefault();
		}

		protected override void OnReplace(ICollectionOnReplaceArgs<T> arg)
		{
			_data.Remove(arg.OldItem);
			_data.Add(arg.NewItem);

			if (EqualityComparer<T>.Default.Equals(_value.Value, arg.OldItem))
			{
				_value.Value = _data.FirstOrDefault();
			}
		}

		protected override void OnRemove(ICollectionOnRemoveArgs<T> arg)
		{
			_data.Remove(arg.Item);

			if (EqualityComparer<T>.Default.Equals(_value.Value, arg.Item))
			{
				_value.Value = _data.FirstOrDefault();
			}
		}

		protected override void OnInsert(ICollectionOnInsertArgs<T> arg)
		{
			_data.Add(arg.Item);

			if (_data.Count == 1)
			{
				_value.Value = _data.FirstOrDefault();
			}
		}

		public T Value => _value.Value;

		public IObservable<ValueChangedArgs<T>> ValueChanged => _value.ValueChanged;
	}
}