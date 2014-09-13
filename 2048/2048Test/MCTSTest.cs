using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using _2048.AI.MCTS;
using _2048;

namespace _2048Test
{
	[TestClass]
	public class MCTSTest
	{
		[TestMethod]
		public void MCTSBisect()
		{
			var expectedValue = 1.0;
			var floor = 0.0;
			var ceil = 10.0;
			var biasExponent = 0;
			var biasCoeff = 0;
			for (var iterations = 1; iterations < 200; iterations++)
			{
				var bisect = new RangeNode(floor, ceil, (x) => ceil - floor - Math.Abs(x - expectedValue));
				MCTS<RangeNode>.SetRandomSeed(0);
				var root = MCTS.Create(bisect, biasCoeff, 1, biasExponent);
				root.Execute(iterations, parallel: false);
				Assert.AreEqual(iterations, root.Visits);
				var actualValue = root.GetBestLeaf().Node.Middle;
				var epsilon = Math.Max(Math.Pow(2, -iterations / 2.0 + 2), EDouble.RELATIVE_EPSILON);
				var actualEpsilon = GetEpsilon(actualValue, expectedValue);
				var epsilonDiff = epsilon / actualEpsilon;
				Assert.IsTrue(actualValue.NearlyEquals(expectedValue, epsilon));
			}
		}


		[TestMethod]
		public void MCTSBisectRandom()
		{

		}


		private double GetEpsilon(double x, double y)
		{
			var diff = Math.Abs(x - y);
			var avg = (x + y) / 2;
			return diff / avg;
		}
	}
}
