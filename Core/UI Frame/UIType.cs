using UnityEngine;
/// <summary>
/// 存储单个UI信息，包括名字和路径
/// </summary>
public class UIType
{
    private string path;
    private string name;
    public string Name{get=>name;}
    public string Path{get=>path;}

    /// <summary>
    /// 获取UI信息
    /// </summary>
    /// <param name="ui_path">对应Panel路径</param>
    /// <param name="ui_name">对应Panrl名称</param>
    public UIType(string ui_path,string ui_name)
    {
        path = ui_path;
        name = ui_name;
    }
}
