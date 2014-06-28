using Microsoft.VisualStudio.TestTools.UnitTesting;
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
	public class EDoubleTest
	{
		[TestMethod]
		public void NearlyEqualsTest1()
		{
			Assert.IsTrue((1d).NearlyEquals(1.2, 0.5));
			Assert.IsFalse((1d).NearlyEquals(3, 0.5));
			Assert.IsTrue((1d).NearlyEquals(0.8, 0.5));
			Assert.IsFalse((1d).NearlyEquals(-1, 0.5));
		}
	}
}
