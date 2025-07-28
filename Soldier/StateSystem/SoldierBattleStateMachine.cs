using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierBattleStateMachine : MonoBehaviour
{
    private ISoldierBattleState currentState;

    public void SetState(ISoldierBattleState newState, SoldierInBattle soldier)
    {
        currentState = newState;

        StartCoroutine(currentState.Enter(soldier));
    }

    public IEnumerator SetStateAndWait(ISoldierBattleState newState, SoldierInBattle soldier)
    {
        currentState = newState;
        yield return currentState.Enter(soldier); // 等待状态完成
    }
}