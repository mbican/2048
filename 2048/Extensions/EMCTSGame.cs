using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using _2048.AI.MCTS;

namespace _2048
{
	static class EMCTSGame
	{
		private static Random random = new Random();
		public static bool RandomMove(this IMCTSGame game)
		{
			if (game == null) throw new ArgumentNullException("game");
			if (game.IsAutoMovePossible)
			{
				game.AutoMove();
				return true;
			}
			else
			{
				var possibleMoves = game.PossibleMoves;
				if (0 < possibleMoves)
				{
					var moveIndex = random.Next(possibleMoves);
					if (game.TryMove(moveIndex))
						return true;
					else
					{
						var moves = new List<int>(possibleMoves);
						int move;
						for (move = 0; move < possibleMoves; move++)
						{
							if (move != moveIndex)
								moves.Add(move);
						}
						bool moved = false;
						do
						{
							moveIndex = random.Next(moves.Count);
							move = moves[moveIndex];
							moves[moveIndex] = moves[moves.Count - 1];
							moves.RemoveAt(moves.Count - 1);
							moved = game.TryMove(move);
						} while (!moved && 0 < moves.Count);
						return moved;
					}
				}
				else
					return false;
			}
		}
		public static void SeedRandom(int seed)
		{
			random = new Random(seed);
		}
	}
}
