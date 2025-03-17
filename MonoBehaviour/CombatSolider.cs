using System;
using UnityEngine;


public class CombatSolider:MonoBehaviour
{
    public SoldierDataSO soldierData;
    public HaloLine line;
    
    public Faction soldier_Faction;
    public SpriteRenderer _solider_Sprite;
    public SpriteRenderer _soldier_Enegy_1;
    public SpriteRenderer _soldier_Enegy_2;
    
    public float solider_Health;
    public float solider_Attack;
    public float solider_Speed;
    public int soldier_AttCount;
    public bool soldier_IsSkill;

}