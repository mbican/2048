using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2048.AI.MCTS
{
	class GameNode : INode<double,GameNode>
	{
		public readonly int ParentsMove;
		public readonly IMCTSGame Game;


		public double Value
		{
			get { return this._value.Value; }
		}
		private readonly Lazy<double> _value;


		public IList<ChildNode<double,GameNode>> Children
		{
			get { return this._children.Value; }
		}
		private readonly Lazy<IList<ChildNode<double, GameNode>>> _children;


		public GameNode(IMCTSGame game, int parentsMove = -1)
		{
			if (game == null)
				throw new ArgumentNullException("game");
			this.ParentsMove = parentsMove;
			this.Game = game;
			this._value = new Lazy<double>(this.ComputeValue);
			this._children = new Lazy<IList<ChildNode<double, GameNode>>>(this.CreateChildren);
		}


		private double ComputeValue()
		{
			var gameClone = this.Game.Clone();
			gameClone.RandomFinish();
			return gameClone.Score;
		}


		private IList<ChildNode<double, GameNode>> CreateChildren()
		{
			var children = new List<ChildNode<double, GameNode>>();
			for (int move = 0; move < this.Game.PossibleMoves; move++)
			{
				var gameClone = this.Game.Clone();
				bool moved;
				do
				{
					moved = gameClone.TryMove(move);
					if (moved)
						children.Add(new ChildNode<double, GameNode>(new GameNode(gameClone, move)));
				} while (!moved && ++move < this.Game.PossibleMoves);
			}
			return children.AsReadOnly();
		}
	}
}
