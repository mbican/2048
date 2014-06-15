using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace _2048
{
	static class EMatrix
	{
		public static string ToDebugString(this Matrix<bool> matrix)
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
	}
}
