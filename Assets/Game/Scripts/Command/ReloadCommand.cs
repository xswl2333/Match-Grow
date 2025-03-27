using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadCommand : AbstractCommand
{
    protected override void OnExecute()
    {
       this.GetSystem<IMatchSystem>().GenerateBlocks(GlobalGameConfig.GridWidth, GlobalGameConfig.GridWidth);
    }
}
