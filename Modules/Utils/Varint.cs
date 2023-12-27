using System;
using System.IO;

namespace SoR4_Studio.Modules.Utils;

internal struct Varint
{
    public Varint() => Encoded = 0;
    public Varint(UInt128 encoded) => Encoded = encoded;
    public Varint(byte[] encodedBytes) => Encoded = encodedBytes.ToUInt128();

    public UInt128 Encoded { get; set; } = 0;

    public readonly int EncodedLength => GetVarintLength(Encoded);

    public readonly byte[] EncodedBytes => ByteArrayConverters.Resize(Encoded.ToBytes(), EncodedLength);

    public ulong Decoded
    {
        readonly get => Decode(Encoded);

        set => Encoded = Encode(value);
    }

    public int DecodedInt32
    {
        readonly get => (int)Decoded;

        set => Encoded = Encode((ulong)value);
    }

    public int ValueZigZag32
    {
        readonly get
        {
            uint decoded = (uint)Decode(Encoded);
            return (int)(decoded >> 1 ^ -(decoded & 1));
        }

        set
        {
            Encoded = Encode((uint)(value << 1 ^ value >> 31));
        }
    }

    // To Varint
    public static UInt128 Encode(ulong value)
    {
        UInt128 result = 0;

        for (int i = 0; value != 0; i++)
        {
            if (value >> 7 != 0)
            {
                result |= (0x80 | (UInt128)value & 0x7F) << i * 8;
            }
            else //(value >> 7) == 0
            {
                result |= ((UInt128)value & 0x7F) << i * 8;
                break;
            }

            value >>= 7;
        }

        return result;
    }

    // To Raw Data
    public static ulong Decode(UInt128 value)
    {
        ulong result = 0;
        int varintLength = GetVarintLength(value);

        for (int i = 0; i < varintLength; i++)
        {
            result |= (ulong)((value & 0x7F) << i * 7);
            value >>= 8;
        }

        return result;
    }

    public static Varint FromDecoded(ulong value)
    {
        return new Varint(Encode(value));
    }

    public static Varint FromDecoded(int value)
    {
        return new Varint(Encode((ulong)value));
    }

    public static Varint FromEncoded(UInt128 value)
    {
        return new Varint(value);
    }

    public static int GetVarintLength(UInt128 value)
    {
        int length = 0;
        bool zeroFound = false;

        for (int i = 0; i < 16; i++)
        {
            byte b = (byte)(value >> i * 8 & 0xFF);

            if ((b & 0x80) == 0)
            {
                zeroFound = true;
                length++;
                break;
            }

            length++;
        }

        if (!zeroFound)
        {
            length++;
        }

        return length;
    }

    public static int GetVarintLength(byte[] source)
    {
        int length = 0;
        bool zeroFound = false;
        foreach (byte b in source)
        {
            if ((b & 0x80) == 0)
            {
                zeroFound = true;
                length++;
                break;
            }

            length++;
        }

        if (!zeroFound)
        {
            length++;
        }

        return length;
    }

    public static int GetVarintLength(Stream stream)
    {
        int length = 0;
        bool zeroFound = false;
        int b;

        while ((b = stream.ReadByte()) != -1)
        {
            if ((b & 0x80) == 0)
            {
                zeroFound = true;
                length++;
                break;
            }

            length++;
        }

        stream.Position -= length;

        if (!zeroFound)
        {
            length++;
        }

        return length;
    }
}
