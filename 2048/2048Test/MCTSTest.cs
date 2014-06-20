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
		public void MCTSTest1()
		{
			MCTS.SetRandomSeed(0);
			var game = new _2048Model(0);
			while (0 < game.PossibleMoves)
			{
				if (game.IsAutoMovePossible)
					game.AutoMove();
				else
				{
					game.TryMove(MCTS.GetNextMove(game, 10));
				}
			}
		}

	}
}
