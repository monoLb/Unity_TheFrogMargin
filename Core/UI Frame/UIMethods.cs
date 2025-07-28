using System;
using UnityEngine;

public class UIMethods
{
    private static UIMethods Instance;

    public static UIMethods GetInstance()
    {
        if (Instance == null)
        {
            Instance = new UIMethods();
            return Instance;
        }
        
        return Instance;
    }
    
    /// <summary>
    /// 获取场景中的Canvas
    /// </summary>
    /// <returns></returns>
    private static GameObject _cachedCanvas;

    public GameObject FindCanvas()
    {
        if (_cachedCanvas != null) return _cachedCanvas;

        GameObject go = GameObject.Find("Canvas");
        if (go == null || go.GetComponent<Canvas>() == null)
        {
            Debug.LogWarning("Named 'Canvas' with Canvas component not found");
            return null;
        }
        _cachedCanvas = go;
        return go;
    }


    public GameObject FindObjectInChild(GameObject panel, string childName)
    {
        Transform[] transforms = panel.GetComponentsInChildren<Transform>();

        foreach (var t in transforms)
        {
            if (t.gameObject.name == childName)
            {
                return t.gameObject;
            }   
        }
        
        Debug.Log("Canvas not found: " + childName);
        return null;
    }
    
    public T GetOrAddComponent<T>(GameObject obj) where T : Component
    {
        T component = obj.GetComponent<T>();
        if (component == null)
        {
            component = obj.AddComponent<T>();
            Debug.Log($"{obj.name} Add Component: {typeof(T)}");
        }
        return component;
    }
    
    
    public T GetOrAddComponentInChild<T>(GameObject parent, string childName) where T : Component
    {
        // 1. 先尝试直接查找子物体
        Transform childTransform = parent.transform.Find(childName);
    
        // 2. 如果找不到，尝试深度搜索
        if (childTransform == null)
        {
            childTransform = FindDeepChild(parent.transform, childName);
            if (childTransform == null)
            {
                Debug.LogError($"Child '{childName}' not found in {parent.name}");
                return null;
            }
        }
    
        // 3. 获取或添加组件
        T component = childTransform.GetComponent<T>();
        if (component == null)
        {
            component = childTransform.gameObject.AddComponent<T>();
            Debug.Log($"Added {typeof(T).Name} to {childName}");
        }
    
        return component;
    }
    
    // 递归深度搜索子物体
    private Transform FindDeepChild(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
                return child;
            
            var found = FindDeepChild(child, childName);
            if (found != null)
                return found;
        }
        return null;
    }

}
