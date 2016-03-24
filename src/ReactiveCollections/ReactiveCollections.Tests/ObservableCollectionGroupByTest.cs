using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using FsCheck;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using ReactiveCollections.Abstract.Collections;
using ReactiveCollections.Implementation;
using ReactiveCollections.Implementation.Collections;
using ReactiveCollections.Implementation.Operations;

namespace ReactiveCollections.Tests
{
	[TestClass]
	public class ObservableCollectionGroupByTest
	{
		private readonly Gen<int> _intGen;
		private readonly Gen<BehaviorSubject<int>> _valueGen;
		private readonly Gen<IReadOnlyList<BehaviorSubject<int>>> _valuesGen;
		private readonly Gen<IReadOnlyList<int>> _listGen;
		private readonly Func<int, int> _selector;

		public ObservableCollectionGroupByTest()
		{
			int count = 0;
			_intGen = Gen.Fresh(() => count++);
			_valueGen = _intGen.Select(x => new BehaviorSubject<int>(x));
			_valuesGen = _valueGen.ListOf(100).Select(x => (IReadOnlyList<BehaviorSubject<int>>)x.ToList());
			_listGen = _intGen.ListOf(100).Select(x => (IReadOnlyList<int>)x.ToList());
			_selector = x => x % 40;
		}

		private void CheckLookup([NotNull] ILookup<int, int> expected, [NotNull] IObservableLookup<int, int> actual)
		{
			var expectedKeys = expected.Select(x => x.Key);
			var actualKeys = actual.Select(x => x.Key);

			Check(expectedKeys, actualKeys);

			foreach (var expectedKey in expectedKeys)
			{
				Check(expected[expectedKey], actual[expectedKey]);
			}
		}

		private void CheckLookup([NotNull] ILookup<int, int> expected, [NotNull] IObservableLookup<int, BehaviorSubject<int>> actual)
		{
			var expectedKeys = expected.Select(x => x.Key);
			var actualKeys = actual.Select(x => x.Key);

			Check(expectedKeys, actualKeys);

			foreach (var expectedKey in expectedKeys)
			{
				Check(expected[expectedKey], actual[expectedKey].Select(x=>x.Value));
			}
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
			var actualOperation = collection.GroupByRc(_selector, x => Observable.Never<int>());

			var actualSelectMany = actualOperation.SelectMany(x => x);
			var expectedSelectMany = collection;

			Action<IReadOnlyList<int>> assertAdd = items =>
			{
				foreach (var item in items)
				{
					collection.Add(item);
				}
				
				var expectedOperation = collection.ToLookup(_selector, x => x);
				CheckLookup(expectedOperation, actualOperation);
				Check(expectedSelectMany, actualSelectMany);
			};

			Prop.ForAll(Arb.From(_listGen), assertAdd).QuickCheckThrowOnFailure();
		}

		[TestMethod]
		public void ItemChanged()
		{
			IObservableCollection<BehaviorSubject<int>> collection = new ObservableCollection<BehaviorSubject<int>>();
			var actualOperation = collection.GroupByRc(x => _selector(x.Value), x => x.Select(y => x));

			var actualSelectMany = actualOperation.SelectMany(x => x).Select(x => x.Value);
			var expectedSelectMany = collection.Select(x => x.Value);

			Action<IReadOnlyList<BehaviorSubject<int>>> assertItemChanged = items =>
			{
				collection.Reset(items);

				var expectedOperation = collection.ToLookup(x => _selector(x.Value), x => x.Value);
				CheckLookup(expectedOperation, actualOperation);
				Check(expectedSelectMany, actualSelectMany);

				foreach (var item in items)
				{
					item.OnNext(item.Value + 1);

					expectedOperation = collection.ToLookup(x => _selector(x.Value), x => x.Value);
					CheckLookup(expectedOperation, actualOperation);
					Check(expectedSelectMany, actualSelectMany);
				}
			};

			Prop.ForAll(Arb.From(_valuesGen), assertItemChanged).QuickCheckThrowOnFailure();
		}

