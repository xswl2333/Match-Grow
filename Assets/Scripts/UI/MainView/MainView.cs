using QFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Path.GUIFramework;
using UnityEngine;
using UnityEngine.UI;

public class MainView : BaseView
{
    private IMatchSystem mMatchSystem;

    void Start()
    {
        uiComponents["StartGame"].button.onClick.AddListener(StartGame);
        uiComponents["ResetGame"].button.onClick.AddListener(ButtonClick);
    }

    void StartGame()
    {
        this.SendCommand<ReloadCommand>();
    }

    void ButtonClick()
    {
        this.Hide();
    }
}
