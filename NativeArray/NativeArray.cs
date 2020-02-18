using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
// ReSharper disable MemberCanBePrivate.Global

namespace NativeArray
{
	public sealed unsafe class NativeArray<T> : IReadOnlyList<T>, IEquatable<NativeArray<T>>, IDisposable where T : unmanaged
	{
		private readonly IMemoryAllocator _memoryAllocator;
		private readonly int _byteCount;

		public readonly T* Pointer;
		public readonly int Length;

		public int Count => Length;

		public NativeArray(int count) : this(count, CoTaskMemAllocator.Allocator) { }
		public NativeArray(T[] array) : this(array, CoTaskMemAllocator.Allocator) { }
		public NativeArray(ArraySegment<T> segment) : this(segment, CoTaskMemAllocator.Allocator) { }
#if !NETSTANDARD2_0
		public NativeArray(Span<T> span) : this(span, CoTaskMemAllocator.Allocator) { }
#endif

		public NativeArray(int count, IMemoryAllocator allocator)
		{
			_memoryAllocator = allocator ?? throw new ArgumentNullException(nameof(allocator));
			Length = count;
			_byteCount = count * Marshal.SizeOf<T>();
			Pointer = (T*)allocator.Allocate(_byteCount).ToPointer();
			if (_byteCount != 0)
			{
				GC.AddMemoryPressure(_byteCount);
			}
		}

		public NativeArray(T[] array, IMemoryAllocator allocator)
		{
			if (array == null)
				throw new ArgumentNullException(nameof(array));
			_memoryAllocator = allocator ?? throw new ArgumentNullException(nameof(allocator));
			Length = array.Length;
			_byteCount = Length * Marshal.SizeOf<T>();
			Pointer = (T*)allocator.Allocate(_byteCount).ToPointer();
			if (_byteCount != 0)
			{
#if NETSTANDARD2_0
				fixed (T* ptr = &array[0])
					Buffer.MemoryCopy(ptr, Pointer, _byteCount, _byteCount);
#else
				new Span<T>(array).CopyTo(new Span<T>(Pointer, Length));
#endif
				GC.AddMemoryPressure(_byteCount);
			}
		}

		public NativeArray(ArraySegment<T> segment, IMemoryAllocator allocator)
		{
			_memoryAllocator = allocator ?? throw new ArgumentNullException(nameof(allocator));
			Length = segment.Count;
			_byteCount = Length * Marshal.SizeOf<T>();
			Pointer = (T*)allocator.Allocate(_byteCount).ToPointer();
			if (_byteCount != 0)
			{
#if NETSTANDARD2_0
				// ReSharper disable once PossibleNullReferenceException
				fixed (T* ptr = &segment.Array[segment.Offset])
					Buffer.MemoryCopy(ptr, Pointer, _byteCount, _byteCount);
#else
				new Span<T>(segment.Array, segment.Offset, segment.Count).CopyTo(new Span<T>(Pointer, Length));
#endif
				GC.AddMemoryPressure(_byteCount);
			}
		}

#if !NETSTANDARD2_0
		public NativeArray(Span<T> span, IMemoryAllocator allocator)
		{
			_memoryAllocator = allocator ?? throw new ArgumentNullException(nameof(allocator));
			Length = span.Length;
			_byteCount = Length * Marshal.SizeOf<T>();
			Pointer = (T*)allocator.Allocate(_byteCount).ToPointer();
			if (_byteCount != 0)
			{
				span.CopyTo(new Span<T>(Pointer, Length));
				GC.AddMemoryPressure(_byteCount);
			}
		}
#endif

		public T this[int index]
		{
			get => Pointer[index];
			set => Pointer[index] = value;
		}

		public static implicit operator IntPtr(NativeArray<T> nativeArray)
		{
			return nativeArray == null ? IntPtr.Zero : new IntPtr(nativeArray.Pointer);
		}

		public static implicit operator T*(NativeArray<T> nativeArray)
		{
			return nativeArray == null ? null : nativeArray.Pointer;
		}

#if !NETSTANDARD2_0
		public static implicit operator Span<T>(NativeArray<T> nativeArray)
		{
			return nativeArray == null ? Span<T>.Empty : new Span<T>(nativeArray.Pointer, nativeArray.Length);
		}
#endif

