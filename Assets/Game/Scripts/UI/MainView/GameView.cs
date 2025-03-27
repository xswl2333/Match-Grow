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
        mGameModel.SkillPoint.Register(OnEnergyChanged);


        // 第一次需要调用一下
        OnEnergyChanged(mGameModel.SkillPoint.Value);

    }

    private void OnEnergyChanged(int count)
    {
        string str = $"能量点 {count}"; 
         uiComponents["Universal"].text.text= str;
    }


    private void OnDestroy()
    {
        mGameModel.SkillPoint.UnRegister(OnEnergyChanged);
        mGameModel = null;

    }
}
