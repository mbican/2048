using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2048.Statistics
{
	public interface IStatistics
	{
		long Count { get; }
		double Mean { get; }
		double StandardDeviation { get; }
		double SampleStandardDeviation { get; }
		double Min { get; }
		double Max { get; }
		void Add(double value);
		void Add(IEnumerable<double> values);
		void Add(IStatistics statistics);
		IStatistics Clone();
	}
}
