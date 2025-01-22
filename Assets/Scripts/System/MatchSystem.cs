using Markdig.Syntax;
using QFramework;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using System.Collections;
using Unity.VisualScripting;
using MessagePack;
using static UnityEngine.RuleTile.TilingRuleOutput;
using UnityEngine.XR;
using Transform = UnityEngine.Transform;
using MoonSharp.Interpreter.IO;
using UnityEditor.Search;
using System.Linq;

public interface IMatchSystem : ISystem
{
    List<List<GameObject>> AllBlocks { get; }

    void GenerateBlocks(int widths, int heights) { }
    void SetParentObject(Transform parentTransform) { }
    void CheckBlock(Block block) { }

    void ExchangeBlock(Block block) { }
}

public class MatchSystem : AbstractSystem, IMatchSystem
{
    private Transform parentTransform;
    private ResLoader mResLoader;
    public GlobalGameConfig gameConfig;

    public List<List<GameObject>> AllBlocks => mAllBlocks;
    private List<List<GameObject>> mAllBlocks = new List<List<GameObject>>();
    private int totalBlockAmount;

    private Block currentSelectBlock;
    private Queue<Block> selectQueObj = new Queue<Block>();

    private List<Block> matchList = new List<Block>();
    private List<Block> sameItemsList = new List<Block>();

    public bool isOperation = false;
    private MatchState matchState = MatchState.Idle;

    protected override void OnInit()
    {

    }

    public void SetParentObject(Transform parentTransform)
    {
        this.parentTransform = parentTransform;
        mResLoader = ResLoader.Allocate();
    }

    public void GenerateBlocks(int widths, int heights)
    {
        if (GameController.Instance.gameConfig.EditorModel)
        {
            var storage = this.GetUtility<IStorage>();
            var blockDataList = storage.BlockDataList;

            BlockState[,] grid = new BlockState[widths, widths];
            foreach (var blockData in blockDataList)
            {
                grid[blockData.Y, blockData.X] = (BlockState)blockData.BlockState;
            }

            for (int i = 0; i < widths; i++)
            {
                List<GameObject> blocks = new List<GameObject>();
                for (int j = 0; j < heights; j++)
                {
                    Vector3 pos = CalculationTool.BlockCoverPos(i, j);
                    Vector3 localPos = this.parentTransform.TransformPoint(pos);//转换相对坐标
                    GameObject block = mResLoader.LoadSync<GameObject>("Block").Instantiate(localPos, Quaternion.identity, this.parentTransform);

                    int iEnum = Random.Range(0, 7);
                    block.GetComponent<Block>().Create((BlockType)iEnum, i, j, grid[i, j]);

                    blocks.Add(block);
                    totalBlockAmount++;
                }
                mAllBlocks.Add(blocks);
            }
        }
        else
        {
            for (int i = 0; i < widths; i++)
            {
                List<GameObject> blocks = new List<GameObject>();
                for (int j = 0; j < heights; j++)
                {
                    Vector3 pos = CalculationTool.BlockCoverPos(i, j);
                    Vector3 localPos = this.parentTransform.TransformPoint(pos);//转换相对坐标
                    GameObject block = mResLoader.LoadSync<GameObject>("Block").Instantiate(localPos, Quaternion.identity, this.parentTransform);

                    int iEnum = Random.Range(0, 7);
                    block.GetComponent<Block>().Create((BlockType)iEnum, i, j);

                    blocks.Add(block);
                    totalBlockAmount++;

                }
                mAllBlocks.Add(blocks);
            }

        }

        this.MatchAllBlock();
    }

    private void MatchAllBlock()
    {
        //有消除
        bool hasBoom = false;

        foreach (var blockList in mAllBlocks)
        {
            for (int i = 0; i < blockList.Count; i++) //foreach遍历,元素为空,报错
            {
                var blockObject = blockList[i];
                if (blockObject)
                {
                    Block block = blockObject.GetComponent<Block>();

                    //指定位置的Item存在，且没有被检测过
                    if (!block.hasCheck)
                    {
                        //检测周围的消除
                        block.CheckAroundBoom();
                        if (matchList.Count > 0)
                        {
                            hasBoom = true;
                            isOperation = true;
                        }
                    }
                }

            }
        }
        if (!hasBoom)
        {
            //操作结束
            isOperation = false;
        }

    }


