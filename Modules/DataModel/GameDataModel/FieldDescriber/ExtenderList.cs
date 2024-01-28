using SoR4_Studio.Modules.Utils.Protobuf.ProtoBinary;
using System.Collections;
using System.Collections.Generic;

namespace SoR4_Studio.Modules.DataModel.GameDataModel.FieldDescriber;

internal class ExtenderList<T>(FieldDescriber describer) : IList<T> where T : FieldExtenderBase, new()
{
    public T this[int index]
    {
        get => FieldExtenderBase.MakeChild<T>(GameData, BaseList[index]);

        set => BaseList[index] = value.BaseField;
    }

    public GameData GameData { get; init; } = describer.GameData;

    public ProtoField BaseField { get; init; } = describer.Result!.UpgradeToRepeated();

    public Repeated BaseList => BaseField.Repeated;

    public int Count => BaseList.Count;

    public bool IsReadOnly => false;

    public void Add(T item)
    {
        BaseList.Add(item.BaseField);
    }

    public void Clear()
    {
        BaseList.Clear();
    }

    public bool Contains(T item)
    {
        return BaseList.Contains(item.BaseField);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        for (int i = arrayIndex; i < Count; i++)
        {
            array[i] = this[i];
        }
    }

    public int IndexOf(T item)
    {
        return BaseList.IndexOf(item.BaseField);
    }

    public void Insert(int index, T item)
    {
        BaseList.Insert(index, item.BaseField);
    }

    public bool Remove(T item)
    {
        return BaseList.Remove(item.BaseField);
    }

    public void RemoveAt(int index)
    {
        BaseList.RemoveAt(index);
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < Count; i++)
        {
            yield return FieldExtenderBase.MakeChild<T>(GameData, BaseList[i]);
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public static implicit operator ExtenderList<T>?(FieldDescriber v) => v.Result is null ? null : new(v);
}
