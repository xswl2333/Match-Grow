using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Path.GUIFramework;
using UnityEngine;

public class MainInit : Architecture<MainInit>
{
    protected override void Init()
    {
        this.RegisterSystem<IMatchSystem>(new MatchSystem());
        this.RegisterSystem<IResourceLoadSystem>(new ResourceLoadSystem());        
        this.RegisterModel<IBlockModel>(new BlockModel());
    }
}
