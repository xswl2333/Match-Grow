using QFramework;
using System.Collections;
using System.Collections.Generic;
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
    }

    private void DrawGrid()
    {

        int gridSize = GlobalGameConfig.GridWidth;
        float cellSize = 50f; // 格子大小
        float spacing = 30f;  // 格子间隔

        // 计算网格的总宽度和高度
        float gridWidth = gridSize * (cellSize + spacing) - spacing;
        float gridHeight = gridSize * (cellSize + spacing) - spacing;

        // 计算网格的起始位置（居中）
        float startX = (position.width - gridWidth) / 2;
        float startY = (position.height - gridHeight) / 2;


        for (int row = 0; row < gridSize; row++)
        {
            for (int col = 0; col < gridSize; col++)
            {

                float posX = startX + col * (cellSize + spacing);
                float posY = startY + row * (cellSize + spacing);

                float enumPopupWidth = cellSize * 1.2f;
                float enumPopupX = posX + (cellSize - enumPopupWidth) / 2; // 水平居中

                Rect enumPopupRect = new Rect(enumPopupX, posY, enumPopupWidth, 20);
                grid[row, col] = (BlockState)EditorGUI.EnumPopup(enumPopupRect, grid[row, col]);

                // 根据枚举值获取颜色
                Color cellColor = GetColorForCellType(grid[row, col]);

                // 绘制格子颜色
                DrawCellWithBorder(new Rect(posX, posY + 25, cellSize, cellSize), cellColor);
            }
        }
    }

    private void DrawCellWithBorder(Rect rect, Color color)
    {
        // 绘制白色描边
        Rect borderRect = new Rect(rect.x - 2, rect.y - 2, rect.width + 4, rect.height + 4);
        DrawCell(borderRect, Color.white);

        // 绘制格子颜色
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
}
