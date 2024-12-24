using QFramework;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;

public interface IMatchSystem : ISystem
{
    List<List<GameObject>> AllBlocks { get; }

    void GenerateBlocks(int widths, int heights) { }
}

public class MatchSystem : AbstractSystem, IMatchSystem
{
    public List<List<GameObject>> AllBlocks => mAllBlocks;
    private List<List<GameObject>> mAllBlocks = new List<List<GameObject>>();

    protected override void OnInit() {}

    public void GenerateBlocks(int widths, int heights)
    {
        for (int i = 0; i < widths; i++)
        {
            List<GameObject> blocks = new List<GameObject>();
            for (int j = 0; j < heights; j++)
            {
                //Vector3 pos = new Vector3(i * GlobalConfig.GapWidth, -j * GlobalConfig.GapWidth, 0);
                //Vector3 localPos = this.parentObject.transform.TransformPoint(pos);//转换相对坐标
                //GameObject block = Instantiate(blockPrefab, localPos, Quaternion.identity, this.parentObject.transform);//pos为世界坐标

                //int iEnum = Random.Range(0, 7);
                //block.GetComponent<Block>().Create((BlockEnum)iEnum, i, j);

                //blocks.Add(block);
            }

            mAllBlocks.Add(blocks);
        }
    }
}