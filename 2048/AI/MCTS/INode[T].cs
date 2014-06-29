using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2048.AI.MCTS
{
	interface INode<TValue, TNode> where TNode : INode<TValue,TNode>
	{
		TValue Value { get; }
		IList<ChildNode<TValue,TNode>> Children { get; }
	}
}
