using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GlobalConfig
{
    //���Ӿ���
    public const float GapWidth = 1.5f;
    //��������
    public const int GridWidth = 6;
    //����ƫ�ƾ���
    public const float offHeight = 0.5f;
    //������������
    public const int TestFreezeBlock = 6;

    public static bool GIsPlay
    {
        get { return GIsPlay; }
        set { GIsPlay = value; }
    }


}
