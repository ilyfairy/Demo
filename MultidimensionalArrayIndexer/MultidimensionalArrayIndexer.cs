using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace MultidimensionalArrayIndexer;

public interface IArray2D
{
    public static abstract int XSize { get; }
    public static abstract int YSize { get; }
}

public interface IArray3D
{
    public static abstract int XSize { get; }
    public static abstract int YSize { get; }
    public static abstract int ZSize { get; }
}

public struct Array2D<T, TArray>()
    where T : unmanaged
    where TArray : unmanaged, IArray2D
{
    public TArray Array;

    public readonly int XSize => TArray.XSize;
    public readonly int YSize => TArray.YSize;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly int GetIndex(int x, int y) => y * TArray.XSize + x;

    public T this[int x, int y]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if (x < 0 || x >= TArray.XSize)
                throw new IndexOutOfRangeException();
            if (y < 0 || y >= TArray.YSize)
                throw new IndexOutOfRangeException();
            return Unsafe.Add(ref Unsafe.As<TArray, T>(ref Array), y * TArray.XSize + x);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            if (x < 0 || x >= TArray.XSize)
                throw new IndexOutOfRangeException();
            if (y < 0 || y >= TArray.YSize)
                throw new IndexOutOfRangeException();
            Unsafe.Add(ref Unsafe.As<TArray, T>(ref Array), y * TArray.XSize + x) = value;
        }
    }
}

public struct Array3D<T, TArray>()
    where T : unmanaged
    where TArray : unmanaged, IArray3D
{
    public TArray Array;

    public readonly int XSize => TArray.XSize;
    public readonly int YSize => TArray.YSize;
    public readonly int ZSize => TArray.ZSize;


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly int GetIndex(int x, int y, int z) => z * TArray.XSize * TArray.YSize + y * TArray.XSize + x;

    public T this[int x, int y, int z]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            if (x < 0 || x >= TArray.XSize)
                throw new IndexOutOfRangeException();
            if (y < 0 || y >= TArray.YSize)
                throw new IndexOutOfRangeException();
            if (z < 0 || z >= TArray.ZSize)
                throw new IndexOutOfRangeException();
            return Unsafe.Add(ref Unsafe.As<TArray, T>(ref Array), z * TArray.XSize * TArray.YSize + y * TArray.XSize + x);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            if (x < 0 || x >= TArray.XSize)
                throw new IndexOutOfRangeException();
            if (y < 0 || y >= TArray.YSize)
                throw new IndexOutOfRangeException();
            if (z < 0 || z >= TArray.ZSize)
                throw new IndexOutOfRangeException();
            Unsafe.Add(ref Unsafe.As<TArray, T>(ref Array), z * TArray.XSize * TArray.YSize + y * TArray.XSize + x) = value;
        }
    }
}

public static class MultidimensionalArrayExtensions
{
    public static Span<T> AsSpan<T, TArray>(this ref Array2D<T, TArray> array)
        where T : unmanaged
        where TArray : unmanaged, IArray2D
        => MemoryMarshal.CreateSpan(ref Unsafe.As<TArray, T>(ref array.Array), TArray.XSize * TArray.YSize);

    public static Span<T> AsSpan<T, TArray>(this ref Array3D<T, TArray> array)
        where T : unmanaged
        where TArray : unmanaged, IArray3D
        => MemoryMarshal.CreateSpan(ref Unsafe.As<TArray, T>(ref array.Array), TArray.XSize * TArray.YSize * TArray.ZSize);
}
