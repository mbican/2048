using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using _2048.Matrix;

namespace _2048
{
	static class EElement
	{
		public static IEnumerable<T> Values<T>(this IEnumerable<Element<T>> elements)
		{
			if (elements == null) throw new ArgumentNullException("elements");
			return elements.Select((element) => element.Value);
		}
	}
}
