using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;


[System.Serializable]
public class NewSoldier
{
    public SoldierDataSO SoldierData;
    public int attack;
    public int health;
    public int speed;
    //其他属性
}
public class TeamManager : MonoBehaviour
{
    public static TeamManager Instance;
    public GameObject SoldierTeamPrefab;
    public Transform _teamTrans;

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



    public void ShowMyTeam(NewSoldier addSoldier)
    {
        Soldier s=Instantiate(SoldierTeamPrefab, _teamTrans).GetComponent<Soldier>();
        s.place = SoldierPlace.Team;
        s.attack = addSoldier.attack;
        s.health = addSoldier.health;
        s.speed = addSoldier.speed;
        s.Init(addSoldier.SoldierData);
    }
}
