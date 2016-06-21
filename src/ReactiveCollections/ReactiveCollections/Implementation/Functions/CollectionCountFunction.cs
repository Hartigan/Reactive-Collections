using System;
using JetBrains.Annotations;
using ReactiveCollections.Abstract.Transactions;
using ReactiveCollections.Abstract.Transactions.Arguments;
using ReactiveCollections.Domain;

namespace ReactiveCollections.Implementation.Functions
{
	public class CollectionCountFunction<T> : CollectionFunctionBase<T>, IObservableValue<int>
	{
		[NotNull]
		private readonly ObservableValue<int> _count;

		public CollectionCountFunction(
			[NotNull] IObservable<IUpdateCollectionQuery<T>> source)
		{
			_count = new ObservableValue<int>(0);
			Subscibe(source);
		}

		protected override void OnEmpty(ICollectionOnEmptyArgs<T> arg)
		{
		}

		protected override void OnReset(ICollectionOnResetArgs<T> arg)
		{
			_count.Value = arg.NewItems.Count;
		}

		protected override void OnReplace(ICollectionOnReplaceArgs<T> arg)
		{
		}

		protected override void OnRemove(ICollectionOnRemoveArgs<T> arg)
		{
			_count.Value--;
		}

		protected override void OnInsert(ICollectionOnInsertArgs<T> arg)
		{
			_count.Value++;
		}

		public int Value => _count.Value;

		public IObservable<ValueChangedArgs<int>> ValueChanged => _count.ValueChanged;
	}
}