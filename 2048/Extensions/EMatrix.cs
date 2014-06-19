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
			var result = new StringBuilder();
			foreach (var row in matrix.Rows())
			{
				foreach (var elem in row)
				{
					if (elem.Value)
						result.Append("#");
					else
						result.Append(" ");
				}
				result.AppendLine();
			}
			return result.ToString();
		}


		public static string ToDebugString<T>(this IMatrix<T> matrix, int colSize = 8)
		{
			var result = new StringBuilder();
			foreach (var row in matrix.Rows())
			{
				foreach (var elem in row)
				{
					result.AppendFormat(String.Format("{{0,{0}{1}", colSize, "}"), elem.Value);
				}
				result.AppendLine();
			}
			return result.ToString();
		}


		public static IMatrix<T> Rotate<T>(this IMatrix<T> matrix, Rotation direction)
		{
			return new MatrixRotator<T>(matrix, direction);
		}


		public static IMatrix<T> AsIMatrix<T>(this IMatrix<T> matrix)
		{
			return matrix;
		}
		

		public static Matrix<T> ToMatrix<T>(this IMatrix<T> matrix, bool readOnly = false)
		{
			return new Matrix<T>(matrix, readOnly);
		}


		public static IMatrix<T> AsReadOnly<T>(this IMatrix<T> matrix)
		{
			return new ReadOnlyMatrix<T>(matrix);
		}


		public static bool MatrixEqual<T>(this IMatrix<T> matrix1, IMatrix<T> matrix2)
		{
			if (matrix1 == null) throw new ArgumentNullException("matrix1");
			if (matrix2 == null) throw new ArgumentNullException("matrix2");
			return matrix1.RowCount == matrix2.RowCount &&
				matrix1.ColumnCount == matrix2.ColumnCount &&
				matrix1.TraverseByRows().Values().SequenceEqual(
					matrix2.TraverseByRows().Values()
				);
		}


		public static IEnumerable<Element<T>> TraverseByRows<T>(this IMatrix<T> matrix)
		{
			if( matrix == null ) throw new ArgumentNullException("matrix");
			for (var rowIndex = 0; rowIndex < matrix.RowCount; rowIndex++)
			{
				for (var columnIndex = 0; columnIndex < matrix.ColumnCount; columnIndex++)
				{
					yield return new Element<T>(matrix, rowIndex, columnIndex);
				}
			}
		}


		public static IEnumerable<Element<T>> Row<T>(this IMatrix<T> matrix, int rowIndex)
		{
			if (matrix == null) throw new ArgumentNullException("matrix");
			if (rowIndex < 0 || matrix.RowCount <= rowIndex)
				throw new ArgumentOutOfRangeException("rowIndex");

			for (var columnIndex = 0; columnIndex < matrix.ColumnCount; columnIndex++)
			{
				yield return new Element<T>(matrix, rowIndex, columnIndex);
			}
		}


		public static IEnumerable<IEnumerable<Element<T>>> Rows<T>(this IMatrix<T> matrix)
		{
			if (matrix == null) throw new ArgumentNullException("matrix");
			for (var rowIndex = 0; rowIndex < matrix.RowCount; rowIndex++)
			{
				yield return matrix.Row(rowIndex);
			}
		}


		public static IEnumerable<Element<T>> Column<T>(this IMatrix<T> matrix, int columnIndex)
		{
			if (matrix == null) throw new ArgumentNullException("matrix");
			if (columnIndex < 0 || matrix.ColumnCount <= columnIndex)
				throw new ArgumentOutOfRangeException("columnIndex");

			for (var rowIndex = 0; rowIndex < matrix.ColumnCount; rowIndex++)
			{
				yield return new Element<T>(matrix, rowIndex, columnIndex);
			}
		}


		public static IEnumerable<IEnumerable<Element<T>>> Columns<T>(this IMatrix<T> matrix)
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
