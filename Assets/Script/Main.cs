using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Main : MonoBehaviour
{
    // 导入BattleFileData数据
    public BattleFiledData battleFIleData;
    public int a = 0;
    private static List<string> li = new List<string>();
    private void Awake()
    {
        DebugConsoleUGUI.Log(1);
    }
    void Start()
    {
        //使用BattleFIleData中的数据创建BattleFile
        Debug.Log(battleFIleData.BattleFiled_line);
        Debug.Log(battleFIleData.BattleFiled_row);
    }

    // Update is called once per frame
    void Update()
    {

        DebugConsoleUGUI.Log(1);
    }
}
