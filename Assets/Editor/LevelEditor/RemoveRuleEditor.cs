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
        // 初始化默认值
        ruleBlockTypeMap[RemoveBlockRuleEnum.Normal] = BlockType.Red;
        ruleBlockTypeMap[RemoveBlockRuleEnum.Same] = BlockType.Blue;
    }

    private void OnGUI()
    {
        // 绘制枚举和 BlockType 绑定
        foreach (var rule in System.Enum.GetValues(typeof(RemoveBlockRuleEnum)))
        {
            RemoveBlockRuleEnum ruleEnum = (RemoveBlockRuleEnum)rule;
            if (!ruleBlockTypeMap.ContainsKey(ruleEnum))
            {
                ruleBlockTypeMap[ruleEnum] = BlockType.Red; // 默认 BlockType
            }

            ruleBlockTypeMap[ruleEnum] = (BlockType)EditorGUILayout.EnumPopup(ruleEnum.ToString(), ruleBlockTypeMap[ruleEnum]);
        }

        // 保存按钮
        if (GUILayout.Button("保存配置"))
        {
            string saveFilePath = EditorUtility.SaveFilePanel("Save Remove Rules", "", "RemoveRules", "msgpack");
            if (!string.IsNullOrEmpty(saveFilePath))
            {
                SaveRules(saveFilePath);
            }
        }

        // 加载按钮
        if (GUILayout.Button("加载配置"))
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
        // 创建数据对象
        RemoveRuleData data = new RemoveRuleData
        {
            RuleBlockTypeMap = ruleBlockTypeMap
        };

        // 序列化为二进制数据
        byte[] serializedData = MessagePackSerializer.Serialize(data);

        // 保存到文件
        File.WriteAllBytes(filePath, serializedData);

        Debug.Log($"Remove rules saved to {filePath}");
    }

    private void LoadRules(string filePath)
    {
        // 从文件读取二进制数据
        byte[] data = File.ReadAllBytes(filePath);

        // 反序列化为数据对象
        RemoveRuleData loadedData = MessagePackSerializer.Deserialize<RemoveRuleData>(data);

        // 更新当前绑定
        ruleBlockTypeMap = loadedData.RuleBlockTypeMap;

        Debug.Log($"Remove rules loaded from {filePath}");
    }
}