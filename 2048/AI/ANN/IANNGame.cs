using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2048.AI.ANN
{
	interface IANNGame
	{
		/// <summary>
		/// Score of the end state of the game. Higher is better. Will be read
		/// only if <see cref="PossibleMoves"/> is 0 (in the end of the game).
		/// Possible implementation: -1: you lost; 0: draw; 1: you won.
		/// </summary>
		int Score { get; }


		/// <summary>
		/// Gets current state of the game. The length of IList must not change.
		/// </summary>
		IList<bool> State { get; }


		/// <summary>
		/// Required count of elements in <see cref="TryMove"/>'s argument.
		/// The value must not change.
		/// </summary>
		int MoveBitsCount { get; }


		/// <summary>
		/// Tries to perform move. Returns true if move was succesfully 
		/// performed; false if move is not possible due to current game state.
		/// </summary>
		/// <param name="move">move id. The count of elements must be equal to
		/// <see cref="MoveBitsCount"/>.</param>
		/// <returns>true if move was succesfully 
		/// performed; false if move is not possible due to current game state.
		/// </returns>
		/// <exception cref="ArgumentNullException">if <paramref name="move"/> 
		/// is null.</exception>
		/// <exception cref="ArgumentException">if <paramref name="move"/>.Count()
		/// is not equal to <see cref="MoveBitsCount"/>.</exception>
		bool TryMove(IEnumerable<bool> move);
		

		/// <summary>
		/// Creates deep copy <see cref="IANNGame"/> object.
		/// </summary>
		IANNGame Clone();
	}
}
