using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;


[CustomEditor(typeof(UITable))]
internal sealed class UITableEditor : Editor
{
    private SerializedProperty style;//可以用于反射一个Unity对象的字段

    private SerializedProperty loaderList;//可以用于反射一个Unity对象的字段

    private SerializedProperty prefabToggle;

    private ReorderableList sort;//支持排序

    private ReorderableList loaderSort;

    private string search;//要搜索的

    private UITable.BindPair[] binds;

    private HashSet<int> find = new HashSet<int>();

    private Dictionary<string, int> nameDic = new Dictionary<string, int>(System.StringComparer.Ordinal);

    private Dictionary<Object, int> weightDic = new Dictionary<Object, int>();

    private GameObject gameObj;

    private Object obj;

    private void OnEnable()
    {
        if (this.target == null)
        {
            return;
        }
        SerializedObject serializedObject = base.serializedObject;
        this.style = serializedObject.FindProperty("binds");//按名称查找序列化属性
        this.loaderList = serializedObject.FindProperty("loaderList");
        this.prefabToggle = serializedObject.FindProperty("prefabToggle");
        this.sort = new ReorderableList(serializedObject, this.style);
        this.sort.drawHeaderCallback = new ReorderableList.HeaderCallbackDelegate(this.fixedInspector);//列表头
        this.sort.elementHeight = EditorGUIUtility.singleLineHeight;
        this.sort.drawElementCallback = new ReorderableList.ElementCallbackDelegate(this.fixedInspector);//列表中间
        this.loaderSort = new ReorderableList(serializedObject, this.loaderList);
        this.loaderSort.drawHeaderCallback = new ReorderableList.HeaderCallbackDelegate(this.InspectorText);//列表头
        this.loaderSort.elementHeight = EditorGUIUtility.singleLineHeight;
        this.loaderSort.drawElementCallback = new ReorderableList.ElementCallbackDelegate(this.InspectorText);//列表中间
        this.fixedInspector();
    }
    /// <summary>
    /// 根据 UI Table 显示列表
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="num"></param>
    /// <param name="flag"></param>
    /// <param name="flag2"></param>
    private void fixedInspector(Rect rect, int num, bool flag, bool flag2)
    {
        this.fixedInspector(this.style, rect, num, flag, flag2);
    }
    /// <summary>
    /// 获取重复列表
    /// </summary>
    private void fixedInspector()
    {
        this.find.Clear();
        this.nameDic.Clear();
        this.weightDic.Clear();
        for (int i = 0; i < this.style.arraySize; i++)
        {
            SerializedProperty arrayElementAtIndex = this.style.GetArrayElementAtIndex(i);
            SerializedProperty serializedProperty = arrayElementAtIndex.FindPropertyRelative("Name");//查找Name属性
            SerializedProperty serializedProperty2 = arrayElementAtIndex.FindPropertyRelative("Widget");//查找Widget属性
            Object objectReferenceValue = serializedProperty2.objectReferenceValue;
            if (this.nameDic.ContainsKey(serializedProperty.stringValue))
            {
                this.find.Add(this.nameDic[serializedProperty.stringValue]);
                this.find.Add(i);
            }
            else if (objectReferenceValue != null && this.weightDic.ContainsKey(objectReferenceValue))
            {
                this.find.Add(this.weightDic[objectReferenceValue]);
                this.find.Add(i);
            }
            else
            {
                this.nameDic.Add(serializedProperty.stringValue, i);
                if (objectReferenceValue != null)
                {
                    this.weightDic.Add(objectReferenceValue, i);
                }
            }
        }
    }
    /// <summary>
    /// 查找单个物体是否重复
    /// </summary>
    /// <param name="object"></param>
    /// <returns></returns>
    private bool fixedInspector(Object @object)
    {
        for (int i = 0; i < this.style.arraySize; i++)
        {
            SerializedProperty arrayElementAtIndex = this.style.GetArrayElementAtIndex(i);
            SerializedProperty serializedProperty = arrayElementAtIndex.FindPropertyRelative("Name");
            SerializedProperty serializedProperty2 = arrayElementAtIndex.FindPropertyRelative("Widget");
            if (serializedProperty2.objectReferenceValue == @object)
            {
                this.obj = @object;//添加重复的obj
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 字体显示
    /// </summary>
    /// <param name="rect"></param>
    private void fixedInspector(Rect rect)
    {
        Rect rect2 = new Rect(rect.x + 13f, rect.y, rect.width / 2f, EditorGUIUtility.singleLineHeight);
        Rect rect3 = new Rect(rect.x + 10f + rect.width / 2f, rect.y, rect.width / 2f, EditorGUIUtility.singleLineHeight);
        GUI.Label(rect2, "Name");
        GUI.Label(rect3, "Widget");
    }

    /// <summary>
    /// 可拖动列表中间显示
    /// </summary>
    /// <param name="serializedProperty"></param>
    /// <param name="rect"></param>
    /// <param name="num"></param>
    /// <param name="flag"></param>
    /// <param name="flag2"></param>
    private void fixedInspector(SerializedProperty serializedProperty, Rect rect, int num, bool flag, bool flag2)
    {
        SerializedProperty arrayElementAtIndex = serializedProperty.GetArrayElementAtIndex(num);
        bool flag3 = this.find.Contains(num);
        Color color = GUI.color;
        if (flag3)//同名（红色）
        {
            GUI.color = (new Color(1f, 0.5f, 0.5f, 1f));
        }
        SerializedProperty serializedProperty2 = arrayElementAtIndex.FindPropertyRelative("Name");
        SerializedProperty serializedProperty3 = arrayElementAtIndex.FindPropertyRelative("Widget");
        if (serializedProperty3.objectReferenceValue == this.obj)//重复的obj（蓝色）
        {
            GUI.color = (new Color(0.2f, 1f, 1f, 1f));
        }
        else if (serializedProperty3.objectReferenceValue == this.gameObj)//刚添加（绿色）
        {
            GUI.color = (new Color(0.5f, 1f, 0.5f, 1f));
        }
        Rect rect2 = new Rect(rect.x, rect.y, rect.width / 2f - 5f, EditorGUIUtility.singleLineHeight);
        Rect rect3 = new Rect(rect.x + rect.width / 2f + 5f, rect.y, rect.width / 2f - 5f, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(rect2, serializedProperty2, GUIContent.none);
        EditorGUI.PropertyField(rect3, serializedProperty3, GUIContent.none);
        GUI.color = (color);
    }
    /// <summary>
    /// 显示Sort列表
    /// </summary>
    private void InspectorText(Rect rect)
    {
        Rect rect2 = new Rect(rect.x + 13f, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
        GUI.Label(rect2, "Name");
    }

    /// <summary>
    /// 显示Sort列表
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="num"></param>
    /// <param name="flag"></param>
    /// <param name="flag2"></param>
    private void InspectorText(Rect rect, int num, bool flag, bool flag2)
    {
        this.InspectorText(this.loaderList, rect, num, flag, flag2);
    }

    /// <summary>
    /// 显示Sort列表
    /// </summary>
    /// <param name="rect"></param>
    /// <param name="num"></param>
    /// <param name="flag"></param>
    /// <param name="flag2"></param>
    private void InspectorText(SerializedProperty serializedProperty, Rect rect, int num, bool flag, bool flag2)
    {
        SerializedProperty arrayElementAtIndex = serializedProperty.GetArrayElementAtIndex(num);
        Color color = GUI.color;
        Rect rect2 = new Rect(rect.x, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
        EditorGUI.PropertyField(rect2, arrayElementAtIndex, GUIContent.none);
        GUI.color = (color);
    }

    //private void FlushPrefabLoader(UITable uINameTable)
    //{
    //    if (uINameTable.loaderList == null)
    //    {
    //        uINameTable.loaderList = new List<UIPrefabLoader>();
    //    }
    //    uINameTable.loaderList.Clear();
    //    var list = uINameTable.GetComponentsInChildren<UIPrefabLoader>(true);
    //    for (int i = 0; i < list.Length; i++)
    //    {
    //        uINameTable.loaderList.Add(list[i]);
    //    }
    //}
    /// <summary>
    /// Inspector面板显示
    /// </summary>
    public override void OnInspectorGUI()
    {
        base.serializedObject.Update();
        this.sort.DoLayoutList();
        //允许属性修改
        if (base.serializedObject.ApplyModifiedProperties())
        {
            this.fixedInspector();
        }
        UITable uINameTable = (UITable)this.target;
        GUILayout.BeginHorizontal(new GUILayoutOption[0]);
        this.gameObj = (EditorGUILayout.ObjectField(this.gameObj, typeof(GameObject), true, new GUILayoutOption[0]) as GameObject);
        if (GUILayout.Button("添加", new GUILayoutOption[0]))
        {
            if (this.gameObj == null)
            {
                return;
            }
            this.obj = null;//清空重复的obj
            if (this.fixedInspector(this.gameObj))
            {
                Debug.LogError("重复的 object");
                return;
            }
            string text = this.gameObj.name;
            int num = 0;
            while (uINameTable.Find(text))//重复命名尾数叠加
            {
                text += num;
                num++;
            }
            Undo.RecordObject(uINameTable, "添加到 Table");//在 Editor 记录撤销操作
            base.serializedObject.Update();
            uINameTable.Add(text, this.gameObj);
            base.serializedObject.ApplyModifiedProperties();
        }
        GUILayout.EndHorizontal();
        string text2 = EditorGUILayout.TextField("搜索:", this.search, new GUILayoutOption[0]);
        if (string.IsNullOrEmpty(text2))
        {
            this.search = null;
            this.binds = null;
        }
        else if (text2 != this.search)
        {
            this.search = text2;
            this.binds = uINameTable.Search(this.search);
        }
        if (this.binds != null)
        {
            GUI.enabled = (false);
            GUILayout.BeginVertical(GUI.skin.textArea, new GUILayoutOption[0]);
            UITable.BindPair[] array = this.binds;
            for (int i = 0; i < array.Length; i++)
            {
                UITable.BindPair bindPair = array[i];
                EditorGUILayout.BeginHorizontal(new GUILayoutOption[0]);
                EditorGUILayout.LabelField(bindPair.Name, new GUILayoutOption[0]);
                EditorGUILayout.ObjectField(bindPair.Widget, bindPair.Widget.GetType(), true, new GUILayoutOption[0]);
                EditorGUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
            GUI.enabled = (true);
        }
        if (GUILayout.Button("排序", new GUILayoutOption[0]))
        {
            Undo.RecordObject(uINameTable, "Sort Table");//在 Editor 记录撤销操作
            base.serializedObject.Update();
            uINameTable.Sort();
            base.serializedObject.ApplyModifiedProperties();
        }
    }
}
