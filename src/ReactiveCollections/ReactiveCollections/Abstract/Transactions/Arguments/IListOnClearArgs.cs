using System.Collections.Generic;
using JetBrains.Annotations;

namespace ReactiveCollections.Abstract.Transactions.Arguments
{
	public interface IListOnClearArgs<out T>
	{
		[NotNull]
		IReadOnlyList<T> Items { get; } 
	}
}
