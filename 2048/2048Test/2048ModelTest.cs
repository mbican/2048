using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using _2048;

namespace _2048Test
{
	[TestClass]
	public class _2048ModelTest
	{
		[TestMethod]
		public void _2048ModelTest2()
		{
			var model = new _2048Model(0);
			EMCTSGame.SeedRandom(0);
			var moves = model.RandomFinish();
			Trace.Write(model.Matrix.ToDebugString(5));
			Assert.IsFalse(model.IsAutoMovePossible);
			Assert.AreEqual(0,model.PossibleMoves); 
			Assert.IsFalse(model.Matrix.TraverseByRows().Any((v) => v.Value == 0));
		}

		[TestMethod]
		public void _2048ModelTest3()
		{
			for (int counter = 0; counter < 1000; ++counter)
			{
				var model = new _2048Model();
				var moves = model.RandomFinish();
				Trace.Write(model.Matrix.ToDebugString(5));
				Assert.IsFalse(model.IsAutoMovePossible);
				Assert.AreEqual(0, model.PossibleMoves);
				if (model.PlayersATurn)
				{
					Assert.IsFalse(model.MoveDown());
					Assert.IsFalse(model.MoveUp());
					Assert.IsFalse(model.MoveLeft());
					Assert.IsFalse(model.MoveRight());
				}
				Assert.IsFalse(model.Matrix.TraverseByRows().Any((v) => v.Value == 0));
			}
		}

