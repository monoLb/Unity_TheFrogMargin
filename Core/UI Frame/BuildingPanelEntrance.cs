using UnityEngine;
using UnityEngine.EventSystems;


public class BuildingPanelEntrance : MonoBehaviour,IPointerClickHandler
{
    private enum BuildingType
    {
        Tavern,
        Military,
        BattleSettings,
        MapOpener,
        Inventory
    }

    [SerializeField] private BuildingType buildingName;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        UIManager.GetInstance().CanvasObj = UIMethods.GetInstance().FindCanvas();

        switch (buildingName)
        {
            case BuildingType.Tavern:
                UIManager.GetInstance().Push(new Tavern_Panel());
                break;
            case BuildingType.Military:
                UIManager.GetInstance().Push(new Military_Panel());
                break;
            case BuildingType.BattleSettings:
                UIManager.GetInstance().Push(new BattleSettings_Panel());
                break;
            case BuildingType.MapOpener:
                UIManager.GetInstance().Push(new MapOpener_Panel());
                break;
            case BuildingType.Inventory:
                UIManager.GetInstance().Push(new Storage_Panel());
                break;  
            default:
                Debug.LogWarning("Unknown building type: " + buildingName);
                break;
        }
    }
}