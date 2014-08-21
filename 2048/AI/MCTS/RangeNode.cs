using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2048.AI.MCTS
{
	class RangeNode : INode<double, RangeNode>
	{
		public readonly double A;
		public readonly double B;
		public readonly Func<double, double> ValueGetter;
		public readonly int SkipLevels;


		public double Value
		{
			get { return this._value.Value; }
		}
		private readonly Lazy<double> _value;


		public IList<ChildNode<double, RangeNode>> Children
		{
			get
			{
				if (this._children == null)
				{
					var children = new List<ChildNode<double, RangeNode>>();
					this._children = children.AsReadOnly();
					double middle = this.Middle;
					int childrenSkipLevels = this.ChildrenSkipLevels;
					if (middle != this.A)
						children.Add(
							new ChildNode<double, RangeNode>(
								new RangeNode(
									this.A, 
									middle, 
									this.ValueGetter, 
									childrenSkipLevels
								)
							)
						);
					if (middle != this.B)
						children.Add(
							new ChildNode<double, RangeNode>(
								new RangeNode(
									middle, 
									this.B, 
									this.ValueGetter, 
									childrenSkipLevels
								)
							)
						);
				}
				return this._children;
			}
		}
		private IList<ChildNode<double, RangeNode>> _children;


		public double Middle { get { return (this.A + this.B) / 2; } }


		private int ChildrenSkipLevels
		{
			get
			{
				var childrenSkipLevels = this.SkipLevels - 1;
				if (childrenSkipLevels < 0)
					childrenSkipLevels = 0;
				return childrenSkipLevels;
			}
		}


		public RangeNode(
			double a,
			double b,
			Func<double, double> valueGetter,
			int skipLevels = 0
		)
		{
			if (valueGetter == null)
				throw new ArgumentException("valueGetter");
			if (skipLevels < 0)
				throw new ArgumentOutOfRangeException("skipLevels");
			this.A = a;
			this.B = b;
			this.ValueGetter = valueGetter;
			this.SkipLevels = skipLevels;
			if (skipLevels <= 0)
				this._value = new Lazy<double>(this.GetValue);
			else
				this._value = new Lazy<double>(0);
		}


		private double GetValue()
		{
			return this.ValueGetter(this.Middle);
		}
	}
}
