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
	public class ObservableListWrapTest
	{
		[NotNull]
		private readonly Gen<int> _intGen;

		[NotNull]
		private readonly Gen<int> _indexGen;

		[NotNull]
		private readonly Func<int, int> _selector;

		public ObservableListWrapTest()
		{
			int count = 0;
			_intGen = Gen.Fresh(() => count++);
			_selector = x => x * 2;
			_indexGen = Arb.Generate<int>().Where(x => x >= 0);
		}

		[TestMethod]
		public void Add()
		{
			IObservableList<int> collection = new ObservableList<int>();
			IObservableReadOnlyList<int> actualOperation = collection
				.SelectRl(_selector, _ => Observable.Never<int>())
				.SelectRl(_selector, _ => Observable.Never<int>());
			IEnumerable<int> expectedOperation = collection.Select(_selector).Select(_selector);

			Action<int> assertAdd = item =>
			{
				collection.Add(item);
				Assert.IsTrue(Enumerable.SequenceEqual(expectedOperation, actualOperation));
			};

			Prop.ForAll(Arb.From(_intGen), assertAdd).QuickCheckThrowOnFailure();
		}

		[TestMethod]
		public void ItemChanged()
		{
			Func<BehaviorSubject<int>, int> subjectToIntSelector = x => x.Value * 2;
			IObservableList<BehaviorSubject<int>> collection = new ObservableList<BehaviorSubject<int>>();
			IObservableReadOnlyCollection<int> actualOperation = collection
				.SelectRl(subjectToIntSelector, x => x.Select(_ => x))
				.SelectRl(x => x, _ => Observable.Never<int>());
			IEnumerable<int> expectedOperation = collection.Select(subjectToIntSelector).Select(x => x);

			Action<BehaviorSubject<int>> assertAdd = item =>
			{
				collection.Add(item);
				Assert.IsTrue(Enumerable.SequenceEqual(expectedOperation, actualOperation));
				item.OnNext(item.Value + 1);
				Assert.IsTrue(Enumerable.SequenceEqual(expectedOperation, actualOperation));
				item.OnNext(item.Value + 1);
				Assert.IsTrue(Enumerable.SequenceEqual(expectedOperation, actualOperation));
			};

			Prop.ForAll(Arb.From(_intGen.Select(x => new BehaviorSubject<int>(x))), assertAdd).QuickCheckThrowOnFailure();
		}

		[TestMethod]
		public void Remove()
		{
			IObservableList<int> collection = new ObservableList<int>();
			IObservableReadOnlyList<int> actualOperation = collection
				.SelectRl(_selector, _ => Observable.Never<int>())
				.SelectRl(_selector, _ => Observable.Never<int>());
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
			IObservableList<int> collection = new ObservableList<int>();
			IObservableReadOnlyList<int> actualOperation = collection
				.SelectRl(_selector, _ => Observable.Never<int>())
				.SelectRl(_selector, _ => Observable.Never<int>());
			IEnumerable<int> expectedOperation = collection.Select(_selector).Select(_selector);

			Action<int, int, int> assertAddAndClear = (item1, item2, item3) =>
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
			IObservableList<int> collection = new ObservableList<int>();
			IObservableReadOnlyList<int> actualOperation = collection
				.SelectRl(_selector, _ => Observable.Never<int>())
				.SelectRl(_selector, _ => Observable.Never<int>());
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

		[TestMethod]
		public void Insert()
		{
			IObservableList<int> collection = new ObservableList<int>();
			IObservableReadOnlyList<int> actualOperation = collection
				.SelectRl(_selector, _ => Observable.Never<int>())
				.SelectRl(_selector, _ => Observable.Never<int>());
			IEnumerable<int> expectedOperation = collection.Select(_selector).Select(_selector);

			Action<int, int> assertInsert = (item, index) =>
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
			IObservableList<int> collection = new ObservableList<int>();
			IObservableReadOnlyList<int> actualOperation = collection
				.SelectRl(_selector, _ => Observable.Never<int>())
				.SelectRl(_selector, _ => Observable.Never<int>());
			IEnumerable<int> expectedOperation = collection.Select(_selector).Select(_selector);

			for (int i = 0; i < 1000; i++)
			{
				collection.Add(i);
			}

			Action<int> assertRemoveAt = index =>
			{
				index = index%collection.Count;
				collection.RemoveAt(index);
				Assert.IsTrue(Enumerable.SequenceEqual(expectedOperation, actualOperation));
			};

			Prop.ForAll(Arb.From(_indexGen), assertRemoveAt).QuickCheckThrowOnFailure();
		}

		[TestMethod]
		public void ReplaceByIndex()
		{
			IObservableList<int> collection = new ObservableList<int>();
			IObservableReadOnlyList<int> actualOperation = collection
				.SelectRl(_selector, _ => Observable.Never<int>())
				.SelectRl(_selector, _ => Observable.Never<int>());
			IEnumerable<int> expectedOperation = collection.Select(_selector).Select(_selector);

			for (int i = 0; i < 1000; i++)
			{
				collection.Add(i);
			}

			Action<int, int> assertReplaceByIndex = (item, index) =>
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
			IObservableList<int> collection = new ObservableList<int>();
			IObservableReadOnlyList<int> actualOperation = collection
				.SelectRl(_selector, _ => Observable.Never<int>())
				.SelectRl(_selector, _ => Observable.Never<int>());
			IEnumerable<int> expectedOperation = collection.Select(_selector).Select(_selector);

			Action<IReadOnlyList<int>, IReadOnlyList<int>> assertReset = (oldItems, newItems) =>
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