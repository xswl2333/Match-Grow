using QFramework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.U2D.Path.GUIFramework;
using UnityEngine;

public class MainInit : Architecture<MainInit>
{
    protected override void Init()
    {
        this.RegisterSystem<IMatchSystem>(new MatchSystem());
        this.RegisterSystem<IBasicPoolSystem>(new BasicPoolSystem());
        this.RegisterSystem<IResourceLoadSystem>(new ResourceLoadSystem());        
    }
}
