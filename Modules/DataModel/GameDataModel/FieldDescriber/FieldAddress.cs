namespace SoR4_Studio.Modules.DataModel.GameDataModel.FieldDescriber;

internal readonly struct FieldAddress
{
    public FieldAddress(string mainKey, string subKey, params int[] ints)
    {
        MainKey = mainKey;
        SubKey = subKey;
        Ints = ints;
    }

    public FieldAddress(string mainKey, params int[] ints)
    {
        MainKey = mainKey;
        SubKey = null;
        Ints = ints;
    }

    public string MainKey { get; init; }

    public string? SubKey { get; init; }

    public bool IsDirectKey => SubKey is null;

    public int[] Ints { get; init; }

    public static FieldAddress operator +(FieldAddress left, int[] right)
    {
        int[] newInts = new int[left.Ints.Length + right.Length];

        left.Ints.CopyTo(newInts, 0);
        right.CopyTo(newInts, left.Ints.Length);

        return left.IsDirectKey
            ? new FieldAddress(left.MainKey, newInts)
            : new FieldAddress(left.MainKey, left.SubKey!, newInts);
    }

    public static FieldAddress operator +(FieldAddress left, int right)
    {
        return left + new int[1] { right };
    }
}
