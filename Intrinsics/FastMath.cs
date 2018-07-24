using System;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace Intrinsics
{
	public static unsafe class FastMath
	{
		private const ulong AlignmentMask = 31UL;
		private const int VectorSizeInInts = 8;
		private const int BlockSizeInInts = 32;

		public static int Sum(int[] source)
		{
			fixed (int* ptr = &source[0])
			{
				var pos = 0;
				var sum = Avx.SetZeroVector256<int>();

				for (; pos <= source.Length - VectorSizeInInts; pos += VectorSizeInInts)
				{
					var current = Avx.LoadVector256(ptr + pos);
					sum = Avx2.Add(current, sum);
				}

				var temp = stackalloc int[VectorSizeInInts];
				Avx.Store(temp, sum);

				var final = Sum(temp, VectorSizeInInts);
				final += Sum(ptr + pos, source.Length - pos);

				return final;
			}
		}

		public static int SumAligned(int[] source)
		{
			fixed (int* ptr = &source[0])
			{
				var aligned = (int*)(((ulong)ptr + AlignmentMask) & ~AlignmentMask);
				var pos = (int)(aligned - ptr);
				var sum = Avx.SetZeroVector256<int>();
				var final = Sum(ptr, pos);

				for (; pos <= source.Length - VectorSizeInInts; pos += VectorSizeInInts)
				{
					var current = Avx.LoadAlignedVector256(ptr + pos);
					sum = Avx2.Add(current, sum);
				}

				var temp = stackalloc int[VectorSizeInInts];
				Avx.Store(temp, sum);

				final += Sum(temp, VectorSizeInInts);
				final += Sum(ptr + pos, source.Length - pos);

				return final;
			}
		}

		public static int SumPipelined(int[] source)
		{
			fixed (int* ptr = &source[0])
			{
				var pos = 0;
				var sum = Avx.SetZeroVector256<int>();

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

				var temp = stackalloc int[VectorSizeInInts];
				Avx.Store(temp, sum);

				var final = Sum(temp, VectorSizeInInts);
				final += Sum(ptr + pos, source.Length - pos);

				return final;
			}
		}

		public static int SumAlignedPipelined(int[] source)
		{
			fixed (int* ptr = &source[0])
			{
				var aligned = (int*)(((ulong)ptr + AlignmentMask) & ~AlignmentMask);
				var pos = (int)(aligned - ptr);
				var sum = Avx.SetZeroVector256<int>();
				var final = Sum(ptr, pos);

				for (; pos <= source.Length - BlockSizeInInts; pos += BlockSizeInInts)
				{
					var block0 = Avx.LoadAlignedVector256(ptr + pos + 0 * VectorSizeInInts);
					var block1 = Avx.LoadAlignedVector256(ptr + pos + 1 * VectorSizeInInts);
					var block2 = Avx.LoadAlignedVector256(ptr + pos + 2 * VectorSizeInInts);
					var block3 = Avx.LoadAlignedVector256(ptr + pos + 3 * VectorSizeInInts);

					sum = Avx2.Add(block0, sum);
					sum = Avx2.Add(block1, sum);
					sum = Avx2.Add(block2, sum);
					sum = Avx2.Add(block3, sum);
				}

				for (; pos <= source.Length - VectorSizeInInts; pos += VectorSizeInInts)
				{
					var current = Avx.LoadAlignedVector256(ptr + pos);
					sum = Avx2.Add(current, sum);
				}

				var temp = stackalloc int[VectorSizeInInts];
				Avx.Store(temp, sum);

				final += Sum(temp, VectorSizeInInts);
				final += Sum(ptr + pos, source.Length - pos);

				return final;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static int Sum(int* source, int length)
		{
			int sum = 0;

			for (int i = 0; i < length; ++i)
			{
				sum += source[i];
			}

			return sum;
		}
	}
}
