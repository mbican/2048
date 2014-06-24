using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _2048.AI.MCTS;

namespace _2048
{
	class MainClass
	{
		public static void Main()
		{
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
			 
			//Console.ReadLine();
			
			_2048MCTS(1000,10);
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

		static void _2048MCTS(int iterations,int log_modulus)
		{
			var root = new MCTS(new _2048Model());
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
					Console.WriteLine(string.Format("score: {0}", root.Game.Score));
					Console.WriteLine(string.Format("missing visits: {0}", iterations - root.Visits));
					Console.Write(((_2048Model)root.Game).Matrix.ToDebugString(5));
				}
				if (!root.TryMove(iterations, out root) && ! root.Complete)
					throw new InvalidOperationException("not moved.");
			}
			var a = "ahoj";
			Console.WriteLine(string.Format("move: {0}", counter));
			Console.WriteLine(string.Format("score: {0}", root.Game.Score));
			Console.Write(((_2048Model)root.Game).Matrix.ToDebugString(5));
		}
	}
}
