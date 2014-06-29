using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2048.AI.MCTS
{
	class ChildNode<TValue, TNode> where TNode: INode<TValue, TNode>
	{
		public readonly double Probability;
		public readonly TNode Node;


		public ChildNode(TNode node, double probability = 1)
		{
			if (node == null)
				throw new ArgumentNullException("node");
			if (probability < 0)
				throw new ArgumentOutOfRangeException("probability");
			this.Node = node;
			this.Probability = probability;
		}
	}
}
