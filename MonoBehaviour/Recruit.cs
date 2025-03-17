using System;
using Unity.VisualScripting;
using UnityEngine;

public class Recruit : MonoBehaviour
{
    [SerializeField]private SoldierDataSO currentSoldier;
    public static Recruit Instance;
    
    public Soldier lastselected;
    public Soldier currentSelected;
    
    private void Awake()
    {
        Instance = this;
    }

    public void RecruitSoldier()
    {
        if(currentSoldier == null)
            return;
        if (GameManager.Instance.Coin >= currentSoldier.soldier_Slot)
        {
            GameManager.Instance.Coin-= currentSoldier.soldier_Slot;
            StateManager.Instance.RefreshUI();
            //一个全新的角色
            NewSoldier ns=new NewSoldier();
            ns.SoldierData = currentSoldier;
            ns.attack = currentSelected.attack;
            ns.health = currentSelected.health;
            ns.speed = currentSelected.speed;
            SoldierManager.Instance.teamList.Add(ns);
            //队伍中生成
            TeamManager.Instance.ShowMyTeam(ns);
            
            //列表中减少
            Destroy(currentSelected.gameObject);
            SoldierManager.Instance.storeShowList.Remove(currentSoldier);
            currentSoldier=null;
        }
        else
        {
            Debug.Log("钱不够哦");
            StateManager.Instance.MyCoin.text = "没米还来沾边？";
        }

    }

    public void SelectedSoldier()
    {
        if (lastselected != null && lastselected != currentSelected)
        {
            lastselected.backgroundImage.color = lastselected.originalColor;
            lastselected.isSelected = false;
        }
        currentSelected.isSelected = true;
        currentSoldier = currentSelected.soldierData;
        currentSelected.backgroundImage.color = new Color(149/255f,177/255f,93/255f);
        lastselected = currentSelected;
        
    }
}
