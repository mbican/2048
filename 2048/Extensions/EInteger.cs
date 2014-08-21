using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _2048
{
	static class EInteger
	{
		public static int MergeHashCode(this int hashCode, int otherHashCode)
		{
			return hashCode * 251 ^ otherHashCode;
		}
	}
}
