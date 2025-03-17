using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum SoldierPlace
{
    Store,
    Team,
    Battle
}

public class Soldier : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IPointerDownHandler
{
    public SoldierDataSO soldierData;
    public Image soldierImage;
    public Image backgroundImage;
    public TextMeshProUGUI nameText,slotText,typeText,descriptionText,attackText;
    public GameObject state;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI speedText,skillText;

    public Color originalColor;
    public bool isSelected;
    
    public SoldierPlace place;

    public int attack;
    public int health;
    public int speed;
    
    private void Start()
    {
        if(place!=SoldierPlace.Battle)   
            originalColor = backgroundImage.color;
        Init(soldierData);
    }

    public void Init(SoldierDataSO data)
    {
        soldierData = data;
        switch (place)
        {
            case SoldierPlace.Store:
                attack = RandomValue(soldierData.soldier_Attack);
                health = RandomValue(soldierData.soldier_Health);
                speed = RandomValue(soldierData.soldier_Speed);
                slotText.text = soldierData.soldier_Slot.ToString();
                descriptionText.text = soldierData.soldier_Description;
                nameText.text = soldierData.soldier_Name;
                attackText.text = attack.ToString();
                speedText.text = speed.ToString();
                skillText.text = soldierData.soldier_Skill;
                
                break;
            case SoldierPlace.Team:
                nameText.text = soldierData.soldier_Name;
                attackText.text = attack.ToString();
                speedText.text = speed.ToString();
                skillText.text = soldierData.soldier_Skill;
                break;
            case SoldierPlace.Battle:
                break;
        }
        typeText.text = data.soldier_Type switch
        {
            SoldierType.Warrior => "战士",
            SoldierType.Wizard => "法师",
            SoldierType.Healer => "辅助"
        };
        soldierImage.sprite = soldierData.soldier_Sprite;
        healthText.text = health.ToString();
        

    }

    public int RandomValue(float value)
    {
        return Mathf.RoundToInt(Random.Range(value*0.8f, value*1.2f));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        
        if (!isSelected)
        {
            backgroundImage.color = new Color(143/255f,197/255f,163/255f);
        }
  
        state.SetActive(true);   
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isSelected)
        {
            backgroundImage.color = originalColor;
            
        }
      
        state.SetActive(false);
           
    }

 
    public void OnPointerDown(PointerEventData eventData)
    {
        switch (place)
        {
            case SoldierPlace.Store:
                Recruit.Instance.currentSelected = this;
                Recruit.Instance.SelectedSoldier();
                break;
            case SoldierPlace.Team:
                isSelected = false;
                break;
            case SoldierPlace.Battle:
                break;
        }
        
    }

}