		[TestMethod]
		public void _2048ModelTest1()
		{
			var model = new _2048Model(0);
			Trace.WriteLine("1:");
			Trace.Write(model.Matrix.ToDebugString(4));
			Assert.AreEqual(2, model.Matrix[2, 3]);
			Assert.AreEqual(2, model.Matrix[3, 0]);
			Assert.IsTrue(model.MoveDown());
			Trace.WriteLine("2:");
			Trace.Write(model.Matrix.ToDebugString(4));
			Assert.AreEqual(2, model.Matrix[0, 2]);
			Assert.AreEqual(2, model.Matrix[3, 0]);
			Assert.AreEqual(2, model.Matrix[3, 3]);
			Assert.IsTrue(model.MoveDown());
			Trace.WriteLine("3:");
			Trace.Write(model.Matrix.ToDebugString(4));
			Assert.AreEqual(2, model.Matrix[2, 3]);
			Assert.AreEqual(2, model.Matrix[3, 0]);
			Assert.AreEqual(2, model.Matrix[3, 2]);
			Assert.AreEqual(2, model.Matrix[3, 3]);
			Assert.IsTrue(model.MoveDown());
			Trace.WriteLine("4:");
			Trace.Write(model.Matrix.ToDebugString(4));
			Assert.AreEqual(2, model.Matrix[3, 0]);
			Assert.AreEqual(2, model.Matrix[3, 1]);
			Assert.AreEqual(2, model.Matrix[3, 2]);
			Assert.AreEqual(4, model.Matrix[3, 3]);
			Assert.IsFalse(model.MoveDown());
			Trace.WriteLine("5:");
			Trace.Write(model.Matrix.ToDebugString(4));
			Assert.AreEqual(2, model.Matrix[3, 0]);
			Assert.AreEqual(2, model.Matrix[3, 1]);
			Assert.AreEqual(2, model.Matrix[3, 2]);
			Assert.AreEqual(4, model.Matrix[3, 3]);
			Assert.IsTrue(model.MoveRight());
			Trace.WriteLine("6:");
			Trace.Write(model.Matrix.ToDebugString(4));
			Assert.AreEqual(2, model.Matrix[3, 1]);
			Assert.AreEqual(4, model.Matrix[3, 2]);
			Assert.AreEqual(4, model.Matrix[3, 3]);
			Assert.IsTrue(model.MoveRight());
			Trace.WriteLine("7:");
			Trace.Write(model.Matrix.ToDebugString(4));
			Assert.AreEqual(2, model.Matrix[0, 3]);
			Assert.AreEqual(2, model.Matrix[2, 1]);
			Assert.AreEqual(2, model.Matrix[3, 2]);
			Assert.AreEqual(8, model.Matrix[3, 3]);
			Assert.IsTrue(model.MoveDown());
			Trace.WriteLine("8:");
			Trace.Write(model.Matrix.ToDebugString(4));
			Assert.AreEqual(2, model.Matrix[2, 3]);
			Assert.AreEqual(4, model.Matrix[3, 0]);
			Assert.AreEqual(2, model.Matrix[3, 1]);
			Assert.AreEqual(2, model.Matrix[3, 2]);
			Assert.AreEqual(8, model.Matrix[3, 3]);
			Assert.IsFalse(model.MoveDown());
			Trace.WriteLine("9:");
			Trace.Write(model.Matrix.ToDebugString(4));
			Assert.AreEqual(2, model.Matrix[2, 3]);
			Assert.AreEqual(4, model.Matrix[3, 0]);
			Assert.AreEqual(2, model.Matrix[3, 1]);
			Assert.AreEqual(2, model.Matrix[3, 2]);
			Assert.AreEqual(8, model.Matrix[3, 3]);
			Assert.IsTrue(model.MoveRight());
			Trace.WriteLine("10:");
			Trace.Write(model.Matrix.ToDebugString(4));
			Assert.AreEqual(2, model.Matrix[2, 2]);
			Assert.AreEqual(2, model.Matrix[2, 3]);
			Assert.AreEqual(4, model.Matrix[3, 1]);
			Assert.AreEqual(4, model.Matrix[3, 2]);
			Assert.AreEqual(8, model.Matrix[3, 3]);
			Assert.IsTrue(model.MoveRight());
			Trace.WriteLine("11:");
			Trace.Write(model.Matrix.ToDebugString(4));
			Assert.AreEqual(2, model.Matrix[2, 0]);
			Assert.AreEqual(4, model.Matrix[2, 3]);
			Assert.AreEqual(8, model.Matrix[3, 2]);
			Assert.AreEqual(8, model.Matrix[3, 3]);
			Assert.IsTrue(model.MoveRight());
			Trace.WriteLine("12:");
			Trace.Write(model.Matrix.ToDebugString(4));
			Assert.AreEqual(2, model.Matrix[2, 2]);
			Assert.AreEqual(4, model.Matrix[2, 3]);
			Assert.AreEqual(2, model.Matrix[3, 0]);
			Assert.AreEqual(16, model.Matrix[3, 3]);
			Assert.IsTrue(model.MoveDown());
			Trace.WriteLine("13:");
			Trace.Write(model.Matrix.ToDebugString(4));
			Assert.AreEqual(4, model.Matrix[2, 3]);
			Assert.AreEqual(2, model.Matrix[3, 0]);
			Assert.AreEqual(4, model.Matrix[3, 1]);
			Assert.AreEqual(2, model.Matrix[3, 2]);
			Assert.AreEqual(16, model.Matrix[3, 3]);
			Assert.IsTrue(model.MoveLeft());
			Trace.WriteLine("14:");
			Trace.Write(model.Matrix.ToDebugString(4));
			Assert.AreEqual(2, model.Matrix[1, 3]);
			Assert.AreEqual(4, model.Matrix[2, 0]);
			Assert.AreEqual(2, model.Matrix[3, 0]);
			Assert.AreEqual(4, model.Matrix[3, 1]);
			Assert.AreEqual(2, model.Matrix[3, 2]);
			Assert.AreEqual(16, model.Matrix[3, 3]);
			Assert.IsTrue(model.MoveRight());
			Trace.WriteLine("15:");
			Trace.Write(model.Matrix.ToDebugString(4));
			Assert.AreEqual(2, model.Matrix[1, 3]);
			Assert.AreEqual(2, model.Matrix[2, 2]);
			Assert.AreEqual(4, model.Matrix[2, 3]);
			Assert.AreEqual(2, model.Matrix[3, 0]);
			Assert.AreEqual(4, model.Matrix[3, 1]);
			Assert.AreEqual(2, model.Matrix[3, 2]);
			Assert.AreEqual(16, model.Matrix[3, 3]);
			Assert.IsTrue(model.MoveLeft());
			Trace.WriteLine("16:");
			Trace.Write(model.Matrix.ToDebugString(4));
			Assert.AreEqual(2, model.Matrix[1, 0]);
			Assert.AreEqual(4, model.Matrix[1, 1]);
			Assert.AreEqual(2, model.Matrix[2, 0]);
			Assert.AreEqual(4, model.Matrix[2, 1]);
			Assert.AreEqual(2, model.Matrix[3, 0]);
			Assert.AreEqual(4, model.Matrix[3, 1]);
			Assert.AreEqual(2, model.Matrix[3, 2]);
			Assert.AreEqual(16, model.Matrix[3, 3]);
			Assert.IsTrue(model.MoveDown());
			Trace.WriteLine("17:");
			Trace.Write(model.Matrix.ToDebugString(4));
			Assert.AreEqual(2, model.Matrix[0, 1]);
			Assert.AreEqual(2, model.Matrix[2, 0]);
			Assert.AreEqual(4, model.Matrix[2, 1]);
			Assert.AreEqual(4, model.Matrix[3, 0]);
			Assert.AreEqual(8, model.Matrix[3, 1]);
			Assert.AreEqual(2, model.Matrix[3, 2]);
			Assert.AreEqual(16, model.Matrix[3, 3]);
			Assert.IsTrue(model.MoveRight());
			Trace.WriteLine("18:");
			Trace.Write(model.Matrix.ToDebugString(4));
			Assert.AreEqual(2, model.Matrix[0, 3]);
			Assert.AreEqual(2, model.Matrix[0, 2]);
			Assert.AreEqual(2, model.Matrix[2, 2]);
			Assert.AreEqual(4, model.Matrix[2, 3]);
			Assert.AreEqual(4, model.Matrix[3, 0]);
			Assert.AreEqual(8, model.Matrix[3, 1]);
			Assert.AreEqual(2, model.Matrix[3, 2]);
			Assert.AreEqual(16, model.Matrix[3, 3]);
			Assert.IsTrue(model.MoveDown());
			Trace.WriteLine("19:");
			Trace.Write(model.Matrix.ToDebugString(4));
			Assert.AreEqual(2, model.Matrix[1, 1]);
			Assert.AreEqual(2, model.Matrix[1, 3]);
			Assert.AreEqual(2, model.Matrix[2, 2]);
			Assert.AreEqual(4, model.Matrix[2, 3]);
			Assert.AreEqual(4, model.Matrix[3, 0]);
			Assert.AreEqual(8, model.Matrix[3, 1]);
			Assert.AreEqual(4, model.Matrix[3, 2]);
			Assert.AreEqual(16, model.Matrix[3, 3]);
			Assert.IsTrue(model.MoveDown());
			Trace.WriteLine("20:");
			Trace.Write(model.Matrix.ToDebugString(4));
			Assert.AreEqual(2, model.Matrix[0, 0]);
			Assert.AreEqual(2, model.Matrix[1, 3]);
			Assert.AreEqual(2, model.Matrix[2, 1]);
			Assert.AreEqual(2, model.Matrix[2, 2]);
			Assert.AreEqual(4, model.Matrix[2, 3]);
			Assert.AreEqual(4, model.Matrix[3, 0]);
			Assert.AreEqual(8, model.Matrix[3, 1]);
			Assert.AreEqual(4, model.Matrix[3, 2]);
			Assert.AreEqual(16, model.Matrix[3, 3]);
			Assert.IsTrue(model.MoveRight());
			Trace.WriteLine("21:");
			Trace.Write(model.Matrix.ToDebugString(4));
			Assert.AreEqual(2, model.Matrix[0, 2]);
			Assert.AreEqual(2, model.Matrix[0, 3]);
			Assert.AreEqual(2, model.Matrix[1, 3]);
			Assert.AreEqual(4, model.Matrix[2, 2]);
			Assert.AreEqual(4, model.Matrix[2, 3]);
			Assert.AreEqual(4, model.Matrix[3, 0]);
			Assert.AreEqual(8, model.Matrix[3, 1]);
			Assert.AreEqual(4, model.Matrix[3, 2]);
			Assert.AreEqual(16, model.Matrix[3, 3]);
			Assert.IsTrue(model.MoveDown());
			Trace.WriteLine("22:");
			Trace.Write(model.Matrix.ToDebugString(4));
			Assert.AreEqual(2, model.Matrix[1, 0]);
			Assert.AreEqual(4, model.Matrix[1, 3]);
			Assert.AreEqual(2, model.Matrix[2, 2]);
			Assert.AreEqual(4, model.Matrix[2, 3]);
			Assert.AreEqual(4, model.Matrix[3, 0]);
			Assert.AreEqual(8, model.Matrix[3, 1]);
			Assert.AreEqual(8, model.Matrix[3, 2]);
			Assert.AreEqual(16, model.Matrix[3, 3]);
			Assert.IsTrue(model.MoveRight());
			Trace.WriteLine("23:");
			Trace.Write(model.Matrix.ToDebugString(4));
			Assert.AreEqual(2, model.Matrix[0, 1]);
			Assert.AreEqual(2, model.Matrix[1, 2]);
			Assert.AreEqual(4, model.Matrix[1, 3]);
			Assert.AreEqual(2, model.Matrix[2, 2]);
			Assert.AreEqual(4, model.Matrix[2, 3]);
			Assert.AreEqual(4, model.Matrix[3, 1]);
			Assert.AreEqual(16, model.Matrix[3, 2]);
			Assert.AreEqual(16, model.Matrix[3, 3]);
			Assert.IsTrue(model.MoveRight());
			Trace.WriteLine("24:");
			Trace.Write(model.Matrix.ToDebugString(4));
			Assert.AreEqual(2, model.Matrix[0, 3]);
			Assert.AreEqual(2, model.Matrix[1, 2]);
			Assert.AreEqual(4, model.Matrix[1, 3]);
			Assert.AreEqual(2, model.Matrix[2, 2]);
			Assert.AreEqual(4, model.Matrix[2, 3]);
			Assert.AreEqual(2, model.Matrix[3, 1]);
			Assert.AreEqual(4, model.Matrix[3, 2]);
			Assert.AreEqual(32, model.Matrix[3, 3]);
			Assert.IsTrue(model.MoveUp());
			Trace.WriteLine("25:");
			Trace.Write(model.Matrix.ToDebugString(4));
			Assert.AreEqual(2, model.Matrix[0, 1]);
			Assert.AreEqual(4, model.Matrix[0, 2]);
			Assert.AreEqual(2, model.Matrix[0, 3]);
			Assert.AreEqual(8, model.Matrix[1, 3]);
			Assert.AreEqual(4, model.Matrix[1, 2]);
			Assert.AreEqual(2, model.Matrix[2, 0]);
			Assert.AreEqual(32, model.Matrix[2, 3]);
			Assert.IsTrue(model.MoveDown());
			Trace.WriteLine("26:");
			Trace.Write(model.Matrix.ToDebugString(4));
			Assert.AreEqual(2, model.Matrix[0, 1]);
			Assert.AreEqual(2, model.Matrix[1, 3]);
			Assert.AreEqual(8, model.Matrix[2, 3]);
			Assert.AreEqual(2, model.Matrix[3, 0]);
			Assert.AreEqual(2, model.Matrix[3, 1]);
			Assert.AreEqual(8, model.Matrix[3, 2]);
			Assert.AreEqual(32, model.Matrix[3, 3]);
			var a = "ahoj";
		}

	
		[TestMethod]
		public void _2048ModelCloneTest()
		{
			var model = new _2048Model(0);
			var model2 = new _2048Model(model);
			Assert.AreNotSame(model, model2);
			Assert.IsTrue(model.Matrix.MatrixEqual(model2.Matrix));
			Trace.WriteLine("1:");
			Trace.Write(model.Matrix.ToDebugString(4));
			Trace.WriteLine("");
			Trace.Write(model2.Matrix.ToDebugString(4));
			Assert.AreEqual(2, model.Matrix[2, 3]);
			Assert.AreEqual(2, model.Matrix[3, 0]);
			Assert.IsTrue(model.MoveDown());
			model2.MoveDown();
			Assert.IsTrue(model.Matrix.MatrixEqual(model2.Matrix));
			Trace.WriteLine("2:");
			Trace.Write(model.Matrix.ToDebugString(4));
			Trace.WriteLine("");
			Trace.Write(model2.Matrix.ToDebugString(4));
			Assert.AreEqual(2, model.Matrix[0, 2]);
			Assert.AreEqual(2, model.Matrix[3, 0]);
			Assert.AreEqual(2, model.Matrix[3, 3]);
			Assert.IsTrue(model.MoveDown());
			model2.MoveDown();
			Assert.IsTrue(model.Matrix.MatrixEqual(model2.Matrix));
			Trace.WriteLine("3:");
			Trace.Write(model.Matrix.ToDebugString(4));
			Trace.WriteLine("");
			Trace.Write(model2.Matrix.ToDebugString(4));
			Assert.AreEqual(2, model.Matrix[2, 3]);
			Assert.AreEqual(2, model.Matrix[3, 0]);
			Assert.AreEqual(2, model.Matrix[3, 2]);
			Assert.AreEqual(2, model.Matrix[3, 3]);
			Assert.IsTrue(model.MoveDown());
			model2.MoveDown();
			Assert.IsTrue(model.Matrix.MatrixEqual(model2.Matrix));
			Trace.WriteLine("4:");
			Trace.Write(model.Matrix.ToDebugString(4));
			Trace.WriteLine("");
			Trace.Write(model2.Matrix.ToDebugString(4));
			Assert.AreEqual(2, model.Matrix[3, 0]);
			Assert.AreEqual(2, model.Matrix[3, 1]);
			Assert.AreEqual(2, model.Matrix[3, 2]);
			Assert.AreEqual(4, model.Matrix[3, 3]);
			Assert.IsFalse(model.MoveDown());
			model2.MoveDown();
			Assert.IsTrue(model.Matrix.MatrixEqual(model2.Matrix));
			Trace.WriteLine("5:");
			Trace.Write(model.Matrix.ToDebugString(4));
			Trace.WriteLine("");
			Trace.Write(model2.Matrix.ToDebugString(4));
			Assert.AreEqual(2, model.Matrix[3, 0]);
			Assert.AreEqual(2, model.Matrix[3, 1]);
			Assert.AreEqual(2, model.Matrix[3, 2]);
			Assert.AreEqual(4, model.Matrix[3, 3]);
			var a = "ahoj";
		}


