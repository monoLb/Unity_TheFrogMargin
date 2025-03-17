using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int Coin;
    public static GameManager Instance;
    
    public AudioSource audioSource;  // 背景音乐播放器
    public AudioClip bgmClip;   
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 保持物体切换场景时不销毁
        }
        else
        {
            Destroy(gameObject); // 只保留一个
        }
    }
    
    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();

        // 从 Resources 目录加载 BGM（需要放在 Assets/Resources 文件夹内）
        AudioClip bgmClip = Resources.Load<AudioClip>("Music/BGM");
        
        if (bgmClip != null)
        {
            audioSource.clip = bgmClip;
            audioSource.loop = true;
            audioSource.volume = 0.5f;
            audioSource.Play();
        }
        else
        {
            Debug.LogError("BGM文件未找到！");
        }
    }
}
