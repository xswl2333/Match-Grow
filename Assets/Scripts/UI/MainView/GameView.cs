using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameView : BaseView
{
    private Text mCount; //ƥ���������������������

    void Start()
    {
        mCount = uiComponents["Universal"].text;
    }

}
