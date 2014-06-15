using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace _2048Test
{
	sealed class Utils
	{
		
		private Utils(){
			throw new InvalidOperationException("static-only class");
		}


		public static void TestException<T>(Action action) where T: Exception
		{
			bool thrown = false;
			try
			{
				action();
			}catch(Exception ex)
			{
				if (ex.GetType() == typeof(T))
				{
					thrown = true;
				}
				else
				{
					throw;
				}
			}
			Assert.IsTrue(thrown);
		}

	}
}
