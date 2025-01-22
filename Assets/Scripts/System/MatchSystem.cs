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
                    Vector3 localPos = this.parentTransform.TransformPoint(pos);//ת���������
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
                    Vector3 localPos = this.parentTransform.TransformPoint(pos);//ת���������
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
        //������
        bool hasBoom = false;

        foreach (var blockList in mAllBlocks)
        {
            for (int i = 0; i < blockList.Count; i++) //foreach����,Ԫ��Ϊ��,����
            {
                var blockObject = blockList[i];
                if (blockObject)
                {
                    Block block = blockObject.GetComponent<Block>();

                    //ָ��λ�õ�Item���ڣ���û�б�����
                    if (!block.hasCheck)
                    {
                        //�����Χ������
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
            //��������
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

    //����λ��
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
            res = 2;//�������ڸ���
            return res;
        }
        //if (!CheckBlockState(lastBlock, currentBlock))
        //{
        //    res = 3;//��������slime
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

        //������
        int rowCount = 0;
        int columnCount = 0;
        //��ʱ�б�
        List<Block> rowTempList = new List<Block>();
        List<Block> columnTempList = new List<Block>();

        ///����������
        foreach (var item in sameItemsList)
        {

            //�����ͬһ��
            if (item.m_x == currentBlock.m_x)
            {
                //�жϸõ���Curren�м����޼�϶
                bool rowCanBoom = CheckItemsInterval(true, currentBlock, item);
                if (rowCanBoom)
                {
                    //����
                    rowCount++;
                    //��ӵ�����ʱ�б�
                    rowTempList.Add(item);
                }
            }
            //�����ͬһ��
            if (item.m_y == currentBlock.m_y)
            {
                //�жϸõ���Curren�м����޼�϶
                bool columnCanBoom = CheckItemsInterval(false, currentBlock, item);
                if (columnCanBoom)
                {
                    //����
                    columnCount++;
                    //��ӵ�����ʱ�б�
                    columnTempList.Add(item);
                }
            }
        }
        //��������
        bool horizontalBoom = false;
        //���������������
        if (rowCount > 2)
        {
            //����ʱ�б��е�Itemȫ������BoomList
            matchList.AddRange(rowTempList);
            //��������
            horizontalBoom = true;
        }
        //���������������
        if (columnCount > 2)
        {
            if (horizontalBoom)
            {
                //�޳��Լ�
                matchList.Remove(currentBlock);
            }
            //����ʱ�б��е�Itemȫ������BoomList
            matchList.AddRange(columnTempList);
        }
        //���û���������󣬷���
        if (matchList.Count == 0)
            return;
        //������ʱ��BoomList
        List<Block> tempBoomList = new List<Block>();
        //ת�Ƶ���ʱ�б�
        tempBoomList.AddRange(matchList);
        HandleMatchList(tempBoomList);
    }


    private void CheckSameBlockList(Block currentBlock)
    {
        //����Ѵ��ڣ�����
        if (sameItemsList.Contains(currentBlock))
            return;
        if (currentBlock.BlockState == BlockState.Freeze)
            return;

        //��ӵ��б�
        sameItemsList.Add(currentBlock);
        //�������ҵ�Item
        Block[] tempItemList = GetArounBlocks(currentBlock);

        for (int i = 0; i < tempItemList.Length; i++)
        {
            //���Item���Ϸ�������
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
            ////�뿪����
            //AudioManager.instance.PlayMagicalAudio();
            //����������Item��ȫ���б����Ƴ�
            DelBlockByIndex(block.m_x, block.m_y);
            totalBlockAmount--;
        }
        //���Item�Ƿ��Ѿ����������뿪����
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
        //TODO ȱ�ټ��ܵ����
        this.GetModel<IGameModel>().SkillPoint.Value++;
        this.matchState = MatchState.BlocksDrop;
        isOperation = true;

        bool hasDropped;
        int maxFallSteps = totalBlockAmount ;//�����ƶ��Ĳ���

        do
        {
            hasDropped = false;
            maxFallSteps--; // �ؼ��޸����ݼ�������

            // �ӵ����ڶ��п�ʼ��⣨��ײ㲻��Ҫ��⣩
            for (int y = GlobalGameConfig.GridHeight - 1; y >= 0; y--)
            {
                // ���м��
                for (int x = 0; x < GlobalGameConfig.GridWidth; x++)
                {
                    Block currentBlock = GetBlockByIndex(x, y);
                    if (currentBlock == null) continue;
                    if (currentBlock.BlockState == BlockState.Freeze) continue;

                    bool res = CheckItemDrop(x, y);
                    // ����·��Ƿ�Ϊ��
                    if (res)
                    {
                        // ִ�е�������
                        DelBlockByIndex(currentBlock.m_x, currentBlock.m_y);
                        currentBlock.UpdatePos(currentBlock.m_x, y + 1, true);
                        mAllBlocks[currentBlock.m_x][currentBlock.m_y] = currentBlock.GetGameObject();
                        hasDropped = true;
                    }
                }
            }

            Debug.LogError(string.Format("��ⲽ��{0}", maxFallSteps));
        }
        while (hasDropped && maxFallSteps > 0); // �������ֱ��û�������ﵽ�����


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
            int key = kvp.Key;   // �׶ε���ʼֵ
            int value = kvp.Value; // �׶εĽ���ֵ

            // ����Ƿ����� key < m_y < value
            if (block.m_y >= key && block.m_y <= value)
            {
                MinY = key;
                MaxY = value;
                break; // �ҵ��������˳�ѭ��
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
        //��ȡͼ��
        BlockType type = begin.BlockType;
        //����Ǻ���
        if (isHorizontal)
        {
            //����յ��к�
            int beginIndex = begin.m_y;
            int endIndex = end.m_y;
            //���������ң���������յ��к�
            if (beginIndex > endIndex)
            {
                beginIndex = end.m_y;
                endIndex = begin.m_y;
            }
            //�����м��Item
            for (int i = beginIndex + 1; i < endIndex; i++)
            {
                Block checkBlock = GetBlockByIndex(begin.m_x, i);
                //�쳣�����м�δ���ɣ���ʶΪ���Ϸ���
                if (checkBlock == null)
                    return false;
                //�����޷�����
                if (checkBlock.BlockState == BlockState.Freeze)
                    return false;
                //����м��м�϶����ͼ����һ�µģ�
                if (checkBlock.BlockType != type)
                {
                    return false;
                }
            }
            return true;
        }
        else
        {
            //����յ��к�
            int beginIndex = begin.m_x;
            int endIndex = end.m_x;
            //���������ϣ���������յ��к�
            if (beginIndex > endIndex)
            {
                beginIndex = end.m_x;
                endIndex = begin.m_x;
            }
            //�����м��Item
            for (int i = beginIndex + 1; i < endIndex; i++)
            {
                Block checkBlock = GetBlockByIndex(i, begin.m_y);
                if (checkBlock == null)
                    return false;
                if (checkBlock.BlockState == BlockState.Freeze)
                    return false;

                //����м��м�϶����ͼ����һ�µģ�
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