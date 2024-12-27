using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Path.GUIFramework;
using UnityEngine;

public class ClickBlockCommand : AbstractCommand
{
    //private readonly Block block;
    private readonly GameObject block;

    public ClickBlockCommand(GameObject block)
    {
        this.block = block;
    }

    protected override void OnExecute()
    {
       this.GetSystem<IMatchSystem>().ExchangeBlock(this.block);
    }
}