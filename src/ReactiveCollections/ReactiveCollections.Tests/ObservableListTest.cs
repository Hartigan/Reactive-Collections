using System;
using System.Collections.Generic;
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
		private readonly Gen<int> _indexGen;
		private readonly Gen<bool> _boolGen;

		public ObservableListTest()
		{
			_intGen = Arb.Generate<int>();
			_indexGen = Arb.Generate<int>().Where(x => x >= 0);
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
						onReset: y => { Assert.Fail(); },
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
						onReset: y => { Assert.Fail(); },
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
						onReset: y => { Assert.IsTrue(Enumerable.SequenceEqual(sourceItems, y.OldItems)); },
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

		[TestMethod]
		public void Add_ListChanged()
		{
			IObservableList<int> collection = new ObservableList<int>();

			Action<int> assertAdd = item =>
			{
				int eventsCount = 0;

				IDisposable sub = collection.ListChanged.Subscribe(x =>
				{
					x.Match(
						onInsert: y => { Assert.AreEqual(item, y.Item); Assert.AreEqual(collection.Count - 1, y.Index);},
						onRemove: y => { Assert.Fail(); },
						onReplace: y => { Assert.Fail(); },
						onMove: y => { Assert.Fail(); },
						onReset: y => { Assert.Fail(); },
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
		public void Insert_ListChanged()
		{
			IObservableList<int> collection = new ObservableList<int>();

			Action<int, int> assertInsert = (item, index) =>
			{
				index = index%(collection.Count+1);
				int eventsCount = 0;

				IDisposable sub = collection.ListChanged.Subscribe(x =>
				{
					x.Match(
						onInsert: y => { Assert.AreEqual(item, y.Item); Assert.AreEqual(index, y.Index); },
						onRemove: y => { Assert.Fail(); },
						onReplace: y => { Assert.Fail(); },
						onMove: y => { Assert.Fail(); },
						onReset: y => { Assert.Fail(); },
						onEmpty: y => { Assert.Fail(); });

					eventsCount++;
				});

				int sourceCount = collection.Count;
				int sourceCountOfItem = collection.Count(x => x == item);
				collection.Insert(index, item);

				Assert.AreEqual(sourceCount + 1, collection.Count);
				Assert.AreEqual(sourceCountOfItem + 1, collection.Count(x => x == item));
				Assert.AreEqual(eventsCount, 1);
				sub.Dispose();
			};

			Prop.ForAll(Arb.From(_intGen), Arb.From(_indexGen), assertInsert).QuickCheckThrowOnFailure();
		}

		[TestMethod]
		public void Remove_ListChanged()
		{
			IObservableList<int> collection = new ObservableList<int>();

			Action<int, bool> assertRemove = (item, withInit) =>
			{
				if (withInit)
				{
					collection.Add(item);
				}

				int eventsCount = 0;
				var srcIndex = collection.IndexOf(item);

				IDisposable sub = collection.ListChanged.Subscribe(x =>
				{
					x.Match(
						onInsert: y => { Assert.Fail(); },
						onRemove: y =>
						{
							if (srcIndex > -1)
							{
								Assert.AreEqual(item, y.Item);
								Assert.AreEqual(srcIndex, y.Index);
							}
							else
							{
								Assert.Fail();
							}
						},
						onReplace: y => { Assert.Fail(); },
						onMove: y => { Assert.Fail(); },
						onReset: y => { Assert.Fail(); },
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
		public void RemoveAt_ListChanged()
		{
			IObservableList<int> collection = new ObservableList<int>();

			Action<int, bool, int> assertRemoveAt = (item, withInit, index) =>
			{
				if (withInit)
				{
					collection.Add(item);
				}

				if (collection.Count == 0)
				{
					return;
				}

				index = index%collection.Count;
				var srcItem = collection[index];

				int eventsCount = 0;

				IDisposable sub = collection.ListChanged.Subscribe(x =>
				{
					x.Match(
						onInsert: y => { Assert.Fail(); },
						onRemove: y =>
						{
							Assert.AreEqual(srcItem, y.Item);
							Assert.AreEqual(index, y.Index);
						},
						onReplace: y => { Assert.Fail(); },
						onMove: y => { Assert.Fail(); },
						onReset: y => { Assert.Fail(); },
						onEmpty: y => { Assert.Fail(); });

					eventsCount++;
				});


				int sourceCount = collection.Count;

				collection.RemoveAt(index);
				Assert.AreEqual(sourceCount - 1, collection.Count);
				Assert.AreEqual(eventsCount, 1);

				sub.Dispose();
			};

			Prop.ForAll(Arb.From(_intGen), Arb.From(_boolGen), Arb.From(_indexGen), assertRemoveAt).QuickCheckThrowOnFailure();
		}

		[TestMethod]
		public void Replace_ListChanged()
		{
			IObservableList<int> collection = new ObservableList<int>();

			Action<int, int, int> assertReplace = (srcItem, newItem , index) =>
			{
				index = index % (collection.Count + 1);

				collection.Insert(index, srcItem);

				int eventsCount = 0;

				IDisposable sub = collection.ListChanged.Subscribe(x =>
				{
					x.Match(
						onInsert: y => { Assert.Fail(); },
						onRemove: y => { Assert.Fail(); },
						onReplace: y =>
						{
							Assert.AreEqual(srcItem, y.OldItem);
							Assert.AreEqual(newItem, y.NewItem);
							Assert.AreEqual(index, y.Index);
						},
						onMove: y => { Assert.Fail(); },
						onReset: y => { Assert.Fail(); },
						onEmpty: y => { Assert.Fail(); });

					eventsCount++;
				});


				int sourceCount = collection.Count;

				collection[index] = newItem;
				Assert.AreEqual(sourceCount, collection.Count);
				Assert.AreEqual(eventsCount, 1);

				sub.Dispose();
			};

			Prop.ForAll(Arb.From(_intGen), Arb.From(_intGen), Arb.From(_indexGen), assertReplace).QuickCheckThrowOnFailure();
		}

		[TestMethod]
		public void Move_ListChanged()
		{
			IObservableList<int> collection = new ObservableList<int>();

			Action<int, bool, int> assertMove = (item, withInit, srcIndex) =>
			{
				if (withInit)
				{
					collection.Add(item);
				}

				if (collection.Count == 0)
				{
					return;
				}

				srcIndex = srcIndex%collection.Count;
				var dstIndex = (srcIndex*2)%collection.Count;

				var srcItem = collection[srcIndex];

				int eventsCount = 0;

				IDisposable sub = collection.ListChanged.Subscribe(x =>
				{
					x.Match(
						onInsert: y => { Assert.Fail(); },
						onRemove: y => { Assert.Fail(); },
						onReplace: y => { Assert.Fail(); },
						onMove: y =>
						{
							Assert.AreEqual(srcItem, y.Item);
							Assert.AreEqual(srcIndex, y.OldIndex);
							Assert.AreEqual(dstIndex, y.NewIndex);
						},
						onReset: y => { Assert.Fail(); },
						onEmpty: y => { Assert.Fail(); });

					eventsCount++;
				});


				int sourceCount = collection.Count;

				collection.Move(srcIndex, dstIndex);

				Assert.AreEqual(sourceCount, collection.Count);
				Assert.AreEqual(srcItem, collection[dstIndex]);
				Assert.AreEqual(eventsCount, 1);

				sub.Dispose();
			};

			Prop.ForAll(Arb.From(_intGen), Arb.From(_boolGen), Arb.From(_indexGen), assertMove).QuickCheckThrowOnFailure();
		}

		[TestMethod]
		public void Clear_ListChanged()
		{
			IObservableList<int> collection = new ObservableList<int>();

			Action<int, bool> assertClear = (item, withInit) =>
			{
				if (withInit)
				{
					collection.Add(item);
				}

				int eventsCount = 0;
				var sourceItems = collection.ToList();

				IDisposable sub = collection.ListChanged.Subscribe(x =>
				{
					x.Match(
						onInsert: y => { Assert.Fail(); },
						onRemove: y => { Assert.Fail(); },
						onReplace: y => { Assert.Fail(); },
						onMove: y => { Assert.Fail(); },
						onReset: y =>
						{
							Assert.IsTrue(Enumerable.SequenceEqual(sourceItems, y.OldItems));
							Assert.IsTrue(y.NewItems.Count == 0);
						},
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
		public void Reset_CollectionChanged()
		{
			IObservableList<int> collection = new ObservableList<int>();

			Action<IReadOnlyList<int>, IReadOnlyList<int>> assertReset = (oldItems, newItems) =>
			{
				int eventsCount = 0;
				collection.Reset(oldItems);

				IDisposable sub = collection.CollectionChanged.Subscribe(x =>
				{
					x.Match(
						onInsert: y => { Assert.Fail(); },
						onRemove: y => { Assert.Fail(); },
						onReplace: y => { Assert.Fail(); },
						onReset: y =>
						{
							Assert.IsTrue(Enumerable.SequenceEqual(oldItems, y.OldItems));
							Assert.IsTrue(Enumerable.SequenceEqual(newItems, y.NewItems));
						},
						onEmpty: y => { Assert.Fail(); });

					eventsCount++;
				});


				collection.Reset(newItems);
				Assert.IsTrue(Enumerable.SequenceEqual(collection, newItems));
				Assert.AreEqual(eventsCount, 1);
				sub.Dispose();
			};

			Prop.ForAll(
				Arb.From(_intGen.ListOf(10).Select(x => x.ToList())),
				Arb.From(_intGen.ListOf(10).Select(x => x.ToList())),
				assertReset)
				.QuickCheckThrowOnFailure();
		}

		[TestMethod]
		public void Reset_ListChanged()
		{
			IObservableList<int> collection = new ObservableList<int>();

			Action<IReadOnlyList<int>, IReadOnlyList<int>> assertReset = (oldItems, newItems) =>
			{
				int eventsCount = 0;
				collection.Reset(oldItems);

				IDisposable sub = collection.ListChanged.Subscribe(x =>
				{
					x.Match(
						onInsert: y => { Assert.Fail(); },
						onRemove: y => { Assert.Fail(); },
						onReplace: y => { Assert.Fail(); },
						onMove: y => { Assert.Fail(); },
						onReset: y =>
						{
							Assert.IsTrue(Enumerable.SequenceEqual(oldItems, y.OldItems));
							Assert.IsTrue(Enumerable.SequenceEqual(newItems, y.NewItems));
						},
						onEmpty: y => { Assert.Fail(); });

					eventsCount++;
				});


				collection.Reset(newItems);
				Assert.IsTrue(Enumerable.SequenceEqual(collection, newItems));
				Assert.AreEqual(eventsCount, 1);
				sub.Dispose();
			};

			Prop.ForAll(
				Arb.From(_intGen.ListOf(10).Select(x => x.ToList())),
				Arb.From(_intGen.ListOf(10).Select(x => x.ToList())),
				assertReset)
				.QuickCheckThrowOnFailure();
		}

		[TestMethod]
		public void Transaction_ListChanged()
		{
			IObservableList<int> collection = new ObservableList<int>();

			System.Random rng = new System.Random(42);
			int eventsCount = 0;
			int operationsCount = 0;

			IDisposable sub = collection.ListChanged.Subscribe(x =>
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