using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using DG.Tweening;
using TMPro;
using UnityEngine.Serialization;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;
    public Slider _enemeyBlood;
    public Slider _playerBlood;
    
    //生成区域
    public Transform PlayerPos;
    public Transform EnemyPos;
    //预制体的区域
    public Transform _friendTrans;
    public Transform _enemyTrans;
    
    public GameObject SoliderBattlePrefab;
    
    public float xRadius = 3f;  // 椭圆的X轴半径
    public float yRadius = 2f;  // 椭圆的Y轴半径
    public int segmentCount = 100;
    
    [FormerlySerializedAs("plyerCurrentHP")] public float friendCurrentHP ;
    public float enemyCurrentHP;

    
    
    public GameObject sidePrefab;
    public Transform _prepareTrans;
    
    //清空预制体
    public List<GameObject> PreparedList = new();
    
    
    public List<CombatSolider> NextAttackList = new();

    private GameObject targetObj;

    public GameObject gameOver;

    public TextMeshProUGUI _timeText;
    public int count=0;
    private void OnDrawGizmos()
    {
        DrawEllipseGizmo(PlayerPos.position);
        DrawEllipseGizmo(EnemyPos.position);
    }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 保持物体切换场景时不销毁
        }
        else
        {
            Destroy(gameObject); 
        }
    }
    private void Start()
    {
        DispatchSoldier(Faction.friend,PlayerPos);
        DispatchSoldier(Faction.enemy,EnemyPos);
        
        Invoke("NextAttack", 1.2f);

        RefreshCountUI();
        //DispatchSoldier(EnemyPos);
    }

    private void RefreshCountUI()
    {
        count++;
        _timeText.text = count.ToString();
    }

    public void CycleBattles()
    {
        if(enemyCurrentHP<=0||friendCurrentHP<=0)
            return;
        foreach (Transform child in _friendTrans)
        {
            NextAttackList.Add(child.GetComponent<CombatSolider>());
        }

        foreach (Transform child in _enemyTrans)
        {
            NextAttackList.Add(child.GetComponent<CombatSolider>());
        }

        RefreshCountUI();
   
        NextAttack();   
    }

    private void DispatchSoldier(Faction facion,Transform pos)
    {
        List<NewSoldier> list=new ();
        if(facion==Faction.friend)
            list = SoldierManager.Instance.teamList;
        else if (facion==Faction.enemy)
        {
            list = SoldierManager.Instance.enemyList;
        }
        
        //遍历队伍
        foreach (var item in list)
        {
            GameObject obj=Instantiate(SoliderBattlePrefab,GetRandomPointInEllipse(pos.position) , Quaternion.identity);
            
            var c=obj.GetComponent<CombatSolider>();
            var b=obj.GetComponent<Battle>();
            
            c.soldierData = item.SoldierData;
            c._solider_Sprite.sprite = item.SoldierData.soldier_Sprite;
            c.solider_Health = item.health;
            c.solider_Attack = item.attack;
            c.solider_Speed = item.speed;
            if (facion==Faction.friend)
            {
                c.transform.SetParent(_friendTrans);
                c.soldier_Faction=Faction.friend;
                b.faction=Faction.friend;
                friendCurrentHP += item.health;
                _playerBlood.maxValue = friendCurrentHP;
            }
            else if (facion==Faction.enemy)
            {
                c.transform.SetParent(_enemyTrans);
                c._solider_Sprite.transform.localScale = new Vector3(-1f,1f,1f);
                c.soldier_Faction = Faction.enemy;
                b.faction = Faction.enemy;
                enemyCurrentHP += item.health;
                _enemeyBlood.maxValue = enemyCurrentHP;
                Debug.Log("当前血量"+enemyCurrentHP+_enemeyBlood.maxValue);
            }
            
            
            NextAttackList.Add(c);
        }
        //遍历敌人
        RefreshBloodUI();
    }
    
    public void RefreshPreparationUI()
    {
        foreach (var obj in PreparedList)
        {
            Destroy(obj);
        }
       
        PreparedList.Clear();
        
         
        foreach (var side in NextAttackList)
        {
         
            GameObject obj = Instantiate(sidePrefab, _prepareTrans);
            Preparation p = obj.GetComponent<Preparation>();
            //如果是List第一个元素
            if (side == NextAttackList.First())
                p._backgroundImage.color=new Color(0/255f,238/255f,255/255f);
            else if(side.soldier_Faction==Faction.enemy)
                p._backgroundImage.color=new Color(199/255f,96/255f,96/255f);
            p._prepareImage.sprite = side._solider_Sprite.sprite;
            PreparedList.Add(obj);
        }
    }
    IEnumerator DelayExecution(CombatSolider nextSolider)
    {
        yield return new WaitForSeconds(0.4f); // 延迟0.4秒
        nextSolider.line.isSkill = false; // 执行操作
    }
    
    // ReSharper disable Unity.PerformanceAnalysis
    public void NextAttack()
    {
        
        // 排序（降序）
        NextAttackList.Sort((a, b) => b.solider_Speed.CompareTo(a.solider_Speed));
        RefreshPreparationUI();
        CombatSolider nextSolider = NextAttackList[0];
        Battle b = nextSolider.GetComponent<Battle>();
        
        
        //切换UI
        switch (nextSolider.soldier_AttCount)
        {
            case 0:
                nextSolider.soldier_AttCount++;
                nextSolider._soldier_Enegy_1.color=new Color(216/255f,216/255f,239/255f);
                break;
            case 1:
                nextSolider.soldier_AttCount++;
                nextSolider.line.isSkill = true;
                nextSolider._soldier_Enegy_2.color=new Color(216/255f,216/255f,239/255f);
                break;
            case 2:
                nextSolider.soldier_AttCount = 0;
                nextSolider.soldier_IsSkill = true;
                StartCoroutine(DelayExecution(nextSolider));
                nextSolider._soldier_Enegy_1.color=new Color(134/255f,163/255f,175/255f);
                nextSolider._soldier_Enegy_2.color=new Color(134/255f,163/255f,175/255f);
                break;
        }
        
        // 攻击一个目标
        b.jumpSequence.Kill();
        b.isAttack = true;

        targetObj = GetRandomTarget(nextSolider);
        CombatSolider targetCombat = targetObj.GetComponent<CombatSolider>();
    
        if (targetCombat.solider_Health >= nextSolider.solider_Attack)
        {
            targetCombat.solider_Health -= nextSolider.solider_Attack;
            if(targetCombat.soldier_Faction==Faction.enemy)
                enemyCurrentHP-= nextSolider.solider_Attack;
            else
                friendCurrentHP-=nextSolider.solider_Attack;
        }
        else
        {
            if(targetCombat.soldier_Faction==Faction.enemy)
                enemyCurrentHP-= targetCombat.solider_Health;
            else
                friendCurrentHP-=targetCombat.solider_Health;
            targetCombat.solider_Health = 0;
            NextAttackList.Remove(targetCombat);
            Destroy(targetObj,0.5f);
        }
        
    
        b.AttackTheTarget(targetObj.transform);
        
        
        if(friendCurrentHP<=0)
            gameOver.SetActive(true);

       
        NextAttackList.RemoveAt(0);
        
        
        if (NextAttackList.Count > 0)
        {
            StartCoroutine(WaitForAttackAndNext());  // 协程，等待攻击执行完后执行下一次攻击
        }
        else
        {
            StartCoroutine(WaitForCycleAndNext());
        }
    }

    IEnumerator WaitForCycleAndNext()
    {
        yield return new WaitForSeconds(1.5f);
        CycleBattles();
    }

 

    private IEnumerator WaitForAttackAndNext()
    {
        yield return new WaitForSeconds(1.6f);
        // 攻击完成后再次执行下一次攻击
        NextAttack();
    }

    private GameObject GetRandomTarget(CombatSolider solider)
    {
        GameObject target = null;
        if (solider.soldier_Faction == Faction.friend)
        {
            int index = Random.Range(0, _enemyTrans.childCount);
            if (_friendTrans.childCount > index)
                target = _enemyTrans.GetChild(index).gameObject;
            
        }
            
        else if (solider.soldier_Faction == Faction.enemy)
        {
            int index = Random.Range(0, _friendTrans.childCount);
            if (_friendTrans.childCount > index)
                target = _friendTrans.GetChild(index).gameObject;
        }
        return target;
    }
    public void RefreshBloodUI()
    {
        _playerBlood.value = friendCurrentHP;
        _enemeyBlood.value = enemyCurrentHP;
    }
    
    private Vector3 GetRandomPointInEllipse(Vector3 centerPosition)
    {
        Gizmos.color = Color.magenta;
        // 生成一个随机角度
        float angle = Random.Range(0f, 2f * Mathf.PI);
        
        float radius = Mathf.Sqrt(Random.Range(0f, 1f));

        // 计算椭圆内的随机点
        float x = centerPosition.x + xRadius * radius * Mathf.Cos(angle);
        float y = centerPosition.y + yRadius * radius * Mathf.Sin(angle);

        return new Vector3(x, y, 0); // Z 轴保持0
    }
    
    void DrawEllipseGizmo(Vector3 centerPosition)
    {
        Gizmos.color = Color.magenta;

        Vector3 previousPoint = Vector3.zero;
        for (int i = 0; i <= segmentCount; i++)
        {
            float angle = (i / (float)segmentCount) * 2 * Mathf.PI;
            float x = centerPosition.x +Mathf.Cos(angle) * xRadius;
            float y = centerPosition.y +Mathf.Sin(angle) * yRadius;
            Vector3 currentPoint = new Vector3(x, y, 0);

            if (i > 0)
            {
                Gizmos.DrawLine(previousPoint, currentPoint); 
            }
            previousPoint = currentPoint;
        }
    }
}
