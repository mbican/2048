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
	class MCTS<T> where T : INode<double,T>
	{
		public readonly T Node;


		private readonly double biasExponent;


		private bool childrenCreated;
		private readonly Statistics.IStatistics score = 
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


		public MCTS(T root, double biasExponent = 0.5)
		{
			if (random == null)
				random = new Random();
			this.Node = root;
			this.biasExponent = biasExponent;
		}


		public void Execute(int visits)
		{
			/*
			int score;
			while (this._visits < visits && !this.Complete)
				this.Execute(out score);
			 */
			int threadId = 0;
			Parallel.For(this._visits, visits,
				() => {
					if (random == null)
						random = new Random((int)DateTime.UtcNow.Ticks ^ (Interlocked.Increment(ref threadId) << 16));
					return 0;
				},
				(visit, loop, dummy) =>
				{
					if (this.Complete)
						loop.Break();
					double score;
					this.Execute(out score);
					return dummy;
				},
				(dummy) => { }
			);
		}


		private bool Execute(out double score)
		{
			score = 0;
			if (this.Complete)
				return false;
			bool result = false;
			if (Interlocked.CompareExchange(ref this._visits, 1, 0) == 0)
			{
				result = true;
				score = this.Node.Value;
				if (score < 0)
					throw new ArgumentOutOfRangeException(
						"IMCTSGame.Score must not be negative."
					);
				this.score.Add(score);
			}
			else
			{
				Interlocked.Increment(ref this._visits);
				this.EnsureChildren();
			}
			if (0 < this._children.Count)
			{
				while (!result)
				{
					int unvisited = 0;
					double maxUct = double.MinValue;
					MCTS<T> chosenChild = null;
					foreach (var child in this._children)
					{
						if (!child.Complete)
						{
							if (child._visits == 0)
							{
								if (random.Next(unvisited++) == 0)
									chosenChild = child;
							}
							else if (unvisited == 0 && maxUct < child.GetUct(this._visits))
							{
								maxUct = child.GetUct(this._visits);
								chosenChild = child;
							}
						}
					}
					if (chosenChild == null)
						break;
					result = chosenChild.Execute(out score);
					this.score.Add(score);
					if (!result && !chosenChild.Complete)
						throw new InvalidOperationException(
							"Node didn't execute and Complete is false."
						);
				}
			}
			if (!result)
			{
				Interlocked.Decrement(ref this._visits);
				this._complete = true;
			}
			return result;
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
						foreach (var child in this.Node.Children)
						{
							this._children.Add(new MCTS<T>(child.Node, this.biasExponent));
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
		}
	}
}
