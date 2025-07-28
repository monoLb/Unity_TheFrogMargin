using UnityEngine;

[CreateAssetMenu(fileName = "BurnBehaviour", menuName = "Buff/Behaviour/BurnBehaviour")]
public class BurnBehavioursSO : DamageOverTimeBehaviourSO
{
    [Header("Burn Base Behaviours")] 
    public int maxStackCount = 3;

    public float hpRate=0.2f;

    [Header("Rogue")] 
    public bool isTwoStack;
    
    protected override int CalculateTickDamage(BuffInstance buff)
    {
        if (buff == null || buff.owner == null)
            return 0;
        
        int damage = 0;
        if (buff.stackCount >= maxStackCount)
        {
            damage = Mathf.FloorToInt(buff.owner.maxHp* hpRate);
            buff.stackCount -= maxStackCount;
        }
        
        return damage;
    }
}
