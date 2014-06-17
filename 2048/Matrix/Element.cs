using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2048.Matrix
{
	class Element<T>
	{
		public readonly IMatrix<T> matrix;
		public readonly int RowIndex;
		public readonly int ColumnIndex;


		public T Value
		{
			get { return this.matrix[this.RowIndex, this.ColumnIndex]; }
			set { this.matrix[this.RowIndex, this.ColumnIndex] = value; }
		}


		public Element(IMatrix<T> matrix, int rowIndex, int columnIndex)
		{
			if (matrix == null) throw new ArgumentNullException("matrix");
			this.matrix = matrix;
			
			// try access value to validate indexes.
			var value = this.matrix[rowIndex, columnIndex];

			this.RowIndex = rowIndex;
			this.ColumnIndex = columnIndex;
		}


		public override string ToString()
		{
			return string.Format(
				"[{0},{1}]: {2}",
				this.RowIndex,
				this.ColumnIndex,
				this.Value
			);
		}
	}
}
