using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using _2048.AI.MCTS;

namespace _2048
{
	class MainClass
	{
		public static void Main()
		{
			/*
			_2048RandomFinishPerformance(
				TimeSpan.FromSeconds(1),
				() => new _2048Model_backup(),
				"_2048Model_backup"
			);
			_2048RandomFinishPerformance(
				TimeSpan.FromSeconds(1),
				()=>new _2048Model(),
				"_2048Model"
			);

			_2048RandomFinishPerformanceParallel(
				TimeSpan.FromSeconds(1),
				() => new _2048Model_backup(),
				"_2048Model_backup"
			);
			_2048RandomFinishPerformanceParallel(
				TimeSpan.FromSeconds(1),
				() => new _2048Model(),
				"_2048Model"
			);
			//Console.ReadLine();
			*/
			//_2048MCTS(20000,1);
			SearchBias(200, 20);
			Console.ReadLine();
		}

		static void _2048RandomFinishPerformance(TimeSpan timespan, Func<IMCTSGame> constructor,string name)
		{
			var stopWatch = new System.Diagnostics.Stopwatch();
			stopWatch.Start();
			int counter = 0;
			int moves = 0;
			while (stopWatch.Elapsed < timespan)
			{
				var model = constructor();
				moves += model.RandomFinish();
				counter++;
			}
			stopWatch.Stop();
			Console.WriteLine(string.Format("{3}.RandomFinish() in {2} sec.: {0} 1/s ({1} moves/s; {4} moves/game)", counter / timespan.TotalSeconds, moves / timespan.TotalSeconds, timespan.TotalSeconds,name, moves / counter));
		}

		static void _2048RandomFinishPerformanceParallel(TimeSpan timespan, Func<IMCTSGame> constructor, string name)
		{
			var stopWatch = new System.Diagnostics.Stopwatch();
			stopWatch.Start();
			int counter = 0;
			int moves = 0;
			Parallel.For(int.MinValue, int.MaxValue, () => Tuple.Create(0, 0),
				(index, loop, counters) =>
				{
					if (stopWatch.Elapsed >= timespan)
						loop.Break();
					else
					{
						var model = constructor();
						counters = Tuple.Create(
							counters.Item1 + model.RandomFinish(),
							counters.Item2 + 1
						);
					}
					return counters;
				}, (counters) =>
				{
					Interlocked.Add(ref counter, counters.Item2);
					Interlocked.Add(ref moves, counters.Item1);
				}
			);
			stopWatch.Stop();
			Console.WriteLine(string.Format("{3}.RandomFinish() Parallel in {2} sec.: {0} 1/s ({1} moves/s; {4} moves/game)", counter / timespan.TotalSeconds, moves / timespan.TotalSeconds, timespan.TotalSeconds, name, moves / counter));
		}

