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
        // ���ػ򴴽� GameConfig
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

        // ��ӿ��ؿؼ�
        _globalGameConfig.EditorModel = GUILayout.Toggle(_globalGameConfig.EditorModel, "���õ���ģʽ");

        // ���ݿ���״̬��ʾ��ͬ������
        if (_globalGameConfig.EditorModel)
        {
            GUILayout.Label("����ģʽ������", EditorStyles.boldLabel);
        }
        else
        {
            GUILayout.Label("����ģʽ�ѽ���", EditorStyles.boldLabel);
        }

        // �����޸�
        if (GUI.changed)
        {
            EditorUtility.SetDirty(_globalGameConfig);
        }
    }
}