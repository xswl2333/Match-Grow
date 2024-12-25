using QFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;


[RequireComponent(typeof(UITable))]
public class BaseView : UIPanel,IController
{
    
    protected Dictionary<string, UIComponent> uiComponents;
    protected virtual void Awake()
    {
        uiComponents = new Dictionary<string, UIComponent>();
        UITable uITable = GetComponent<UITable>();
        uiComponents = uITable.uiComponents;
    }

    protected override void OnShow()
    {
       
    }

    protected override void OnHide()
    {
       
    }

    protected override void OnClose()
    {
       
    }

    IArchitecture IBelongToArchitecture.GetArchitecture()
    {
        return MainInit.Interface;
    }

}
