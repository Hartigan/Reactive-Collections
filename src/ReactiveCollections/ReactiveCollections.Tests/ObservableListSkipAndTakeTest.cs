using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using FsCheck;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using ReactiveCollections.Abstract.Collections;
using ReactiveCollections.Domain;
using ReactiveCollections.Implementation.Collections;
using ReactiveCollections.Implementation.Operations;

namespace ReactiveCollections.Tests
{
	[TestClass]
	public class ObservableListSkipAndTakeTest
	{
		private class ListArguments
		{
			public ListArguments(
				[NotNull] IReadOnlyList<int> values,
				[NotNull] ObservableValue<int> skip,
				[NotNull] ObservableValue<int> take)
			{
				Values = values;
				Skip = skip;
				Take = take;
			}

			[NotNull]
			public IReadOnlyList<int> Values { get; }

			[NotNull]
			public ObservableValue<int> Skip { get; }

			[NotNull]
			public ObservableValue<int> Take { get; }
		}

		[NotNull]
		private readonly Gen<int> _intGen;

		[NotNull]
		private readonly Gen<int> _indexGen;

		[NotNull]
		private readonly Gen<IReadOnlyList<int>> _valuesGen;

		[NotNull]
		private readonly Gen<ObservableValue<int>> _obValueGen;

		[NotNull]
		private readonly Gen<ListArguments> _listArgsGen;

		[NotNull]
		private readonly Func<int, int> _selector;

		public ObservableListSkipAndTakeTest()
		{
			int maxList = 100;
			int count = 0;
			_intGen = Gen.Fresh(() => count++);
			_selector = x => x * 2;
			_valuesGen = _intGen.ListOf(maxList).Select(x => (IReadOnlyList<int>)x.ToList());
			_indexGen = Arb.Generate<int>().Where(x => x >= 0 && x <= maxList);
			_obValueGen = _indexGen.Select(x => new ObservableValue<int>(x));

			_listArgsGen =
				from values in _valuesGen
				from skip in _obValueGen
				from take in _obValueGen
				select new ListArguments(values, skip, take);
		}

		[TestMethod]
		public void Add()
		{
			Action<int, ListArguments> assertAdd = (item, args) =>
			{
				IObservableList<int> collection = new ObservableList<int>(args.Values.ToList());
				IObservableReadOnlyList<int> actualOperation = collection
					.SkipAndTakeRl(args.Skip, args.Take)
					.SelectRl(_selector, _ => Observable.Never<int>());
				IEnumerable<int> expectedOperation = collection
					.Skip(args.Skip)
					.Take(args.Take)
					.Select(_selector);

				collection.Add(item);
				Assert.IsTrue(Enumerable.SequenceEqual(expectedOperation, actualOperation));
			};

			Prop.ForAll(
				Arb.From(_intGen),
				Arb.From(_listArgsGen),
				assertAdd).QuickCheckThrowOnFailure();
		}

		[TestMethod]
		public void Remove()
		{
			Action<int, ListArguments> assertAddAndRemove = (item, args) =>
			{
				IObservableList<int> collection = new ObservableList<int>(args.Values.ToList());
				IObservableReadOnlyList<int> actualOperation = collection
					.SkipAndTakeRl(args.Skip, args.Take)
					.SelectRl(_selector, _ => Observable.Never<int>());
				IEnumerable<int> expectedOperation = collection
					.Skip(args.Skip)
					.Take(args.Take)
					.Select(_selector);

				collection.Add(item);
				Assert.IsTrue(Enumerable.SequenceEqual(expectedOperation, actualOperation));
				collection.Remove(item);
				Assert.IsTrue(Enumerable.SequenceEqual(expectedOperation, actualOperation));
			};

			Prop.ForAll(
				Arb.From(_intGen),
				Arb.From(_listArgsGen),
				assertAddAndRemove).QuickCheckThrowOnFailure();
		}

		[TestMethod]
		public void Clear()
		{
			Action<ListArguments> assertClear = (args) =>
			{
				IObservableList<int> collection = new ObservableList<int>(args.Values.ToList());
				IObservableReadOnlyList<int> actualOperation = collection
					.SkipAndTakeRl(args.Skip, args.Take)
					.SelectRl(_selector, _ => Observable.Never<int>());
				IEnumerable<int> expectedOperation = collection
					.Skip(args.Skip)
					.Take(args.Take)
					.Select(_selector);

				Assert.IsTrue(Enumerable.SequenceEqual(expectedOperation, actualOperation));
				collection.Clear();
				Assert.IsTrue(Enumerable.SequenceEqual(expectedOperation, actualOperation));
			};

			Prop.ForAll(
				Arb.From(_listArgsGen),
				assertClear).QuickCheckThrowOnFailure();
		}

		[TestMethod]
		public void Replace()
		{
			Action<ListArguments, int, int> assertAddAndReplace = (args, oldItem, newItem) =>
			{
				IObservableList<int> collection = new ObservableList<int>(args.Values.ToList());
				IObservableReadOnlyList<int> actualOperation = collection
					.SkipAndTakeRl(args.Skip, args.Take)
					.SelectRl(_selector, _ => Observable.Never<int>());
				IEnumerable<int> expectedOperation = collection
					.Skip(args.Skip)
					.Take(args.Take)
					.Select(_selector);

				Assert.IsTrue(Enumerable.SequenceEqual(expectedOperation, actualOperation));
				collection.Add(oldItem);
				Assert.IsTrue(Enumerable.SequenceEqual(expectedOperation, actualOperation));
				collection.Replace(oldItem, newItem);
				Assert.IsTrue(Enumerable.SequenceEqual(expectedOperation, actualOperation));
			};

			Prop.ForAll(
				Arb.From(_listArgsGen),
				Arb.From(_intGen),
				Arb.From(_intGen),
				assertAddAndReplace).QuickCheckThrowOnFailure();
		}

