using QFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;


[RequireComponent(typeof(UITable))]
public class BaseView : MainController
{
    protected bool isRemove = false;
    protected new string name;

    protected Dictionary<string, UIComponent> uiComponents;
    protected virtual void Awake()
    {
        uiComponents = new Dictionary<string, UIComponent>();
        UITable uITable = GetComponent<UITable>();
        uiComponents = uITable.uiComponents;
    }

    public virtual void SetActive(bool active)
    {
        gameObject.SetActive(active);
    }

    public virtual void OpenPanel(string name)
    {
        this.name = name;
        SetActive(true);
    }

    public virtual void ClosePanel()
    {
        isRemove = true;
        SetActive(false);
        Destroy(gameObject);
        //if (UIManager.Instance.panelDict.ContainsKey(name))
        //{
        //    UIManager.Instance.panelDict.Remove(name);
        //}

    }
}
