using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class SceneControl
{
    private static SceneControl Instance;

    public StringEventSO SceneLoadEventSO;

    public Dictionary<string, SceneBase> dict_Scenes;
    
    private AsyncOperationHandle<StringEventSO> sceneEventHandle;
    
    private ScriptableObject senderMarker;
    
    public static SceneControl GetInstance()
    {
        if (Instance == null)
        {
            Debug.Log("SceneControl Instance is null");
            Instance = new SceneControl();

            Instance.LoadSceneEventSOAsync(); // 异步加载事件 SO
            return null;
        }

        return Instance;
    }

    public SceneControl()
    {
        dict_Scenes = new Dictionary<string, SceneBase>();

        // 创建一个专门的 ScriptableObject 作为 sender 标识
        senderMarker = ScriptableObject.CreateInstance<ScriptableObject>();
        senderMarker.name = "SceneControl";
    }
    
    /// <summary>
    /// 异步加载 Addressable 的 StringEventSO
    /// </summary>
    private async void LoadSceneEventSOAsync()
    {
        sceneEventHandle = Addressables.LoadAssetAsync<StringEventSO>("ScenesChange"); // Addressable 地址
        await sceneEventHandle.Task;

        if (sceneEventHandle.Status == AsyncOperationStatus.Succeeded)
        {
            SceneLoadEventSO = sceneEventHandle.Result;
        }
        else
        {
            Debug.LogError("加载 ScenesChange StringEventSO 失败！");
        }
    }

    /// <summary>
    /// 加载一个场景
    /// </summary>
    /// <param name="sceneName">目标场景名称</param>
    /// <param name="sceneBase">目标场景</param>
    public void LoadScene(string sceneName, SceneBase sceneBase)
    {
        if (!dict_Scenes.ContainsKey(sceneName))
        {
            dict_Scenes.Add(sceneName, sceneBase);
        }

        var currentSceneName = SceneManager.GetActiveScene().name;
        if (dict_Scenes.ContainsKey(currentSceneName))
        {
            dict_Scenes[currentSceneName].ExitScene();
        }
        else
        {
            Debug.Log("dict_Scenes does not contain this scene.");
        }

        UIManager.GetInstance().Pop(true);

        // 异步加载新场景
        Addressables.LoadSceneAsync(sceneName, LoadSceneMode.Single);

        sceneBase.EnterScene();

        // 触发事件（如果已加载完成）
        SceneLoadEventSO?.RaiseEvent(sceneName, senderMarker);
    }


    /// <summary>
    /// 可选：释放 SO
    /// </summary>
    public void ReleaseEventSO()
    {
        if (sceneEventHandle.IsValid())
        {
            Addressables.Release(sceneEventHandle);
        }
    }
    
}
