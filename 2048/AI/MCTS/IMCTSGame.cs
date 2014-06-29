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
		/// Must not be negative!
		/// Possible implementation: 0: you lost; 1: draw; 2: you won.
		/// </summary>
		int Score { get; }


		/// <summary>
		/// True if it is player's A turn; false if it is player's B turn.
		/// </summary>
		bool PlayersATurn { get; }
		
		
		/// <summary>
		/// How much moves is possible to try to perform in current state of
		/// the game. Will be used as limit for argument for <see cref="Move"/> 
		/// method.
		/// </summary>
		int PossibleMoves { get; }


		/// <summary>
		/// True if game can perform the move by itself by <see cref="AutoMove"/>
		/// method. e.g. when it's an one player game with some random component
		/// which can be seen as player B.
		/// </summary>
		bool IsAutoMovePossible { get; }


		// IList<double> AutoMoveProbabilities { get; }


		/// <summary>
		/// Select move automatically by game model. This is prefered method
		/// over <see cref="TryMove"/> because there can be hidden probability 
		/// distribution of different moves.
		/// </summary>
		/// <returns>
		/// Returns move index. To be passed into <see cref="TryMove"/>.
		/// </returns>
		/// <exception cref="InvalidOperationException">
		/// if <see cref="IsAutoMovePossible"/> is false.</exception>
		int GetAutoMoveIndex();


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
