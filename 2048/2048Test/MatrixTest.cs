using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using _2048.Matrix;
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
			m = new Matrix<int>(15, 0, 0);
			Assert.AreEqual(0, m.ColumnCount);
			Assert.AreEqual(0, m.RowCount);
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


		[TestMethod]
		public void CopyConstructor()
		{
			Utils.TestException<ArgumentNullException>(
				() => new Matrix<int>(null)
			);
			var m1 = new Matrix<int>(0, 0, 0);
			Assert.AreEqual(0, m1.ColumnCount);
			Assert.AreEqual(0, m1.RowCount);
			var m2 = new Matrix<int>(m1);
			Assert.AreNotSame(m1, m2);
			Assert.AreEqual(0, m2.ColumnCount);
			Assert.AreEqual(0, m2.RowCount);

			m1 = new Matrix<int>(2, 3, 0);
			m1[0, 2] = 5;
			m1[1, 1] = 6;
			m1[1, 2] = 7;
			m2 = m1.ToMatrix();
			Assert.AreNotSame(m1, m2);

			Assert.AreEqual(2, m1.RowCount);
			Assert.AreEqual(2, m2.RowCount);
			Assert.AreEqual(3, m1.ColumnCount);
			Assert.AreEqual(3, m2.ColumnCount);
			Assert.AreEqual(0, m1[0, 0]);
			Assert.AreEqual(0, m2[0, 0]);
			Assert.AreEqual(5, m1[0, 2]);
			Assert.AreEqual(5, m2[0, 2]);
			Assert.AreEqual(6, m1[1, 1]);
			Assert.AreEqual(6, m2[1, 1]);
			Assert.AreEqual(7, m1[1, 2]);
			Assert.AreEqual(7, m2[1, 2]);
		}


		[TestMethod]
		public void Rotate()
		{
			IMatrix<int> m1 = null;
			Utils.TestException<ArgumentNullException>(
				() => m1.Rotate(Rotation._0)
			);
			m1 = new Matrix<int>(0, 0, 0);
			var m2 = m1.Rotate<int>(Rotation.left);
			Assert.AreEqual(0, m2.RowCount);
			Assert.AreEqual(0, m2.ColumnCount);

			m1 = new Matrix<int>(1, 2, 0);
			m1[0, 0] = 1;
			m1[0, 1] = 2;
			var _0 = m1.Rotate(Rotation._0);
			var _180 = m1.Rotate(Rotation._180);
			var left = m1.Rotate(Rotation.left);
			var right = m1.Rotate(Rotation.right);
			Assert.IsTrue(_0.MatrixEqual(m1));
			Assert.IsTrue(_180.Rotate(Rotation._180).MatrixEqual(m1));
			Assert.IsTrue(left.Rotate(Rotation.right).MatrixEqual(m1));
			Assert.IsTrue(right.Rotate(Rotation.left).MatrixEqual(m1));
			
			Assert.AreEqual(1, _0.RowCount);
			Assert.AreEqual(2, _0.ColumnCount);
			Assert.AreEqual(1, _0[0, 0]);
			Assert.AreEqual(2, _0[0, 1]);

			Assert.AreEqual(1, _180.RowCount);
			Assert.AreEqual(2, _180.ColumnCount);
			Assert.AreEqual(2, _180[0, 0]);
			Assert.AreEqual(1, _180[0, 1]);

			Assert.AreEqual(2, left.RowCount);
			Assert.AreEqual(1, left.ColumnCount);
			Assert.AreEqual(2, left[0, 0]);
			Assert.AreEqual(1, left[1, 0]);

			Assert.AreEqual(2, right.RowCount);
			Assert.AreEqual(1, right.ColumnCount);
			Assert.AreEqual(1, right[0, 0]);
			Assert.AreEqual(2, right[1, 0]);

		}

	}
}