		[TestMethod]
		public void Remove()
		{
			IObservableCollection<int> collection = new ObservableCollection<int>();
			var actualOperation = collection.GroupByRc(_selector, x => Observable.Never<int>());

			var actualSelectMany = actualOperation.SelectMany(x => x);
			var expectedSelectMany = collection;

			Action<IReadOnlyList<int>> assertAddAndRemove = items =>
			{
				foreach (var item in items)
				{
					collection.Add(item);
				}

				foreach (var item in items)
				{
					collection.Remove(item);
					var expectedOperation = collection.ToLookup(_selector, x => x);
					CheckLookup(expectedOperation, actualOperation);
					Check(expectedSelectMany, actualSelectMany);
				}
			};

			Prop.ForAll(Arb.From(_listGen), assertAddAndRemove).QuickCheckThrowOnFailure();
		}

		[TestMethod]
		public void Clear()
		{
			IObservableCollection<int> collection = new ObservableCollection<int>();
			var actualOperation = collection.GroupByRc(_selector, x => Observable.Never<int>());

			var actualSelectMany = actualOperation.SelectMany(x => x);
			var expectedSelectMany = collection;

			Action<IReadOnlyList<int>> assertAddAndClear = items =>
			{
				foreach (var item in items)
				{
					collection.Add(item);
				}
				var expectedOperation = collection.ToLookup(_selector, x => x);
				CheckLookup(expectedOperation, actualOperation);
				Check(expectedSelectMany, actualSelectMany);

				collection.Clear();
				expectedOperation = collection.ToLookup(_selector, x => x);
				CheckLookup(expectedOperation, actualOperation);
				Check(expectedSelectMany, actualSelectMany);
			};

			Prop.ForAll(Arb.From(_listGen), assertAddAndClear).QuickCheckThrowOnFailure();
		}

		[TestMethod]
		public void Replace()
		{
			IObservableCollection<int> collection = new ObservableCollection<int>();
			var actualOperation = collection.GroupByRc(_selector, x => Observable.Never<int>());

			var actualSelectMany = actualOperation.SelectMany(x => x);
			var expectedSelectMany = collection;

			Action<IReadOnlyList <int>, IReadOnlyList <int>> assertAddAndReplace = (oldItems, newItems) =>
			{
				foreach (var item in oldItems)
				{
					collection.Add(item);
				}
				var expectedOperation = collection.ToLookup(_selector, x => x);
				CheckLookup(expectedOperation, actualOperation);
				Check(expectedSelectMany, actualSelectMany);

				for (int i = 0; i < Math.Min(oldItems.Count, newItems.Count); i++)
				{
					collection.Replace(oldItems[i], newItems[i]);

					expectedOperation = collection.ToLookup(_selector, x => x);
					CheckLookup(expectedOperation, actualOperation);
					Check(expectedSelectMany, actualSelectMany);
				}
			};

			Prop.ForAll(Arb.From(_listGen), Arb.From(_listGen), assertAddAndReplace).QuickCheckThrowOnFailure();
		}

		[TestMethod]
		public void Reset()
		{
			IObservableCollection<int> collection = new ObservableCollection<int>();
			var actualOperation = collection.GroupByRc(_selector, x => Observable.Never<int>());

			var actualSelectMany = actualOperation.SelectMany(x => x);
			var expectedSelectMany = collection;

			Action<IReadOnlyList<int>, IReadOnlyList<int>> assertReset = (oldItems, newItems) =>
			{
				collection.Reset(oldItems);

				var expectedOperation = collection.ToLookup(_selector, x => x);
				CheckLookup(expectedOperation, actualOperation);
				Check(expectedSelectMany, actualSelectMany);

				collection.Reset(newItems);

				expectedOperation = collection.ToLookup(_selector, x => x);
				CheckLookup(expectedOperation, actualOperation);
				Check(expectedSelectMany, actualSelectMany);
			};

			Prop.ForAll(Arb.From(_listGen), Arb.From(_listGen), assertReset).QuickCheckThrowOnFailure();
		}
	}
}