using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleFiledData: MonoBehaviour
{
    public int BattleFiled_row = 10;//BattleFiled行数
    public int BattleFiled_line = 10;//BattleFiled列数

    //BattleFileData数据库为单例
    private static BattleFiledData instance = null;
    private static readonly object padlock = new object();
    private BattleFiledData()
    {

    }
    public static BattleFiledData Instance
    {
        get
        {
            lock (padlock)
            {
                if (instance == null)
                {
                    instance = new BattleFiledData();
                }
                return instance;
            }
        }
    }
}
