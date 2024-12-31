using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GlobalConfig
{
    //格子距离
    public const float GapWidth = 1.5f;
    //格子数量
    public const int GridWidth = 6;
    //格子偏移距离
    public const float offHeight = 0.5f;
    //冰冻格子上限
    public const int TestFreezeBlock = 6;

    public static bool GIsPlay
    {
        get { return GIsPlay; }
        set { GIsPlay = value; }
    }


}
