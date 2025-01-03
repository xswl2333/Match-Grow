using QFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameView : BaseView
{
    
    private IGameModel mGameModel;


    private void Start()
    {
        mGameModel = this.GetModel<IGameModel>();
        mGameModel.EnergyPoint_Count.Register(OnEnergyChanged);


        // ��һ����Ҫ����һ��
        OnEnergyChanged(mGameModel.EnergyPoint_Count.Value);

    }

    private void OnEnergyChanged(int count)
    {
        string str = $"������ {count}"; 
         uiComponents["Universal"].text.text= str;
    }


    private void OnDestroy()
    {
        mGameModel.EnergyPoint_Count.UnRegister(OnEnergyChanged);
        mGameModel = null;

    }
}
