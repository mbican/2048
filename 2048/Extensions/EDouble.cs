using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2048
{
	static class EDouble
	{
		/// <summary>
		/// Smallest positive normalized value
		/// </summary>
		public const double MIN_NORMAL = 2.2250738585072014e-308;
		/// <summary>
		/// const used for relative comparison. If relative difference of two
		/// double values is smaller than RELATIVE_EPSILON they might be
		/// considered equal.
		/// </summary>
		/// <remarks>
		/// (2.3e-13 = (2^10) / (2^52)) means that 10 least
		/// significant bits from 52 bits long mantisa can be different.
		/// </remarks>
		public const double RELATIVE_EPSILON = 2.3e-13;


		/// <summary>
		/// Tests if two values are nearly equal within given relative epsilon.
		/// </summary>
		/// <param name="a">value to compare</param>
		/// <param name="b">value to compare</param>
		/// <param name="epsilon">maximum allowed relative error (e.g. 0.01 for 1% accurancy)</param>
		/// <returns>True if values are nearly equal; False otherwise.</returns>
		/// <remarks>
		/// epsilon is relative to the avarege value of values <paramref name="a"/> and
		/// <paramref name="b"/>. It will have the same result if you exchange
		/// the values a and b.
		/// </remarks>
		public static bool NearlyEquals(
			this double a, 
			double b, 
			double epsilon = RELATIVE_EPSILON
		)
		{
			if (a == b)
			{ // shortcut, handles infinities
				return true;
			}

			double absA = Math.Abs(a);
			double absB = Math.Abs(b);
			double diff = Math.Abs(a - b);

			if (a == 0 || b == 0 || diff < MIN_NORMAL)
			{
				// a or b is zero or both are extremely close to it
				// relative error is less meaningful here
				return diff <= (epsilon * MIN_NORMAL);
			}
			else
			{ // use relative error
				return diff / (absA + absB) <= epsilon / 2;
			}
		}
	}
}
