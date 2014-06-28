using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _2048.Statistics
{
	/// <summary>
	/// Thread safe class for incremental computation of mean, standard deviation,
	/// sample standard deviation
	/// </summary>	
	class StandardDeviation
	{
		// http://en.wikipedia.org/wiki/Standard_deviation#Rapid_calculation_methods
		private double q;
		private Object _lock = new Object();


		public long Count { get { lock(this._lock) return this._count; } }
		private long _count;


		public double Mean { get { lock(this._lock) return this._mean; } }
		private double _mean;


		public double StandardDeviation
		{
			get
			{
				lock (this._lock)
				{
					if (!this._standardDeviation.HasValue)
						this._standardDeviation = Math.Sqrt(this.q / this._count);
					return this._standardDeviation.Value;
				}
			}
		}
		private double? _standardDeviation;


		public double SampleStandardDeviation
		{
			get
			{
				lock (this._lock)
				{
					if (!this._sampleStandardDeviation.HasValue)
						this._sampleStandardDeviation =
							Math.Sqrt(this.q / (this._count - 1));
					return this._sampleStandardDeviation.Value;
				}
			}
		}
		private double? _sampleStandardDeviation;


		public double Min { get { lock(this._lock) return this._min; } }
		private double _min = double.PositiveInfinity;


		public double Max { get { lock(this._lock) return this._max; } }
		private double _max = double.NegativeInfinity;


		public StandardDeviation() { }


		public StandardDeviation(StandardDeviation initialValue)
		{
			if (initialValue == null)
				throw new ArgumentNullException("initialValue");
			lock (initialValue._lock)
			{
				this._count = initialValue._count;
				this._mean = initialValue._mean;
				this.q = initialValue.q;
				this._min = initialValue._min;
				this._max = initialValue._max;
			}
		}


		public void Add(double value)
		{
			lock (this._lock)
			{
				var prevMean = this._mean;
				// http://en.wikipedia.org/wiki/Standard_deviation#Rapid_calculation_methods
				this._mean += (value - this._mean) / ++this._count;
				this.q += (value - prevMean) * (value - this._mean);
				this._standardDeviation = null;
				this._sampleStandardDeviation = null;
				if (value < this._min)
					this._min = value;
				if (this._max < value)
					this._max = value;
			}
		}


		public void Add(IEnumerable<double> values)
		{
			if (values == null)
				throw new ArgumentNullException("values");
			lock (this._lock)
			{
				foreach (var value in values)
					this.Add(value);
			}
		}


		public void Add(StandardDeviation value)
		{
			if (value == null)
				throw new ArgumentNullException("value");
			lock (this._lock)
			{
				var prevCount = this._count;
				var prevMean = this._mean;
				this._count += value._count;
				this._mean += (value._mean - this._mean) * value._count / this._count;
				this.q += value._count * (value._mean - prevMean) * (value._mean - this._mean);
				this._standardDeviation = null;
				this._sampleStandardDeviation = null;
				if (value._min < this._min)
					this._min = value._min;
				if (this._max < value._max)
					this._max = value._max;
			}
		}


		public StandardDeviation Clone()
		{
			return new StandardDeviation(this);
		}
	}
}
