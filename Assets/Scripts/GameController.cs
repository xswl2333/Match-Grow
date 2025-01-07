using QFramework;
using UnityEngine;

public class GameController : MonoBehaviour, ISingleton
{

    private RectTransform UICanvas;
    private Camera UICamera;
    public Transform GameRoot;
    public GlobalGameConfig gameConfig;

    public static GameController Instance => MonoSingletonProperty<GameController>.Instance;


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
        gameConfig = ResLoader.Allocate().LoadSync<GlobalGameConfig>("GlobalGameConfig");
    }

    private void Start()
    {
        UIKit.OpenPanel<InitView>();
    }



}