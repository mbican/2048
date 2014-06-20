using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

#if DEBUG
[assembly: InternalsVisibleTo("2048Test")]
#endif

namespace _2048.AI.MCTS
{
	/// <summary>
	/// This class implements Monte Carlo tree search algorithm.
	/// </summary>
	class MCTS
	{
		// sqrt(1/5) - from Go game
		private const double biasConst = 0.44721359549995793928183473374626;

		private readonly IMCTSGame game;
		private readonly MCTS parent;
		private readonly List<MCTS> children = new List<MCTS>();


		public int BestMove { get { return 0; } }


		public int Visits { get { return this._visits; } }
		private int _visits;
		private static int _static_visits;
		public int Score { get { return this._score; } }
		private int _score;


		private static Random random = new Random();


		public double WinRate
		{
			get
			{
				return ((double)this._score / this._visits);
			}
		}


		public double Bias
		{
			get
			{
				return biasConst * Math.Sqrt(Math.Log(this.parentsVisits) / this._visits);
			}
		}


		public double UCT
		{
			get
			{
				return WinRate + Bias;
			}
		}


		public bool Complete { get { return this._complete; } }
		private bool _complete;


		private int parentsVisits
		{
			get
			{
				if (this.parent == null)
					return this._visits;
				else
					return this.parent.parentsVisits;
			}
		}


		private int depth
		{
			get
			{
				if(this._depth < 0)
				{
					this._depth = 0;
					var parent = this.parent;
					while(parent != null)
					{
						this._depth++;
						parent = parent.parent;
					}
				}
				return this._depth;
			}
		}
		private int _depth = -1;


		public MCTS(IMCTSGame game, MCTS parent = null)
		{
			this.game = game;
			this.parent = parent;
		}


		bool Execute()
		{
			bool result = false;
			if (this.Visits == 0)
			{
				for (int move = 0; move < this.game.PossibleMoves; move++)
				{
					var gameClone = this.game.Clone();
					bool moved;
					do
					{
						moved = gameClone.TryMove(move);
						if (moved)
							this.children.Add(new MCTS(gameClone, this));
					} while (!moved && ++move < this.game.PossibleMoves);
				}
				if (this.children.Count == 0)
				{
					result = true;
					this._score += this.game.Score;
					this._complete = true;
				}
			}
			if (0 < this.children.Count)
			{
				while (!result)
				{
					this.children.Sort(
						(x, y) =>
						{
							if (x.Complete || y.Complete)
								return (x.Complete ? 1 : 0) - (y.Complete ? 1 : 0);
							else if (x.Visits == 0 || y.Visits == 0)
								return x.Visits - y.Visits;
							else
								return x.UCT.CompareTo(y.UCT);
						}
					);
					int emptyChildren = 0;
					foreach (var child in this.children)
					{
						if (child.Visits == 0)
							emptyChildren += 1;
						else
							break;
					}
					var chosen_child = this.children[random.Next(emptyChildren)];
					if (chosen_child.Complete)
						break;
					this._score -= chosen_child.Score;
					try
					{
						result = chosen_child.Execute();
					}
					finally
					{
						this._score += chosen_child.Score;
					}
					if (!result && !chosen_child.Complete)
						throw new InvalidOperationException(
							"Node didn't execute and Complete is false."
						);
				}
			}
			if (result)
			{
				this._visits += 1;
				_static_visits += 1;
			}
			else
			{
				this._complete = true;
			}
			return result;
		}


		public static int GetNextMove(IMCTSGame game, int iterations)
		{
			if (game == null)
				throw new ArgumentNullException("game");
			if (iterations <= 0)
				throw new ArgumentOutOfRangeException("iterations");
			var root = new MCTS(game);
			for (int count = 0; count < iterations; count++)
			{
				if (!root.Execute())
					break;
			}
			return root.BestMove;
		}


		public static void SetRandomSeed(int seed)
		{
			random = new Random(seed);
		}
	}
}
