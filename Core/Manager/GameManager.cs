using System;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    private static readonly int Changed = Animator.StringToHash("Changed");

    public AudioSource audioSource;  // 背景音乐播放器
    public AudioClip bgmClip;
    
    public NewGameStartSO newGameStartSO;
    public ItemLibrarySO itemLibrarySO;
    public TextMeshProUGUI moneyEvent;

    public Animator coinAnim;
    public GameObject dontDestroyCanvas;

    public int flipTimes;

    [Header("Cores")]
    [SerializeField] private ItemLibrarySO _itemLibraryDB;

    [SerializeField] private MasterModsSO _modDB;

    public static MasterModsSO ModDB => Instance._modDB;
    public static ItemLibrarySO ItemLibraryDB => Instance._itemLibraryDB;
    
    public IntEventSO onSlotChangedEvent;
    public void RefreshMoneyUI()
    {
        Debug.Log("RefreshMoneyUI");
        moneyEvent.text = PlayerSaveData.Instance.globalData.playerMoney.ToString();

        coinAnim.SetTrigger(Changed);// 强制从头播放一次

    }
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            if (dontDestroyCanvas != null)
                DontDestroyOnLoad(dontDestroyCanvas);
            
            InitializeAllModules(); 
        }
        else
        {
            // 如果发现了重复的 GameManager，直接销毁自己，并立即 Return！
            // 严禁在此处执行任何其他逻辑！
            Destroy(gameObject);
            return; 
        }
    }

    private void InitializeAllModules()
    {
        // 1. 初始化词缀总库
        if (_modDB != null)
        {
            _modDB.Init(); 
        }
        else
        {
            Debug.LogError("[GameManager] 严重错误：未挂载 Master Mod Database！");
        }

        // 2. 初始化图纸总库
        if (_itemLibraryDB != null)
        {
            _itemLibraryDB.Init();
        }
        else
        {
            Debug.LogError("[GameManager] 严重错误：未挂载 Item Library DB！");
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
        
        NewGameStart();
    }

    private void NewGameStart()
    {
        PlayerSaveData.Instance.globalData = new SaveData(20);
        
        PlayerSaveData.Instance.MoneyChanged(newGameStartSO.money, true);
        
        PlayerSaveData.Instance.Save();
    }

    public bool AddItem(ItemInstance newItem, int amountToAdd)
    {
        var invArray = PlayerSaveData.Instance.globalData.storageArray;
        ItemBaseSO blueprint = itemLibrarySO.GetBlueprint(newItem.blueprintID);

        if (blueprint.IsStackable)
        {
            for (int i = 0; i < invArray.Length; i++)
            {
                if (!invArray[i].isEmpty && invArray[i].itemInstance.blueprintID == newItem.blueprintID &&
                    invArray[i].amount < blueprint.maxStackSize)
                {
                    // 找到可叠加的格子
                    int spaceLeft=blueprint.maxStackSize-invArray[i].amount;
                    if (amountToAdd <= spaceLeft)
                    {
                        invArray[i].amount += amountToAdd;
                        onSlotChangedEvent.RaiseEvent(i,this);
                        return true;
                    }
                    // 剩下的部分去下面的格子
                    else
                    {
                        invArray[i].amount += spaceLeft;
                        amountToAdd -= spaceLeft;
                        onSlotChangedEvent.RaiseEvent(i, this);
                    }
                }
            }
        }
        
        // 没能之前堆砌的格子情（第一步+不可堆叠）
        for (int i = 0; i < invArray.Length; i++)
        {
            if (invArray[i].isEmpty)
            {
                invArray[i].itemInstance = newItem;
                invArray[i].amount = amountToAdd;
                
                onSlotChangedEvent.RaiseEvent(i, this);
                return true;
            }
        }
        
        Debug.LogWarning("Can't add item to Inventory Slot");
        return false;
    }
    
}
