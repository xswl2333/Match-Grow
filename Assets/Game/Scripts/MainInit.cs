using QFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MainInit : Architecture<MainInit>
{
    protected override void Init()
    {
        this.RegisterSystem<IMatchSystem>(new MatchSystem());
        this.RegisterSystem<IBasicPoolSystem<Block>>(new BasicPoolSystem<Block>());
        this.RegisterSystem<IResourceLoadSystem>(new ResourceLoadSystem());
        this.RegisterModel<IGameModel>(new GameModel());
        this.RegisterUtility<IStorage>(new BlockDataStorage());
    }

    
}
