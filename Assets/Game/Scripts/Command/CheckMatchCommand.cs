using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckMatchCommand : AbstractCommand
{
    private readonly Block block;

    public CheckMatchCommand(Block block)
    {
        this.block = block;
    }

    protected override void OnExecute()
    {
       this.GetSystem<IMatchSystem>().CheckBlock(this.block);
    }
}
