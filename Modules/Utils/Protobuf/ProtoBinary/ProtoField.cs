using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace SoR4_Studio.Modules.Utils.Protobuf.ProtoBinary;

internal unsafe class ProtoField : IDisposable
{
    public ProtoFieldType Type { get; private set; }

    public bool Match(ProtoFieldType type) => (type & Type) != 0;

    #region Byteable

    public byte* BaseBytePtr { get; private set; }

    public int ByteSize { get; private set; }

    public int Int32
    {
        get
        {
            ThrowIfTypeMismatch(ProtoFieldType.Fixed32 | ProtoFieldType.Fixed64 | ProtoFieldType.Varint);
            return *(int*)BaseBytePtr;
        }
        set
        {
            ThrowIfTypeMismatch(ProtoFieldType.Fixed32 | ProtoFieldType.Fixed64 | ProtoFieldType.Varint);
            *(int*)BaseBytePtr = value;
        }
    }

    public uint UInt32
    {
        get
        {
            ThrowIfTypeMismatch(ProtoFieldType.Fixed32 | ProtoFieldType.Fixed64 | ProtoFieldType.Varint);
            return *(uint*)BaseBytePtr;
        }
        set
        {
            ThrowIfTypeMismatch(ProtoFieldType.Fixed32 | ProtoFieldType.Fixed64 | ProtoFieldType.Varint);
            *(uint*)BaseBytePtr = value;
        }
    }

    public float Single
    {
        get
        {
            ThrowIfTypeMismatch(ProtoFieldType.Fixed32 | ProtoFieldType.Fixed64 | ProtoFieldType.Varint);
            return *(float*)BaseBytePtr;
        }
        set
        {
            ThrowIfTypeMismatch(ProtoFieldType.Fixed32 | ProtoFieldType.Fixed64 | ProtoFieldType.Varint);
            *(float*)BaseBytePtr = value;
        }
    }

    public long Int64
    {
        get
        {
            ThrowIfTypeMismatch(ProtoFieldType.Fixed64 | ProtoFieldType.Varint);
            return *(long*)BaseBytePtr;
        }
        set
        {
            ThrowIfTypeMismatch(ProtoFieldType.Fixed64 | ProtoFieldType.Varint);
            *(long*)BaseBytePtr = value;
        }
    }

    public ulong UInt64
    {
        get
        {
            ThrowIfTypeMismatch(ProtoFieldType.Fixed64 | ProtoFieldType.Varint);
            return *(ulong*)BaseBytePtr;
        }
        set
        {
            ThrowIfTypeMismatch(ProtoFieldType.Fixed64 | ProtoFieldType.Varint);
            *(ulong*)BaseBytePtr = value;
        }
    }

    public double Double
    {
        get
        {
            ThrowIfTypeMismatch(ProtoFieldType.Fixed64 | ProtoFieldType.Varint);
            return *(double*)BaseBytePtr;
        }
        set
        {
            ThrowIfTypeMismatch(ProtoFieldType.Fixed64 | ProtoFieldType.Varint);
            *(double*)BaseBytePtr = value;
        }
    }

    public string String
    {
        get
        {
            ThrowIfTypeMismatch(ProtoFieldType.String);
            return Encoding.UTF8.GetString(BaseBytePtr, ByteSize);
        }
        set
        {
            ThrowIfTypeMismatch(ProtoFieldType.String);
            Marshal.FreeCoTaskMem((nint)BaseBytePtr);
            ByteSize = Encoding.UTF8.GetByteCount(value);
            BaseBytePtr = (byte*)Marshal.StringToCoTaskMemUTF8(value);
        }
    }

    #endregion

    #region Container

    private Message? _message;
    public Message Message
    {
        get
        {
            ThrowIfTypeMismatch(ProtoFieldType.Message);
            return _message!;
        }

        set
        {
            ThrowIfTypeMismatch(ProtoFieldType.Message);
            _message = value;
        }
    }

