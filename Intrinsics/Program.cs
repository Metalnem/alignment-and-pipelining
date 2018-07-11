using System;
using System.Linq;

namespace Intrinsics
{
	public class Program
	{
		public static void Main(string[] args)
		{
			Console.WriteLine(FastMath.Sum(Enumerable.Range(0, 1000).ToArray()));
		}
	}
}
