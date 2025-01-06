using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IGameModel : IModel
{
    //����ƥ�����֮���õ���
    BindableProperty<int> SkillPoint  { get; }
}

public class GameModel : AbstractModel, IGameModel
{
    public BindableProperty<int> SkillPoint { get; } = new BindableProperty<int>()
    { 
        Value = 0
    };

    protected override void OnInit()
    {
        

    }
}


