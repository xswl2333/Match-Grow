using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Path.GUIFramework;
using UnityEngine;
using UnityEngine.UI;

public class InitView : BaseView
{

    void Start()
    {
        uiComponents["StartGame"].button.onClick.AddListener(StartGame);
        uiComponents["ResetGame"].button.onClick.AddListener(ButtonClick);

        this.GetSystem<IMatchSystem>().SetParentObject(GameController.Instance.GameRoot);
    }

    void StartGame()
    {
        this.SendCommand<ReloadCommand>();
        UIKit.OpenPanel<GameView>();
        this.Hide();
    }

    void ButtonClick()
    {
        this.Hide();
    }
}
