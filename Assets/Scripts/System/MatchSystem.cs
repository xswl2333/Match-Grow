using Markdig.Syntax;
using QFramework;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public interface IMatchSystem : ISystem
{
    List<List<GameObject>> AllBlocks { get; }

    void GenerateBlocks(int widths, int heights) { }
    void SetParentObject(Transform parentTransform) { }
}

public class MatchSystem : AbstractSystem, IMatchSystem
{
    private Transform parentTransform;
    private ResLoader mResLoader;
    public List<List<GameObject>> AllBlocks => mAllBlocks;
    private List<List<GameObject>> mAllBlocks = new List<List<GameObject>>();

    protected override void OnInit() { }

    public void SetParentObject(Transform parentTransform)
    {
        this.parentTransform = parentTransform;
        mResLoader = ResLoader.Allocate();
    }

    public void GenerateBlocks(int widths, int heights)
    {
        int startX = -widths / 2;
        int endX = - startX;

        int startY = -heights / 2;
        int endY = -startY;

        for (int i = startX; i < endX; i++)
        {
            List<GameObject> blocks = new List<GameObject>();
            for (int j = startY; j < endY; j++)
            {
                Vector3 pos = new Vector3(i * GlobalConfig.GapWidth, -j * GlobalConfig.GapWidth, 0);
                Vector3 localPos = this.parentTransform.TransformPoint(pos);//转换相对坐标
                GameObject block = mResLoader.LoadSync<GameObject>("Block").Instantiate(localPos, Quaternion.identity, this.parentTransform);

                int iEnum = Random.Range(0, 7);
                block.GetComponent<Block>().Create((BlockEnum)iEnum, i, j);

                blocks.Add(block);
            }

            mAllBlocks.Add(blocks);
        }
    }
}