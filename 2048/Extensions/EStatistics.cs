using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _2048.Statistics;

namespace _2048
{
	static class EStatistics
	{

		/// <summary>
		/// Tests if two objects are equal within given relative epsilon.
		/// </summary>
		/// <param name="epsilon">maximum allowed relative error (e.g. 0.01 for 1% accurancy)</param>
		/// <returns>True if two objects are equal; false otherwise.</returns>
		public static bool NearlyEquals(this IStatistics a, IStatistics b, double epsilon = EDouble.RELATIVE_EPSILON)
		{
			if (a == null)
				throw new ArgumentNullException("a");
			if (b == null)
				throw new ArgumentNullException("b");
			return a.Count == b.Count &&
				a.Mean.NearlyEquals(b.Mean, epsilon) &&
				a.StandardDeviation.NearlyEquals(b.StandardDeviation, epsilon) &&
				a.Min == b.Min &&
				a.Max == b.Max;
		}

	}
}
