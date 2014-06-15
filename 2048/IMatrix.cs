using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2048
{
	interface IMatrix<T>
	{
		int RowCount
		{
			get;
		}
		int ColumnCount
		{
			get;
		}
		T this[int rowIndex, int columnIndex]
		{
			get;
			set;
		}
	}
}
