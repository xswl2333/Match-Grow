using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.Progress;


public struct PoolItem
{
    public GameObject item;
    public PoolIdEnum itemType;//0为block

    public PoolItem(GameObject _item, PoolIdEnum _itemType)
    {
        item = _item;
        itemType = _itemType;
    }
}


public interface IBasicPoolSystem : ISystem
{
    //集合
    public Dictionary<int, Stack<PoolItem>> ItemPrefab { get; }
    public int MaxCount { get; }

    GameObject PopByPoolIdType(PoolIdEnum type);

    void PushByPoolIdType(GameObject go, PoolIdEnum type);
}

public class BasicPoolSystem : AbstractSystem, IBasicPoolSystem
{
    public Dictionary<int, Stack<PoolItem>> ItemPrefab => mItemPrefab;
    public int MaxCount => m_MaxCount;


    public Dictionary<int, Stack<PoolItem>> mItemPrefab = new Dictionary<int, Stack<PoolItem>>();
    public int m_MaxCount = 10;

    protected override void OnInit()
    {
        mItemPrefab.Clear();
    }

    //对象保存到对象池中
    public void PushByPoolIdType(GameObject go, PoolIdEnum type)
    {
        Stack<PoolItem> pool;
        if (!mItemPrefab.ContainsKey((int)type))
        {
            pool = new Stack<PoolItem>();
            mItemPrefab[(int)type] = pool;
        }
        else
        {
            pool = mItemPrefab[(int)type];
        }

        if (pool.Count < m_MaxCount)
        {
            PoolItem item = new PoolItem(go, type);
            pool.Push(item);
            go.SetActive(false);
        }
        else
        {
            if (type == PoolIdEnum.BlockPoolId)
            {
                Block block = go.GetComponent<Block>();
                block.DestroyBlock();
            }
        }
    }

    public GameObject PopByPoolIdType(PoolIdEnum type)
    {

        Stack<PoolItem> pool;
        if (!mItemPrefab.ContainsKey((int)type))
        {
            pool = new Stack<PoolItem>();
            mItemPrefab[(int)type] = pool;
        }
        else
        {
            pool = mItemPrefab[(int)type];
        }


        if (pool.Count > 0)
        {
            PoolItem go = pool.Pop();
            GameObject item = go.item;
            return item;
        }
        else
        {
            return null;
        }
    }

}
