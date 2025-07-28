using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager
{
    public Stack<PanelBase> stack_UI;
    public Dictionary<string, GameObject> dict_UIObject;

    private GameObject _canvasObj;
    public GameObject CanvasObj
    {
        get
        {
            if (_canvasObj == null)
            {
                Canvas c = Object.FindAnyObjectByType<Canvas>();
                if (c != null) _canvasObj = c.gameObject;
            }
            return _canvasObj;
        }
        set => _canvasObj = value;
    }

    private GameObject globalMask;
    private static UIManager Instance;

    public static UIManager GetInstance()
    {
        // 修复：如果为空，必须实例化，不能直接 return null 导致后续全崩！
        if (Instance == null) Instance = new UIManager();
        return Instance;
    }

    public UIManager()
    {
        Instance = this;
        stack_UI = new Stack<PanelBase>();
        dict_UIObject = new Dictionary<string, GameObject>();
    }

    private void EnsureMaskInit()
    {
        if (globalMask != null) return;
        if (CanvasObj == null) return;

        globalMask = new GameObject("Global_UI_Mask");
        globalMask.transform.SetParent(CanvasObj.transform, false);

        var image = globalMask.AddComponent<Image>();
        image.color = new Color(0, 0, 0, 0.5f); 
        image.raycastTarget = true;

        RectTransform rt = globalMask.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.sizeDelta = Vector2.zero;
        rt.anchoredPosition = Vector2.zero; // 确保绝对居中

        var btn = globalMask.AddComponent<Button>();
        btn.transition = Selectable.Transition.None; 
        btn.onClick.AddListener(OnBackdropClicked);

        globalMask.SetActive(false);
    }

    private void OnBackdropClicked()
    {
        if (stack_UI.Count == 0) return;
        if (stack_UI.Peek().CanClickBackdropToClose)
        {
            Pop(false);
        }
    }

    public void Push(PanelBase panelBase)
    {
        string uiName = panelBase.UItype.Name;

        // ==========================================
        // 1. 防线：拦截重复推送
        // ==========================================
        if (stack_UI.Count > 0 && stack_UI.Peek().UItype.Name == uiName)
        {
            Debug.LogWarning($"[UI 系统] 拦截重复推送: {uiName} 已经在栈顶！");
            
            // 【史诗级防呆机制】：如果玩家用非正规手段隐藏了 UI，强行把它拉回来！
            if (stack_UI.Peek().ActiveObj != null)
            {
                stack_UI.Peek().ActiveObj.SetActive(true);
                stack_UI.Peek().ActiveObj.transform.SetAsLastSibling();
            }
            return; 
        }

        // ==========================================
        // 2. 挂起旧 UI
        // ==========================================
        if (stack_UI.Count > 0)
        {
            stack_UI.Peek().OnDisable(); 
        }

        // ==========================================
        // 3. 获取或实例化 UI 对象
        // ==========================================
        GameObject uiObj;
        if (dict_UIObject.TryGetValue(uiName, out GameObject cachedObj))
        {
            uiObj = cachedObj;
        }
        else
        {
            if (CanvasObj == null) { Debug.LogError("找不到 Canvas!"); return; }
            
            // 优化：不再调额外的函数，直接在这里处理干净
            GameObject prefab = Resources.Load<GameObject>(panelBase.UItype.Path);
            if (prefab == null) { Debug.LogError($"找不到预制体: {panelBase.UItype.Path}"); return; }

            uiObj = GameObject.Instantiate(prefab, CanvasObj.transform);
            dict_UIObject.TryAdd(uiName, uiObj);
        }

        panelBase.ActiveObj = uiObj;

        // ==========================================
        // 4. 三明治排序法 (遮罩与层级)
        // ==========================================
        EnsureMaskInit();
        if (globalMask != null)
        {
            // 只要 Push，遮罩必定开启！绝不判断 Count！
            globalMask.SetActive(true); 
            globalMask.transform.SetAsLastSibling(); // 遮罩去倒数第二层
        }
        
        uiObj.transform.SetAsLastSibling(); // 新 UI 去倒数第一层 (最表面)

        // ==========================================
        // 5. 核心修复：强制唤醒！
        // ==========================================
        uiObj.SetActive(true); // 解决缓存拿出来不显示的 Bug！
        
        stack_UI.Push(panelBase);
        panelBase.OnStart();

        Debug.Log($"[UI 系统] {uiName} 成功 Push！栈深度: {stack_UI.Count}");
    }

    public void Pop(bool isLoadClearAll)
    {
        if (stack_UI.Count <= 0) return;

        if (isLoadClearAll)
        {
            while (stack_UI.Count > 0)
            {
                PanelBase topPanel = stack_UI.Pop();
                topPanel.OnDisable();
                topPanel.OnDestroy();

                string uiName = topPanel.UItype.Name;
                if (dict_UIObject.ContainsKey(uiName))
                {
                    GameObject.Destroy(dict_UIObject[uiName]);
                    dict_UIObject.Remove(uiName);
                }
            }
            if (globalMask != null) globalMask.SetActive(false);
        }
        else
        {
            PanelBase topPanel = stack_UI.Pop();
            topPanel.OnDisable();
            
            // 正常 Pop 只隐藏，不销毁！
            if (topPanel.ActiveObj != null) topPanel.ActiveObj.SetActive(false);

            if (stack_UI.Count > 0)
            {
                PanelBase newTopPanel = stack_UI.Peek();
                newTopPanel.OnEnable(); 
                
                if (newTopPanel.ActiveObj != null) 
                {
                    newTopPanel.ActiveObj.SetActive(true);
                    
                    // 重新垫底遮罩
                    if (globalMask != null)
                    {
                        globalMask.transform.SetAsLastSibling();
                        newTopPanel.ActiveObj.transform.SetAsLastSibling();
                    }
                }
            }
            else
            {
                // 栈彻底空了，关闭遮罩
                if (globalMask != null) globalMask.SetActive(false);
            }
        }
    }
}