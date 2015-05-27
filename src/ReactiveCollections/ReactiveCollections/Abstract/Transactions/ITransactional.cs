using System;
using JetBrains.Annotations;

namespace ReactiveCollections.Abstract.Transactions
{
	public interface ITransactional
	{
		[NotNull]
		IDisposable Transaction();
	}
}
