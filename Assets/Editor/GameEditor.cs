using UnityEditor;
using UnityEngine;

public class GameEditor : EditorWindow
{
    private GlobalGameConfig _globalGameConfig;

    [MenuItem("Tools/GameDebug")]
    public static void ShowWindow()
    {
        GetWindow<GameEditor>("GameDebug");
    }

    private void OnEnable()
    {
        // 加载或创建 GameConfig
        _globalGameConfig = AssetDatabase.LoadAssetAtPath<GlobalGameConfig>("Assets/Resources/Data/GlobalGameConfig.asset");
        if (_globalGameConfig == null)
        {
            _globalGameConfig = CreateInstance<GlobalGameConfig>();
            AssetDatabase.CreateAsset(_globalGameConfig, "Assets/Resources/Data/GlobalGameConfig.asset");
            AssetDatabase.SaveAssets();
        }
    }

    private void OnGUI()
    {
        if (_globalGameConfig == null) return;

        // 添加开关控件
        _globalGameConfig.EditorModel = GUILayout.Toggle(_globalGameConfig.EditorModel, "启用调试模式");

        // 根据开关状态显示不同的内容
        if (_globalGameConfig.EditorModel)
        {
            GUILayout.Label("调试模式已启用", EditorStyles.boldLabel);
        }
        else
        {
            GUILayout.Label("调试模式已禁用", EditorStyles.boldLabel);
        }

        // 保存修改
        if (GUI.changed)
        {
            EditorUtility.SetDirty(_globalGameConfig);
        }
    }
}