    public void ExchangeBlock(Block clickBlock)
    {
        //SoundManager.Instance.PlaySound(SoundsConfig.Click1);
        currentSelectBlock = clickBlock;
        currentSelectBlock.SetBg(true);
        selectQueObj.Enqueue(currentSelectBlock);

        if (selectQueObj.Count > 1)
        {
            Block lastObj = selectQueObj.Peek();
            lastObj.SetBg(false);
            HandleExchange();

        }
    }


    private void HandleExchange()
    {
        int result = this.Exchange();
        if (result == 2)
        {
            currentSelectBlock.SetBg(false);
            selectQueObj.Clear();
            return;
        }
        else if (result == 3)
        {
            selectQueObj.Dequeue();
        }


        var block = currentSelectBlock;
        block.CheckAroundBoom();

        Block lastObj = selectQueObj.Peek();
        lastObj.CheckAroundBoom();

        selectQueObj.Clear();

    }

    //交换位置
    private int Exchange()
    {
        int res = 1;
        int firstPosX = 0, firstPosY = 0;
        int secondPosX = 0, secondPosY = 0;

        Block lastObj = selectQueObj.Peek();
        var lastBlock = lastObj.GetComponent<Block>();
        var currentBlock = currentSelectBlock;

        if (!CheckAroundBlock(lastBlock, currentBlock))
        {
            res = 2;//不是相邻格子
            return res;
        }
        //if (!CheckBlockState(lastBlock, currentBlock))
        //{
        //    res = 3;//格子上有slime
        //    return res;
        //}

        lastBlock.GetBlockPos(ref firstPosX, ref firstPosY);
        currentSelectBlock.GetBlockPos(ref secondPosX, ref secondPosY);

        lastBlock.UpdatePos(secondPosX, secondPosY, false);
        currentSelectBlock.UpdatePos(firstPosX, firstPosY, true);
        lastBlock.SetBg(false);
        currentSelectBlock.SetBg(false);

        mAllBlocks[firstPosX][firstPosY] = currentSelectBlock.GetGameObject();
        mAllBlocks[secondPosX][secondPosY] = lastObj.GetGameObject();

        return res;
    }

    public bool CheckAroundBlock(Block currentBlock, Block targetBlock)
    {
        bool res = false;

        Block[] tempItemList = GetArounBlocks(currentBlock);

        foreach (Block block in tempItemList)
        {
            if (block == targetBlock)
            {
                res = true;
                break;
            }
        }

        return res;
    }

    public void CheckBlock(Block currentBlock)
    {
        this.matchState = MatchState.RemoveBlock;
        this.ClearData();
        this.CheckSameBlockList(currentBlock);

        //计数器
        int rowCount = 0;
        int columnCount = 0;
        //临时列表
        List<Block> rowTempList = new List<Block>();
        List<Block> columnTempList = new List<Block>();

        ///横向纵向检测
        foreach (var item in sameItemsList)
        {

            //如果在同一行
            if (item.m_x == currentBlock.m_x)
            {
                //判断该点与Curren中间有无间隙
                bool rowCanBoom = CheckItemsInterval(true, currentBlock, item);
                if (rowCanBoom)
                {
                    //计数
                    rowCount++;
                    //添加到行临时列表
                    rowTempList.Add(item);
                }
            }
            //如果在同一列
            if (item.m_y == currentBlock.m_y)
            {
                //判断该点与Curren中间有无间隙
                bool columnCanBoom = CheckItemsInterval(false, currentBlock, item);
                if (columnCanBoom)
                {
                    //计数
                    columnCount++;
                    //添加到列临时列表
                    columnTempList.Add(item);
                }
            }
        }
        //横向消除
        bool horizontalBoom = false;
        //如果横向三个以上
        if (rowCount > 2)
        {
            //将临时列表中的Item全部放入BoomList
            matchList.AddRange(rowTempList);
            //横向消除
            horizontalBoom = true;
        }
        //如果纵向三个以上
        if (columnCount > 2)
        {
            if (horizontalBoom)
            {
                //剔除自己
                matchList.Remove(currentBlock);
            }
            //将临时列表中的Item全部放入BoomList
            matchList.AddRange(columnTempList);
        }
        //如果没有消除对象，返回
        if (matchList.Count == 0)
            return;
        //创建临时的BoomList
        List<Block> tempBoomList = new List<Block>();
        //转移到临时列表
        tempBoomList.AddRange(matchList);
        HandleMatchList(tempBoomList);
    }


