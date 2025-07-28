using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;


public class SoldierOnStore : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IPointerDownHandler
{ 
    [Header("Set")]
    public Image cardImage;
    public Image soldierImage;
    public TextMeshProUGUI costText;
    
    public SoldierInstance soldier;
    public SoldierDataSO soldierData;
    
    public int attack;
    public int health;
    public int speed;
    public int magic;

    public List<ElementType> elementTypeList;

    public bool isSelected;
    

    public void Init(SoldierDataSO data)
    {
        soldierData = data;
        
        attack = RandomInitValue(soldierData.soldier_Attack,0.2f);
        magic = RandomInitValue(soldierData.soldier_Magic,0.2f);
        health = RandomInitValue(soldierData.soldier_Health,0.2f);
        speed = RandomInitValue(soldierData.soldier_Speed,0.2f);
        
        soldierImage.sprite = soldierData.soldier_Sprite;
        costText.text = data.soldier_Price.ToString()+" G";
        
        soldier=new SoldierInstance(soldierData,attack,magic,health,speed,elementTypeList);
    }

    private int RandomInitValue(float value, float offset = 0)
    {
        return Mathf.RoundToInt(Random.Range(value*(1-offset)*(1.2f), value*(1+offset)*(1.2f)));
    }
    

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isSelected)
            cardImage.color = new Color(188 / 255f, 188 / 255f, 188 / 255f);


        // attributePanel.SetActive(true);   
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
        //backgroundImage.color = originalColor;
        if(!isSelected)
            cardImage.color = Color.white;
    }

 
    public void OnPointerDown(PointerEventData eventData)
    {
        Recruit.Instance.currentSelectedOnStore = this;
        Recruit.Instance.SelectedSoldier();
        cardImage.color = new Color(135 / 255f, 135 / 255f, 135 / 255f);
    }

}
