using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2048.Matrix
{
	class MatrixDecorator<T>: IMatrix<T>
	{
		protected readonly IMatrix<T> decorated;


		public virtual T this[int rowIndex, int columnIndex]
		{
			get { return this.decorated[rowIndex, columnIndex];	}
			set {
				if (this.ReadOnly) throw new NotSupportedException(
					 "Trying to set value into read-only matrix."
				);
				this.decorated[rowIndex, columnIndex] = value; 
			}
		}


		public virtual int RowCount { get { return this.decorated.RowCount; } }


		public virtual int ColumnCount { get { return this.decorated.ColumnCount; }	}


		public virtual bool ReadOnly { get { return this.decorated.ReadOnly; } }


		public MatrixDecorator(IMatrix<T> matrix)
		{
			if (matrix == null) throw new ArgumentNullException("matrix");
			this.decorated = matrix;
		}

	}
}