    private void CheckSameBlockList(Block currentBlock)
    {
        //如果已存在，跳过
        if (sameItemsList.Contains(currentBlock))
            return;
        if (currentBlock.BlockState == BlockState.Freeze)
            return;

        //添加到列表
        sameItemsList.Add(currentBlock);
        //上下左右的Item
        Block[] tempItemList = GetArounBlocks(currentBlock);

        for (int i = 0; i < tempItemList.Length; i++)
        {
            //如果Item不合法，跳过
            if (tempItemList[i] == null)
                continue;
            if (currentBlock.BlockType == tempItemList[i].BlockType)
            {
                CheckSameBlockList(tempItemList[i]);
            }
        }
    }


    private void HandleMatchList(List<Block> tempBoomList)
    {
        foreach (var block in tempBoomList)
        {
            block.hasCheck = true;
            //block.GetComponent<Image>(). = randomColor * 2;
            ////离开动画
            //AudioManager.instance.PlayMagicalAudio();
            //将被消除的Item在全局列表中移除
            DelBlockByIndex(block.m_x, block.m_y);
            totalBlockAmount--;
        }
        //检测Item是否已经开发播放离开动画
        //while (!tempBoomList[0].GetComponent<AnimatedButton>().CheckPlayExit())
        //{
        //    yield return 0;
        //}

        this.RemoveMatchBlock();
        TimeTask dropTime = new TimeTask()
        {
            id = 0,
            taskTime = 0.38f,
            onCompleted = this.BlocksDrop,
        };
        TimeTaskSystem.Instance.AddTimeTask(dropTime);

    }


    private void BlocksDrop()
    {
        //TODO 缺少技能点规则
        this.GetModel<IGameModel>().SkillPoint.Value++;
        this.matchState = MatchState.BlocksDrop;
        isOperation = true;

        bool hasDropped;
        int maxFallSteps = totalBlockAmount ;//下落移动的步数

        do
        {
            hasDropped = false;
            maxFallSteps--; // 关键修复：递减计数器

            // 从倒数第二行开始检测（最底层不需要检测）
            for (int y = GlobalGameConfig.GridHeight - 1; y >= 0; y--)
            {
                // 逐列检测
                for (int x = 0; x < GlobalGameConfig.GridWidth; x++)
                {
                    Block currentBlock = GetBlockByIndex(x, y);
                    if (currentBlock == null) continue;
                    if (currentBlock.BlockState == BlockState.Freeze) continue;

                    bool res = CheckItemDrop(x, y);
                    // 检测下方是否为空
                    if (res)
                    {
                        // 执行单格下落
                        DelBlockByIndex(currentBlock.m_x, currentBlock.m_y);
                        currentBlock.UpdatePos(currentBlock.m_x, y + 1, true);
                        mAllBlocks[currentBlock.m_x][currentBlock.m_y] = currentBlock.GetGameObject();
                        hasDropped = true;
                    }
                }
            }

            Debug.LogError(string.Format("检测步数{0}", maxFallSteps));
        }
        while (hasDropped && maxFallSteps > 0); // 持续检测直到没有下落或达到最大步数


        TimeTask createTime = new TimeTask()
        {
            id = 1,
            taskTime = 0.3f,
            onCompleted = this.CreateNewBlock
        };
        TimeTaskSystem.Instance.AddTimeTask(createTime);

    }

    public int BlockDropPos(Block block, Dictionary<int, int> stageMap)
    {
        int resY = -1;
        int MinY = 0;
        int MaxY = GlobalGameConfig.GridWidth;
        foreach (var kvp in stageMap)
        {
            int key = kvp.Key;   // 阶段的起始值
            int value = kvp.Value; // 阶段的结束值

            // 检查是否满足 key < m_y < value
            if (block.m_y >= key && block.m_y <= value)
            {
                MinY = key;
                MaxY = value;
                break; // 找到后立即退出循环
            }
        }

        for (int i = MaxY; i >= MinY; --i)
        {
            Block checkBlock = GetBlockByIndex(block.m_x, i);
            if (checkBlock == null)
            {
                resY = i;
                break;
            }

        }

        return resY;

    }

