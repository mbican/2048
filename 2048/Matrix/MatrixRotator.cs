using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2048.Matrix
{
	class MatrixRotator<T>: MatrixDecorator<T>
	{
		private readonly Rotation rotation;


		public override T this[int rowIndex, int columnIndex]
		{
			get {
				return this.decorated[
					this.transformRowIndex(rowIndex, columnIndex),
					this.transformColumnIndex(rowIndex, columnIndex)
				];
			}
			set {
				this.validateIndexes(rowIndex, columnIndex);
				this.decorated[rowIndex, columnIndex] = value; 
			}
		}


		public override int RowCount
		{
			get 
			{
				if (this.rotation == Rotation.left || 
					this.rotation == Rotation.right
				){
					return this.decorated.ColumnCount;
				}
				else
				{
					return this.decorated.RowCount;
				}

			}
		}


		public override int ColumnCount
		{
			get
			{
				if (this.rotation == Rotation.left || 
					this.rotation == Rotation.right
				){
					return this.decorated.RowCount;
				}
				else
				{
					return this.decorated.ColumnCount;
				}

			}
		}		


		public MatrixRotator(IMatrix<T> matrix, Rotation rotation) : base(matrix) 
		{
			if (rotation != Rotation._0 && rotation != Rotation._180 &&
				rotation != Rotation.left && rotation != Rotation.right)
			{
				throw new ArgumentException("unknown Rotation value", "rotation");
			}
			this.rotation = rotation;
		}


		private void validateIndexes(int rowIndex, int columnIndex)
		{
			if (rowIndex < 0 || this.RowCount <= rowIndex)
				throw new ArgumentOutOfRangeException("rowIndex");
			if (columnIndex < 0 || this.ColumnCount <= columnIndex)
				throw new ArgumentOutOfRangeException("columnIndex");
		}


		private int transformRowIndex(int rowIndex, int columnIndex)
		{
			this.validateIndexes(rowIndex, columnIndex);
			switch (this.rotation)
			{
				case Rotation._0:
					return rowIndex;
				case Rotation._180:
					return this.RowCount - rowIndex - 1;
				case Rotation.left:
					return columnIndex;
				case Rotation.right:
					return this.ColumnCount - columnIndex - 1;
				default:
					throw new InvalidOperationException(
						"uknown Matrix Rotation value."
					);
			}
		}


		private int transformColumnIndex(int rowIndex, int columnIndex)
		{
			this.validateIndexes(rowIndex, columnIndex);
			switch (this.rotation)
			{
				case Rotation._0:
					return columnIndex;
				case Rotation._180:
					return this.ColumnCount - columnIndex - 1;
				case Rotation.left:
					return this.RowCount - rowIndex - 1;
				case Rotation.right:
					return rowIndex;
				default:
					throw new InvalidOperationException(
						"uknown Matrix Rotation value."
					);
			}
		}
	}
}
