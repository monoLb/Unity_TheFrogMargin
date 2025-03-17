using TMPro;
using UnityEngine;

public class Description : MonoBehaviour
{
    public TextMeshProUGUI attackText,shieldText,skillText;

    public void ShowDescription(Transform pos, SoldierDataSO soldierData)
    {
        this.transform.position = pos.position + Vector3.right * 275;
        attackText.text = soldierData.soldier_Attack.ToString();
        shieldText.text = soldierData.soldier_Health.ToString();
        skillText.text = soldierData.soldier_Skill;
    }
}