    public void CreateNewBlock()
    {
        isOperation = true;
        for (int i = 0; i < GlobalGameConfig.GridWidth; i++)
        {
            int count = 0;
            Queue<GameObject> newItemQueue = new Queue<GameObject>();
            for (int j = 0; j < GlobalGameConfig.GridWidth; j++)
            {
                if (GetBlockByIndex(i, j) == null)
                {
                    GameObject block = this.GetSystem<IBasicPoolSystem>().PopByPoolIdType(PoolIdEnum.BlockPoolId);
                    block.GetComponent<Block>().UpdatePos(i, j, false);

                    int iEnum = Random.Range(0, 7);
                    block.GetComponent<Block>().SetBlockType((BlockType)iEnum);
                    block.SetActive(true);
                    mAllBlocks[i][j] = block;

                    newItemQueue.Enqueue(block);
                    count++;
                }
            }
        }

        MatchAllBlock();

    }

    private void RemoveMatchBlock()
    {
        for (int i = 0; i < matchList.Count; ++i)
        {
            var item = matchList[i];
            item.RemoveBlock();

        }

    }

    public void DelBlockByIndex(int indexX, int indexY)
    {
        mAllBlocks[indexX][indexY] = null;
    }


    private void ClearData()
    {
        this.matchList.Clear();
        this.sameItemsList.Clear();
    }


    private bool CheckItemsInterval(bool isHorizontal, Block begin, Block end)
    {
        //获取图案
        BlockType type = begin.BlockType;
        //如果是横向
        if (isHorizontal)
        {
            //起点终点列号
            int beginIndex = begin.m_y;
            int endIndex = end.m_y;
            //如果起点在右，交换起点终点列号
            if (beginIndex > endIndex)
            {
                beginIndex = end.m_y;
                endIndex = begin.m_y;
            }
            //遍历中间的Item
            for (int i = beginIndex + 1; i < endIndex; i++)
            {
                Block checkBlock = GetBlockByIndex(begin.m_x, i);
                //异常处理（中间未生成，标识为不合法）
                if (checkBlock == null)
                    return false;
                //冻结无法交换
                if (checkBlock.BlockState == BlockState.Freeze)
                    return false;
                //如果中间有间隙（有图案不一致的）
                if (checkBlock.BlockType != type)
                {
                    return false;
                }
            }
            return true;
        }
        else
        {
            //起点终点行号
            int beginIndex = begin.m_x;
            int endIndex = end.m_x;
            //如果起点在上，交换起点终点列号
            if (beginIndex > endIndex)
            {
                beginIndex = end.m_x;
                endIndex = begin.m_x;
            }
            //遍历中间的Item
            for (int i = beginIndex + 1; i < endIndex; i++)
            {
                Block checkBlock = GetBlockByIndex(i, begin.m_y);
                if (checkBlock == null)
                    return false;
                if (checkBlock.BlockState == BlockState.Freeze)
                    return false;

                //如果中间有间隙（有图案不一致的）
                if (checkBlock.BlockType != type)
                {
                    return false;
                }
            }
            return true;
        }
    }

    public Block GetBlockByIndex(int indexX, int indexY)
    {
        Block res = null;
        if (indexX < GlobalGameConfig.GridWidth && indexX >= 0 && indexY >= 0 && indexY < GlobalGameConfig.GridWidth)
        {
            if (mAllBlocks[indexX][indexY] != null)
            {
                res = mAllBlocks[indexX][indexY].GetComponent<Block>();
            }
        }
        return res;
    }

    public Block[] GetArounBlocks(Block currentBlock)
    {
        Block[] result = new Block[]{
            GetUpItem(currentBlock),GetDownItem(currentBlock),
            GetLeftItem(currentBlock),GetRightItem(currentBlock)};

        return result;
    }

    private Block GetUpItem(Block block)
    {
        int row = block.m_x;
        int column = block.m_y - 1;
        return GetBlockByIndex(row, column);
    }


    private Block GetDownItem(Block block)
    {
        int row = block.m_x;
        int column = block.m_y + 1;
        return GetBlockByIndex(row, column);
    }

    private bool CheckItemDrop(int x, int y)
    {
        bool res = false;
        int row = x;
        int column = y + 1;
        if (column >= GlobalGameConfig.GridHeight)
        {
            res = false;
            return res;
        }
        var Downblock =mAllBlocks[row][column];
        if (Downblock == null)
        {
            res = true;
        }
        return res;
    }

    private Block GetLeftItem(Block block)
    {
        int row = block.m_x - 1;
        int column = block.m_y;
        return GetBlockByIndex(row, column);
    }
    private Block GetRightItem(Block block)
    {
        int row = block.m_x + 1;
        int column = block.m_y;
        return GetBlockByIndex(row, column);
    }



}