using FsCheck;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using ReactiveCollections.Abstract.Collections;
using ReactiveCollections.Implementation.Collections;
using System;

namespace ReactiveCollections.Tests
{
	[TestClass]
	public class ObservableCollectionTest
	{
		private readonly Gen<int> _intGen;

		public ObservableCollectionTest()
		{
			_intGen = Arb.Generate<int>();
		}

		[TestMethod]
		public void Add()
		{
			IObservableCollection<int> collection = new ObservableCollection<int>();

			Action<int> assertAdd = item =>
			{
				int sourceCount = collection.Count;
				collection.Add(item);

				Assert.AreEqual(sourceCount + 1, collection.Count);
				Assert.IsTrue(collection.Contains(item));
			};

			Prop.ForAll(Arb.From(_intGen), assertAdd).QuickCheckThrowOnFailure();
		}
	}
}
