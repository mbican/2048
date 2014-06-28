﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _2048;
using _2048.Statistics;

namespace _2048Test
{
	[TestClass]
	public class StandardDeviationCounterTest
	{
		[TestMethod]
		public void StandardDeviationPredefinedValuesTest1()
		{
			List<double> values = new List<double>(){1,2};
			var statistics = values.StandardDeviation();
			Assert.AreEqual(2, statistics.Count);
			Assert.AreEqual(1, statistics.Min);
			Assert.AreEqual(2, statistics.Max);
			Assert.AreEqual(1.5, statistics.Mean);
			Assert.AreEqual(0.5, statistics.StandardDeviation);
			Assert.AreEqual(Math.Sqrt(0.5), statistics.SampleStandardDeviation);
			statistics.Add(statistics);
			Assert.AreEqual(4, statistics.Count);
			Assert.AreEqual(1, statistics.Min);
			Assert.AreEqual(2, statistics.Max);
			Assert.AreEqual(1.5, statistics.Mean);
			Assert.AreEqual(0.5, statistics.StandardDeviation);
			Assert.AreEqual(Math.Sqrt(1d/3), statistics.SampleStandardDeviation);
			var statistics2 = new[] { 3d, 4 }.StandardDeviation();
			Assert.AreEqual(2, statistics2.Count);
			Assert.AreEqual(3, statistics2.Min);
			Assert.AreEqual(4, statistics2.Max);
			Assert.AreEqual(3.5, statistics2.Mean);
			Assert.AreEqual(0.5, statistics2.StandardDeviation);
			Assert.AreEqual(Math.Sqrt(0.5), statistics2.SampleStandardDeviation);
			var statistics_clone = statistics.Clone();
			Assert.AreNotSame(statistics, statistics_clone);
			Assert.AreEqual(statistics, statistics_clone);
			var statistics2_clone = statistics2.Clone();
			Assert.AreNotSame(statistics2, statistics2_clone);
			Assert.AreEqual(statistics2, statistics2_clone);
			statistics_clone.Add(statistics2);
			statistics2_clone.Add(statistics);
			Assert.IsTrue(statistics_clone.NearlyEquals(statistics2_clone));
			statistics.Add(3);
			statistics.Add(4);
			Assert.IsTrue(statistics.NearlyEquals(statistics_clone));
			statistics2.Add(1);
			statistics2.Add(2);
			statistics2.Add(1);
			statistics2.Add(2);
			Assert.IsTrue(statistics.NearlyEquals(statistics2));
		}


		[TestMethod]
		public void StandardDeviationPredefinedValuesTest2()
		{
			var statistics = new[] { 1d, 3, 4 }.StandardDeviation();
			Assert.AreEqual(3, statistics.Count);
			Assert.IsTrue(statistics.Mean.NearlyEquals(2.7, 0.02));
			Assert.IsTrue(statistics.StandardDeviation.NearlyEquals(1.24722, 0.000002));
			Assert.IsTrue(statistics.SampleStandardDeviation.NearlyEquals(1.52753, 0.000002));
			var statistics2 = new StandardDeviationCounter();
			statistics2.Add(1);
			var statistics3 = new StandardDeviationCounter();
			statistics3.Add(3);
			statistics2.Add(statistics3);
			statistics3 = new StandardDeviationCounter();
			statistics3.Add(4);
			var statistics4 = statistics3.Clone();
			statistics4.Add(statistics2);
			Assert.IsTrue(statistics4.NearlyEquals(statistics));
			statistics2.Add(statistics3);
			Assert.IsTrue(statistics.NearlyEquals(statistics2));
		}


		[TestMethod]
		public void StandardDeviationRandomAutomaticTest1()
		{
			const int elements = 1000000;
			var list = new List<double>();
			var rand = new Random();
			for (var i = 0; i < elements; ++i)
			{
				list.Add(rand.NextDouble());
			}
			var statistics1 = list.StandardDeviation();
			var statistics2 = new StandardDeviationCounter();
			Parallel.ForEach(
				list,
				(e) => statistics2.Add(e)
			);
			var statistics3 = new StandardDeviationCounter();
			Parallel.ForEach(
				list,
				() => new StandardDeviationCounter(),
				(e, loop, counter) =>
				{
					counter.Add(e);
					return counter;
				},
				(counter) => statistics3.Add(counter)
			);
			Assert.IsTrue(statistics1.NearlyEquals(statistics2));
			Assert.IsTrue(statistics1.NearlyEquals(statistics3));
			int group = 1;
			do
			{
				int counter = 0;
				var statistics4 = new StandardDeviationCounter();
				var temp_statistics = new StandardDeviationCounter();
				foreach (var e in list)
				{
					temp_statistics.Add(e);
					if (group <= ++counter)
					{
						counter = 0;
						statistics4.Add(temp_statistics);
						temp_statistics = new StandardDeviationCounter();
					}
				}
				statistics4.Add(temp_statistics);
				Assert.IsTrue(statistics1.NearlyEquals(statistics4));
			} while ((group *= 2) < elements);
		}
	}
}
