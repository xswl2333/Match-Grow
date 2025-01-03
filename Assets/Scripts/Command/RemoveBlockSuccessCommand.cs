using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Path.GUIFramework;
using UnityEngine;

public class RemoveBlockSuccessCommand : AbstractCommand
{

    protected override void OnExecute()
    {
        var gameModel = this.GetModel<IGameModel>();
        gameModel.EnergyPoint_Count.Value++;

        

    }
}
