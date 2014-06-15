using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using _2048;

namespace _2048Test
{
	[TestClass]
	public class MatrixTest
	{
		[TestMethod]
		public void MatrixTest1()
		{
			var m = new Matrix<int>(0, 0, 0);
			Assert.AreEqual(0, m.RowCount);
			Assert.AreEqual(0, m.ColumnCount);
			Utils.TestException<ArgumentOutOfRangeException>(
				() => new Matrix<int>(-1, 0, 0)
			);
			Utils.TestException<ArgumentOutOfRangeException>(
				() => new Matrix<int>(0, -1, 0)
			);
			Utils.TestException<ArgumentOutOfRangeException>(
				() => new Matrix<int>(-1, -1, 0)
			);
			Utils.TestException<ArgumentOutOfRangeException>(
				() => m[0, 0] = 1
			);
			Utils.TestException<ArgumentOutOfRangeException>(
				() => Assert.AreEqual(1, m[0, 0])
			);
			m = new Matrix<int>(1, 1, 0);
			Assert.AreEqual(0, m[0, 0]);
			Utils.TestException<ArgumentOutOfRangeException>(
				() => Assert.AreEqual(0, m[1, 0])
			);
			Utils.TestException<ArgumentOutOfRangeException>(
				() => Assert.AreEqual(0, m[0, 1])
			);
			Utils.TestException<ArgumentOutOfRangeException>(
				() => Assert.AreEqual(0, m[1, 1])
			);
			m[0, 0] = 5;
			Assert.AreEqual(5, m[0, 0]);
			m = new Matrix<int>(2, 3, 1);
			Assert.AreEqual(1, m[0, 0]);
			Assert.AreEqual(1, m[0, 1]);
			Assert.AreEqual(1, m[0, 2]);
			Assert.AreEqual(1, m[1, 0]);
			Assert.AreEqual(1, m[1, 1]);
			Assert.AreEqual(1, m[1, 2]);
			m[0, 2] = 5;
			m[1, 2] = 6;
			Assert.AreEqual(5, m[0, 2]);
			Assert.AreEqual(6, m[1, 2]);
			Utils.TestException<ArgumentOutOfRangeException>(
				() => Assert.AreEqual(0, m[0, 3])
			);
			Utils.TestException<ArgumentOutOfRangeException>(
				() => Assert.AreEqual(0, m[2, 0])
			);
		}
	}
}
