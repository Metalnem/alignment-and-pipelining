using System;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics.X86;

namespace Intrinsics
{
	public static class FastMath
	{
		public static unsafe int Sum(int[] source)
		{
			const int VectorSizeInInts = 8;

			var pos = 0;
			var sum = Avx.SetZeroVector256<int>();

			fixed (int* ptr = &source[0])
			{
				for (; pos <= source.Length - VectorSizeInInts; pos += VectorSizeInInts)
				{
					var current = Avx.LoadVector256(ptr + pos);
					sum = Avx2.Add(current, sum);
				}
			}

			var temp = stackalloc int[VectorSizeInInts];
			Avx.Store(temp, sum);

			var final = 0;
			Sum(new ReadOnlySpan<int>(temp, VectorSizeInInts), ref final);
			Sum(source.AsSpan(pos), ref final);

			return final;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void Sum(ReadOnlySpan<int> source, ref int sum)
		{
			for (int i = 0; i < source.Length; ++i)
			{
				sum += source[i];
			}
		}
	}
}
