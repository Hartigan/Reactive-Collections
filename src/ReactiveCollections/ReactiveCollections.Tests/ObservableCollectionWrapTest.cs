using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using FsCheck;
using FsCheck.Experimental;
using JetBrains.Annotations;
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

		private void Check([NotNull] IEnumerable<int> expected, [NotNull] IEnumerable<int> actual)
		{
			var expectedList = expected.ToList();
			expectedList.Sort();
			var actualList = actual.ToList();
			actualList.Sort();
			Assert.IsTrue(Enumerable.SequenceEqual(expectedList, actualList));
		}

		[TestMethod]
		public void Add()
		{
			IObservableCollection<int> collection = new ObservableCollection<int>();
			IObservableReadOnlyCollection<int> actualOperation = collection
				.SelectRc(_selector, _ => Observable.Never<int>())
				.SelectRc(_selector, _ => Observable.Never<int>());
			IEnumerable<int> expectedOperation = collection.Select(_selector).Select(_selector);

			Action<int> assertAdd = item =>
			{
				collection.Add(item);
				Check(expectedOperation, actualOperation);
			};

			Prop.ForAll(Arb.From(_intGen), assertAdd).QuickCheckThrowOnFailure();
		}

		[TestMethod]
		public void ItemChanged()
		{
			Func<BehaviorSubject<int>, int> subjectToIntSelector = x => x.Value*2;
			IObservableCollection<BehaviorSubject<int>> collection = new ObservableCollection<BehaviorSubject<int>>();
			IObservableReadOnlyCollection<int> actualOperation = collection
				.SelectRc(subjectToIntSelector, x => x.Select(_ => x))
				.SelectRc(x => x, _ => Observable.Never<int>());
			IEnumerable<int> expectedOperation = collection.Select(subjectToIntSelector).Select(x => x);

			Action<BehaviorSubject<int>> assertAdd = item =>
			{
				collection.Add(item);
				Check(expectedOperation, actualOperation);
				item.OnNext(item.Value + 1);
				Check(expectedOperation, actualOperation);
				item.OnNext(item.Value + 1);
				Check(expectedOperation, actualOperation);
			};

			Prop.ForAll(Arb.From(_intGen.Select(x => new BehaviorSubject<int>(x))), assertAdd).QuickCheckThrowOnFailure();
		}

		[TestMethod]
		public void Remove()
		{
			IObservableCollection<int> collection = new ObservableCollection<int>();
			IObservableReadOnlyCollection<int> actualOperation = collection
				.SelectRc(_selector, _ => Observable.Never<int>())
				.SelectRc(_selector, _ => Observable.Never<int>());
			IEnumerable<int> expectedOperation = collection.Select(_selector).Select(_selector);

			Action<int> assertAddAndRemove = item =>
			{
				collection.Add(item);
				Check(expectedOperation, actualOperation);
				collection.Remove(item);
				Check(expectedOperation, actualOperation);
			};

			Prop.ForAll(Arb.From(_intGen), assertAddAndRemove).QuickCheckThrowOnFailure();
		}

		[TestMethod]
		public void Clear()
		{
			IObservableCollection<int> collection = new ObservableCollection<int>();
			IObservableReadOnlyCollection<int> actualOperation = collection
				.SelectRc(_selector, _ => Observable.Never<int>())
				.SelectRc(_selector, _ => Observable.Never<int>());
			IEnumerable<int> expectedOperation = collection.Select(_selector).Select(_selector);

			Action<int, int, int> assertAddAndClear = (item1, item2, item3) =>
			{
				collection.Add(item1);
				collection.Add(item2);
				collection.Add(item3);
				Check(expectedOperation, actualOperation);
				collection.Clear();
				Check(expectedOperation, actualOperation);
			};

			Prop.ForAll(Arb.From(_intGen), Arb.From(_intGen), Arb.From(_intGen), assertAddAndClear).QuickCheckThrowOnFailure();
		}

		[TestMethod]
		public void Replace()
		{
			IObservableCollection<int> collection = new ObservableCollection<int>();
			IObservableReadOnlyCollection<int> actualOperation = collection
				.SelectRc(_selector, _ => Observable.Never<int>())
				.SelectRc(_selector, _ => Observable.Never<int>());
			IEnumerable<int> expectedOperation = collection.Select(_selector).Select(_selector);

			Action<int, int> assertAddAndReplace = (oldItem, newItem) =>
			{
				collection.Add(oldItem);
				Check(expectedOperation, actualOperation);
				collection.Replace(oldItem, newItem);
				Check(expectedOperation, actualOperation);
			};

			Prop.ForAll(Arb.From(_intGen), Arb.From(_intGen), assertAddAndReplace).QuickCheckThrowOnFailure();
		}

		[TestMethod]
		public void Reset()
		{
			IObservableCollection<int> collection = new ObservableCollection<int>();
			IObservableReadOnlyCollection<int> actualOperation = collection
				.SelectRc(_selector, _ => Observable.Never<int>())
				.SelectRc(_selector, _ => Observable.Never<int>());
			IEnumerable<int> expectedOperation = collection.Select(_selector).Select(_selector);

			Action<IReadOnlyList<int>, IReadOnlyList<int>> assertReset = (oldItems, newItems) =>
			{
				collection.Reset(oldItems);
				Check(expectedOperation, actualOperation);
				collection.Reset(newItems);
				Check(expectedOperation, actualOperation);
			};

			Prop.ForAll(
				Arb.From(_intGen.ListOf(10).Select(x => x.ToList())),
				Arb.From(_intGen.ListOf(10).Select(x => x.ToList())),
				assertReset)
				.QuickCheckThrowOnFailure();
		}
	}
}