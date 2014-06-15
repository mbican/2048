using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2048
{
	class _2048Model
	{
		public bool left()
		{
			return this.move(_2048MoveDirection.left);
		}

		public bool right()
		{
			return this.move(_2048MoveDirection.right);
		}

		public bool up()
		{
			return this.move(_2048MoveDirection.up);
		}

		public bool down()
		{
			return this.move(_2048MoveDirection.down);
		}

		public bool move(_2048MoveDirection move)
		{
			switch (move)
			{
				case _2048MoveDirection.left:
					return this.left();
				case _2048MoveDirection.right:
					return this.right();
				case _2048MoveDirection.up:
					return this.up();
				case _2048MoveDirection.down:
					return this.down();
				default:
					throw new ArgumentException("unknown move","move");
			}
		}

	}

}
