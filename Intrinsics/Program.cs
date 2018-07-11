using BenchmarkDotNet.Running;

namespace Intrinsics
{
	public class Program
	{
		public static void Main(string[] args)
		{
			BenchmarkRunner.Run<SumBenchmark>();
		}
	}
}
