using Markdig.Parsers;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IBlockModel : IModel
{

}

public class BlockConfig
{
    public BlockConfig(BlockEnum BlockType, int x, int y)
    {
        m_BlockType = BlockType;
        m_x = x;
        m_y = y;
    }

    public BlockEnum m_BlockType { get; private set; }
    public int m_x { get; private set; }
    public int m_y { get; private set; }

}


public class BlockModel : AbstractModel, IBlockModel
{

    protected override void OnInit()
    {

    }

}
