using System;
using System.Linq;
using FsCheck;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using ReactiveCollections.Abstract.Collections;
using ReactiveCollections.Implementation.Collections;

namespace ReactiveCollections.Tests
{
	[TestClass]
	public class ObservableListTest
	{
		private readonly Gen<int> _intGen;
		private readonly Gen<bool> _boolGen;

		public ObservableListTest()
		{
			_intGen = Arb.Generate<int>();
			_boolGen = Arb.Generate<bool>();
		}

		[TestMethod]
		public void Add_CollectionChanged()
		{
			IObservableCollection<int> collection = new ObservableList<int>();

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
		public void Remove_CollectionChanged()
		{
			IObservableCollection<int> collection = new ObservableList<int>();

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
		public void Clear_CollectionChanged()
		{
			IObservableCollection<int> collection = new ObservableList<int>();

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
		public void Transaction_CollectionChanged()
		{
			IObservableCollection<int> collection = new ObservableList<int>();

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

			using (var transaction = collection.Transaction())
			{
				Prop.ForAll(Arb.From(_intGen), assertTransaction).QuickCheckThrowOnFailure();
				Assert.AreEqual(0, eventsCount);
			}

			Assert.AreEqual(operationsCount, eventsCount);

			sub.Dispose();
		}
	}
}