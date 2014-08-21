using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2048.DesignPatterns
{
	class Disposable: IDisposable
	{
		private Action action;
		public Disposable(Action action=null)
		{
			this.action = action;
		}


		public void Dispose()
		{
			if(this.action != null)
				this.action();
			this.action=null;
		}
	}
}
