using Unity.VisualScripting;
using UnityEngine;
/// <summary>
/// 所有UI面板的父类，包含UI面板的状态信息
/// </summary>
public class PanelBase
{
    public UIType UItype;

    public GameObject ActiveObj;

    public PanelBase(UIType uItype)
    {
        this.UItype = uItype;
    }

    public virtual bool CanClickBackdropToClose => true;

    /// <summary>
    /// UI进入时执行的操作（初始化）
    /// </summary>
    public virtual void OnStart()
    {
        Debug.Log("OnStart"+UItype.Name);
        UIMethods.GetInstance().GetOrAddComponent<CanvasGroup>(ActiveObj).interactable = true;
    }

    /// <summary>
    /// 启用时执行的操作
    /// </summary>
    public virtual void OnEnable()
    {
        UIMethods.GetInstance().GetOrAddComponent<CanvasGroup>(ActiveObj).interactable = true;
    }

    /// <summary>
    /// 关闭时执行的操作
    /// </summary>
    public virtual void OnDisable()
    {
        UIMethods.GetInstance().GetOrAddComponent<CanvasGroup>(ActiveObj).interactable = false;
    }

    /// <summary>
    /// 被摧毁时时执行的操作
    /// </summary>
    public virtual void OnDestroy()
    {
        UIMethods.GetInstance().GetOrAddComponent<CanvasGroup>(ActiveObj).interactable = false;
    }
}
