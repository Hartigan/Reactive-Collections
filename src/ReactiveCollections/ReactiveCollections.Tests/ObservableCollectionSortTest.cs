using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using FsCheck;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using ReactiveCollections.Abstract.Collections;
using ReactiveCollections.Implementation.Collections;
using ReactiveCollections.Implementation.Operations;

namespace ReactiveCollections.Tests
{
	[TestClass]
	public class ObservableCollectionSortTest
	{
		[NotNull]
		private readonly Gen<BehaviorSubject<int>> _intGen;
		[NotNull]
		private readonly Func<BehaviorSubject<int>, int> _selector;
		[NotNull]
		private readonly Func<BehaviorSubject<int>, bool> _filter;
		[NotNull]
		private readonly Func<BehaviorSubject<int>, IObservable<Unit>> _getUpdater;
		[NotNull]
		private readonly IComparer<int> _comparer;

		public ObservableCollectionSortTest()
		{
			int count = 0;
			int current = 1;
			int i = -1;


			_intGen = Gen.Fresh(() =>
			{
				count++;
				current *= i;
				return new BehaviorSubject<int>(count*current);
			});
			_selector = x => x.Value;
			_filter = x => x.Value % 3 != 0;
			_getUpdater = x => x.Select(_ => Unit.Default);
			_comparer = Comparer<int>.Default;
		}

		private void Check(
			[NotNull] IEnumerable<int> expected,
			[NotNull] IReadOnlyList<int> actual)
		{
			var expectedList = expected.ToList();
			expectedList.Sort(_comparer);
			Assert.IsTrue(Enumerable.SequenceEqual(expectedList, actual));
		}

		[TestMethod]
		public void Add()
		{
			IObservableCollection<BehaviorSubject<int>> collection = new ObservableCollection<BehaviorSubject<int>>();
			IObservableReadOnlyList<int> actualOperation = collection
				.WhereRc(_filter, _getUpdater)
				.SortRc(x => x.Value, _comparer, _getUpdater)
				.SelectRl(_selector);
			IEnumerable<int> expectedOperation = collection.Where(_filter).Select(_selector);

			Action<BehaviorSubject<int>> assertAdd = item =>
			{
				collection.Add(item);
				Check(expectedOperation, actualOperation);
			};

			Prop.ForAll(Arb.From(_intGen), assertAdd).QuickCheckThrowOnFailure();
		}

		[TestMethod]
		public void UpdateItem()
		{
			IObservableCollection<BehaviorSubject<int>> collection = new ObservableCollection<BehaviorSubject<int>>();
			IObservableReadOnlyList<BehaviorSubject<int>> actualOperation = collection
				.WhereRc(_filter, _getUpdater)
				.SortRc(x => x.Value, _comparer, _getUpdater)
				.SelectRl(x => x);
			IEnumerable<int> expectedOperation = collection.Where(_filter).Select(x => x.Value);

			Action<BehaviorSubject<int>> assertAddAndUpdate = item =>
			{
				collection.Add(item);
				Check(expectedOperation, actualOperation.Select(x => x.Value).ToList());
				item.OnNext(item.Value + 1);
				Check(expectedOperation, actualOperation.Select(x => x.Value).ToList());
				item.OnNext(item.Value + 1);
				Check(expectedOperation, actualOperation.Select(x => x.Value).ToList());
			};

			Prop.ForAll(Arb.From(_intGen), assertAddAndUpdate).QuickCheckThrowOnFailure();
		}

		[TestMethod]
		public void Remove()
		{
			IObservableCollection<BehaviorSubject<int>> collection = new ObservableCollection<BehaviorSubject<int>>();
			IObservableReadOnlyList<int> actualOperation = collection
				.WhereRc(_filter, _getUpdater)
				.SortRc(x => x.Value, _comparer, _getUpdater)
				.SelectRl(_selector);
			IEnumerable<int> expectedOperation = collection.Where(_filter).Select(_selector);

			Action<BehaviorSubject<int>> assertAddAndRemove = item =>
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
			IObservableCollection<BehaviorSubject<int>> collection = new ObservableCollection<BehaviorSubject<int>>();
			IObservableReadOnlyList<int> actualOperation = collection
				.WhereRc(_filter, _getUpdater)
				.SortRc(x => x.Value, _comparer, _getUpdater)
				.SelectRl(_selector);
			IEnumerable<int> expectedOperation = collection.Where(_filter).Select(_selector);

			Action<BehaviorSubject<int>, BehaviorSubject<int>, BehaviorSubject<int>> assertAddAndClear = (item1, item2, item3) =>
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
			IObservableCollection<BehaviorSubject<int>> collection = new ObservableCollection<BehaviorSubject<int>>();
			IObservableReadOnlyList<int> actualOperation = collection
				.WhereRc(_filter, _getUpdater)
				.SortRc(x => x.Value, _comparer, _getUpdater)
				.SelectRl(_selector);
			IEnumerable<int> expectedOperation = collection.Where(_filter).Select(_selector);

			Action<BehaviorSubject<int>, BehaviorSubject<int>> assertAddAndReplace = (oldItem, newItem) =>
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
			IObservableCollection<BehaviorSubject<int>> collection = new ObservableCollection<BehaviorSubject<int>>();
			IObservableReadOnlyList<int> actualOperation = collection
				.WhereRc(_filter, _getUpdater)
				.SortRc(x => x.Value, _comparer, _getUpdater)
				.SelectRl(_selector);
			IEnumerable<int> expectedOperation = collection.Where(_filter).Select(_selector);

			Action<IReadOnlyList<BehaviorSubject<int>>, IReadOnlyList<BehaviorSubject<int>>> assertReset = (oldItems, newItems) =>
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