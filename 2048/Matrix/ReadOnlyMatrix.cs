using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2048.Matrix
{
	class ReadOnlyMatrix<T>: MatrixDecorator<T>
	{
		public override bool ReadOnly { get { return true; } }


		public ReadOnlyMatrix(IMatrix<T> matrix) : base(matrix) {}
	}
}
