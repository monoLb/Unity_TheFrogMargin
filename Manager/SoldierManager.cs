using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class SoldierManager : MonoBehaviour
{
    public SoldierStore store;
    public Transform _deckTrans;
    public static SoldierManager Instance;
    
    public List<SoldierDataSO> storeDeckList = new();
    public List<SoldierDataSO> storeShowList = new();
   
    public List<NewSoldier> teamList = new ();
    public List<NewSoldier> enemyList = new();
    
    [SerializeField] private List<SoldierDataSO> discardDeck = new();
    
    public int DrawNumber = 4;
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
        InitializeDeck();
        
        DrawCard(DrawNumber);
    }

    [ContextMenu("抽牌")]
    public void TestDrawDeck()
    {
        if (GameManager.Instance.Coin >= 1)
        {
            DrawCard(DrawNumber);
            GameManager.Instance.Coin -= 1;
        }
        
        
        StateManager.Instance.RefreshUI();
         
    }

    private void DrawCard(int amount)
    {
        if(storeDeckList.Count < 0)
            return;
        amount=Mathf.Min(amount, storeDeckList.Count+storeShowList.Count);
        if (storeShowList.Count > 0)
        {
            foreach (var item in storeShowList)
            {
                storeDeckList.Add(item);
            }

            foreach (Transform child in _deckTrans)
            {
                Destroy(child.gameObject);
            }
        }
        storeShowList.Clear();
        
        RandomDeck(storeDeckList);

        for (int i = 0; i < amount; i++)
        {
            SoldierDataSO soldierData = storeDeckList[0];
            storeDeckList.RemoveAt(0);
            storeShowList.Add(soldierData);
            
            Soldier s=store.GetSoldierObject().GetComponent<Soldier>();
            s.Init(soldierData);
            s.transform.SetParent(_deckTrans);
            
        }
    }
    private void InitializeDeck()
    {
        storeDeckList.Clear();
        foreach (var entry in store.storeLibrary.soldierLibraryList)
        {
            for (int i = 0; i < entry.amount; i++)
            {
                storeDeckList.Add(entry.soldierData);
            }
        }

        RandomDeck(storeDeckList);
    }
    
    private void RandomDeck<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            var temp = list[i];
            int randomIndex = Random.Range(0, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex]= temp;
        }
    }

    
}
