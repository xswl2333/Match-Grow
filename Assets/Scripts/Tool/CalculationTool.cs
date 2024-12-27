

using UnityEngine;

public static class CalculationTool
{
    public static Vector3 BlockCoverPos(int posX, int posY)
    {

        int startX = posX - GlobalConfig.GridWidth / 2;
        float X = GlobalConfig.GapWidth * startX;

        int startY = GlobalConfig.GridWidth / 2 - posY;
        float Y = GlobalConfig.GapWidth * startY;

        Vector3 pos = new Vector3(X, Y, 0);
        return pos;
    }

}