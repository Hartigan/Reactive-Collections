using FsCheck;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using ReactiveCollections.Abstract.Collections;
using ReactiveCollections.Implementation.Collections;
using System;
using System.Linq;

namespace ReactiveCollections.Tests
{
	[TestClass]
	public class ObservableCollectionTest
	{
		private readonly Gen<int> _intGen;
		private readonly Gen<bool> _boolGen;

		public ObservableCollectionTest()
		{
			int count = 0;
			_intGen = Gen.Fresh(() => count++);
			_boolGen = Arb.Generate<bool>();
		}

		[TestMethod]
		public void Add()
		{
			IObservableCollection<int> collection = new ObservableCollection<int>();

			Action<int> assertAdd = item =>
			{
				int eventsCount = 0;

				IDisposable sub = collection.CollectionChanged.Subscribe(x =>
				{
					x.Match(
						onInsert: y => { Assert.AreEqual(item, y.Item); },
						onRemove: y => { Assert.Fail(); },
						onReplace: y => { Assert.Fail(); },
						onClear: y => { Assert.Fail(); },
						onEmpty: y => { Assert.Fail(); });

					eventsCount++;
				});

				int sourceCount = collection.Count;
				int sourceCountOfItem = collection.Count(x => x == item);
				collection.Add(item);

				Assert.AreEqual(sourceCount + 1, collection.Count);
				Assert.AreEqual(sourceCountOfItem + 1, collection.Count(x => x == item));
				Assert.AreEqual(eventsCount, 1);
				sub.Dispose();
			};

			Prop.ForAll(Arb.From(_intGen), assertAdd).QuickCheckThrowOnFailure();
		}

		[TestMethod]
		public void Remove()
		{
			IObservableCollection<int> collection = new ObservableCollection<int>();

			Action<int, bool> assertRemove = (item, withInit) =>
			{
				if (withInit)
				{
					collection.Add(item);
				}

				int eventsCount = 0;

				IDisposable sub = collection.CollectionChanged.Subscribe(x =>
				{
					x.Match(
						onInsert: y => { Assert.Fail(); },
						onRemove: y => { Assert.AreEqual(item, y.Item); },
						onReplace: y => { Assert.Fail(); },
						onClear: y => { Assert.Fail(); },
						onEmpty: y => { Assert.Fail(); });

					eventsCount++;
				});


				int sourceCount = collection.Count;
				int sourceCountOfItem = collection.Count(x => x == item);

				if (collection.Remove(item))
				{
					Assert.AreEqual(sourceCount - 1, collection.Count);
					Assert.AreEqual(sourceCountOfItem - 1, collection.Count(x => x == item));
					Assert.AreEqual(eventsCount, 1);
				}
				else
				{
					Assert.AreEqual(sourceCount, collection.Count);
					Assert.AreEqual(sourceCountOfItem, collection.Count(x => x == item));
					Assert.AreEqual(eventsCount, 0);
				}

				sub.Dispose();
			};

			Prop.ForAll(Arb.From(_intGen), Arb.From(_boolGen), assertRemove).QuickCheckThrowOnFailure();
		}

		[TestMethod]
		public void Clear()
		{
			IObservableCollection<int> collection = new ObservableCollection<int>();

			Action<int, bool> assertClear = (item, withInit) =>
			{
				if (withInit)
				{
					collection.Add(item);
				}

				int eventsCount = 0;
				var sourceItems = collection.ToList();

				IDisposable sub = collection.CollectionChanged.Subscribe(x =>
				{
					x.Match(
						onInsert: y => { Assert.Fail(); },
						onRemove: y => { Assert.Fail(); },
						onReplace: y => { Assert.Fail(); },
						onClear: y => { Assert.IsTrue(Enumerable.SequenceEqual(sourceItems, y.Items)); },
						onEmpty: y => { Assert.Fail(); });

					eventsCount++;
				});


				collection.Clear();
				Assert.AreEqual(0, collection.Count);
				Assert.AreEqual(eventsCount, 1);
								sub.Dispose();
			};

			Prop.ForAll(Arb.From(_intGen), Arb.From(_boolGen), assertClear).QuickCheckThrowOnFailure();
		}

		[TestMethod]
		public void Replace()
		{
			IObservableCollection<int> collection = new ObservableCollection<int>();

			Action<int, int> assertReplace = (oldItem, newItem) =>
			{
				collection.Add(oldItem);

				int eventsCount = 0;

				IDisposable sub = collection.CollectionChanged.Subscribe(x =>
				{
					x.Match(
						onInsert: y => { Assert.Fail(); },
						onRemove: y => { Assert.Fail(); },
						onReplace: y => { Assert.AreEqual(oldItem, y.OldItem); Assert.AreEqual(newItem, y.NewItem); },
						onClear: y => { Assert.Fail(); },
						onEmpty: y => { Assert.Fail(); });

					eventsCount++;
				});


				int sourceCount = collection.Count;
				int sourceCountOfOldItem = collection.Count(x => x == oldItem);
				int sourceCountOfNewItem = collection.Count(x => x == newItem);

				if (collection.Replace(oldItem, newItem))
				{
					Assert.AreEqual(sourceCount, collection.Count);
					Assert.AreEqual(sourceCountOfOldItem - 1, collection.Count(x => x == oldItem));
					Assert.AreEqual(sourceCountOfNewItem + 1, collection.Count(x => x == newItem));
					Assert.AreEqual(eventsCount, 1);
				}
				else
				{
					Assert.AreEqual(sourceCount, collection.Count);
					Assert.AreEqual(sourceCountOfOldItem, 0);
					Assert.AreEqual(sourceCountOfNewItem, collection.Count(x => x == newItem));
					Assert.AreEqual(eventsCount, 0);
				}

				sub.Dispose();
			};

			Prop.ForAll(Arb.From(_intGen), Arb.From(_intGen), assertReplace).QuickCheckThrowOnFailure();
		}

		[TestMethod]
		public void Transaction()
		{
			IObservableCollection<int> collection = new ObservableCollection<int>();

			System.Random rng = new System.Random(42);
			int eventsCount = 0;
			int operationsCount = 0;

			IDisposable sub = collection.CollectionChanged.Subscribe(x =>
			{
				eventsCount++;
			});

			Action<int> assertTransaction = item =>
			{
				var i = rng.Next() % 20;

				if (i < 10)
				{
					collection.Add(i);
				}
				else if (i < 18 && collection.Count > 0)
				{
					collection.Remove(collection.First());
				}
				else
				{
					collection.Clear();
				}

				operationsCount++;
			};

			using(var transaction = collection.Transaction())
			{
				Prop.ForAll(Arb.From(_intGen), assertTransaction).QuickCheckThrowOnFailure();
				Assert.AreEqual(0, eventsCount);
			}

			Assert.AreEqual(operationsCount, eventsCount);

			sub.Dispose();
		}
	}
}
