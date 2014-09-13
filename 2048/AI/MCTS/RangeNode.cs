using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2048.AI.MCTS
{
	class RangeNode : INode<double,RangeNode>
	{
		public readonly double A;
		public readonly double B;
		public readonly Func<double, double> ValueGetter;


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
					if (middle != this.A)
						children.Add(
							new ChildNode<double, RangeNode>(
								new RangeNode(this.A, middle, this.ValueGetter)
							)
						);
					if (middle != this.B)
						children.Add(
							new ChildNode<double, RangeNode>(
								new RangeNode(middle, this.B, this.ValueGetter)
							)
						);
				}
				return this._children; 
			}
		}
		private IList<ChildNode<double, RangeNode>> _children;


		public double Middle { get { return (this.A + this.B) / 2; } }


		public double AverageScoreCoeff { get { return 1; } }


		public RangeNode(double a, double b, Func<double,double> valueGetter)
		{
			if (valueGetter == null)
				throw new ArgumentException("valueGetter");
			this.A = a;
			this.B = b;
			this.ValueGetter = valueGetter;
			this._value = new Lazy<double>(this.GetValue);
		}


		public override string ToString()
		{
			return this.Middle.ToString();
		}


		private double GetValue()
		{
			return this.ValueGetter(this.Middle);
		}
	}
}
