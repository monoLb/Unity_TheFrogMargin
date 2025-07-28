using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
public abstract class ExtraBaseSO : ScriptableObject
{
    public string extraName;
    [TextArea]
    public string extraDescription;

    public float extraPower = 1f;

    [Range(0, 1)] 
    public float extraAddValue = 0.2f;
    public int addRage = 50;
    public int batterCount = 2;

    public float attributeAddValue = 0.1f;
    
    public GameObject casterEffectPrefab;
    public GameObject targetEffectPrefab;

    public abstract IEnumerator ExtraActivate(SoldierInBattle caster, SoldierInBattle target, float value);

}
