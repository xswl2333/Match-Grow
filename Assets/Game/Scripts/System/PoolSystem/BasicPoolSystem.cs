using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Progress;


public interface IBasicPoolSystem<T> : ISystem
{
    //集合
    public Stack<T> ItemPrefab { get; }
    public int MaxCount { get; }

    T PopByPoolIdType();

    void PushByPoolIdType(T go, PoolIdEnum type);
}

public class BasicPoolSystem<T> : AbstractSystem, IBasicPoolSystem<T> where T : Entity
{
    public Stack<T> ItemPrefab => mItemPrefab;
    public int MaxCount => m_MaxCount;


    private Stack<T> mItemPrefab = new Stack<T>();
    public int m_MaxCount = 10;

    protected override void OnInit()
    {
        mItemPrefab.Clear();
    }

    //对象保存到对象池中
    public void PushByPoolIdType(T go, PoolIdEnum type)
    {
        if (mItemPrefab.Count < m_MaxCount)
        {
            if (!mItemPrefab.Contains(go))
            {
                mItemPrefab.Push(go);
                go.SetActive(false);
            }

        }
        else
        {
            go.Destroy();
        }
    }

    public T PopByPoolIdType()
    {

        if (mItemPrefab.Count > 0)
        {
            T go = mItemPrefab.Pop();
            return go;
        }
        else
        {
            return null;
        }
    }

}
