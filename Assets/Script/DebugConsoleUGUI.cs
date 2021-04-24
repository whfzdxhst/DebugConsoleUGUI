/*--------------------------------------------------------------------
 * Author   Name: 斯卡帕的铁十字
 * Creation Time: 2021.4.24
 * File Describe: 提供开发者在游戏中一个数值可视化界面，用于进行实机调试
 * ------------------------------------------------------------------*/

using System;
using System.Collections;
using System.Collections.Generic;   
using UnityEngine;
using UnityEngine.UI;

public class DebugConsoleUGUI : MonoBehaviour
{
    //======================================变量部分=======================================
    //开发者模式检测
    [Title("开发者模式检测")]
    [InspectorShow("开启代码")]
    [Tooltip("开启开发者控制台的代码")]
    public string DeveloperTest_codeStr = "DEVE";//代码本体
    [InspectorShow("保留时间")]
    [Tooltip("从键盘输入的字符串保留在内存中的时间")]
    public int DeveloperTest_time = 1000;        //临时字符串保留时间，默认设置500帧
    private string DeveloperTest_str;            //临时字符串
    private int DeveloperTest_spenTime;          //记录帧数

    //开发者控制台操作句柄与路径
    [Title("开发者控制台操作句柄与路径")]
    [InspectorShow("指令输入框")]
    [Tooltip("用于输入指令的Inputfiled控件")]
    public InputField Handle_inputField;         //指令输入框句柄
    [InspectorShow("消息窗口")]
    [Tooltip("用于显示消息的ScrollView控件")]
    public ScrollRect Handle_scrollRect;         //窗口句柄
    private RectTransform Handle_Content_rect;
    private RectTransform Handle_LogList_rect;
    private Text Handle_LogList_text;
    

    //参数与标志
    [Title("参数与标志")]
    [InspectorShow("字体大小")]
    [Tooltip("Form Size")]
    public int ParmAndFlag_FontSize = 13;      //字体大小
    [InspectorShow("行间距")]
    [Tooltip("Line Spacing")]                    
    public int ParmAndFlag_LineSpacing = 1;    //行间距
    [InspectorShow("边框间距")]
    [Tooltip("LogList和Content的横向间隔")]
    public int ParmAndFlag_ySpacing = 10;    //边框间距
    [InspectorShow("消息列表上限")]
    [Tooltip("总共可显示的消息数目，超过上限将被清除")]
    public int ParmAndFlag_LogLimit = 100;
    [InspectorShow("单页消息上限")]
    [Tooltip("单页可显示的消息数目，超过上限不会显示")]
    public int ParmAndFlag_ShowLimit = 10;
    [InspectorShow("消息窗口宽度")]
    [Tooltip("Rect.Width")]
    public float ParmAndFlag_width = 400;
    private static List<string> ParmAndFlag_Logs = new List<string>();
                                               //记录列表
    private static bool ParmAndFlag_ChangeFlag = false;
    private static int ParmAndFlag_LogLimit_static;
    private static bool ParmAndFlag_Stop = false;
    //-+-+-+-+-+-+安卓操作接口：手势反馈-+-+-+-+-+-+-+-+
    private static bool ParmAndFlag_isTouchLine = false;
    //-+-+-+-+-+-+安卓操作接口：手势反馈-+-+-+-+-+-+-+-+

    //着色参数
    [Title("着色参数")]
    [InspectorShow("命令记录字体颜色")]
    [Tooltip("着色方案为富文本")]
    public Color Color_FontColor_Orderlog;
    [InspectorShow("消息记录字体颜色")]
    [Tooltip("着色方案为富文本")]
    public Color Color_FontColor_messagelog;
    private static string Color_FontColor16bit_Orderlog;
    private static string Color_FontColor16bit_messagelog;

    //-+-+-+-+-+-+1、在此处填写新的指令枚举-+-+-+-+-+-+
    private enum OrderList
    {
        clear,
        stop,
        run,
        close
    };
    //-+-+-+-+-+-+在此处填写新的指令枚举-+-+-+-+-+-+

