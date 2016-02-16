using FsCheck;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace ReactiveCollections.Tests
{
	[TestClass]
	public class ObservableCollectionWrapTest
	{
		private readonly Gen<int> _intGen;
		private readonly Gen<bool> _boolGen;

		public ObservableCollectionWrapTest()
		{
			_intGen = Arb.Generate<int>();
			_boolGen = Arb.Generate<bool>();
		}


	}
}