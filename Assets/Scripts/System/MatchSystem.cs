using Markdig.Syntax;
using QFramework;
using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using System;
using Random = UnityEngine.Random;
using System.Collections;
using Unity.VisualScripting;

public interface IMatchSystem : ISystem
{
    List<List<GameObject>> AllBlocks { get; }

    void GenerateBlocks(int widths, int heights) { }
    void SetParentObject(Transform parentTransform) { }
    void CheckBlock(Block block) { }

    void ExchangeBlock(GameObject block) { }
}

public class MatchSystem : AbstractSystem, IMatchSystem
{
    private Transform parentTransform;
    private ResLoader mResLoader;
    public List<List<GameObject>> AllBlocks => mAllBlocks;
    private List<List<GameObject>> mAllBlocks = new List<List<GameObject>>();

    private GameObject currentSelectObj;
    private Queue<GameObject> selectQueObj = new Queue<GameObject>();

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
        
        for (int i = 0; i < widths; i++)
        {
            List<GameObject> blocks = new List<GameObject>();
            for (int j = 0; j < heights; j++)
            {
                Vector3 pos = CalculationTool.BlockCoverPos(i, j); 
                Vector3 localPos = this.parentTransform.TransformPoint(pos);//转换相对坐标
                GameObject block = mResLoader.LoadSync<GameObject>("Block").Instantiate(localPos, Quaternion.identity, this.parentTransform);

                int iEnum = Random.Range(0, 7);
                block.GetComponent<Block>().Create((BlockEnum)iEnum, i, j);

                blocks.Add(block);
            }
            mAllBlocks.Add(blocks);
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


    public void ExchangeBlock(GameObject clickObj)
    {
        //SoundManager.Instance.PlaySound(SoundsConfig.Click1);
        currentSelectObj = clickObj;
        currentSelectObj.GetComponent<Block>().SetBg(true);
        selectQueObj.Enqueue(currentSelectObj);

        if (selectQueObj.Count > 1)
        {
            GameObject lastObj = selectQueObj.Peek();
            lastObj.GetComponent<Block>().SetBg(false);

            HandleExchange();

        }
    }


    private void HandleExchange()
    {
        int result = this.Exchange();
        if (result == 2)
        {
            var currentBlock = currentSelectObj.GetComponent<Block>();
            currentBlock.SetBg(false);
            selectQueObj.Clear();


        }
        else if (result == 3)
        {
            selectQueObj.Dequeue();
        }

        //yield return new WaitForSeconds(0.3f);

        var block = currentSelectObj.GetComponent<Block>();
        block.CheckAroundBoom();

        GameObject lastObj = selectQueObj.Peek();
        block = lastObj.GetComponent<Block>();
        block.CheckAroundBoom();

        selectQueObj.Clear();

    }

    //交换位置
    private int Exchange()
    {
        int res = 1;
        int firstPosX = 0, firstPosY = 0;
        int secondPosX = 0, secondPosY = 0;

        GameObject lastObj = selectQueObj.Peek();
        var lastBlock = lastObj.GetComponent<Block>();
        var currentBlock = currentSelectObj.GetComponent<Block>();

        if (!CheckAroundBlock(lastBlock, currentBlock))
        {
            res = 2;//周围格子不相邻
            return res;
        }
        if (!CheckBlockState(lastBlock, currentBlock))
        {
            res = 3;//格子上有slime
            return res;
        }

        lastBlock.GetBlockPos(ref firstPosX, ref firstPosY);
        currentSelectObj.GetComponent<Block>().GetBlockPos(ref secondPosX, ref secondPosY);

        lastBlock.UpdatePos(secondPosX, secondPosY, false);
        currentSelectObj.GetComponent<Block>().UpdatePos(firstPosX, firstPosY, false);
        lastBlock.SetBg(false);
        currentSelectObj.GetComponent<Block>().SetBg(false);

        mAllBlocks[firstPosX][firstPosY] = currentSelectObj;
        mAllBlocks[secondPosX][secondPosY] = lastObj;

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

    public bool CheckBlockState(Block currentBlock, Block targetBlock)
    {
        bool res = true;
        if (currentBlock.BlockState == BlockState.Slime || targetBlock.BlockState == BlockState.Slime)
        {
            res = false;
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
            taskTime = 1.0f,
            onCompleted = this.BlocksDrop
        };
        TimeTaskSystem.Instance.AddTimeTask(dropTime);

    }


    private void BlocksDrop()
    {
        this.matchState = MatchState.BlocksDrop;

        isOperation = true;
        //逐列检测
        for (int i = 0; i < GlobalConfig.GridWidth; i++)
        {
            //计数器
            int count = 0;
            //下落队列
            Queue<Block> dropQueue = new Queue<Block>();
            //逐行检测
            for (int j = GlobalConfig.GridWidth; j >= 0; --j)
            {
                if (GetBlockByIndex(i, j) != null)
                {
                    //计数
                    count++;
                    //放入队列
                    dropQueue.Enqueue(mAllBlocks[i][j].GetComponent<Block>());
                }
            }

            if (count == GlobalConfig.GridWidth)
            {
                dropQueue.Clear();
                continue;
            }

            //下落
            for (int k = 0; k < count; k++)
            {
                //获取要下落的Item
                Block current = dropQueue.Dequeue();
                //修改全局数组(原位置情况)
                DelBlockByIndex(current.m_x, current.m_y);
                //下落
                current.UpdatePos(current.m_x, GlobalConfig.GridWidth - k - 1, true);
                mAllBlocks[current.m_x][current.m_y] = current.GetGameObject();
            }
        }

        TimeTask createTime = new TimeTask()
        {
            id = 1,
            taskTime = 0.5f,
            onCompleted = this.CreateNewBlock
        };
        TimeTaskSystem.Instance.AddTimeTask(createTime);

    }


    public void CreateNewBlock()
    {
        isOperation = true;
        for (int i = 0; i < GlobalConfig.GridWidth; i++)
        {
            int count = 0;
            Queue<GameObject> newItemQueue = new Queue<GameObject>();
            for (int j = 0; j < GlobalConfig.GridWidth; j++)
            {
                if (GetBlockByIndex(i, j) == null)
                {
                    GameObject block = this.GetSystem<IBasicPoolSystem>().PopByPoolIdType(PoolIdEnum.BlockPoolId);
                    block.GetComponent<Block>().UpdatePos(i, j, false);
                    
                    int iEnum = Random.Range(0, 7);
                    block.GetComponent<Block>().SetBlockType((BlockEnum)iEnum);
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
        BlockEnum type = begin.BlockType;
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
                //异常处理（中间未生成，标识为不合法）
                if (GetBlockByIndex(begin.m_x, i) == null)
                    return false;
                //如果中间有间隙（有图案不一致的）
                if (GetBlockByIndex(begin.m_x, i).BlockType != type)
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
                //如果中间有间隙（有图案不一致的）
                if (GetBlockByIndex(i, begin.m_y).BlockType != type)
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
        if (indexX < GlobalConfig.GridWidth && indexX >= 0 && indexY >= 0 && indexY < GlobalConfig.GridWidth)
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