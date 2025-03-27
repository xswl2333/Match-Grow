using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;


public sealed class UITable : MonoBehaviour
{
    [Serializable]
    public struct BindPair
    {
        [Tooltip("The name of this bind.")]
        public string Name;

        [Tooltip("The widget of this UI.")]
        public GameObject Widget;
    }

    [SerializeField, Tooltip("The bind list.")]
    public List<UITable.BindPair> binds = new List<UITable.BindPair>();

    private Dictionary<string, GameObject> map = new Dictionary<string, GameObject>();

    //[SerializeField]
    //public List<UIPrefabLoader> loaderList = new List<UIPrefabLoader>();

    public bool prefabToggle = false;

    [CompilerGenerated]
    private static Comparison<UITable.BindPair> b;

    public Dictionary<string, GameObject> Lookup
    {
        get
        {
            this.map.Clear();
            if (this.binds != null)
            {
                foreach (UITable.BindPair current in this.binds)
                {
                    this.map.Add(current.Name, current.Widget);
                }
            }
            return this.map;
        }
    }

    private Dictionary<string, UIComponent> m_uiComponents = new Dictionary<string, UIComponent>();
    public Dictionary<string, UIComponent> uiComponents
    {
        get
        {
            m_uiComponents.Clear();
            if (this.binds != null)
            {
                foreach (UITable.BindPair current in this.binds)
                {
                    UIComponent uiObj = new UIComponent(current.Widget);
                    m_uiComponents.Add(current.Name, uiObj);
                }
            }
            return m_uiComponents;
        }
    }

    /// <summary>
    /// 查找同名数量
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public GameObject Find(string key)
    {
        GameObject result;
        if (this.Lookup.TryGetValue(key, out result))
            return result;
        return null;
    }

    /// <summary>
    /// 添加到列表
    /// </summary>
    /// <param name="key"></param>
    /// <param name="obj"></param>
    /// <returns></returns>
    public bool Add(string key, GameObject obj)
    {
        if (!this.map.ContainsKey(key))
        {
            this.map.Add(key, obj);
            UITable.BindPair item = default(UITable.BindPair);
            item.Name = key;
            item.Widget = obj;
            this.binds.Add(item);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 节点排序（根据字母）
    /// </summary>
    public void Sort()
    {
        this.binds.Sort((Comparison<UITable.BindPair>)((obj0, obj1) => obj0.Name.CompareTo(obj1.Name)));
    }

    /// <summary>
    /// 搜索节点(根据name)
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public UITable.BindPair[] Search(string key)
    {
        List<UITable.BindPair> list = new List<UITable.BindPair>();
        foreach (UITable.BindPair current in this.binds)
        {
            if (current.Name.StartsWith(key))
                list.Add(current);
        }
        return list.ToArray();
    }

    public void OnDestroy()
    {
        map.Clear();
        m_uiComponents.Clear();
    }
}