    private Repeated? _repeated;
    public Repeated Repeated
    {
        get
        {
            ThrowIfTypeMismatch(ProtoFieldType.Repeated);
            return _repeated!;
        }

        set
        {
            ThrowIfTypeMismatch(ProtoFieldType.Repeated);
            _repeated = value;
        }
    }

    public ProtoField? this[params int[] address] => Locate(address);

    public ProtoField UpgradeToRepeated()
    {
        if (Match(ProtoFieldType.Repeated))
        {
            return this;
        }

        ProtoField copy = ShallowClone();
        Reset();
        Type = ProtoFieldType.Repeated;
        _repeated = [copy];

        return this;
    }

    private ProtoField? Locate(params int[] address)
    {
        int addressLength = address.Length;

        if (addressLength == 0)
        {
            return this;
        }

        ProtoField iField = this;

        for (int index = 0; index < addressLength; index++)
        {
            int iAddress = address[index];

            if (iField.Match(ProtoFieldType.Message))
            {
                if (iField.Message.Find((x) => x.ID == iAddress).Field is not ProtoField found)
                {
                    return null;
                }

                iField = found;
            }
            else if (iField.Match(ProtoFieldType.Repeated))
            {
                if (iAddress >= iField.Repeated.Count || iAddress < 0)
                {
                    return null;
                }

                iField = iField.Repeated[iAddress];
            }
        }

        return iField;
    }

    #endregion

    #region Creators

    private ProtoField(ProtoFieldType type, int length)
    {
        BaseBytePtr = (byte*)Marshal.AllocCoTaskMem(length);
        Type = type;
        ByteSize = length;
    }

    public ProtoField(ProtoFieldType type = ProtoFieldType.Unknown)
    {
        Type = type;
    }

    public static ProtoField CreateFixed32(int value)
        => new(ProtoFieldType.Fixed32, 4) { Int32 = value };
    public static ProtoField CreateFixed32(uint value)
        => new(ProtoFieldType.Fixed32, 4) { UInt32 = value };
    public static ProtoField CreateFixed32(float value)
        => new(ProtoFieldType.Fixed32, 4) { Single = value };
    public static ProtoField CreateFixed64(long value)
        => new(ProtoFieldType.Fixed64, 8) { Int64 = value };
    public static ProtoField CreateFixed64(ulong value)
        => new(ProtoFieldType.Fixed64, 8) { UInt64 = value };
    public static ProtoField CreateFixed64(double value)
        => new(ProtoFieldType.Fixed64, 8) { Double = value };
    public static ProtoField CreateVarint(ulong value)
        => new(ProtoFieldType.Varint, 8) { UInt64 = value };
    public static ProtoField CreateString(string value)
        => new(ProtoFieldType.String) { String = value };
    public static ProtoField CreateMessage(Message value)
        => new(ProtoFieldType.Message) { _message = value };
    public static ProtoField CreateRepeated(Repeated value)
        => new(ProtoFieldType.Repeated) { _repeated = value };

    #endregion

    public IEnumerable<ProtoField> EnumerateAllField()
    {
        if (Match(ProtoFieldType.Message))
        {
            foreach (var (ID, Field) in Message)
            {
                foreach (var iField in Field.EnumerateAllField())
                {
                    yield return iField;
                }
            }
        }
        else if (Match(ProtoFieldType.Repeated))
        {
            foreach (ProtoField field in Repeated)
            {
                foreach (var iField in field.EnumerateAllField())
                {
                    yield return iField;
                }
            }
        }
        else
        {
            yield return this;
        }
    }

    public ProtoField ShallowClone() => new()
    {
        Type = Type,
        BaseBytePtr = BaseBytePtr,
        ByteSize = ByteSize,
        _message = _message,
        _repeated = _repeated,
    };

