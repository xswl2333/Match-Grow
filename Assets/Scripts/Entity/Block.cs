using DG.Tweening;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class Block : Entity<Block>
{
    public BlockType BlockType { get; private set; }
    public BlockState BlockState { get; private set; }
    public int m_x { get; private set; }
    public int m_y { get; private set; }
    public GameObject m_BlockBg;
    public GameObject m_SelectBg;
    public bool hasCheck = false;

    #region UI显示相关
    private ResLoader mResLoader;
    private bool isOver;
    private bool isShow;
    #endregion

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
        mResLoader = ResLoader.Allocate();

    }


    //public void OnMouseEnter()
    //{
    //    isOver = true;
    //    print("进入");
    //}

    ////当鼠标离开UI
    //public void OnMouseExit()
    //{
    //    print("移出");
    //    isOver = false;
    //}

    //public void Update()
    //{
    //    if (isOver && this.BlockState == BlockState.Slime && !isShow)
    //    {
    //        //SlimeView panel = (SlimeView)UIManager.Instance.OpenPanel(UIConst.SlimeView);
    //        //panel.SetSlimeData(this.m_slime.m_data);
    //        isShow = true;
    //    }
    //    else if (!isOver && isShow)
    //    {
    //        //UIManager.Instance.ClosePanel(UIConst.SlimeView);
    //        isShow = false;
    //    }

    //}

    public void Create(BlockType BlockType, int x, int y, BlockState state = BlockState.Empty)
    {
        this.SetBlockType(BlockType);
        this.SetBlockPos(x, y);
        this.SetBlockState(state);
    }

    public void SetBlockType(BlockType BlockType)
    {
        this.BlockType = BlockType;
        this.SetColor();
    }

    public void SetBlockState(BlockState state)
    {
        this.BlockState = state;
        if (this.BlockState == BlockState.Freeze)
        {
            Material mat = mResLoader.LoadSync<Material>("Ice");
            this.m_BlockBg.GetComponent<Image>().material = mat;
        }
    }

    public void SetBlockPos(int x, int y)
    {
        this.m_x = x;
        this.m_y = y;
    }

    public void GetBlockPos(ref int x, ref int y)
    {
        x = this.m_x;
        y = this.m_y;
    }

    private void SetColor()
    {
        switch (this.BlockType)
        {
            case BlockType.Red:
                m_BlockBg.GetComponent<Image>().color = Color.red;
                break;
            case BlockType.Green:
                m_BlockBg.GetComponent<Image>().color = Color.green;
                break;
            case BlockType.Blue:
                m_BlockBg.GetComponent<Image>().color = Color.blue;
                break;
            case BlockType.Yellow:
                m_BlockBg.GetComponent<Image>().color = Color.yellow;
                break;
            case BlockType.Orange:
                m_BlockBg.GetComponent<Image>().color = new Color32(191, 117, 26, 255);
                break;
            case BlockType.Purple:
                m_BlockBg.GetComponent<Image>().color = new Color32(174, 26, 190, 255);
                break;
            case BlockType.Pink:
                m_BlockBg.GetComponent<Image>().color = new Color32(188, 68, 110, 255);
                break;
            default:
                break;
        }

    }

    public void SetBg(bool isSelect)
    {
        if (isSelect)
        {
            m_SelectBg.GetComponent<Image>().color = Color.red;
        }
        else
        {
            m_SelectBg.GetComponent<Image>().color = Color.white;
        }

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="targetPosX"></param>
    /// <param name="targetPoxY"></param>
    /// <param name="dotween"></param>
    /// <param name="fallTime"></param>
    public void UpdatePos(int targetPosX, int targetPoxY, bool dotween = false,float fallTime = 0.3f)
    {

        this.m_x = targetPosX;
        this.m_y = targetPoxY;

        Vector3 targetPos = CalculationTool.BlockCoverPos(this.m_x, this.m_y);
        if (dotween)
        {
            transform.DOLocalMove(targetPos, fallTime);
        }
        else
        {
            transform.localPosition = targetPos;
        }

        if (this.BlockState == BlockState.Freeze)
        {
            //this.m_slime.MoveTarget(this.m_x, this.m_y);
        }
    }

    public void OnClick()
    {
        if (this.BlockState == BlockState.Freeze)
        {
            transform.DOShakePosition(
                duration: 0.2f,          // 持续时间
                strength: new Vector3(0.1f, 0, 0), // 只在X和Y轴晃动
                vibrato: 50,           // 震动次数
                randomness: 90,      // 随机性
                fadeOut: true
                );
            return;
        }
        this.SendCommand(new ClickBlockCommand(this));
        Debug.Log($"m_x{m_x}**m_y{m_y}");

    }

    public void CheckAroundBoom()
    {
        this.SendCommand(new CheckMatchCommand(this));
    }

    public void RemoveBlock()
    {
        switch (this.BlockState)
        {
            case BlockState.Empty:
                break;
            case BlockState.Freeze:
                //this.m_slime.Destroy();
                this.RemoveSlime();
                break;
        }

        this.GetSystem<IBasicPoolSystem>().PushByPoolIdType(this.gameObject, PoolIdEnum.BlockPoolId);
    }

    public void DestroyBlock()
    {
        Destroy(this.gameObject);
    }

    public GameObject GetGameObject()
    {
        return this.gameObject;
    }

    //public void SetSlime(Slime slime)
    //{
    //    m_slime = slime;
    //    this.BlockState = BlockState.Slime;
    //}

    //移除格子上的slime
    public void RemoveSlime()
    {
        //this.m_slime = null;
        this.BlockState = BlockState.Empty;
    }
}
