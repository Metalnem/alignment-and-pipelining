using System;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace Intrinsics
{
	public static unsafe class FastMath
	{
		public static int Sum(int[] source)
		{
			const int VectorSizeInInts = 8;

			int pos = 0;
			Vector256<int> sum = Avx.SetZeroVector256<int>();

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

		public static int SumAligned(int[] source)
		{
			const int VectorSizeInInts = 8;
			const ulong AlignmentMask = 31UL;

			int pos = 0;
			Vector256<int> sum;

			fixed (int* ptr = &source[0])
			{
				int* aligned = (int*)(((ulong)ptr + AlignmentMask) & ~AlignmentMask);
				pos = (int)(aligned - ptr);

				sum = Avx2.And(Avx.LoadVector256(ptr), Avx.SetVector256(
					pos > 7 ? -1 : 0,
					pos > 6 ? -1 : 0,
					pos > 5 ? -1 : 0,
					pos > 4 ? -1 : 0,
					pos > 3 ? -1 : 0,
					pos > 2 ? -1 : 0,
					pos > 1 ? -1 : 0,
					pos > 0 ? -1 : 0
				));

				for (; pos <= source.Length - VectorSizeInInts; pos += VectorSizeInInts)
				{
					var current = Avx.LoadAlignedVector256(ptr + pos);
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

		public static int SumPipelined(int[] source)
		{
			const int VectorSizeInInts = 8;
			const int BlockSizeInInts = 4 * VectorSizeInInts;

			int pos = 0;
			Vector256<int> sum = Avx.SetZeroVector256<int>();

			fixed (int* ptr = &source[0])
			{
				for (; pos <= source.Length - BlockSizeInInts; pos += BlockSizeInInts)
				{
					var block0 = Avx.LoadVector256(ptr + pos + 0 * VectorSizeInInts);
					var block1 = Avx.LoadVector256(ptr + pos + 1 * VectorSizeInInts);
					var block2 = Avx.LoadVector256(ptr + pos + 2 * VectorSizeInInts);
					var block3 = Avx.LoadVector256(ptr + pos + 3 * VectorSizeInInts);

					sum = Avx2.Add(block0, sum);
					sum = Avx2.Add(block1, sum);
					sum = Avx2.Add(block2, sum);
					sum = Avx2.Add(block3, sum);
				}

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
