using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace SoR4_Studio.Modules.ViewModel;

internal class ObservableList<T>(IList<T> list) : IList, IList<T>, INotifyCollectionChanged
{
    protected IList<T> Items { get; set; } = list;

    #region IList<T>

    public T this[int index]
    {
        get => Items[index];
        set
        {
            Items[index] = value;
            OnCollectionReset();
        }
    }

    public void Clear()
    {
        Items.Clear();

        OnCollectionReset();
    }

    public int IndexOf(T item)
    {
        return Items.IndexOf(item);
    }

    public void Insert(int index, T item)
    {
        Items.Insert(index, item);

        OnCollectionReset();
    }

    public void Add(T item)
    {
        Items.Add(item);

        OnCollectionReset();
    }

    public bool Contains(T item)
    {
        return Items.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex)
    {
        Items.CopyTo(array, arrayIndex);
    }

    public bool Remove(T item)
    {
        bool result = Items.Remove(item);

        OnCollectionReset();

        return result;
    }

    public void RemoveAt(int index)
    {
        Items.RemoveAt(index);

        OnCollectionReset();
    }

    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        return Items.GetEnumerator();
    }

    #endregion

    #region IList

    public bool IsFixedSize => false;

    public bool IsReadOnly => false;

    public int Count => Items.Count;

    public bool IsSynchronized => false;

    public object SyncRoot => this;

    object? IList.this[int index]
    {
        get => Items[index];
        set
        {
            Items[index] = (T)value!;
            OnCollectionReset();
        }
    }

    int IList.Add(object? value)
    {
        Add((T)value!);

        return Count - 1;
    }

    bool IList.Contains(object? value)
    {
        return Items.Contains((T)value!);
    }

    int IList.IndexOf(object? value)
    {
        return Items.IndexOf((T)value!);
    }

    void IList.Insert(int index, object? value)
    {
        Insert(index, (T)value!);
    }

    void IList.Remove(object? value)
    {
        Remove((T)value!);
    }

    void ICollection.CopyTo(Array array, int index)
    {
        Items.CopyTo((T[])array, index);
    }

    public IEnumerator GetEnumerator()
    {
        return Items.GetEnumerator();
    }

    #endregion

    #region INotifyCollectionChanged

    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    protected void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        => CollectionChanged?.Invoke(this, e);

    protected void OnCollectionReset()
        => OnCollectionChanged(new(NotifyCollectionChangedAction.Reset));

    #endregion

    #region Extra

    public void New(IList<T> list)
    {
        Items = list;

        OnCollectionReset();
    }

    public void Reverse()
    {
        if (Count == 0)
        {
            return;
        }

        int count = Count;

        for (int i = 0; i < count / 2; i++)
        {
            (Items[i], Items[count - i - 1]) = (Items[count - i - 1], Items[i]);
        }

        OnCollectionReset();
    }

    public void Swap(int index1, int index2)
    {
        (Items[index1], Items[index2]) = (Items[index2], Items[index1]);

        OnCollectionReset();
    }

    #endregion
}
