
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;
    
    public List<SoldierInstance> playerTeamList = new List<SoldierInstance>();
    public List<SoldierInstance> enemyTeamList=new List<SoldierInstance>();



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
    }

    private void Start()
    {
        if (PlayerSaveData.Instance.globalData.battleList!=null)
        {
            playerTeamList.Clear();
        
            playerTeamList = PlayerSaveData.Instance.globalData.battleList.ToList();
        }
        

    }
}
