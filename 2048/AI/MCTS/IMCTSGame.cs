using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2048.AI.MCTS
{
	interface IMCTSGame
	{
		/// <summary>
		/// Score of the end state of the game. Higher is better. Will be read
		/// only if <see cref="PossibleMoves"/> is 0 (in the end of the game).
		/// Possible implementation: -1: you lost; 0: draw; 1: you won.
		/// </summary>
		int Score { get; }
		
		
		/// <summary>
		/// How much moves is possible to try to perform in current state of
		/// the game. Will be used as limit for argument for <see cref="Move"/> 
		/// method.
		/// </summary>
		int PossibleMoves { get; }


		/// <summary>
		/// Tries to perform move.
		/// </summary>
		/// <param name="move">Chosen move to perform. Must be Zero or higher 
		/// and lower than <see cref="PossibleMoves"/>.</param>
		/// <exception cref="ArgumentOutOfRangeException">
		/// if <paramref name="move"/> is lower then zero or higher or equal
		/// <see cref="PossibleMoves"/>.</exception>
		/// <returns>True if move was performed; false if move is not possible.
		/// </returns>
		bool TryMove(int move);


		/// <summary>
		/// Creates deep copy <see cref="IMCTSGame"/> object.
		/// </summary>
		IMCTSGame Clone();
	}
}
