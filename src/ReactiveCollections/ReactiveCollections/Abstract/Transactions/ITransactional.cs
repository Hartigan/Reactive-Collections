using System;

namespace ReactiveCollections.Abstract.Transactions
{
	public interface ITransactional
	{
		IDisposable Transaction();
	}
}
