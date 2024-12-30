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
                Vector3 localPos = this.parentTransform.TransformPoint(pos);//ת���������
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

    //����λ��
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
            res = 2;//��Χ���Ӳ�����
            return res;
        }
        if (!CheckBlockState(lastBlock, currentBlock))
        {
            res = 3;//��������slime
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
            taskTime = 1.0f,
            onCompleted = this.BlocksDrop
        };
        TimeTaskSystem.Instance.AddTimeTask(dropTime);

    }


    private void BlocksDrop()
    {
        this.matchState = MatchState.BlocksDrop;

        isOperation = true;
        //���м��
        for (int i = 0; i < GlobalConfig.GridWidth; i++)
        {
            //������
            int count = 0;
            //�������
            Queue<Block> dropQueue = new Queue<Block>();
            //���м��
            for (int j = GlobalConfig.GridWidth; j >= 0; --j)
            {
                if (GetBlockByIndex(i, j) != null)
                {
                    //����
                    count++;
                    //�������
                    dropQueue.Enqueue(mAllBlocks[i][j].GetComponent<Block>());
                }
            }

            if (count == GlobalConfig.GridWidth)
            {
                dropQueue.Clear();
                continue;
            }

            //����
            for (int k = 0; k < count; k++)
            {
                //��ȡҪ�����Item
                Block current = dropQueue.Dequeue();
                //�޸�ȫ������(ԭλ�����)
                DelBlockByIndex(current.m_x, current.m_y);
                //����
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
        //��ȡͼ��
        BlockEnum type = begin.BlockType;
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
                //�쳣�����м�δ���ɣ���ʶΪ���Ϸ���
                if (GetBlockByIndex(begin.m_x, i) == null)
                    return false;
                //����м��м�϶����ͼ����һ�µģ�
                if (GetBlockByIndex(begin.m_x, i).BlockType != type)
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
                //����м��м�϶����ͼ����һ�µģ�
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