		static void _2048MCTS(int iterations,int log_modulus)
		{
			/* biasExponent by iterations
			 * 20: 0.00237352999996185
			 * 20: 0.0043530732132652
			 * 20: 0.00479875209416746
			 * 20: best biasExponent: 0,013431258317143700 score: 14271,36 (s: 6940,72141844670, c: 100)
			 * 20: best biasExponent: 0,002995863277033120 score: 14088,64 (s: 6820,91629781646, c: 100)
			 * 20: best biasExponent: 0,003611261854512600 score: 13818,04 (s: 6830,36352304690, c: 100)
			 * 20: best biasExponent: 0,002896164722634640 score: 13349,96 (s: 6127,00251646494, c: 100)
			 * 20: best biasExponent: 0,001130312571907960 score: 13334,64 (s: 6070,33165938645, c: 100)
			 * 20: best biasExponent: 0,002050672042579470 score: 13282,40 (s: 6245,27094138542, c: 100)
			 * 20: best biasExponent: 0,002939616663326970 score: 13002,68 (s: 6500,03447524657, c: 100)
			 * 20: best biasExponent: 0,005991726554066230 score: 12908,00 (s: 6655,55313670889, c: 100)
			 * 20: best biasExponent: 0,000736896693160851 score: 12572,24 (s: 6198,14900541551, c: 100)
			 * 20: best biasExponent: 0,020762964440718000 score: 12466,36 (s: 5385,50701873917, c: 100)
			 * 20: best biasExponent: 0,001100118768172110 score: 12359,12 (s: 5827,95236019713, c: 100)
			 * 20: best biasExponent: 0,000577531597160323 score: 12226,44 (s: 5456,60602383881, c: 100)
			 * 20: best biasExponent: 0,004236790477395740 score: 12215,36 (s: 7427,48165312273, c: 100)
			 * 20: best biasExponent: 0,008081440143545950 score: 11983,68 (s: 6323,34747507244, c: 100)
			 * 20: best biasExponent: 0,000582241886026929 score: 11665,56 (s: 6392,44493198689, c: 100)
			 */
			var root = new MCTS<GameNode>(new GameNode(new _2048Model()),0.03);
			MCTS<GameNode> oldRoot = root;
			int counter = -1;
			var stopWatch = new System.Diagnostics.Stopwatch();
			stopWatch.Start();
			while (!root.Complete)
			{
				if (++counter % log_modulus == 0)
				{
					Console.WriteLine(stopWatch.Elapsed);
					stopWatch.Restart();
					Console.WriteLine(string.Format("move: {0}", counter));
					Console.WriteLine(string.Format("score: {0}", root.Node.Game.Score));
					Console.WriteLine(string.Format("missing visits: {0}", iterations - root.Visits));
					Console.Write(((_2048Model)root.Node.Game).Matrix.ToDebugString(5));
					//Console.Write(oldRoot.DumpDebugInfo(1).Value);
				}
				oldRoot = root;
				if (!root.TryMove(iterations, out root) && ! root.Complete)
					throw new InvalidOperationException("not moved.");
				//Console.ReadLine();
			}
			var a = "ahoj";
			Console.WriteLine(string.Format("move: {0}", counter));
			Console.WriteLine(string.Format("score: {0}", root.Node.Game.Score));
			Console.Write(((_2048Model)root.Node.Game).Matrix.ToDebugString(5));
		}

		static void SearchBias(int visits, int _2048Visits, int k = 10)
		{
			double c = Math.Pow(2, k);
			var _lock = new Object();
			MCTS<RangeNode> root = null;
			root = new MCTS<RangeNode>(
				new RangeNode(
					-0.75, 
					0.25, 
					(x) => {
						var biasExponent = Math.Pow(2, x * c);
						var _2048Root = new MCTS<GameNode>(
							new GameNode(
								new _2048Model()
							), 
							biasExponent
						);
						while (_2048Root.TryMove(_2048Visits, out _2048Root)) ;
						lock (_lock)
						{
							Console.WriteLine(string.Format("biasExponent: {0}", biasExponent));
							Console.WriteLine(string.Format("score: {0}", _2048Root.Node.Game.Score));
							Console.Write(((_2048Model)_2048Root.Node.Game).Matrix.ToDebugString(5));
							Console.WriteLine(string.Format("best biasExponent: {0} score: {1}", Math.Pow(2,c*root.GetBestLeaf().Node.Middle), root.GetBestLeaf().WinRate));
						}
						return _2048Root.Node.Game.Score;
					}
				),
				1
			);
			root.Execute(visits);
			var bestBiasExponent = Math.Pow(2, c * root.GetBestLeaf().Node.Middle);
			var statistics = new Statistics.StandardDeviationCounter();
			Parallel.For(0, visits/4, () => new Statistics.StandardDeviationCounter(),
				(i, loop, _statistics) =>
				{
					var _2048Root2 = new MCTS<GameNode>(
						new GameNode(
							new _2048Model()
						),
						bestBiasExponent
					);
					while (_2048Root2.TryMove(_2048Visits, out _2048Root2)) ;
					_statistics.Add(_2048Root2.Node.Game.Score);
					Console.WriteLine(string.Format("best biasExponent: {0} score: {1} (s: {2}, c: {3})", bestBiasExponent, _statistics.Mean, _statistics.SampleStandardDeviation, _statistics.Count));
					return _statistics;
				},
				(_statistics) => statistics.Add(_statistics)
			);
			Console.WriteLine(string.Format("best biasExponent: {0} score: {1} (s: {2}, c: {3})", bestBiasExponent, statistics.Mean, statistics.SampleStandardDeviation,statistics.Count));
		}
	}
}
