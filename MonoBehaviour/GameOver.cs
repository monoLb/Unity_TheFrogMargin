using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public void GameOverAndRestart()
    {
        StartCoroutine(WaitAndRestart());
    }    
    IEnumerator WaitAndRestart()
    {
        // 等待 2 秒
        yield return new WaitForSeconds(2f);

        // 在等待 2 秒后执行的代码，例如重新加载场景
        SceneManager.LoadScene(0);
    }
}