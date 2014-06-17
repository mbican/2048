using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using _2048.Matrix;

namespace _2048
{
	static class EMatrix
	{
		public static string ToDebugString(this IMatrix<bool> matrix)
		{
			var result=new StringBuilder();
			for (var row = 0; row < matrix.RowCount; ++row)
			{
				for (var column = 0; column < matrix.ColumnCount; ++column)
				{
					if ( matrix[row,column])
					{
						result.Append("#");
					}else{
						result.Append(" ");
					}
				}
				result.AppendLine();
			}
			return result.ToString();
		}


		public static IMatrix<T> Rotate<T>(this IMatrix<T> matrix, Rotation direction)
		{
			return new MatrixRotator<T>(matrix, direction);
		}
		

		public static Matrix<T> ToMatrix<T>(this IMatrix<T> matrix)
		{
			return new Matrix<T>(matrix);
		}


		public static bool MatrixEqual<T>(this IMatrix<T> matrix1, IMatrix<T> matrix2)
		{
			if (matrix1 == null) throw new ArgumentNullException("matrix1");
			if (matrix2 == null) throw new ArgumentNullException("matrix2");
			return matrix1.RowCount == matrix2.RowCount &&
				matrix1.ColumnCount == matrix2.ColumnCount &&
				matrix1.TraverseByRows().SequenceEqual(matrix2.TraverseByRows());
		}


		public static IEnumerable<T> TraverseByRows<T>(this IMatrix<T> matrix)
		{
			if( matrix == null ) throw new ArgumentNullException("matrix");
			for (var rowIndex = 0; rowIndex < matrix.RowCount; rowIndex++)
			{
				for (var columnIndex = 0; columnIndex < matrix.ColumnCount; columnIndex++)
				{
					yield return matrix[rowIndex, columnIndex];
				}
			}
		}


		public static IEnumerable<T> Row<T>(this IMatrix<T> matrix, int rowIndex)
		{
			if (matrix == null) throw new ArgumentNullException("matrix");
			if (rowIndex < 0 || matrix.RowCount <= rowIndex)
				throw new ArgumentOutOfRangeException("rowIndex");

			for (var columnIndex = 0; columnIndex < matrix.ColumnCount; columnIndex++)
			{
				yield return matrix[rowIndex, columnIndex];
			}
		}


		public static IEnumerable<IEnumerable<T>> Rows<T>(this IMatrix<T> matrix)
		{
			if (matrix == null) throw new ArgumentNullException("matrix");
			for (var rowIndex = 0; rowIndex < matrix.RowCount; rowIndex++)
			{
				yield return matrix.Row(rowIndex);
			}
		}


		public static IEnumerable<T> Column<T>(this IMatrix<T> matrix, int columnIndex)
		{
			if (matrix == null) throw new ArgumentNullException("matrix");
			if (columnIndex < 0 || matrix.ColumnCount <= columnIndex)
				throw new ArgumentOutOfRangeException("columnIndex");

			for (var rowIndex = 0; rowIndex < matrix.ColumnCount; rowIndex++)
			{
				yield return matrix[rowIndex, columnIndex];
			}
		}


		public static IEnumerable<IEnumerable<T>> Columns<T>(this IMatrix<T> matrix)
		{
			if (matrix == null) throw new ArgumentNullException("matrix");
			for (var columnIndex = 0; columnIndex < matrix.ColumnCount; columnIndex++)
			{
				yield return matrix.Column(columnIndex);
			}
		}

	}

	public enum Rotation
	{
		left,
		right,
		_180,
		_0,
	}
}
