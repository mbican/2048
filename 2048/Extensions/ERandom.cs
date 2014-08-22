using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2048
{
	class ERandom
	{
		/// <summary>
		/// Generates pseudo random number with exponential distribution.
		/// </summary>
		/// <param name="generator">Pseudo random number generator.</param>
		/// <param name="mean">Arithmetic mean of distribution.</param>
		/// <returns>Returns positive, pseudoramly generated number.</returns>
		public static double NextExponential(this Random generator, double mean = 1)
		{
			if (generator == null)
				throw new ArgumentNullException("generator");
			if (mean <= 0)
				throw new ArgumentOutOfRangeException("mean");
			return -mean * Math.Log(1 - generator.NextDouble());
		}
	}
}
