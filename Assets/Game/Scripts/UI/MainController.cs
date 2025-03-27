using QFramework;
using UnityEngine;

public abstract class MainController : MonoBehaviour, IController
{
    IArchitecture IBelongToArchitecture.GetArchitecture()
    {
        return MainInit.Interface;
    }
}