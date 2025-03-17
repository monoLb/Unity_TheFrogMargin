using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SoldierStore : MonoBehaviour
{
    public PoolTool poolTool;
    [Header("卡牌库")]
    public SoldierLibrarySO storeLibrary;
    public SoldierLibrarySO TeamLibrary;

    public List<SoldierDataSO> SoldierDataList;
    private void Awake()
    {
        InitializeSoldierDataList();
    }

    private void InitializeSoldierDataList()
    {
        Addressables.LoadAssetsAsync<SoldierDataSO>("Soldier", null).Completed += OnSoldierLoaded;
    }
    
    public GameObject GetSoldierObject()
    {
        var cardObj = poolTool.GetObjectFromPool();
        return cardObj;
    }
    
    public void DiscardSoldier(GameObject cardObj)
    {
        poolTool.ReturnObjectToPool(cardObj);
    }

    private void OnSoldierLoaded(AsyncOperationHandle<IList<SoldierDataSO>> handle)
    {
        if (handle.Status == AsyncOperationStatus.Succeeded) 
            SoldierDataList=new List<SoldierDataSO>(handle.Result);
        else
            Debug.Log("Error handle "+handle.Status);

    }
}
