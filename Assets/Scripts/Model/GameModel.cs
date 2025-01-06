using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IGameModel : IModel
{
    //消除匹配规则之后获得点数
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


