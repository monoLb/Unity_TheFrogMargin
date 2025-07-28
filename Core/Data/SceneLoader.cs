using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }

    private string currentScene;
    private string lastScene;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        currentScene = SceneManager.GetActiveScene().name;
    }

    public void LoadScene(string sceneName)
    {
        lastScene = currentScene;
        currentScene = sceneName;

        SceneManager.LoadScene(sceneName);
    }

    public void LoadSceneAsync(string sceneName, Action<float> onProgress = null, Action onComplete = null)
    {
        StartCoroutine(LoadAsyncRoutine(sceneName, onProgress, onComplete));
    }

    private IEnumerator LoadAsyncRoutine(string sceneName, Action<float> onProgress, Action onComplete)
    {
        lastScene = currentScene;
        currentScene = sceneName;

        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        while (op.progress < 0.9f)
        {
            onProgress?.Invoke(op.progress);
            yield return null;
        }

        onProgress?.Invoke(1f);

        // 延迟激活场景（等待动画/加载完成）
        yield return new WaitForSeconds(0.3f);
        op.allowSceneActivation = true;

        onComplete?.Invoke();
    }

    public void ReturnToLastScene()
    {
        if (!string.IsNullOrEmpty(lastScene))
        {
            LoadScene(lastScene);
        }
    }
}