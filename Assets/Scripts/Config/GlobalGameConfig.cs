using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "GlobalGameConfig", menuName = "Assets/Resources/Data")]
public class GlobalGameConfig : ScriptableObject
{
    //���Ӿ���
    public const float GapWidth = 1.5f;
    //��������
    public const int GridWidth = 6;
    public const int GridHeight = 6;
    //����ƫ�ƾ���
    public const float offHeight = 0.5f;

    public bool EditorModel = false;


}
