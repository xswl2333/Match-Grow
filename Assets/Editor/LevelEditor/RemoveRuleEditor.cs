using UnityEditor;
using UnityEngine;
using MessagePack;
using System.Collections.Generic;
using System.IO;

public class RemoveRuleEditor : EditorWindow
{
    private Dictionary<RemoveBlockRuleEnum, BlockType> ruleBlockTypeMap = new Dictionary<RemoveBlockRuleEnum, BlockType>();

    [MenuItem("Tools/Remove Rule Editor")]
    public static void ShowWindow()
    {
        GetWindow<RemoveRuleEditor>("Remove Rule Editor");
    }

    private void OnEnable()
    {
        // ��ʼ��Ĭ��ֵ
        ruleBlockTypeMap[RemoveBlockRuleEnum.Normal] = BlockType.Red;
        ruleBlockTypeMap[RemoveBlockRuleEnum.Same] = BlockType.Blue;
    }

    private void OnGUI()
    {
        // ����ö�ٺ� BlockType ��
        foreach (var rule in System.Enum.GetValues(typeof(RemoveBlockRuleEnum)))
        {
            RemoveBlockRuleEnum ruleEnum = (RemoveBlockRuleEnum)rule;
            if (!ruleBlockTypeMap.ContainsKey(ruleEnum))
            {
                ruleBlockTypeMap[ruleEnum] = BlockType.Red; // Ĭ�� BlockType
            }

            ruleBlockTypeMap[ruleEnum] = (BlockType)EditorGUILayout.EnumPopup(ruleEnum.ToString(), ruleBlockTypeMap[ruleEnum]);
        }

        // ���水ť
        if (GUILayout.Button("��������"))
        {
            string saveFilePath = EditorUtility.SaveFilePanel("Save Remove Rules", "", "RemoveRules", "msgpack");
            if (!string.IsNullOrEmpty(saveFilePath))
            {
                SaveRules(saveFilePath);
            }
        }

        // ���ذ�ť
        if (GUILayout.Button("��������"))
        {
            string loadFilePath = EditorUtility.OpenFilePanel("Load Remove Rules", "", "msgpack");
            if (!string.IsNullOrEmpty(loadFilePath))
            {
                LoadRules(loadFilePath);
            }
        }
    }

    private void SaveRules(string filePath)
    {
        // �������ݶ���
        RemoveRuleData data = new RemoveRuleData
        {
            RuleBlockTypeMap = ruleBlockTypeMap
        };

        // ���л�Ϊ����������
        byte[] serializedData = MessagePackSerializer.Serialize(data);

        // ���浽�ļ�
        File.WriteAllBytes(filePath, serializedData);

        Debug.Log($"Remove rules saved to {filePath}");
    }

    private void LoadRules(string filePath)
    {
        // ���ļ���ȡ����������
        byte[] data = File.ReadAllBytes(filePath);

        // �����л�Ϊ���ݶ���
        RemoveRuleData loadedData = MessagePackSerializer.Deserialize<RemoveRuleData>(data);

        // ���µ�ǰ��
        ruleBlockTypeMap = loadedData.RuleBlockTypeMap;

        Debug.Log($"Remove rules loaded from {filePath}");
    }
}