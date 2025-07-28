using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIFrameRoot : MonoBehaviour
{
    private UIManager UIManager;

    public UIManager UIManager_Root
    {
        get => UIManager;
    }

    private SceneControl sceneControl;

    public SceneControl SceneControlRoot
    {
        get => sceneControl;
    }

    private static UIFrameRoot Instance;

    public static UIFrameRoot GetInstance()
    {
        if (Instance == null)
        {
            Debug.Log("UIRoot is null");
            return Instance;
        }
        
        return Instance;
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
        UIManager = new UIManager();
        sceneControl = new SceneControl();
    }

    private void Start()
    {
        //test
        // UIManager_Root.CanvasObj = UIMethods.GetInstance().FindCanvas();
        //
        // //推入面板
        // UIManager_Root.Push(new Military_Panel());
    }

    public void Load()
    {
        AdventureMap_Scene adventureMap = new AdventureMap_Scene();
        SceneControlRoot.LoadScene(adventureMap.sceneName, adventureMap);
    }
}
