using MessagePack;
using QFramework;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class BlockEditor : EditorWindow
{
    private BlockState[,] grid;
    [MenuItem("Tools/Grid Editor")]
    public static void ShowWindow()
    {
        GetWindow<BlockEditor>("Grid Editor");
    }

    private void OnEnable()
    {
        int gridSize = GlobalGameConfig.GridWidth;
        grid = new BlockState[gridSize, gridSize];
    }

    // Draw the GUI
    private void OnGUI()
    {
        DrawGrid();

        // ��ӱ��水ť
        if (GUILayout.Button("��������"))
        {
            string saveFilePath = EditorUtility.SaveFilePanel("Save Grid Data", "", "Block", "msgpack");
            if (!string.IsNullOrEmpty(saveFilePath))
            {
                SaveGridData(grid, GlobalGameConfig.GridWidth, saveFilePath);
            }
        }

        // ��Ӽ��ذ�ť
        if (GUILayout.Button("��������"))
        {
            string loadFilePath = EditorUtility.OpenFilePanel("Load Grid Data", "", "msgpack");
            if (!string.IsNullOrEmpty(loadFilePath))
            {
                grid = LoadGridData(GlobalGameConfig.GridWidth, loadFilePath);
                Debug.Log($"Grid data loaded from {loadFilePath}");
                Repaint(); // ǿ��ˢ�½���
            }
        }
    }

    private void DrawGrid()
    {

        int gridSize = GlobalGameConfig.GridWidth;
        float cellSize = 50f; // ���Ӵ�С
        float spacing = 30f;  // ���Ӽ��

        // ����������ܿ�Ⱥ͸߶�
        float gridWidth = gridSize * (cellSize + spacing) - spacing;
        float gridHeight = gridSize * (cellSize + spacing) - spacing;

        // �����������ʼλ�ã����У�
        float startX = (position.width - gridWidth) / 2;
        float startY = (position.height - gridHeight) / 2;


        for (int row = 0; row < gridSize; row++)
        {
            for (int col = 0; col < gridSize; col++)
            {

                float posX = startX + col * (cellSize + spacing);
                float posY = startY + row * (cellSize + spacing);

                float enumPopupWidth = cellSize * 1.2f;
                float enumPopupX = posX + (cellSize - enumPopupWidth) / 2; // ˮƽ����

                Rect enumPopupRect = new Rect(enumPopupX, posY, enumPopupWidth, 20);
                grid[row, col] = (BlockState)EditorGUI.EnumPopup(enumPopupRect, grid[row, col]);

                // ����ö��ֵ��ȡ��ɫ
                Color cellColor = GetColorForCellType(grid[row, col]);

                // ���Ƹ�����ɫ
                DrawCellWithBorder(new Rect(posX, posY + 25, cellSize, cellSize), cellColor);
            }
        }
    }

    private void DrawCellWithBorder(Rect rect, Color color)
    {
        // ���ư�ɫ���
        Rect borderRect = new Rect(rect.x - 2, rect.y - 2, rect.width + 4, rect.height + 4);
        DrawCell(borderRect, Color.white);

        // ���Ƹ�����ɫ
        DrawCell(rect, color);
    }

    private void DrawCell(Rect rect, Color color)
    {
        EditorGUI.DrawRect(rect, color);
    }

    private Color GetColorForCellType(BlockState cellType)
    {
        switch (cellType)
        {
            case BlockState.Empty: return Color.gray;
            case BlockState.Freeze: return Color.blue;
            case BlockState.Universal: return Color.red;
            default: return Color.gray; // Default color for Empty
        }
    }


    private List<BlockData> ConvertGridToBlockDataList(BlockState[,] grid, int gridSize)
    {
        List<BlockData> blockDataList = new List<BlockData>();
        for (int row = 0; row < gridSize; row++)
        {
            for (int col = 0; col < gridSize; col++)
            {
                blockDataList.Add(new BlockData((int)grid[row, col], row, col));
            }
        }
        return blockDataList;
    }

    private void SaveGridData(BlockState[,] grid, int gridSize, string filePath)
    {
        // ����ά����ת��Ϊ BlockData �б�
        List<BlockData> blockDataList = ConvertGridToBlockDataList(grid, gridSize);

        // ���л�Ϊ����������
        byte[] serializedData = MessagePackSerializer.Serialize(blockDataList);

        // ���浽�ļ�
        File.WriteAllBytes(filePath, serializedData);

        Debug.Log($"Grid data saved to {filePath}");
    }


    public static BlockState[,] LoadGridData(int gridSize, string filePath)
    {
        // ���ļ���ȡ����������
        byte[] data = File.ReadAllBytes(filePath);

        // �����л�Ϊ BlockData �б�
        List<BlockData> blockDataList = MessagePackSerializer.Deserialize<List<BlockData>>(data);

        // �� BlockData �б�ת��Ϊ��ά����
        BlockState[,] grid = new BlockState[gridSize, gridSize];
        foreach (var blockData in blockDataList)
        {
            grid[blockData.X, blockData.Y] = (BlockState)blockData.BlockState;
        }

        return grid;
    }

}


