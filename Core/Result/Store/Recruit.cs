using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class Recruit : MonoBehaviour
{
    [SerializeField]private SoldierDataSO currentSoldier;
    public static Recruit Instance;
    
    [FormerlySerializedAs("lastselectedInStore")] public SoldierOnStore lastselectedOnStore;
    [FormerlySerializedAs("currentSelectedInStore")] public SoldierOnStore currentSelectedOnStore;

    [SerializeField]
    private IntEventSO moneyEvent;
    private void Awake()
    {
        Instance = this;
    }

    public void RecruitSoldier()
    {
        if(lastselectedOnStore == null)
            return;
        if (PlayerSaveData.Instance.globalData.playerMoney >= currentSoldier.soldier_Price)
        {

            PlayerSaveData.Instance.MoneyChanged(currentSoldier.soldier_Price, false);
            moneyEvent.RaiseEvent(PlayerSaveData.Instance.globalData.playerMoney,this);
           
           //进入TeamList
           SoldierInstance soldier = new SoldierInstance(
               lastselectedOnStore.soldierData, 
               lastselectedOnStore.attack, 
               lastselectedOnStore.magic, 
               lastselectedOnStore.health,
               lastselectedOnStore.speed, 
               lastselectedOnStore.elementTypeList);
          
            //进入当前数据集
            PlayerSaveData.Instance.globalData.teamList.Add(soldier);
            
            //列表中减少
            Destroy(currentSelectedOnStore.gameObject);
            SoldierStorePanelAdapter.Instance.storeShowList.Remove(currentSoldier);
            currentSoldier=null;
        }
        
    }

    public void SelectedSoldier()
    {
        if (lastselectedOnStore != null && lastselectedOnStore != currentSelectedOnStore)
        {
            lastselectedOnStore.cardImage.color = Color.white;
            lastselectedOnStore.isSelected = false;
        }
        currentSelectedOnStore.isSelected = true;
        currentSoldier = currentSelectedOnStore.soldierData;
        lastselectedOnStore = currentSelectedOnStore;
        
    }
}
