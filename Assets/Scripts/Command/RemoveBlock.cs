using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Path.GUIFramework;
using UnityEngine;

public class RemoveBlock : AbstractCommand
{
    //private readonly Block block;
    private readonly GameObject block;

    public RemoveBlock(GameObject block)
    {
        this.block = block;
    }

    protected override void OnExecute()
    {
       
    }
}
