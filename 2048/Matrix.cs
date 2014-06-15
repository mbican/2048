﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("2048Test")]

namespace _2048
{
	class Matrix<T>: IMatrix<T>
	{

		public int RowCount
		{
			get
			{
				return this._rowCount;
			}
		}
		private readonly int _rowCount;


		public int ColumnCount
		{
			get
			{
				return this._columnCount;
			}
		}
		private readonly int _columnCount;


		private readonly List<T> data = new List<T>();


		public T this[int rowIndex,int columnIndex]
		{
			get
			{
				return this.data[this.GetIndex(rowIndex, columnIndex)];
			}
			set
			{
				this.data[this.GetIndex(rowIndex, columnIndex)] = value;
			}
		}



		public Matrix(int rowCount, int columnCount, T initValue)
		{
			if (rowCount < 0)
			{
				throw new ArgumentOutOfRangeException("rowCount", "rowCount < 0");
			}
			if (columnCount < 0)
			{
				throw new ArgumentOutOfRangeException("columnCount", "columnCount < 0");
			}


			this._rowCount = rowCount;
			this._columnCount = columnCount;


			var elemCount = this.RowCount * this.ColumnCount;
			this.data.Capacity = elemCount;
			for (var index = 0; index < elemCount; ++index)
			{
				data.Add(initValue);
			}

		}


		public Matrix(IMatrix<T> matrix)
		{
			if (matrix.RowCount < 0)
			{
				throw new ArgumentException("matrix.RowCount < 0","matrix");
			}
			if (matrix.ColumnCount < 0)
			{
				throw new ArgumentException("matrix.ColumnCount < 0","matrix");
			}


			this._rowCount = matrix.RowCount;
			this._columnCount = matrix.ColumnCount;


			var elemCount = this._rowCount * this._columnCount;
			for (var rowIndex = 0; rowIndex < elemCount; rowIndex += this._columnCount)
			{
				for (var colIndex = 0; colIndex < this._columnCount; colIndex++)
				{
					data.Add(matrix[rowIndex,colIndex]);
				}
			}
		}


		private int GetIndex(int rowIndex, int columnIndex)
		{
			if (rowIndex < 0 || this.RowCount <= rowIndex)
			{
				throw new ArgumentOutOfRangeException(
					"rowIndex",
					"rowIndex is out of range."
				);
			}
			if (columnIndex < 0 || this.ColumnCount<=columnIndex)
			{
				throw new ArgumentOutOfRangeException(
					"columnIndex",
					"columnIndex is out of range."
				);
			}


			return rowIndex * this.ColumnCount + columnIndex;

		}

	}
}