    public ProtoField DeepClone()
    {
        ProtoField result;

        if (Match(ProtoFieldType.Fixed32))
        {
            result = CreateFixed32(Int32);
        }
        else if (Match(ProtoFieldType.Fixed64))
        {
            result = CreateFixed64(Int64);
        }
        else if (Match(ProtoFieldType.Varint))
        {
            result = CreateVarint(UInt64);
        }
        else if (Match(ProtoFieldType.String))
        {
            result = CreateString(String);
        }
        else if (Match(ProtoFieldType.Message))
        {
            Message messageClone = [];
            foreach ((int ID, ProtoField Field) in Message)
            {
                messageClone.Add((ID, Field.DeepClone()));
            }
            result = CreateMessage(messageClone);
        }
        else if (Match(ProtoFieldType.Repeated))
        {
            Repeated repeatedClone = [];
            foreach (var iField in Repeated)
            {
                repeatedClone.Add(iField.DeepClone());
            }
            result = CreateRepeated(repeatedClone);
        }
        else
        {
            result = new();
        }

        return result;
    }

    private void ThrowIfTypeMismatch(ProtoFieldType type)
    {
        if (!Match(type))
        {
            throw ProtobufExceptions.FieldTypeMismatchException;
        }
    }

    // !危险!
    // 该方法不释放BaseBytePtr
    // 使用不当将造成内存泄漏
    private void Reset()
    {
        Type = ProtoFieldType.Unknown;
        BaseBytePtr = null;
        ByteSize = 0;
        _message = null;
        _repeated = null;
    }

    #region IDisposable

    private void DisposeMessage()
    {
        if (_message is null)
        {
            return;
        }

        foreach (var (_, Field) in _message)
        {
            Field.Dispose();
        }

        _message.Clear();
    }

    private void DisposeRepeated()
    {
        if (_repeated is null)
        {
            return;
        }

        foreach (ProtoField iField in _repeated)
        {
            iField.Dispose();
        }

        _repeated.Clear();
    }

    private bool _disposedValue = false;

    private void Dispose(bool disposing)
    {
        if (_disposedValue)
        {
            return;
        }

        if (disposing)
        {
            DisposeMessage();
            DisposeRepeated();
        }

        Marshal.FreeCoTaskMem((nint)BaseBytePtr);
        Reset();

        _disposedValue = true;

    }

    ~ProtoField()
    {
        Dispose(disposing: false);
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    #endregion

    public override string ToString()
    {
#if DEBUG
        return ToString([]);
#else
        return string.Empty;
#endif
    }

    private string ToString(int[] previousAddress)
    {
        string result = "";
        if (Match(ProtoFieldType.Fixed32 | ProtoFieldType.Fixed64 | ProtoFieldType.Varint | ProtoFieldType.String))
        {
            result += "[";
            int addressLength = previousAddress.Length;

            for (int i = 0; i < addressLength - 1; i++)
            {
                int node = previousAddress[i];
                if (node <= 0)
                {
                    result += $"[{-node}],";
                }
                else // node > 0
                {
                    result += $"{node},";
                }
            }

            if (previousAddress.Length > 0)
            {
                result += $"{previousAddress[^1]}] ";
            }
            else
            {
                result += $"]";
            }
        }

        if (Match(ProtoFieldType.Fixed32))
        {
            result += $"<Fixed32 : {Single} | {Int32} | {UInt32}>\n";
        }
        else if (Match(ProtoFieldType.Fixed64))
        {
            result += $"<Fixed64 : {Double} | {Int64} | {UInt64}>\n";
        }
        else if (Match(ProtoFieldType.Varint))
        {
            result += $"<Varint : {Int32} | {UInt64}>\n";
        }
        else if (Match(ProtoFieldType.String))
        {
            result += $"<String : \"{String}\">\n";
        }
        else if (Match(ProtoFieldType.Message))
        {
            foreach (var (ID, Field) in Message)
            {
                result += $"{Field.ToString([.. previousAddress, ID])}";
            }
        }
        else if (Match(ProtoFieldType.Repeated))
        {
            int index = 0;
            foreach (var iField in Repeated)
            {
                result += $"{iField.ToString([.. previousAddress, -index])}";
                index++;
            }
        }
        else
        {
            result = $"\n\n<Unknown>\n\n";
        }

        return result;
    }
}
