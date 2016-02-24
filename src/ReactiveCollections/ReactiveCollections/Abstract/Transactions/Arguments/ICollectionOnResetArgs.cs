using System.Collections.Generic;
using JetBrains.Annotations;

namespace ReactiveCollections.Abstract.Transactions.Arguments
{
	public interface ICollectionOnResetArgs<out T>
	{
		[NotNull]
		IReadOnlyList<T> OldItems { get; }

		[NotNull]
		IReadOnlyList<T> NewItems { get; } 
	}
}
