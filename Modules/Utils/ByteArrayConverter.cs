using System;

namespace SoR4_Studio.Modules.Utils;

internal static class ByteArrayConverter
{
    #region Bytes to Number

    public static int ToInt32(this byte[] bytes, int readLength = 4)
    {
        int result = 0;
        Memory<byte> sliced = bytes.AsMemory()[..(readLength = Math.Min(bytes.Length, readLength))];

        for (int i = 0; i < readLength; i++)
        {
            result |= sliced.Span[i] << (8 * i);
        }

        return result;
    }

    public static long ToInt64(this byte[] bytes, int readLength = 8)
    {
        long result = 0;
        Memory<byte> sliced = bytes.AsMemory()[..(readLength = Math.Min(bytes.Length, readLength))];

        for (int i = 0; i < readLength; i++)
        {
            result |= (long)sliced.Span[i] << (8 * i);
        }

        return result;
    }

    public static Int128 ToInt128(this byte[] bytes, int readLength = 16)
    {
        Int128 result = 0;
        Memory<byte> sliced = bytes.AsMemory()[..(readLength = Math.Min(bytes.Length, readLength))];

        for (int i = 0; i < readLength; i++)
        {
            result |= (Int128)sliced.Span[i] << (8 * i);
        }

        return result;
    }

    public static uint ToUInt32(this byte[] bytes, int readLength = 4)
    {
        uint result = 0;
        Memory<byte> sliced = bytes.AsMemory()[..(readLength = Math.Min(bytes.Length, readLength))];

        for (int i = 0; i < readLength; i++)
        {
            result |= (uint)sliced.Span[i] << (8 * i);
        }

        return result;
    }

    public static ulong ToUInt64(this byte[] bytes, int readLength = 8)
    {
        ulong result = 0;
        Memory<byte> sliced = bytes.AsMemory()[..(readLength = Math.Min(bytes.Length, readLength))];

        for (int i = 0; i < readLength; i++)
        {
            result |= (ulong)sliced.Span[i] << (8 * i);
        }

        return result;
    }

    public static UInt128 ToUInt128(this byte[] bytes, int readLength = 16)
    {
        UInt128 result = 0;
        Memory<byte> sliced = bytes.AsMemory()[..(readLength = Math.Min(bytes.Length, readLength))];

        for (int i = 0; i < readLength; i++)
        {
            result |= (UInt128)sliced.Span[i] << (8 * i);
        }

        return result;
    }

    #endregion

    #region Number to Bytes

    public static byte[] ToBytes(this int value, int outputLength = 4)
    {
        return ToBytes((UInt128)value, outputLength);
    }

    public static byte[] ToBytes(this long value, int outputLength = 8)
    {
        return ToBytes((UInt128)value, outputLength);
    }

    public static byte[] ToBytes(this Int128 value, int outputLength = 16)
    {
        return ToBytes((UInt128)value, outputLength);
    }

    public static byte[] ToBytes(this uint value, int outputLength = 4)
    {
        return ToBytes((UInt128)value, outputLength);
    }

    public static byte[] ToBytes(this ulong value, int outputLength = 8)
    {
        return ToBytes((UInt128)value, outputLength);
    }

    public static byte[] ToBytes(this UInt128 value, int outputLength = 16)
    {
        byte[] bytes = new byte[outputLength];

        for (int i = 0; i < outputLength; i++)
        {
            bytes[i] = (byte)(value >> (i * 8));
        }

        return bytes;
    }

    #endregion

    // 如果 newSize < oldSize 则相当于 ReadOnlySpan<byte>.Slice()
    // 如果 newSize > oldSize 将产生复制，并填充0
    public static byte[] Resize(this byte[] bytes, int newSize)
    {
        int oldSize = bytes.Length;

        if (newSize == oldSize)
        {
            return bytes;
        }

        if (newSize == 0)
        {
            return [];
        }

        if (newSize > oldSize)
        {
            return [.. bytes, .. new byte[newSize - oldSize]];
        }
        else // newSize < oldLength
        {
            return bytes[0..newSize];
        }
    }
}
