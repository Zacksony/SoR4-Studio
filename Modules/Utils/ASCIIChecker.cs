using System;
using System.IO;

namespace SoR4_Studio.Modules.Utils;

internal static class ASCIIChecker
{
    public static bool IsDisplayables(this UInt128 value, int length)
    {
        for (int i = 0; i < length; i++)
        {
            byte iByte = (byte)((value >> (i * 8)) & 0xFF);

            if (!(iByte >= 0x20 && iByte <= 0x7E))
            {
                break;
            }

            if (i == length - 1)
            {
                return true;
            }
        }

        return false;
    }

    public static bool IsDisplayables(this byte[] bytes, int length)
    {
        for (int i = 0; i < length; i++)
        {
            byte iByte = bytes[i];

            if (!(iByte >= 0x20 && iByte <= 0x7E))
            {
                break;
            }

            if (i == length - 1)
            {
                return true;
            }
        }

        return false;
    }

    public static bool IsDisplayables(this Stream stream, int length)
    {
        int i = 0;
        for (; i < length; i++)
        {
            byte iByte = (byte)stream.ReadByte();

            if (!(iByte >= 0x20 && iByte <= 0x7E))
            {
                break;
            }

            if (i == length - 1)
            {
                stream.Position -= length;
                return true;
            }
        }

        stream.Position -= i + 1;
        return false;
    }
}