    IEnumerator InsSrollBar()
    {
        yield return new WaitForEndOfFrame();
        Handle_scrollRect.transform.Find("Scrollbar Vertical").GetComponent<Scrollbar>().value = 0;
    }

    //======================================生命周期=======================================
    private void OnEnable()
    {
        //-+-+-+-静态参数设置-+-+-+-
        Color_FontColor16bit_Orderlog = ColorUtility.ToHtmlStringRGB(Color_FontColor_Orderlog);
        Color_FontColor16bit_messagelog = ColorUtility.ToHtmlStringRGB(Color_FontColor_messagelog);
        ParmAndFlag_LogLimit_static = ParmAndFlag_LogLimit;

        //-+-+-+-初始化开发者模式检测变量-+-+-+-
        DeveloperTest_spenTime = DeveloperTest_time;
        //DeveloperTest_isEnter = false;
        DeveloperTest_codeStr.ToUpper();
        try
        {
            if (DeveloperTest_spenTime < 0)
            {
                throw (new DeveloperTest_timeException(DeveloperTest_spenTime));
            }
        }
        catch (DeveloperTest_timeException d)
        {
            d.DeveloperTest_timeException_message();
        }

        //-+-+-+-初始化开发者控制台操作句柄-+-+-+-
        try
        {
            if (Handle_inputField == null || Handle_scrollRect == null)
            {
                throw (new Handle_lossException());
            }
        }
        catch (Handle_lossException d)
        {
            d.Handle_lossException_message();
        }
        float _height = (ParmAndFlag_FontSize + 2 * ParmAndFlag_LineSpacing) * ParmAndFlag_ShowLimit + 2* ParmAndFlag_ySpacing;
        Handle_scrollRect.GetComponent<RectTransform>().sizeDelta = new Vector2(ParmAndFlag_width, _height);

        //-+-+-+初始化可视区域与消息列表-+-+-+-
        Handle_LogList_text = Handle_scrollRect.transform.Find("Viewport").Find("Content").Find("LogList").GetComponent<Text>();
        Handle_LogList_text.fontSize = ParmAndFlag_FontSize;
        Handle_LogList_text.lineSpacing = ParmAndFlag_LineSpacing;
        Handle_LogList_text.alignment = TextAnchor.MiddleLeft;
        Handle_Content_rect = 
            Handle_scrollRect.transform.Find("Viewport").Find("Content").GetComponent<RectTransform>();
        Handle_LogList_rect = 
            Handle_scrollRect.transform.Find("Viewport").Find("Content").Find("LogList").GetComponent<RectTransform>();
        Handle_Content_rect.anchoredPosition =
            new Vector2(-ParmAndFlag_width / 2, Handle_Content_rect.anchoredPosition.y);
        Handle_LogList_rect.anchoredPosition =
            new Vector2(0, 0);
        Handle_scrollRect_up();
        Handle_developerUI_switch(false);
    }

