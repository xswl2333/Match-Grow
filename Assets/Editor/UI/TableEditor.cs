using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;


[CustomEditor(typeof(UITable))]
internal sealed class UITableEditor : Editor
{
    private SerializedProperty style;//�������ڷ���һ��Unity������ֶ�

    private SerializedProperty loaderList;//�������ڷ���һ��Unity������ֶ�

    private SerializedProperty prefabToggle;

    private ReorderableList sort;//֧������

    private ReorderableList loaderSort;

    private string search;//Ҫ������

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
        this.style = serializedObject.FindProperty("binds");//�����Ʋ������л�����
        this.loaderList = serializedObject.FindProperty("loaderList");
        this.prefabToggle = serializedObject.FindProperty("prefabToggle");
        this.sort = new ReorderableList(serializedObject, this.style);
        this.sort.drawHeaderCallback = new ReorderableList.HeaderCallbackDelegate(this.fixedInspector);//�б�ͷ
        this.sort.elementHeight = EditorGUIUtility.singleLineHeight;
        this.sort.drawElementCallback = new ReorderableList.ElementCallbackDelegate(this.fixedInspector);//�б��м�
        this.loaderSort = new ReorderableList(serializedObject, this.loaderList);
        this.loaderSort.drawHeaderCallback = new ReorderableList.HeaderCallbackDelegate(this.InspectorText);//�б�ͷ
        this.loaderSort.elementHeight = EditorGUIUtility.singleLineHeight;
        this.loaderSort.drawElementCallback = new ReorderableList.ElementCallbackDelegate(this.InspectorText);//�б��м�
        this.fixedInspector();
    }
    /// <summary>
    /// ���� UI Table ��ʾ�б�
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
    /// ��ȡ�ظ��б�
    /// </summary>
    private void fixedInspector()
    {
        this.find.Clear();
        this.nameDic.Clear();
        this.weightDic.Clear();
        for (int i = 0; i < this.style.arraySize; i++)
        {
            SerializedProperty arrayElementAtIndex = this.style.GetArrayElementAtIndex(i);
            SerializedProperty serializedProperty = arrayElementAtIndex.FindPropertyRelative("Name");//����Name����
            SerializedProperty serializedProperty2 = arrayElementAtIndex.FindPropertyRelative("Widget");//����Widget����
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
    /// ���ҵ��������Ƿ��ظ�
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
                this.obj = @object;//����ظ���obj
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// ������ʾ
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
    /// ���϶��б��м���ʾ
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
        if (flag3)//ͬ������ɫ��
        {
            GUI.color = (new Color(1f, 0.5f, 0.5f, 1f));
        }
        SerializedProperty serializedProperty2 = arrayElementAtIndex.FindPropertyRelative("Name");
        SerializedProperty serializedProperty3 = arrayElementAtIndex.FindPropertyRelative("Widget");
        if (serializedProperty3.objectReferenceValue == this.obj)//�ظ���obj����ɫ��
        {
            GUI.color = (new Color(0.2f, 1f, 1f, 1f));
        }
        else if (serializedProperty3.objectReferenceValue == this.gameObj)//����ӣ���ɫ��
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
    /// ��ʾSort�б�
    /// </summary>
    private void InspectorText(Rect rect)
    {
        Rect rect2 = new Rect(rect.x + 13f, rect.y, rect.width, EditorGUIUtility.singleLineHeight);
        GUI.Label(rect2, "Name");
    }

    /// <summary>
    /// ��ʾSort�б�
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
    /// ��ʾSort�б�
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
    /// Inspector�����ʾ
    /// </summary>
    public override void OnInspectorGUI()
    {
        base.serializedObject.Update();
        this.sort.DoLayoutList();
        //���������޸�
        if (base.serializedObject.ApplyModifiedProperties())
        {
            this.fixedInspector();
        }
        UITable uINameTable = (UITable)this.target;
        GUILayout.BeginHorizontal(new GUILayoutOption[0]);
        this.gameObj = (EditorGUILayout.ObjectField(this.gameObj, typeof(GameObject), true, new GUILayoutOption[0]) as GameObject);
        if (GUILayout.Button("���", new GUILayoutOption[0]))
        {
            if (this.gameObj == null)
            {
                return;
            }
            this.obj = null;//����ظ���obj
            if (this.fixedInspector(this.gameObj))
            {
                Debug.LogError("�ظ��� object");
                return;
            }
            string text = this.gameObj.name;
            int num = 0;
            while (uINameTable.Find(text))//�ظ�����β������
            {
                text += num;
                num++;
            }
            Undo.RecordObject(uINameTable, "��ӵ� Table");//�� Editor ��¼��������
            base.serializedObject.Update();
            uINameTable.Add(text, this.gameObj);
            base.serializedObject.ApplyModifiedProperties();
        }
        GUILayout.EndHorizontal();
        string text2 = EditorGUILayout.TextField("����:", this.search, new GUILayoutOption[0]);
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
        if (GUILayout.Button("����", new GUILayoutOption[0]))
        {
            Undo.RecordObject(uINameTable, "Sort Table");//�� Editor ��¼��������
            base.serializedObject.Update();
            uINameTable.Sort();
            base.serializedObject.ApplyModifiedProperties();
        }
    }
}
