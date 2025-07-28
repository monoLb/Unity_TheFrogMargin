using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneJumpProxy : MonoBehaviour
{
    public string targetScene;

    public void Jump()
    {
        SceneManager.LoadScene(targetScene);
    }
}