    private void Update()
    {
        if (!Handle_scrollRect.gameObject.activeSelf)
        {
#if UNITY_STANDALONE_WIN
            //开发者模式检测
            DeveloperTest_GetKeyCode();
            if (DeveloperTest_spenTime == 0)
            {
                DeveloperTest_str = null;
                DeveloperTest_spenTime = DeveloperTest_time;
            }
            DeveloperTest_Compare();
#endif

#if UNITY_ANDROID
            if(ParmAndFlag_isTouchLine){
                Handle_developerUI_switch(true);
            }
#endif
        }
        else
        {
            //通过指令输入框句柄向指令输入框输入命令
            Handle_inputFiled_input();
            Handle_scrollRect_show();
        }
        
    }
    //======================================功能部分=======================================
    /// <summary>
    /// 得到当前按下的按键并存储在暂存列表里
    /// </summary>
    /// <returns></returns>
    private void DeveloperTest_GetKeyCode()
    {
        if (Input.anyKeyDown)
        {
            foreach (KeyCode k in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(k))
                {
                    DeveloperTest_str += k.ToString();
                }
            }
        }
        DeveloperTest_spenTime--;
    }

    /// <summary>
    /// 比较键入字符串和代码，如果键入字符串有完整的代码就显示开发者控制台
    /// </summary>
    private void DeveloperTest_Compare()
    {
        if (DeveloperTest_str == null)
        {
            return;
        }

        if (DeveloperTest_str.IndexOf(DeveloperTest_codeStr) != -1)
        {
            Handle_developerUI_switch(true);
            DeveloperTest_str = null;
            DeveloperTest_spenTime = DeveloperTest_time;
        }
    }

    /// <summary>
    /// 开发者控制台开关
    /// </summary>
    private void Handle_developerUI_switch(bool DeveloperTest_isEnter)
    {
        Handle_scrollRect.gameObject.SetActive(DeveloperTest_isEnter);
    }

    /// <summary>
    /// 指令输入框句柄接受输入信号
    /// </summary>
    private void Handle_inputFiled_input()
    {
        if(Handle_inputField.textComponent.text.Length != 0 && Input.GetKeyDown(KeyCode.Return))
        {
            foreach (OrderList i in Enum.GetValues(typeof(OrderList)))
            {
                if(Handle_inputField.textComponent.text == i.ToString())
                {
                    Order_OP_Choose(i);
                }
            }
            //清空指令输入框
            Handle_inputField.text = "";
        }
    }

    /// <summary>
    /// 消息窗口大小变化
    /// </summary>
    private void Handle_scrollRect_up()
    {
        float size = ParmAndFlag_FontSize + 2 * ParmAndFlag_LineSpacing;
        float size_ = ParmAndFlag_Logs.Count * size;
        float _width = ParmAndFlag_width;
        if (ParmAndFlag_Logs.Count > ParmAndFlag_ShowLimit)
        {
            Handle_Content_rect.anchoredPosition =
            new Vector2(-ParmAndFlag_width / 2+ ParmAndFlag_ySpacing, Handle_Content_rect.anchoredPosition.y);
        }
        Handle_Content_rect.sizeDelta = new Vector2(_width, size_+ 20);
        Handle_LogList_rect.sizeDelta = new Vector2(_width - 20, size_);
        
    }

    /// <summary>
    /// 窗口句柄显示消息
    /// </summary>
    /// <param name="i">窗口输入指令</param>
    private void Handle_scrollRect_show()
    {
        if (!ParmAndFlag_ChangeFlag)
        {
            return;
        }

        if (ParmAndFlag_Logs.Count > ParmAndFlag_LogLimit)
        {
            ParmAndFlag_Logs.RemoveRange(0, (ParmAndFlag_Logs.Count - ParmAndFlag_LogLimit));
        }
        else
        {
            //扩大消息窗口范围
            Handle_scrollRect_up();
        }
        ParmAndFlag_ChangeFlag = false;
        //加载文本
        string strLine = null;
        foreach(string s in ParmAndFlag_Logs)
        {
            strLine += s;
        }
        Handle_LogList_text.text = strLine;
    }

    private static string ParmAndFlag_font(string content,string color_fontColor16bit)
    {
        return "<color=#" + color_fontColor16bit + ">" + content + "</color>\n";
    }

    //======================================外部接口=======================================
    public static void Log(string message)
    {
        if (ParmAndFlag_Stop)
        {
            return;
        }
        int hour = DateTime.Now.Hour;
        int minute = DateTime.Now.Minute;
        int second = DateTime.Now.Second;
        string time = string.Format("{0:D2}:{1:D2}:{2:D2} ", hour, minute, second);
        ParmAndFlag_Logs.Add(ParmAndFlag_font(("["+time+"]: " + message + ";"), Color_FontColor16bit_messagelog));
        if (ParmAndFlag_Logs.Count > ParmAndFlag_LogLimit_static)
        {
            ParmAndFlag_Logs.RemoveAt(0);
        }
        ParmAndFlag_ChangeFlag = true;
    }
    public static void Log(int message)
    {
        if (ParmAndFlag_Stop)
        {
            return;
        }
        int hour = DateTime.Now.Hour;
        int minute = DateTime.Now.Minute;
        int second = DateTime.Now.Second;
        string time = string.Format("{0:D2}:{1:D2}:{2:D2} ", hour, minute, second);
        ParmAndFlag_Logs.Add(ParmAndFlag_font(("[" + time + "]: " + message.ToString() + ";"), Color_FontColor16bit_messagelog));
        if (ParmAndFlag_Logs.Count > ParmAndFlag_LogLimit_static)
        {
            ParmAndFlag_Logs.RemoveAt(0);
        }
        ParmAndFlag_ChangeFlag = true;
    }
    //-+-+-+-+-+-+新类型与自定义类型-+-+-+-+-+-+
    public static void Log<T>(T message)
    {
        if (ParmAndFlag_Stop)
        {
            return;
        }
        int hour = DateTime.Now.Hour;
        int minute = DateTime.Now.Minute;
        int second = DateTime.Now.Second;
        string time = string.Format("{0:D2}:{1:D2}:{2:D2} ", hour, minute, second);
        ParmAndFlag_Logs.Add(ParmAndFlag_font(("[" + time + "]: " + message.ToString() + ";"), Color_FontColor16bit_messagelog));
        if (ParmAndFlag_Logs.Count > ParmAndFlag_LogLimit_static)
        {
            ParmAndFlag_Logs.RemoveAt(0);
        }
        ParmAndFlag_ChangeFlag = true;

    }
    //======================================基本命令=======================================
    private void Order_OP_Log(string i)
    {
        ParmAndFlag_Logs.Add
            (ParmAndFlag_font(("[Order]: " + i.ToString() + " is run;"), Color_FontColor16bit_Orderlog));
        if (ParmAndFlag_Logs.Count > ParmAndFlag_LogLimit_static)
        {
            ParmAndFlag_Logs.RemoveAt(0);
        }
        ParmAndFlag_ChangeFlag = true;
    }

    private void Order_OP_Choose(OrderList odl)
    {
        switch (odl)
        {
            case OrderList.clear:
                Order_Base_Clear();
                break;
            case OrderList.stop:
                Order_Base_Stop();
                break;
            case OrderList.run:
                Order_Base_Run();
                break;
            case OrderList.close:
                Order_Base_Close();
                break;
        //-+-+-+-+-+-+3、在此处填写新的指令入口-+-+-+-+-+-+
        //-                                               -
        //+                                               +
        //-                                               -
        //+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
        }
        Order_OP_Log(odl.ToString());
    }

    private void Order_Base_Clear()
    {
        ParmAndFlag_Logs.Clear();
    }

    private void Order_Base_Stop()
    {
        ParmAndFlag_Stop = true;
    }

    private void Order_Base_Run()
    {
        ParmAndFlag_Stop = false;
    }

    private void Order_Base_Close()
    {
        Handle_developerUI_switch(false);
        ParmAndFlag_isTouchLine = false;
    }

    //-+-+-+-+-+-+2、在此处填写新的指令函数-+-+-+-+-+-+
    //-                                               -
    //+                                               +
    //-                                               -
    //+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
}

//======================================自定义异常类=======================================
public class DeveloperTest_timeException : Exception
{
    int time;
    public override string Message { get; }
    public DeveloperTest_timeException(int _time)
    {
        time = _time;
        
    }

    public void DeveloperTest_timeException_message()
    {
        Debug.Log(string.Format("Exception caught {DeveloperTest_timeException}:The input value of time is wrong, " +
            "please check whether it is less than zero：{1}",time));
    }
}

public class Handle_lossException : Exception
{
    public override string Message { get; }
    public Handle_lossException()
    {
    }

    public void Handle_lossException_message()
    {
        Debug.Log(string.Format("Exception caught {Handle_lossException}:The object is loss" ));
    }
}