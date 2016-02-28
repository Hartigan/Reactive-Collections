using System;
using System.Collections.Generic;
using System.Linq;
using FsCheck;
using FsCheck.Experimental;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using ReactiveCollections.Abstract.Collections;
using ReactiveCollections.Implementation.Collections;
using ReactiveCollections.Implementation.Operations;

namespace ReactiveCollections.Tests
{
	[TestClass]
	public class ObservableCollectionWrapTest
	{
		private readonly Gen<int> _intGen;
		private readonly Func<int, int> _selector;

		public ObservableCollectionWrapTest()
		{
			int count = 0;
			_intGen = Gen.Fresh(() => count++);
			_selector = x => x*2;
		}

		[TestMethod]
		public void Add()
		{
			IObservableCollection<int> collection = new ObservableCollection<int>();
			IObservableReadOnlyCollection<int> actualOperation = collection.SelectRc(_selector).SelectRc(_selector);
			IEnumerable<int> expectedOperation = collection.Select(_selector).Select(_selector);

			Action<int> assertAdd = item =>
			{
				collection.Add(item);
				Assert.IsTrue(Enumerable.SequenceEqual(expectedOperation, actualOperation));
			};

			Prop.ForAll(Arb.From(_intGen), assertAdd).QuickCheckThrowOnFailure();
		}

		[TestMethod]
		public void Remove()
		{
			IObservableCollection<int> collection = new ObservableCollection<int>();
			IObservableReadOnlyCollection<int> actualOperation = collection.SelectRc(_selector).SelectRc(_selector);
			IEnumerable<int> expectedOperation = collection.Select(_selector).Select(_selector);

			Action<int> assertAddAndRemove = item =>
			{
				collection.Add(item);
				Assert.IsTrue(Enumerable.SequenceEqual(expectedOperation, actualOperation));
				collection.Remove(item);
				Assert.IsTrue(Enumerable.SequenceEqual(expectedOperation, actualOperation));
			};

			Prop.ForAll(Arb.From(_intGen), assertAddAndRemove).QuickCheckThrowOnFailure();
		}

		[TestMethod]
		public void Clear()
		{
			IObservableCollection<int> collection = new ObservableCollection<int>();
			IObservableReadOnlyCollection<int> actualOperation = collection.SelectRc(_selector).SelectRc(_selector);
			IEnumerable<int> expectedOperation = collection.Select(_selector).Select(_selector);

			Action<int, int, int> assertAddAndClear = (item1, item2, item3) =>
			{
				collection.Add(item1);
				collection.Add(item2);
				collection.Add(item3);
				Assert.IsTrue(Enumerable.SequenceEqual(expectedOperation, actualOperation));
				collection.Clear();
				Assert.IsTrue(Enumerable.SequenceEqual(expectedOperation, actualOperation));
			};

			Prop.ForAll(Arb.From(_intGen), Arb.From(_intGen), Arb.From(_intGen), assertAddAndClear).QuickCheckThrowOnFailure();
		}

		[TestMethod]
		public void Replace()
		{
			IObservableCollection<int> collection = new ObservableCollection<int>();
			IObservableReadOnlyCollection<int> actualOperation = collection.SelectRc(_selector).SelectRc(_selector);
			IEnumerable<int> expectedOperation = collection.Select(_selector).Select(_selector);

			Action<int, int> assertAddAndReplace = (oldItem, newItem) =>
			{
				collection.Add(oldItem);
				Assert.IsTrue(Enumerable.SequenceEqual(expectedOperation, actualOperation));
				collection.Replace(oldItem, newItem);
				Assert.IsTrue(Enumerable.SequenceEqual(expectedOperation, actualOperation));
			};

			Prop.ForAll(Arb.From(_intGen), Arb.From(_intGen), assertAddAndReplace).QuickCheckThrowOnFailure();
		}
	}
}