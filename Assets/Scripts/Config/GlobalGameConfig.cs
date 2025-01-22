using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "GlobalGameConfig", menuName = "Assets/Resources/Data")]
public class GlobalGameConfig : ScriptableObject
{
    //格子距离
    public const float GapWidth = 1.5f;
    //格子数量
    public const int GridWidth = 6;
    public const int GridHeight = 6;
    //格子偏移距离
    public const float offHeight = 0.5f;

    public bool EditorModel = false;


}
