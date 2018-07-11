using System.Linq;
using BenchmarkDotNet.Attributes;

namespace Intrinsics
{
	public class SumBenchmark
	{
		private const int Length = 32 * 1024;
		private int[] data;

		[GlobalSetup]
		public void GlobalSetup()
		{
			data = Enumerable.Range(0, Length).ToArray();
		}

		[Benchmark(Baseline = true)]
		public int Sum() => FastMath.Sum(data);
	}
}
