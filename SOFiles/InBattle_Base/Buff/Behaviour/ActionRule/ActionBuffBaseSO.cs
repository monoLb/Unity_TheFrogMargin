using System.Collections;
using UnityEngine;

public class ActionBuffBaseSO : BuffBehaviourSO
{
    public override IEnumerator OnBeforeDamage(BuffInstance buff)
    {
        // 眩晕动画
        Debug.Log("那我眩晕了啊");

        yield return null;
    }
}
