using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
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
	public class ObservableCollectionSelectManyTest
	{
		private readonly Gen<int> _intGen;
		private readonly Gen<IObservableCollection<IObservableCollection<int>>> _targetGen;
		private readonly Gen<IObservableCollection<int>> _collectionGen;
		private readonly Func<IObservableCollection<int>, IObservableCollection<int>> _selectMany;
		private readonly Func<int, int> _select;

		public ObservableCollectionSelectManyTest()
		{
			int count = 0;
			_intGen = Gen.Fresh(() => count++);
			_selectMany = x => x;
			_select = x => x;

			_collectionGen = _intGen.ListOf(10)
				.Select(x =>(IObservableCollection<int>)new ObservableCollection<int>(x));
			_targetGen = _collectionGen.ListOf(10)
				.Select(x => (IObservableCollection<IObservableCollection<int>>)new ObservableCollection<IObservableCollection<int>>(x));
		}

		[NotNull]
		private IObservableReadOnlyCollection<int> GetActual(
			[NotNull] IObservableCollection<IObservableCollection<int>> collection)
		{
			return collection.SelectManyRc(_selectMany).SelectRc(_select, _ => Observable.Never<int>());
		}

		[NotNull]
		private IEnumerable<int> GetExpected(
			[NotNull] IObservableCollection<IObservableCollection<int>> collection)
		{
			return collection.SelectMany(_selectMany).Select(_select);
		}

		private void Check([NotNull] IEnumerable<int> expected, [NotNull] IEnumerable<int> actual)
		{
			var exp = expected.ToList();
			exp.Sort();
			var act = actual.ToList();
			act.Sort();

			Assert.IsTrue(Enumerable.SequenceEqual(exp, act));
		}

		[TestMethod]
		public void Add()
		{
			Action<IObservableCollection<IObservableCollection<int>>, IObservableCollection<int>> assertAdd = (collections, collection) =>
			{
				var expectedOperation = GetExpected(collections);
				var actualOperation = GetActual(collections);

				Check(expectedOperation, actualOperation);
				collections.Add(collection);
				Check(expectedOperation, actualOperation);
			};

			Prop.ForAll(Arb.From(_targetGen), Arb.From(_collectionGen), assertAdd).QuickCheckThrowOnFailure();
		}

		[TestMethod]
		public void Remove()
		{
			Action<IObservableCollection<IObservableCollection<int>>, int> assertRemove = (collections, index) =>
			{
				index = index%(collections.Count - 1) + 1;

				var expectedOperation = GetExpected(collections);
				var actualOperation = GetActual(collections);

				Check(expectedOperation, actualOperation);
				collections.Remove(collections.Skip(index - 1).First());
				Check(expectedOperation, actualOperation);
			};

			Prop.ForAll(Arb.From(_targetGen), Arb.From(_intGen), assertRemove).QuickCheckThrowOnFailure();
		}

		[TestMethod]
		public void Clear()
		{
			Action<IObservableCollection<IObservableCollection<int>>> assertClear = collections =>
			{
				var expectedOperation = GetExpected(collections);
				var actualOperation = GetActual(collections);

				Check(expectedOperation, actualOperation);
				collections.Clear();
				Check(expectedOperation, actualOperation);
			};

			Prop.ForAll(Arb.From(_targetGen), assertClear).QuickCheckThrowOnFailure();
		}

		[TestMethod]
		public void Replace()
		{
			Action<IObservableCollection<IObservableCollection<int>>, IObservableCollection<int>, int> assertReplace = (collections, collection, index) =>
			{
				index = index % (collections.Count - 1) + 1;

				var expectedOperation = GetExpected(collections);
				var actualOperation = GetActual(collections);

				Check(expectedOperation, actualOperation);
				collections.Replace(collections.Skip(index - 1).First(), collection);
				Check(expectedOperation, actualOperation);
			};

			Prop.ForAll(Arb.From(_targetGen), Arb.From(_collectionGen), Arb.From(_intGen), assertReplace).QuickCheckThrowOnFailure();
		}

		[TestMethod]
		public void Reset()
		{
			Action<IObservableCollection<IObservableCollection<int>>, IReadOnlyList<IObservableCollection<int>>> assertReset = (collections, newCollections) =>
			{
				var expectedOperation = GetExpected(collections);
				var actualOperation = GetActual(collections);

				Check(expectedOperation, actualOperation);
				collections.Reset(newCollections);
				Check(expectedOperation, actualOperation);
			};

			Prop.ForAll(
				Arb.From(_targetGen),
				Arb.From(_collectionGen.ListOf(10).Select(x => x.ToList())),
				assertReset).QuickCheckThrowOnFailure();
		}

		[TestMethod]
		public void UpdateSubCollection()
		{
			Action<IObservableCollection<IObservableCollection<int>>, int, int> assertUpdateSubCollection = (collections, index, item) =>
			{
				index = index%collections.Count + 1;

				var expectedOperation = GetExpected(collections);
				var actualOperation = GetActual(collections);

				Check(expectedOperation, actualOperation);

				var collection = collections.Take(index).Last();

				collection.Add(item);
				Check(expectedOperation, actualOperation);

				collection.Remove(item);
				Check(expectedOperation, actualOperation);

				collection.Add(item);
				collection.Replace(item, item*2);
				Check(expectedOperation, actualOperation);

				collection.Reset(new [] { item });
				Check(expectedOperation, actualOperation);

				collection.Clear();
				Check(expectedOperation, actualOperation);
			};

			Prop.ForAll(
				Arb.From(_targetGen),
				Arb.From(_intGen),
				Arb.From(_intGen),
				assertUpdateSubCollection).QuickCheckThrowOnFailure();
		}
	}
}