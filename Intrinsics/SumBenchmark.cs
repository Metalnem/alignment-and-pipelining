using System.Linq;
using BenchmarkDotNet.Attributes;

namespace Intrinsics
{
	public class SumBenchmark
	{
		private const int Length = 32 * 1024;
		private int[] data;

		[Params(8, 32)]
		public int Alignment { get; set; }

		[GlobalSetup]
		public unsafe void GlobalSetup()
		{
			for (; ; )
			{
				data = Enumerable.Range(0, Length).ToArray();

				fixed (int* ptr = data)
				{
					if ((Alignment == 32 && (uint)ptr % 32 == 0) || (Alignment == 8 && (uint)ptr % 16 != 0))
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

		[Benchmark]
		public int SumAlignedPipelined() => FastMath.SumAlignedPipelined(data);
	}
}
