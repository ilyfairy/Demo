using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MultidimensionalArrayIndexer;

namespace Demo;

internal unsafe struct MultidimensionalArrayIndexerTestStruct
{
    public Array2D<int, Int10_20> Array1; // Size: 10 * 20 * sizeof(int)
    public Array2D<ushort, Array50_50<ushort>> Array2; // Size: 50 * 50 * sizeof(ushort)
    public Array2D<long, Array50_50<long>> Array3; // 50 * 50 * sizeof(long)
    public Array3D<ushort, Array10_10_10<ushort>> Array4; // 10 * 10 * 10 * sizeof(ushort)

    public static void Test()
    {
        MultidimensionalArrayIndexerTestStruct myStruct = new();
        myStruct.Array1[1, 1] = 12345;
        myStruct.Array2[10, 20] = 2;
        myStruct.Array3[10, 20] = 2;
        myStruct.Array4[2, 3, 4] = 2;

        Debug.Assert(sizeof(Array2D<int, Int10_20>) == 10 * 20 * sizeof(int));
        Debug.Assert(myStruct.Array1.XSize * myStruct.Array1.YSize == 10 * 20);
        Debug.Assert(myStruct.Array1.AsSpan().Length == 10 * 20);
        myStruct.Array1[0, 0] = 123456;
        Debug.Assert(*(int*)&myStruct.Array1 == myStruct.Array1[0, 0]);
        Debug.Assert(myStruct.Array1[1, 1] == myStruct.Array1.AsSpan()[myStruct.Array1.GetIndex(1, 1)]);

        Debug.Assert(sizeof(Array2D<ushort, Array50_50<ushort>>) == 50 * 50 * sizeof(ushort));
        Debug.Assert(myStruct.Array2.XSize * myStruct.Array2.YSize == 50 * 50);
        Debug.Assert(myStruct.Array2.AsSpan().Length == 50 * 50);

        Debug.Assert(sizeof(Array2D<long, Array50_50<long>>) == 50 * 50 * sizeof(long));
        myStruct.Array3.AsSpan().Fill(0x123456);
        for (int y = 0; y < myStruct.Array3.YSize; y++)
        {
            for (int x = 0; x < myStruct.Array3.XSize; x++)
            {
                Debug.Assert(myStruct.Array3[x, y] == 0x123456);
            }
        }

        Debug.Assert(sizeof(Array3D<ushort, Array10_10_10<ushort>>) == 10 * 10 * 10 * sizeof(ushort));
        Debug.Assert(myStruct.Array4.XSize * myStruct.Array4.YSize * myStruct.Array4.ZSize == 10 * 10 * 10);
        Debug.Assert(myStruct.Array4.AsSpan().Length == 10 * 10 * 10);
        Random.Shared.NextBytes(MemoryMarshal.AsBytes(myStruct.Array4.AsSpan()));
        Debug.Assert(myStruct.Array4[0, 0, 0] == myStruct.Array4.AsSpan()[0]);
        Debug.Assert(myStruct.Array4[9, 9, 9] == myStruct.Array4.AsSpan()[^1]);
        Debug.Assert(myStruct.Array4[9, 9, 9] == myStruct.Array4.AsSpan()[myStruct.Array4.GetIndex(9, 9, 9)]);
    }
}

[InlineArray(XSize * YSize)]
internal struct Int10_20 : IArray2D
{
    public int First;
    public const int XSize = 10, YSize = 20;
    static int IArray2D.XSize => XSize;
    static int IArray2D.YSize => YSize;
}

[InlineArray(XSize * YSize)]
internal struct Array50_50<T> : IArray2D
{
    public T First;
    public const int XSize = 50, YSize = 50;
    static int IArray2D.XSize => XSize;
    static int IArray2D.YSize => YSize;
}

[InlineArray(XSize * YSize * ZSize)]
internal struct Array10_10_10<T> : IArray3D
{
    public T First;
    public const int XSize = 10, YSize = 10, ZSize = 10;
    static int IArray3D.XSize => XSize;
    static int IArray3D.YSize => YSize;
    static int IArray3D.ZSize => ZSize;
}
