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
	public class ObservableCollectionWhereTest
	{
		[NotNull] private readonly Gen<BehaviorSubject<int>> _intGen;
		[NotNull] private readonly Func<BehaviorSubject<int>, int> _selector;
		[NotNull] private readonly Func<BehaviorSubject<int>, bool> _filter;
		[NotNull] private readonly Func<BehaviorSubject<int>, IObservable<Unit>> _getUpdater;

		public ObservableCollectionWhereTest()
		{
			int count = 0;
			
			_intGen = Gen.Fresh(() => new BehaviorSubject<int>(count++));
			_selector = x => x.Value;
			_filter = x => x.Value%2 == 0;
			_getUpdater = x => x.Select(_ => Unit.Default);
		}

		[TestMethod]
		public void Add()
		{
			IObservableCollection<BehaviorSubject<int>> collection = new ObservableCollection<BehaviorSubject<int>>();
			IObservableReadOnlyCollection<int> actualOperation = collection.WhereRc(_filter, _getUpdater).SelectRc(_selector);
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
			IObservableCollection<BehaviorSubject<int>> collection = new ObservableCollection<BehaviorSubject<int>>();
			IObservableReadOnlyCollection<int> actualOperation = collection.WhereRc(_filter, _getUpdater).SelectRc(_selector);
			IEnumerable<int> expectedOperation = collection.Where(_filter).Select(_selector);

			Action<BehaviorSubject<int>> assertAddAndUpdate = item =>
			{
				collection.Add(item);
				Assert.IsTrue(Enumerable.SequenceEqual(expectedOperation, actualOperation));
				item.OnNext(item.Value + 1);
				Assert.IsTrue(Enumerable.SequenceEqual(expectedOperation, actualOperation));
				item.OnNext(item.Value + 1);
				Assert.IsTrue(Enumerable.SequenceEqual(expectedOperation, actualOperation));
			};

			Prop.ForAll(Arb.From(_intGen), assertAddAndUpdate).QuickCheckThrowOnFailure();
		}

		[TestMethod]
		public void Remove()
		{
			IObservableCollection<BehaviorSubject<int>> collection = new ObservableCollection<BehaviorSubject<int>>();
			IObservableReadOnlyCollection<int> actualOperation = collection.WhereRc(_filter, _getUpdater).SelectRc(_selector);
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
			IObservableCollection<BehaviorSubject<int>> collection = new ObservableCollection<BehaviorSubject<int>>();
			IObservableReadOnlyCollection<int> actualOperation = collection.WhereRc(_filter, _getUpdater).SelectRc(_selector);
			IEnumerable<int> expectedOperation = collection.Where(_filter).Select(_selector);

			Action<BehaviorSubject<int>, BehaviorSubject<int>, BehaviorSubject<int>> assertAddAndClear = (item1, item2, item3) =>
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
			IObservableCollection<BehaviorSubject<int>> collection = new ObservableCollection<BehaviorSubject<int>>();
			IObservableReadOnlyCollection<int> actualOperation = collection.WhereRc(_filter, _getUpdater).SelectRc(_selector);
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
	}
}