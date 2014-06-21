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

	
		/// <summary>
		/// Perform's move automatically by game model. This is prefered method
		/// over <see cref="TryMove"/> because there can be hidden probability 
		/// distribution of different moves.
		/// </summary>
		/// <returns>
		/// Returns move index. If you passed this value as argument into 
		/// <see cref="TryMove"/> it would perform the same action.
		/// </returns>
		/// <exception cref="InvalidOperationException">
		/// if <see cref="IsAutoMovePossible"/> is false.</exception>
		public static int AutoMove(this IMCTSGame game)
		{
			if(game==null) throw new ArgumentNullException("game");
			if (!game.IsAutoMovePossible)
				throw new InvalidOperationException(
					"Auto-Move is not possible."
				);
			var move = game.GetAutoMoveIndex();
			if (!game.TryMove(move))
				throw new InvalidOperationException("auto-move wasn't performed.");
			return move;
		}


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


		public static int RandomFinish(this IMCTSGame game)
		{
			if (game == null) throw new ArgumentNullException("game");
			int moves = 0;
			while (game.RandomMove()) ++moves;
			return moves;
		}
	}
}
