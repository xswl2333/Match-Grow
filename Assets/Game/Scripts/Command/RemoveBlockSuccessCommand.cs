using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveBlockSuccessCommand : AbstractCommand
{

    protected override void OnExecute()
    {
        var gameModel = this.GetModel<IGameModel>();
        gameModel.SkillPoint.Value++;

        

    }
}
