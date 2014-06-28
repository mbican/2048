﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using _2048.Statistics;

namespace _2048
{
	static class EEnumerable
	{
		/// <summary>
		/// Computes standard deviation of values. (It also computes 
		/// count of values, arithmetic mean, minimal value, maximal value)
		/// </summary>
		/// <param name="values">values to compute standard deviation from.</param>
		public static StandardDeviationCounter StandardDeviation(this IEnumerable<double> values)
		{
			return new StandardDeviationCounter(values);
		}
	}
}