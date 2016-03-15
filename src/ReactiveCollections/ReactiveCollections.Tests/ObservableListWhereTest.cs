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
	public class ObservableListWhereTest
	{
		[NotNull]
		private readonly Gen<int> _indexGen;
		[NotNull]
		private readonly Gen<BehaviorSubject<int>> _intGen;
		[NotNull]
		private readonly Func<BehaviorSubject<int>, int> _selector;
		[NotNull]
		private readonly Func<BehaviorSubject<int>, bool> _filter;
		[NotNull]
		private readonly Func<BehaviorSubject<int>, IObservable<BehaviorSubject<int>>> _getUpdater;

		public ObservableListWhereTest()
		{
			int count = 0;
			_intGen = Gen.Fresh(() => new BehaviorSubject<int>(count++));
			_selector = x => x.Value;
			_filter = x => x.Value % 2 == 0;
			_getUpdater = x => x.Select(_ => x);
			_indexGen = Arb.Generate<int>().Where(x => x >= 0);
		}

		[TestMethod]
		public void Add()
		{
			IObservableList<BehaviorSubject<int>> collection = new ObservableList<BehaviorSubject<int>>();
			IObservableReadOnlyList<int> actualOperation = collection.WhereRl(_filter, _getUpdater).SelectRl(_selector, _getUpdater);
			IEnumerable<int> expectedOperation = collection.Where(_filter).Select(_selector);

			Action<BehaviorSubject<int>> assertAdd = item =>
			{
				collection.Add(item);
				Assert.IsTrue(Enumerable.SequenceEqual(expectedOperation, actualOperation));
			};

			Prop.ForAll(Arb.From(_intGen), assertAdd).QuickCheckThrowOnFailure();
		}

		[TestMethod]
		public void UpdateItem()
		{
			IObservableList<BehaviorSubject<int>> collection = new ObservableList<BehaviorSubject<int>>();
			IObservableReadOnlyList<int> actualOperation = collection.WhereRl(_filter, _getUpdater).SelectRl(_selector, _getUpdater);
			IEnumerable<int> expectedOperation = collection.Where(_filter).Select(_selector);

			Action<BehaviorSubject<int>> assertAdd = item =>
			{
				collection.Add(item);
				Assert.IsTrue(Enumerable.SequenceEqual(expectedOperation, actualOperation));
				item.OnNext(item.Value + 1);
				Assert.IsTrue(Enumerable.SequenceEqual(expectedOperation, actualOperation));
				item.OnNext(item.Value + 1);
				Assert.IsTrue(Enumerable.SequenceEqual(expectedOperation, actualOperation));

			};

			Prop.ForAll(Arb.From(_intGen), assertAdd).QuickCheckThrowOnFailure();
		}

		[TestMethod]
		public void Remove()
		{
			IObservableList<BehaviorSubject<int>> collection = new ObservableList<BehaviorSubject<int>>();
			IObservableReadOnlyList<int> actualOperation = collection.WhereRl(_filter, _getUpdater).SelectRl(_selector, _getUpdater);
			IEnumerable<int> expectedOperation = collection.Where(_filter).Select(_selector);

			Action<BehaviorSubject<int>> assertAddAndRemove = item =>
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
			IObservableList<BehaviorSubject<int>> collection = new ObservableList<BehaviorSubject<int>>();
			IObservableReadOnlyList<int> actualOperation = collection.WhereRl(_filter, _getUpdater).SelectRl(_selector, _getUpdater);
			IEnumerable<int> expectedOperation = collection.Where(_filter).Select(_selector);

			Action<BehaviorSubject<int>, BehaviorSubject<int>, BehaviorSubject<int>> assertAddAndClear = (item1, item2, item3) =>
			{
				collection.Add(item1);
				collection.Add(item2);
				collection.Add(item3);
				Assert.IsTrue(Enumerable.SequenceEqual(expectedOperation, actualOperation), "before clear");
				collection.Clear();
				Assert.IsTrue(Enumerable.SequenceEqual(expectedOperation, actualOperation), "after clear");
			};

			Prop.ForAll(Arb.From(_intGen), Arb.From(_intGen), Arb.From(_intGen), assertAddAndClear).QuickCheckThrowOnFailure();
		}

		[TestMethod]
		public void Replace()
		{
			IObservableList<BehaviorSubject<int>> collection = new ObservableList<BehaviorSubject<int>>();
			IObservableReadOnlyList<int> actualOperation = collection.WhereRl(_filter, _getUpdater).SelectRl(_selector, _getUpdater);
			IEnumerable<int> expectedOperation = collection.Where(_filter).Select(_selector);

			Action<BehaviorSubject<int>, BehaviorSubject<int>> assertAddAndReplace = (oldItem, newItem) =>
			{
				collection.Add(oldItem);
				Assert.IsTrue(Enumerable.SequenceEqual(expectedOperation, actualOperation));
				collection.Replace(oldItem, newItem);
				Assert.IsTrue(Enumerable.SequenceEqual(expectedOperation, actualOperation));
			};

			Prop.ForAll(Arb.From(_intGen), Arb.From(_intGen), assertAddAndReplace).QuickCheckThrowOnFailure();
		}

		[TestMethod]
		public void Insert()
		{
			IObservableList<BehaviorSubject<int>> collection = new ObservableList<BehaviorSubject<int>>();
			IObservableReadOnlyList<int> actualOperation = collection.WhereRl(_filter, _getUpdater).SelectRl(_selector, _getUpdater);
			IEnumerable<int> expectedOperation = collection.Where(_filter).Select(_selector);

			Action<BehaviorSubject<int>, int> assertInsert = (item, index) =>
			{
				index = index % (collection.Count + 1);
				collection.Insert(index, item);
				Assert.IsTrue(Enumerable.SequenceEqual(expectedOperation, actualOperation));
			};

			Prop.ForAll(Arb.From(_intGen), Arb.From(_indexGen), assertInsert).QuickCheckThrowOnFailure();
		}

		[TestMethod]
		public void RemoveAt()
		{
			IObservableList<BehaviorSubject<int>> collection = new ObservableList<BehaviorSubject<int>>();
			IObservableReadOnlyList<int> actualOperation = collection.WhereRl(_filter, _getUpdater).SelectRl(_selector, _getUpdater);
			IEnumerable<int> expectedOperation = collection.Where(_filter).Select(_selector);

			for (int i = 0; i < 1000; i++)
			{
				collection.Add(new BehaviorSubject<int>(i));
			}

			Action<int> assertRemoveAt = index =>
			{
				index = index % collection.Count;
				collection.RemoveAt(index);
				Assert.IsTrue(Enumerable.SequenceEqual(expectedOperation, actualOperation));
			};

			Prop.ForAll(Arb.From(_indexGen), assertRemoveAt).QuickCheckThrowOnFailure();
		}

		[TestMethod]
		public void ReplaceByIndex()
		{
			IObservableList<BehaviorSubject<int>> collection = new ObservableList<BehaviorSubject<int>>();
			IObservableReadOnlyList<int> actualOperation = collection.WhereRl(_filter, _getUpdater).SelectRl(_selector, _getUpdater);
			IEnumerable<int> expectedOperation = collection.Where(_filter).Select(_selector);

			for (int i = 0; i < 10; i++)
			{
				collection.Add(new BehaviorSubject<int>(i));
			}

			Action<BehaviorSubject<int>, int> assertReplaceByIndex = (item, index) =>
			{
				index = index % collection.Count;
				collection[index] = item;
				Assert.IsTrue(Enumerable.SequenceEqual(expectedOperation, actualOperation));
			};

			Prop.ForAll(Arb.From(_intGen), Arb.From(_indexGen), assertReplaceByIndex).QuickCheckThrowOnFailure();
		}

		[TestMethod]
		public void Reset()
		{
			IObservableList<BehaviorSubject<int>> collection = new ObservableList<BehaviorSubject<int>>();
			IObservableReadOnlyList<int> actualOperation = collection.WhereRl(_filter, _getUpdater).SelectRl(_selector, _getUpdater);
			IEnumerable<int> expectedOperation = collection.Where(_filter).Select(_selector);

			Action<IReadOnlyList<BehaviorSubject<int>>, IReadOnlyList<BehaviorSubject<int>>> assertReset = (oldItems, newItems) =>
			{
				collection.Reset(oldItems);
				Assert.IsTrue(Enumerable.SequenceEqual(expectedOperation, actualOperation));
				collection.Reset(newItems);
				Assert.IsTrue(Enumerable.SequenceEqual(expectedOperation, actualOperation));
			};

			Prop.ForAll(
				Arb.From(_intGen.ListOf(10).Select(x => x.ToList())),
				Arb.From(_intGen.ListOf(10).Select(x => x.ToList())),
				assertReset)
				.QuickCheckThrowOnFailure();
		}
	}
}