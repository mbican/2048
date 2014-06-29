using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _2048.AI.MCTS;

namespace _2048
{
	static class EMCTS
	{
		public static bool TryAutoMove(this MCTS<GameNode> root, out MCTS<GameNode> newRoot)
		{
			if (root.Node.Game.IsAutoMovePossible)
			{
				int move = root.Node.Game.GetAutoMoveIndex();
				newRoot = root.GetChildByMove(move);
				return true;
			}
			else
			{
				newRoot = root;
				return false;
			}
		}


		public static bool TryMove(this MCTS<GameNode> root, int iterations, out MCTS<GameNode> newRoot, bool preferAutoMove = true)
		{
			if (preferAutoMove && root.TryAutoMove(out newRoot))
				return true;
			else
			{
				root.Execute(iterations);
				if (root.GetBestMove() < 0)
				{
					newRoot = root;
					return false;
				}
				else
				{
					newRoot = root.GetChildByMove(root.GetBestMove());
					return true;
				}
			}
		}

		private const string indentStr = "   ";
		public static Lazy<string> DumpDebugInfo(this MCTS<GameNode> root, int depth = 2, int indent = 0, StringBuilder result = null)
		{
			if (depth < 0)
				return new Lazy<string>(() => "");
			if (result == null)
				result = new StringBuilder();
			string currentIndentStr;
			StringBuilder currentIndentBuilder = new StringBuilder();
			for (int i = 0; i < indent; ++i)
			{
				currentIndentBuilder.Append(indentStr);
			}
			currentIndentStr = currentIndentBuilder.ToString();
			result.AppendFormat("{0}move: {1}; id: {2}\n", currentIndentStr, root.Node.ParentsMove, root.ToString());
			result.AppendFormat("{0}{1}score: {2}\n", currentIndentStr, indentStr, root.Node.Value);
			result.AppendFormat("{0}{1}expected_score: {2}\n", currentIndentStr, indentStr, root.WinRate);
			foreach (var child in root.Children)
				child.DumpDebugInfo(depth - 1, indent + 1, result);
			return new Lazy<string>(result.ToString);
		}


		public static int GetBestMove(this MCTS<GameNode> root)
		{
			int maxVisits = int.MinValue;
			int bestMove = -1;
			foreach (var child in root.Children)
			{
				if (maxVisits < child.Visits)
				{
					maxVisits = child.Visits;
					bestMove = child.Node.ParentsMove;
				}
			}
			return bestMove;
		}


		public static MCTS<T> GetBestLeaf<T>(this MCTS<T> root) where T : INode<double,T>
		{
			int visits = int.MinValue + 1;
			while (0 < root.Visits && 0 < root.Children.Count && int.MinValue < visits)
			{
				visits = int.MinValue;
				foreach (var child in root.Children)
				{
					if (visits < child.Visits && 0 < child.Visits)
					{
						visits = child.Visits;
						root = child;
					}
				}
			}
			return root;
		}


		public static MCTS<GameNode> GetChildByMove(this MCTS<GameNode> root, int move)
		{
			foreach (var child in root.Children)
			{
				if (child.Node.ParentsMove == move)
				{
					return child;
				}
			}
			throw new ArgumentOutOfRangeException("move");
		}
	}
}
