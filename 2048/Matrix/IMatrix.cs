using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2048.Matrix
{
	interface IMatrix<T>
	{
		T this[int rowIndex, int columnIndex] { get; set; }
		int RowCount { get; }
		int ColumnCount { get; }
		bool ReadOnly { get; }
	}
}
