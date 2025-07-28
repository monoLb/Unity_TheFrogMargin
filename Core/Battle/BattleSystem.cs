using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
public class BattleSystem : MonoBehaviour
{
    public static BattleSystem Instance;
    
    //结算系统
    public List<ResultBagItem> currentBagList = new();
    public ResultSystem resultSystem;
    
    public BattleState currentState = BattleState.Start;
    public List<SoldierInBattle> battleList = new();
    public GameObject soldierInBattlePrefab;

    public GameObject preparedPrefab;
    public Transform preparedParent;
    
    List<SoldierInBattle> currentList =new List<SoldierInBattle>();

    // 类成员
    private Dictionary<SoldierInBattle, GameObject> _prepUIMap = new Dictionary<SoldierInBattle, GameObject>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ShowTeamInBoard();
        
        StartBattle();
    }

    public void StartBattle()
    {
        if (currentState != BattleState.Start)
            return;

        // 按速度从高到低排序战斗列表，确保出手顺序正确
        battleList = battleList.OrderByDescending(s => s.spe).ToList();

        // 战斗准备（比如初始化buff等，后续可扩展）

        currentState = BattleState.Combat;

        StartCoroutine(BattleRunning());
    }


    private List<GameObject> preparationUIs = new List<GameObject>();

    private IEnumerator BattleRunning()
    {
        while (currentState == BattleState.Combat)
        {
            // 新回合开始：从battleList筛选存活角色，排序赋值给currentList
            currentList = battleList.Where(s => !s.isDead).OrderByDescending(s => s.spe).ToList();

            CreatePreparationUI(currentList);

            while (currentList.Count > 0)
            {
                // 在角色开始行动前检查战斗状态
                if (currentState == BattleState.End)
                {
                    yield break; // 直接退出整个协程
                }

                var currentSoldier = currentList[0];
                bool finished = false;

                yield return currentSoldier.RunFullTurnRoutine(() =>
                {
                    finished = true;
                });

                yield return new WaitUntil(() => finished);

                // 移除UI显示该角色已行动完成
                RemovePreparationUIFor(currentSoldier);

                currentList.RemoveAt(0);
            }

            // 本回合所有角色行动完，休息0.5秒，准备下一回合
            yield return new WaitForSeconds(0.5f);
        }
    }


    private void CreatePreparationUI(List<SoldierInBattle> soldiers)
    {
        // 清空之前所有 UI，并清空字典
        foreach (var go in _prepUIMap.Values) Destroy(go);
        _prepUIMap.Clear();

        // 重新生成
        foreach (var soldier in soldiers)
        {
            if (soldier == null || soldier.isDead) continue;

            GameObject ui = Instantiate(preparedPrefab, preparedParent);
            var image = ui.transform.GetChild(0).GetComponent<Image>();
            image.sprite = soldier.soldierInstance.soldierData.soldier_Sprite;
            
            image.preserveAspect = true;
            image.SetNativeSize();
            image.transform.localScale = Vector3.one * 0.06f;  

            
            if (soldier.factionType == FactionType.Enemy)
                ui.GetComponent<Image>().color = new Color(212/255f,155/255f,155/255f);

            // 记录映射
            _prepUIMap[soldier] = ui;
        }
    }


    public void RemovePreparationUIFor(SoldierInBattle soldier)
    {
        if (_prepUIMap.TryGetValue(soldier, out var ui))
        {
            Destroy(ui);
            _prepUIMap.Remove(soldier);
        }
    }





    void InitSoldier(SoldierInstance soldier,Transform container, FactionType factionType)
    {
        if (soldier.formationIndex >= 0 && soldier.formationIndex <= 8)
        {
            string slotName = $"SideSoldier_{soldier.formationIndex + 1}";
            Transform slot = container.Find(slotName);
            GameObject soldierInBattleObj = Instantiate(soldierInBattlePrefab, slot);
            soldierInBattleObj.name = $"INDEX_{soldier.formationIndex + 1}";
            SoldierInBattle soldierInBattle= soldierInBattleObj.GetComponent<SoldierInBattle>();
            soldierInBattle.Init(soldier,factionType);

            battleList.Add(soldierInBattle);

            if (slot == null)
            {
                return;
            }
            SpriteRenderer sr = soldierInBattleObj.GetComponent<SpriteRenderer>();
            if (sr == null)
            {
                 Debug.LogWarning($"槽位 {slotName} 没有 SpriteRenderer");
                 return;
            }

            MaterialPropertyBlock _mpb = new MaterialPropertyBlock(); 

            // 显示 sprite，确保 SpriteRenderer 正确设置了 UV
            sr.sprite = soldier.soldierData.soldier_Sprite;

            // 设置 MaterialPropertyBlock 贴图
            sr.GetPropertyBlock(_mpb);
            _mpb.SetTexture("_MainTex", soldier.soldierData.soldier_Sprite.texture);
            sr.SetPropertyBlock(_mpb);
            if (factionType == FactionType.Enemy) 
            {
                sr.flipX = true;
            }
         
        }
    }
    void ShowTeamInBoard()
    {
        Transform playerContainer = GameObject.Find("PlayerContainer")?.transform;
        Transform enemyContainer = GameObject.Find("EnemyContainer")?.transform;

        if (playerContainer == null || enemyContainer == null)
        {
            Debug.LogError("找不到 PlayerContainer 或 EnemyContainer，请检查场景中的对象命名是否正确！");
            return;
        }

        //玩家
        foreach (var soldier in BattleManager.Instance.playerTeamList)
        {
            if (soldier.formationIndex is >= 0 and <= 8)
            {
                InitSoldier(soldier, playerContainer, FactionType.Friend);
            }
        }
        //敌人
        foreach (var soldier in BattleManager.Instance.enemyTeamList)
        {
            if (soldier.formationIndex is >= 0 and <= 8)
            {
                InitSoldier(soldier, enemyContainer, FactionType.Enemy);
            }
        }
    }


    public void OnNoMoreTargets()
    {
        Debug.Log("无可攻击目标，战斗结束");
        currentState = BattleState.End;
    }

    public void CheckBattleEnd()
    {
        bool allEnemiesDead = battleList.All(s => s.factionType == FactionType.Friend || s.isDead);
        bool allPlayersDead = battleList.All(s => s.factionType == FactionType.Enemy || s.isDead);

        if (allEnemiesDead)
        {
            Debug.Log("🎉 玩家胜利！");
            currentState = BattleState.End;
            StartCoroutine(HandleBattleEnd(true));
        }
        else if (allPlayersDead)
        {
            Debug.Log("💀 敌方胜利！");
            currentState = BattleState.End;
            StartCoroutine(HandleBattleEnd(true));
        }
    }

    
    private IEnumerator HandleBattleEnd(bool isPlayerWin)
    {
        yield return new WaitForSeconds(1f); // 等待动画结束
        
        UIFrameRoot.GetInstance().UIManager_Root.Pop(false);
        UIManager.GetInstance().CanvasObj = UIMethods.GetInstance().FindCanvas();
        

        if (isPlayerWin)
        {
            Debug.Log("成功了成功了成功了成功了成功了成功了成功了");
            //获取战胜奖励
            currentBagList = resultSystem.GetResultBag(PlayerSaveData.Instance.globalData.chapter, PlayerSaveData.Instance.globalData.floor);
            
            // 显示胜利，画面、奖励，收复随从等
            UIManager.GetInstance().Push(new Result_Panel(true));
     
        }
        else
        {
            Debug.Log("失败了失败了失败了失败了失败了失败了失败了");
            // 显示失败画面，获取偷取的道具
            UIManager.GetInstance().Push(new Result_Panel(false));
            // 扣除生命值，结算命定牌，
        }

        // 停止战斗流程，退出战斗场景等
    }


}
