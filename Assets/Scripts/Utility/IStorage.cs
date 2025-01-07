using MessagePack;
using QFramework;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public interface IStorage : IUtility
{
    public List<BlockData> BlockDataList { get; }

    void LoadGrid();
}

public class BlockDataStorage : IStorage
{
    private List<BlockData> mGird = new List<BlockData>();
    public List<BlockData> BlockDataList => mGird;
    public void LoadGrid()
    {
        byte[] data = File.ReadAllBytes("Assets/Resources/Data/Block.msgpack");

        // 反序列化为 BlockData 列表
        mGird = MessagePackSerializer.Deserialize<List<BlockData>>(data);
    }
}