		[TestMethod]
		public void Insert()
		{
			Action<ListArguments, int, int> assertInsert = (args, item, index) =>
			{
				IObservableList<int> collection = new ObservableList<int>(args.Values.ToList());
				IObservableReadOnlyList<int> actualOperation = collection
					.SkipAndTakeRl(args.Skip, args.Take)
					.SelectRl(_selector, _ => Observable.Never<int>());
				IEnumerable<int> expectedOperation = collection
					.Skip(args.Skip)
					.Take(args.Take)
					.Select(_selector);

				Assert.IsTrue(Enumerable.SequenceEqual(expectedOperation, actualOperation));
				collection.Insert(index, item);
				Assert.IsTrue(Enumerable.SequenceEqual(expectedOperation, actualOperation));
			};

			Prop.ForAll(
				Arb.From(_listArgsGen),
				Arb.From(_intGen),
				Arb.From(_indexGen),
				assertInsert).QuickCheckThrowOnFailure();
		}

		[TestMethod]
		public void RemoveAt()
		{
			Action<ListArguments, int, int> assertRemoveAt = (args, item, index) =>
			{
				IObservableList<int> collection = new ObservableList<int>(args.Values.ToList());
				IObservableReadOnlyList<int> actualOperation = collection
					.SkipAndTakeRl(args.Skip, args.Take)
					.SelectRl(_selector, _ => Observable.Never<int>());
				IEnumerable<int> expectedOperation = collection
					.Skip(args.Skip)
					.Take(args.Take)
					.Select(_selector);

				Assert.IsTrue(Enumerable.SequenceEqual(expectedOperation, actualOperation));
				collection.Insert(index, item);
				Assert.IsTrue(Enumerable.SequenceEqual(expectedOperation, actualOperation));
				collection.RemoveAt(index);
				Assert.IsTrue(Enumerable.SequenceEqual(expectedOperation, actualOperation));
			};

			Prop.ForAll(
				Arb.From(_listArgsGen),
				Arb.From(_intGen),
				Arb.From(_indexGen),
				assertRemoveAt).QuickCheckThrowOnFailure();
		}

		[TestMethod]
		public void ReplaceByIndex()
		{
			Action<ListArguments, int, int> assertReplaceByIndex = (args, item, index) =>
			{
				IObservableList<int> collection = new ObservableList<int>(args.Values.ToList());
				IObservableReadOnlyList<int> actualOperation = collection
					.SkipAndTakeRl(args.Skip, args.Take)
					.SelectRl(_selector, _ => Observable.Never<int>());
				IEnumerable<int> expectedOperation = collection
					.Skip(args.Skip)
					.Take(args.Take)
					.Select(_selector);

				Assert.IsTrue(Enumerable.SequenceEqual(expectedOperation, actualOperation));
				index = index%collection.Count;
				collection[index] = item;
				Assert.IsTrue(Enumerable.SequenceEqual(expectedOperation, actualOperation));
			};

			Prop.ForAll(
				Arb.From(_listArgsGen),
				Arb.From(_intGen),
				Arb.From(_indexGen),
				assertReplaceByIndex).QuickCheckThrowOnFailure();
		}

		[TestMethod]
		public void Reset()
		{
			Action<ListArguments, IReadOnlyList<int>> assertReset = (args, newItems) =>
			{
				IObservableList<int> collection = new ObservableList<int>(args.Values.ToList());
				IObservableReadOnlyList<int> actualOperation = collection
					.SkipAndTakeRl(args.Skip, args.Take)
					.SelectRl(_selector, _ => Observable.Never<int>());
				IEnumerable<int> expectedOperation = collection
					.Skip(args.Skip)
					.Take(args.Take)
					.Select(_selector);

				Assert.IsTrue(Enumerable.SequenceEqual(expectedOperation, actualOperation));
				collection.Reset(newItems);
				Assert.IsTrue(Enumerable.SequenceEqual(expectedOperation, actualOperation));
			};

			Prop.ForAll(
				Arb.From(_listArgsGen),
				Arb.From(_valuesGen),
				assertReset).QuickCheckThrowOnFailure();
		}

		[TestMethod]
		public void UpdateSkipAndTakeValues()
		{
			Action<ListArguments, int, int> assertUpdateSkipAndTakeValues = (args, newSkip, newTake) =>
			{
				IObservableList<int> collection = new ObservableList<int>(args.Values.ToList());
				IObservableReadOnlyList<int> actualOperation = collection
					.SkipAndTakeRl(args.Skip, args.Take)
					.SelectRl(_selector, _ => Observable.Never<int>());
				IEnumerable<int> expectedOperation = collection
					.Skip(args.Skip)
					.Take(args.Take)
					.Select(_selector);

				Assert.IsTrue(Enumerable.SequenceEqual(expectedOperation, actualOperation));

				var oldSkip = args.Skip.Value;
				var oldTake = args.Take.Value;

				args.Skip.Value = newSkip;
				expectedOperation = collection
					.Skip(args.Skip)
					.Take(args.Take)
					.Select(_selector);
				Assert.IsTrue(Enumerable.SequenceEqual(expectedOperation, actualOperation), $"{oldSkip}");

				args.Take.Value = newTake;
				expectedOperation = collection
					.Skip(args.Skip)
					.Take(args.Take)
					.Select(_selector);
				Assert.IsTrue(Enumerable.SequenceEqual(expectedOperation, actualOperation), $"{oldTake}");
			};

			Prop.ForAll(
				Arb.From(_listArgsGen),
				Arb.From(_indexGen),
				Arb.From(_indexGen),
				assertUpdateSkipAndTakeValues).QuickCheckThrowOnFailure();
		}
	}
}