		public void CopyTo(T[] array)
		{
#if !NETSTANDARD2_0
			CopyTo((Span<T>)array);
#else
			if (array.Length < Length)
			{
				throw new ArgumentOutOfRangeException(nameof(array));
			}
			fixed (T* ptr = &array[0])
				Buffer.MemoryCopy(Pointer, ptr, _byteCount, _byteCount);
#endif
		}

		public void CopyTo(T[] array, int index)
		{
#if !NETSTANDARD2_0
			CopyTo(new Span<T>(array, index, array.Length - index));
#else
			if (array.Length - index < Length)
			{
				throw new ArgumentOutOfRangeException(nameof(array));
			}
			fixed (T* ptr = &array[index])
				Buffer.MemoryCopy(Pointer, ptr, _byteCount, _byteCount);
#endif
		}

		public void CopyTo(NativeArray<T> array)
		{
#if !NETSTANDARD2_0
			CopyTo((Span<T>)array);
#else
			if (array.Length < Length)
			{
				throw new ArgumentOutOfRangeException(nameof(array));
			}
			Buffer.MemoryCopy(Pointer, array.Pointer, _byteCount, _byteCount);
#endif
		}

		public void CopyTo(NativeArray<T> array, int index)
		{
#if !NETSTANDARD2_0
			CopyTo(new Span<T>(array.Pointer + index, array.Length - index));
#else
			if (array.Length - index < Length)
			{
				throw new ArgumentOutOfRangeException(nameof(array));
			}
			Buffer.MemoryCopy(Pointer, array.Pointer + index, _byteCount, _byteCount);
#endif
		}

#if !NETSTANDARD2_0
		public void CopyTo(Span<T> array)
		{
			new Span<T>(Pointer, Length).CopyTo(array);
		}
#endif

		public T[] ToArray()
		{
#if !NETSTANDARD2_0
			return new Span<T>(Pointer, Length).ToArray();
#else
			T[] array = new T[Length];
			CopyTo(array);
			return array;
#endif
		}

		public List<T> ToList()
		{
			return new List<T>(this);
		}

		public void Dispose()
		{
			ReleaseUnmanagedResources();
			GC.SuppressFinalize(this);
		}

		~NativeArray()
		{
			ReleaseUnmanagedResources();
		}

		private void ReleaseUnmanagedResources()
		{
			if (_byteCount != 0)
			{
				GC.RemoveMemoryPressure(_byteCount);
			}
			_memoryAllocator.Free(new IntPtr(Pointer));
		}

		public PointerEnumerator GetEnumerator()
		{
			return new PointerEnumerator(Pointer, Length);
		}

		// ReSharper disable HeapView.BoxingAllocation
		IEnumerator<T> IEnumerable<T>.GetEnumerator() => GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		// ReSharper restore HeapView.BoxingAllocation

		public struct PointerEnumerator : IEnumerator<T>
		{
			private readonly T* _pointer;
			private readonly int _count;
			private int _index;

			internal PointerEnumerator(T* pointer, int count)
			{
				_pointer = pointer;
				_count = count;
				_index = -1;
				Current = default;
			}

			public bool MoveNext()
			{
				if (_count > ++_index)
				{
					Current = _pointer[_index];
					return true;
				}

				return false;
			}

			public void Reset()
			{
				_index = -1;
			}

			public T Current { get; private set; }

			// ReSharper disable once HeapView.BoxingAllocation
			object IEnumerator.Current => Current;

			public void Dispose()
			{

			}
		}

		public bool Equals(NativeArray<T> other)
		{
			if (other is null) return false;
			if (ReferenceEquals(this, other)) return true;
			return _memoryAllocator == other._memoryAllocator && _byteCount == other._byteCount && Pointer == other.Pointer && Length == other.Length;
		}

		public override bool Equals(object obj)
		{
			return ReferenceEquals(this, obj) || obj is NativeArray<T> other && Equals(other);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = (_memoryAllocator != null ? _memoryAllocator.GetHashCode() : 0);
				hashCode = (hashCode * 397) ^ _byteCount;
				hashCode = (hashCode * 397) ^ unchecked((int)(long)Pointer);
				hashCode = (hashCode * 397) ^ Length;
				return hashCode;
			}
		}

		public static bool operator ==(NativeArray<T> left, NativeArray<T> right)
		{
			if (ReferenceEquals(left, right)) return true;
			return !(left is null) && left.Equals(right);
		}

		public static bool operator !=(NativeArray<T> left, NativeArray<T> right)
		{
			if (ReferenceEquals(left, right)) return false;
			return left is null || !left.Equals(right);
		}
	}
}
