using System.Linq;
using BenchmarkDotNet.Attributes;

namespace Intrinsics
{
	public class SumBenchmark
	{
		private const int Length = 32 * 1024;
		private int[] data;

		[Params(true, false)]
		public bool Aligned { get; set; }

		[GlobalSetup]
		public unsafe void GlobalSetup()
		{
			for (; ; )
			{
				data = Enumerable.Range(0, Length).ToArray();

				fixed (int* ptr = data)
				{
					if ((Aligned && (uint)ptr % 32 == 0) || (!Aligned && (uint)ptr % 16 != 0))
					{
						break;
					}
				}
			}
		}

		[Benchmark(Baseline = true)]
		public int Sum() => FastMath.Sum(data);

		[Benchmark]
		public int SumAligned() => FastMath.SumAligned(data);

		[Benchmark]
		public int SumPipelined() => FastMath.SumPipelined(data);
	}
}
