﻿using System;
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
		public readonly int ParentsMove;
		public readonly IMCTSGame Game;


		private const double biasExponent = 0.5;


		private MCTS parent;
		private readonly List<MCTS> children = new List<MCTS>();
		private bool childrenCreated;


		private static Random random = new Random();


		public int BestMove 
		{ 
			get 
			{ 
				if (this._bestMove < 0)
				{
					int maxVisits = 0;
					foreach (var child in this.children)
					{
						if (maxVisits < child.Visits)
						{
							maxVisits = child.Visits;
							this._bestMove = child.ParentsMove;
						}
					}
				}
				return this._bestMove;
			} 
		}
		private int _bestMove = -1;


		public int Visits { get { return this._visits; } }
		private int _visits;
		private static int _static_visits;
		public int Score { get { return this._score; } }
		private int _score;


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


		public MCTS(IMCTSGame game, int parentsMove = 0, MCTS parent = null)
		{
			this.Game = game;
			this.ParentsMove = parentsMove;
			this.parent = parent;
		}


		public override string ToString()
		{
			return string.Format("^{0}#{1}:{2:F0}({3:F2}*{4:F0})", this.depth, this.Visits, this.Uct, this.Bias, this.WinRate);
		}


		bool Execute()
		{
			bool result = false;
			if (this.Visits == 0)
			{
				var gameClone = this.Game.Clone();
				gameClone.RandomFinish();
				result = true;
				this._score += gameClone.Score;
			}
			else
				this.EnsureChildren();
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
							if (child.Visits == 0)
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
					this._score -= chosenChild.Score;
					try
					{
						result = chosenChild.Execute();
					}
					finally
					{
						this._score += chosenChild.Score;
					}
					if (!result && !chosenChild.Complete)
						throw new InvalidOperationException(
							"Node didn't execute and Complete is false."
						);
				}
			}
			if (result)
			{
				this._visits += 1;
				_static_visits += 1;
				this._bestMove = -1;
			}
			else
			{
				this._complete = true;
			}
			return result;
		}


		public void Execute(int visits)
		{
			while (this.Visits < visits && this.Execute()) ;
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
				this.childrenCreated = true;
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
			}
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
			EMCTSGame.SeedRandom(seed);
		}
	}
}
