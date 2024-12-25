using QFramework;
using UnityEngine;

public class GameController : MonoBehaviour, ISingleton
{

    private RectTransform UICanvas;
    private Camera UICamera;
    public RectTransform GameRoot;

    void ISingleton.OnSingletonInit()
    {

    }

    private void Awake()
    {
        ResKit.Init();
        UIKit.Root.SetResolution(2340, 1080, 1);
        UICamera = UIKit.Root.Camera;
        UICamera.orthographic = true;
        Vector3 pos = UICamera.transform.position;
        pos.z = -1000;
        UICamera.transform.position = pos;   
        UIKit.Root.ScreenSpaceCameraRenderMode();
    }

    private void Start()
    {
        UIKit.OpenPanel<MainView>();
    }



}