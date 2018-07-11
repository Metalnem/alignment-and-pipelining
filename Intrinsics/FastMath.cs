using System;
using System.Runtime.Intrinsics.X86;

namespace Intrinsics
{
	public static class FastMath
	{
		public static unsafe int Sum(int[] source)
		{
			const int VectorSizeInInts = 8;

			var pos = 0;
			var max = Avx.SetAllVector256(Int32.MinValue);

			fixed (int* ptr = &source[0])
			{
				for (; pos <= source.Length - VectorSizeInInts; pos += VectorSizeInInts)
				{
					var current = Avx.LoadVector256(ptr + pos);
					max = Avx2.Add(current, max);
				}
			}

			var temp = stackalloc int[VectorSizeInInts];
			Avx.Store(temp, max);

			var sum = 0;
			SumSlow(new ReadOnlySpan<int>(temp, VectorSizeInInts), ref sum);
			SumSlow(source.AsSpan(pos), ref sum);

			return sum;
		}

		private static void SumSlow(ReadOnlySpan<int> source, ref int sum)
		{
			for (int i = 0; i < source.Length; ++i)
			{
				sum += source[i];
			}
		}
	}
}
