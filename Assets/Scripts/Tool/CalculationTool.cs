

using UnityEngine;

public static class CalculationTool
{
    public static Vector3 BlockCoverPos(int posX, int posY)
    {

        int startX = posX - GlobalGameConfig.GridWidth / 2;
        float X = GlobalGameConfig.GapWidth * startX;

        int startY = GlobalGameConfig.GridWidth / 2 - posY;
        float Y = GlobalGameConfig.GapWidth * startY;

        Vector3 pos = new Vector3(X, Y, 0);
        return pos;
    }

}