		[TestMethod]
		public void _2048ModelMCTSInterfaceTest()
		{
			var model = new _2048Model(0);
			EMCTSGame.SeedRandom(0);
			Trace.WriteLine("1:");
			Trace.Write(model.Matrix.ToDebugString(4));
			Assert.IsTrue(model.PlayersATurn);
			int i = 1;
			while (model.RandomMove())
			{
				Trace.WriteLine(string.Format("{0}: (score: {1})", ++i, model.Score));
				Trace.Write(model.Matrix.ToDebugString(4));
				Assert.AreEqual((i & 1) != 0, model.PlayersATurn);
				Assert.AreEqual((i & 1) == 0, model.IsAutoMovePossible);
			}
			Assert.AreEqual(0, model.PossibleMoves);
			Assert.IsFalse(model.IsAutoMovePossible);
			var a = "ahoj";
		}


		[TestMethod]
		public void _2048ModelMCTSInterfaceCloneTest()
		{
			var model = new _2048Model(0);
			EMCTSGame.SeedRandom(0);
			Trace.WriteLine("1:");
			Trace.Write(model.Matrix.ToDebugString(4));
			Assert.IsTrue(model.PlayersATurn);
			Assert.IsFalse(model.IsAutoMovePossible);
			model.RandomMove();
			Assert.IsFalse(model.PlayersATurn);
			Assert.IsTrue(model.IsAutoMovePossible);
			var model2 = new _2048Model(model);
			Assert.IsFalse(model2.PlayersATurn);
			Assert.IsTrue(model2.IsAutoMovePossible);
			model.AutoMove();
			Assert.IsFalse(model2.PlayersATurn);
			Assert.IsTrue(model2.IsAutoMovePossible);
			Assert.IsFalse(model.Matrix.MatrixEqual(model2.Matrix));
			Assert.IsTrue(model.PlayersATurn);
			Assert.IsFalse(model.IsAutoMovePossible);
			model2.AutoMove();
			Assert.IsTrue(model.Matrix.MatrixEqual(model2.Matrix));
			Assert.IsTrue(model.PlayersATurn);
			Assert.IsFalse(model.IsAutoMovePossible);
			Assert.IsTrue(model2.PlayersATurn);
			Assert.IsFalse(model2.IsAutoMovePossible);
		}


	}
}
