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
	class MCTS<T> where T : INode<double, T>
	{
		public readonly T Node;


		private readonly double biasExponent;
		private readonly int skipLevels;


		private bool childrenCreated;
		private bool skipLevelsHandled;
		private readonly Statistics.StatisticsTSLock score =
			new Statistics.StatisticsTSLock();


		[ThreadStatic]
		private static Random random;


		public IList<MCTS<T>> Children
		{
			get
			{
				this.EnsureChildren();
				if (this._roChildren == null)
					this._roChildren = this._children.AsReadOnly();
				return this._roChildren;
			}
		}
		private readonly List<MCTS<T>> _children = new List<MCTS<T>>();
		private IList<MCTS<T>> _roChildren;


		public int Visits { get { return this._visits; } }
		private int _visits;


		public double WinRate { get { return this.score.Mean; } }


		public double GetBias(int parentsVisits)
		{
			return Math.Pow(Math.Log(parentsVisits) / this._visits, biasExponent);
		}


		public double GetUct(int parentsVisits)
		{
			return this.WinRate * this.GetBias(parentsVisits);
		}


		public bool Complete { get { return this._complete; } }
		private bool _complete;


		public MCTS(T root, double biasExponent = 0.5, int skipLevels = 1)
		{ // random is ThreadStatic so we must initialize it
		  // for each thread
			if (random == null)
				random = new Random();
			this.Node = root;
			this.biasExponent = biasExponent;
			this.skipLevels = skipLevels;
			if (this.skipLevels <= 0)
				this.skipLevelsHandled = true;
		}


		public void Execute(int visits, bool parallel = true)
		{
			if (parallel)
			{
				int threadId = 0;
				Parallel.For(this._visits, visits,
					() =>
					{ // random is ThreadStatic so we must initialize it
					  // for each thread
						if (random == null)
							random = new Random(
								(int)DateTime.UtcNow.Ticks ^ 
									(Interlocked.Increment(ref threadId) * 251)
							);
						return 0;
					},
					(visit, loop, dummy) =>
					{
						if (this._complete)
							loop.Break();
						double score;
						this.Execute(out score);
						return dummy;
					},
					(dummy) => { }
				);
			}
			else
			{
				double score;
				while (this._visits < visits && !this._complete)
					this.Execute(out score);
			}
		}


		public override string ToString()
		{
			return string.Format("#{0}^{1}@{2}",this._visits,this.score.Mean,this.Node.ToString());
		}


		private bool Execute(out double score)
		{
			score = 0;
			bool result = false;
			if (this._complete)
				return false;
			else if (!this.skipLevelsHandled)
			{
				Interlocked.Increment(ref this._visits);
				if (this.TryHandleSkipLevels(out score))
				{
					if (score < 0)
						throw new ArgumentOutOfRangeException(
							"IMCTSGame.Score must not be negative."
						);
					result = true;
				}
				else
					Interlocked.Decrement(ref this._visits);
			}
			if (!result)
				if ( Interlocked.CompareExchange(ref this._visits, 1, 0) == 0)
				{
					result = true;
					score = this.Node.Value;
					if (score < 0)
						throw new ArgumentOutOfRangeException(
							"IMCTSGame.Score must not be negative."
						);
				}
				else
				{
					Interlocked.Increment(ref this._visits);
					while (!result)
					{
						MCTS<T> chosenChild;
						if (!TryChooseChildForExecute(out chosenChild))
							break;
						result = chosenChild.Execute(out score);
						if (!result && !chosenChild.Complete)
							throw new InvalidOperationException(
								"Node didn't execute and Complete is false."
							);
					}
				}


			if (result)
				this.score.Add(score);
			else
			{
				Interlocked.Decrement(ref this._visits);
				this._complete = true;
			}
			return result;
		}


		private bool TryHandleSkipLevels(out double score)
		{
			if (this.skipLevelsHandled)
			{
				score = 0;
				return false;
			}
			else
			{
				if (this.skipLevels <= 0)
					return this.HandleSkipLevelsFailed(out score);
				else if (this.skipLevels == 1)
				{
					MCTS<T> chosenChild;
					if (!TryChooseChildForExecute(out chosenChild) ||
						0 < chosenChild._visits
					)
						return this.HandleSkipLevelsFailed(out score);
					else
						if (chosenChild.Execute(out score))
							return true;
						else
							throw new InvalidOperationException();
				}
				else
				{
					while (true)
					{
						MCTS<T> chosenChild;
						if (!TryChooseChildForTryHandleSkipLevels(out chosenChild))
							return this.HandleSkipLevelsFailed(out score);
						else if (chosenChild.TryHandleSkipLevels(out score))
							return true;
						else if (!chosenChild.skipLevelsHandled)
							throw new InvalidOperationException(
								"Child didn't handle skip levels."
							);
					}
				}
			}
		}


		private bool TryChooseChildForExecute(out MCTS<T> chosenChild)
		{
			this.EnsureChildren();
			int unvisited = 0;
			double maxUct = double.NegativeInfinity;
			chosenChild = null;
			var visits = this._visits;
			foreach (var child in this._children)
			{
				if (!child._complete)
				{
					double childUCT;
					if (child._visits == 0)
					{
						if (random.Next(++unvisited) == 0)
							chosenChild = child;
					}
					else if (unvisited == 0 && maxUct <
						((childUCT = child.GetUct(visits))) 
					)
					{
						maxUct = childUCT;
						chosenChild = child;
					}
				}
			}
			return chosenChild != null;
		}


		private bool TryChooseChildForTryHandleSkipLevels(out MCTS<T> chosenChild)
		{
			this.EnsureChildren();
			chosenChild =
				this._children.Where(
					(child) => !child.skipLevelsHandled
				).RandomOrDefault();
			return chosenChild != null;
		}


		private bool HandleSkipLevelsFailed(out double score)
		{
			this.skipLevelsHandled = true;
			score = 0;
			return false;
		}


		private void EnsureChildren()
		{
			if (!this.childrenCreated)
			{
				lock (this._children)
				{
					// multiple threads can get into the lock, only the first
					// can create children
					if (!this.childrenCreated)
					{
						var childrenSkipLevels = this.skipLevels;
						if (0 < childrenSkipLevels)
							childrenSkipLevels--;
						foreach (var child in this.Node.Children)
						{
							this._children.Add(new MCTS<T>(child.Node, this.biasExponent,childrenSkipLevels));
						}
						this.childrenCreated = true;
					}
				}
			}
		}


		public static void SetRandomSeed(int seed)
		{
			random = new Random(seed);
			EMCTSGame.SeedRandom(seed);
			EEnumerable.SeedRandom(seed);
		}
	}


	class MCTS
	{
		public static MCTS<T> Create<T>(
			T root, 
			double biasExponent = 0.5, 
			int skipLevels = 1
		) where T : INode<double,T>
		{
			return new MCTS<T>(root, biasExponent, skipLevels);
		}
	}
}
