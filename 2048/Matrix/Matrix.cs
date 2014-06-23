using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

#if DEBUG
[assembly: InternalsVisibleTo("2048Test")]
#endif

namespace _2048.Matrix
{
	class Matrix<T>: IMatrix<T>
	{

		private readonly List<T> data = new List<T>();


		public T this[int rowIndex, int columnIndex]
		{
			get { return this.data[this.GetIndex(rowIndex, columnIndex)]; }
			set	{
				if (this._readOnly) throw new NotSupportedException(
					"Trying to set value into read-only matrix."
				);
				this.data[this.GetIndex(rowIndex, columnIndex)] = value; 
			}
		}


		public int RowCount	{ get { return this._rowCount; } }
		private int _rowCount;


		public int ColumnCount { get {	return this._columnCount; } }
		private int _columnCount;


		public bool ReadOnly { get { return this._readOnly; } }
		private bool _readOnly;


		public Matrix(int rowCount, int columnCount, T initValue, bool readOnly = false)
		{
			this.processSize(rowCount, columnCount);
			
			var elemCount = this.RowCount * this.ColumnCount;
			this.data.Capacity = elemCount;
			for (var index = 0; index < elemCount; ++index)
			{
				data.Add(initValue);
			}
			this._readOnly = readOnly;
		}


		public Matrix(IMatrix<T> matrix, bool readOnly = false)
		{
			if (matrix == null)
			{
				throw new ArgumentNullException("matrix");
			}
			this.processSize(matrix.RowCount, matrix.ColumnCount);


			var elemCount = this._rowCount * this._columnCount;
			this.data.Capacity = elemCount;
			data.AddRange(from cell in matrix.TraverseByRows() select cell.Value);
			this._readOnly = readOnly;
		}


		public void Freeze()
		{
			this._readOnly = true;
		}


		private int GetIndex(int rowIndex, int columnIndex)
		{
			if (rowIndex < 0 || this._rowCount <= rowIndex)
			{
				throw new ArgumentOutOfRangeException(
					"rowIndex",
					"rowIndex is out of range."
				);
			}
			if (columnIndex < 0 || this._columnCount <= columnIndex)
			{
				throw new ArgumentOutOfRangeException(
					"columnIndex",
					"columnIndex is out of range."
				);
			}


			return rowIndex * this._columnCount + columnIndex;

		}


		private void processSize(int rowCount, int columnCount)
		{
			if (rowCount < 0)
			{
				throw new ArgumentOutOfRangeException("rowCount", "rowCount < 0");
			}
			if (columnCount < 0)
			{
				throw new ArgumentOutOfRangeException("columnCount", "columnCount < 0");
			}
			if (rowCount == 0 || columnCount == 0)
			{
				rowCount = 0;
				columnCount = 0;
			}

			this._rowCount = rowCount;
			this._columnCount = columnCount;
		}


		public static Matrix<T> FromArray(Array array, int rowLength)
		{
			if (array == null)
				throw new ArgumentNullException("array");
			if (array.Length % rowLength != 0)
				throw new ArgumentException("invalid row length");
			var result = new Matrix<T>(array.Length / rowLength, rowLength, default(T));
			var en = array.GetEnumerator();
			foreach(var element in result.TraverseByRows())
			{
				if (!en.MoveNext())
					throw new InvalidOperationException();
				element.Value = (T)en.Current;
			}
			return result;
		}
	}
}
