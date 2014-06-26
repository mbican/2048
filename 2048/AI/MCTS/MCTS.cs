using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
		public readonly int ParentsMove;
		public readonly IMCTSGame Game;


		private const double biasExponent = 0.5;


		private MCTS parent;
		private readonly List<MCTS> children = new List<MCTS>();
		private bool childrenCreated;
		private int _score;


		[ThreadStatic]
		private static Random random;


		public int BestMove 
		{ 
			get 
			{ 
				if (this._bestMove < 0)
				{
					lock (this)
					{
						if (this._bestMove < 0)
						{
							this.EnsureChildren();
							int maxVisits = int.MinValue;
							foreach (var child in this.children)
							{
								if (maxVisits < child._visits)
								{
									maxVisits = child._visits;
									this._bestMove = child.ParentsMove;
								}
							}
						}
					}
				}
				return this._bestMove;
			} 
		}
		private int _bestMove = -1;


		public int Visits { get { return this._visits; } }
		private int _visits;


		public double WinRate { get { return (double)this._score / this._visits; } }


		public double Bias
		{
			get
			{
				return Math.Pow(Math.Log(this.parentsVisits) / this._visits, biasExponent);
			}
		}


		public double Uct
		{
			get
			{
				return WinRate * Bias;
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


		private readonly int depth;


		public MCTS(IMCTSGame game, int parentsMove = 0, MCTS parent = null)
		{
			if (random == null)
				random = new Random();
			this.Game = game;
			this.ParentsMove = parentsMove;
			this.parent = parent;
			if (this.parent != null)
				this.depth = this.parent.depth + 1;
		}


		public override string ToString()
		{
			return string.Format("^{0}#{1}:{2:F0}({3:F2}*{4:F0})", this.depth, this.Visits, this.Uct, this.Bias, this.WinRate);
		}


		bool Execute(out int score)
		{
			score = 0;
			if (this.Complete)
				return false;
			bool result = false;
			if (Interlocked.CompareExchange(ref this._visits, 1, 0) == 0)
			{
				var gameClone = this.Game.Clone();
				gameClone.RandomFinish();
				result = true;
				score = gameClone.Score;
				Interlocked.Add(ref this._score, score);
			}
			else
			{
				Interlocked.Increment(ref this._visits);
				this.EnsureChildren();
			}
			if (0 < this.children.Count)
			{
				while (!result)
				{
					int unvisited = 0;
					double maxUct = double.MinValue;
					MCTS chosenChild = null;
					foreach(var child in this.children)
					{
						if (!child.Complete)
						{
							if (child._visits == 0)
							{
								if (random.Next(unvisited++) == 0)
									chosenChild = child;
							}
							else if (unvisited == 0 && maxUct < child.Uct)
							{
								maxUct = child.Uct;
								chosenChild = child;
							}
						}
					}
					if (chosenChild == null)
						break;
					result = chosenChild.Execute(out score);
					Interlocked.Add(ref this._score, score);
					if (!result && !chosenChild.Complete)
						throw new InvalidOperationException(
							"Node didn't execute and Complete is false."
						);
				}
			}
			if (result)
			{
				this._bestMove = -1;
			}
			else
			{
				Interlocked.Decrement(ref this._visits);
				this._complete = true;
			}
			return result;
		}


		public void Execute(int visits)
		{
			Parallel.For(this._visits, visits,
				(visit, state) =>
				{
					if (this.Complete)
						state.Break();
					int score;
					this.Execute(out score);
				}
			);
		}


		public bool TryAutoMove(out MCTS newRoot)
		{
			if (this.Game.IsAutoMovePossible)
			{
				int move = this.Game.GetAutoMoveIndex();
				newRoot = this.GetChildByMove(move);
				Object buga; // nastavovat parent na null není dobré (je to optimalizace kvůli GC)
				newRoot.parent = null;
				return true;
			}
			else
				newRoot = this;
				return false;
		}


		public bool TryMove(int iterations, out MCTS newRoot, bool preferAutoMove = true)
		{
			if (preferAutoMove && this.TryAutoMove(out newRoot))
				return true;
			else
			{
				this.Execute(iterations);
				if (this.BestMove < 0)
				{
					newRoot = this;
					return false;
				}
				newRoot = this.GetChildByMove(this.BestMove);
				Object buga; // nastavovat parent na null není dobré (je to optimalizace kvůli GC)
				newRoot.parent = null;
				return true;
			}
		}


		private MCTS GetChildByMove(int move)
		{
			this.EnsureChildren();
			foreach (var child in this.children)
			{
				if (child.ParentsMove == move)
				{
					return child;
				}
			}
			throw new InvalidOperationException();
		}


		private void EnsureChildren()
		{
			if (!this.childrenCreated)
			{
				lock (this.children)
				{
					// multiple threads can get into the lock, only the first
					// can create children
					if (!this.childrenCreated)
					{
						for (int move = 0; move < this.Game.PossibleMoves; move++)
						{
							var gameClone = this.Game.Clone();
							bool moved;
							do
							{
								moved = gameClone.TryMove(move);
								if (moved)
									this.children.Add(new MCTS(gameClone, move, this));
							} while (!moved && ++move < this.Game.PossibleMoves);
						}
						this.childrenCreated = true;
					}
				}
			}
		}


		public static int GetNextMove(IMCTSGame game, int iterations)
		{
			if (game == null)
				throw new ArgumentNullException("game");
			if (iterations <= 0)
				throw new ArgumentOutOfRangeException("iterations");
			var root = new MCTS(game);
			root.Execute(iterations);
			return root.BestMove;
		}


		public static void SetRandomSeed(int seed)
		{
			random = new Random(seed);
			EMCTSGame.SeedRandom(seed);
		}
	}
}
