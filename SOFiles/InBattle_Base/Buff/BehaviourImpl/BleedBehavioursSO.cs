using UnityEngine;

[CreateAssetMenu(fileName = "BleedBehaviour", menuName = "Buff/Behaviour/BleedBehaviour")]
public class BleedBehavioursSO : DamageAfterBehaviourSO
{
    // 基于出击者的Att
    protected override int CalculateTriggerDamage(BuffInstance buff)
    {
        return CalculateBaseDamage(buff);
    }

    public override void ModifyOutgoingDamage(BuffInstance buff)
    {
        
    }
    
}
