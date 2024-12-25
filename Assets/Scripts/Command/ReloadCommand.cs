using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Path.GUIFramework;
using UnityEngine;

public class ReloadCommand : AbstractCommand
{
    protected override void OnExecute()
    {
       this.GetSystem<IMatchSystem>().GenerateBlocks(GlobalConfig.GridWidth, GlobalConfig.GridWidth);
    }
}
