using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using _2048.AI.MCTS;
using _2048;

namespace _2048Test
{
	[TestClass]
	public class MCTSTest
	{
		[TestMethod] public void MCTSBisect()
		{
			var value = 1.0;
			var bisect = new RangeNode(0, 10, (x) => 10 - Math.Abs(x - value));
			MCTS<RangeNode>.SetRandomSeed(0);
			var root = MCTS.Create(bisect);
			root.Execute(10, parallel: false);
			Assert.IsTrue(root.GetBestLeaf().Node.Value.NearlyEquals(value));
		}
	}
}
