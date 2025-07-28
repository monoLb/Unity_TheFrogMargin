using System;
using System.IO;
using UnityEngine;

public static class SaveSystem
{
    #region Prefs
    
    /// <summary>
    /// 将对象保存为 JSON 存入 PlayerPrefs
    /// </summary>
    public static void SaveByPlayerPrefs(string key, object data)
    {
        string json = JsonUtility.ToJson(data);
        Debug.Log("保存JSON: " + json);
        PlayerPrefs.SetString(key, json);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 从 PlayerPrefs 读取 JSON 并转换成对象
    /// </summary>
    public static T LoadByPlayerPrefs<T>(string key)
    {
        if (!PlayerPrefs.HasKey(key))
        {
            Debug.LogWarning($"未找到保存数据：{key}");
            return default;
        }

        string json = PlayerPrefs.GetString(key);
        Debug.Log("读取JSON: " + json);
        return JsonUtility.FromJson<T>(json);
    }
    

    #endregion

    #region Json
    //Save
    public static void SaveByJson(string saveFileName, object data)
    {
        var json=JsonUtility.ToJson(data);
        var path = Path.Combine(Application.persistentDataPath,saveFileName);//跨平台本地存储路径
        Debug.Log(path);

        try
        {
            File.WriteAllText(path, json);//路径文件存在则覆盖
            Debug.Log($"Successfully saved file to {path}.");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save file to{path}: {e}");
        }
    }
    //读取
    public static T LoadByJson<T>(string saveFileName)
    {
        var path = Path.Combine(Application.persistentDataPath, saveFileName);
        try
        {
            var json = File.ReadAllText(path);
            var data = JsonUtility.FromJson<T>(json);
            return data;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load file from{path}: {e}");
            
            return default;
        }
    }
    
    //删档
    public static void DeleteSaveFile(string saveFileName)
    {
        var path=Path.Combine(Application.persistentDataPath, saveFileName);
        try
        {
            File.Delete(path);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to delete file from{path}: {e}");
        }
    }
    
    #endregion
}

