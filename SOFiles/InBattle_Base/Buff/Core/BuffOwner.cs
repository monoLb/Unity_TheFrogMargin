using System.Collections;
using UnityEngine;

public class BuffOwer : ScriptableObject
{
    public virtual IEnumerator OnTurnStart(
        SoldierInBattle Owner,
        BuffInstance buff)
    { yield break; }

    public virtual IEnumerator OnTurnend(
        SoldierInBattle ower,
        BuffInstance buff)
    { yield break; }
    
    public virtual void OnBeforeAction(
        SoldierInBattle Owner,
        ref bool allowAction)
    {}
}
