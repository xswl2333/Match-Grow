using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class GameView : BaseView
{
    private Text mCount; //匹配规则数量或者消除次数

    void Start()
    {
        mCount = uiComponents["Universal"].text;
    }

}
