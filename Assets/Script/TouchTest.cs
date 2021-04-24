/* 
 * author: AnYuanLzh 
 * date:   2014/01/18 
 * */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class TouchTest : MonoBehaviour
{
    private Touch oldTouch1;  //上次触摸点1(手指1)  
    private Touch oldTouch2;  //上次触摸点2(手指2)  

    void Update()
    {

        //没有触摸  
        if (Input.touchCount <= 0)
        {
            return;
        }

        //单点触摸， 水平上下移动
        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved)
        {
            Debug.Log("Yes");
            var deltaposition = Input.GetTouch(0).deltaPosition;
            transform.Translate(-deltaposition.x * 0.1f, 0f, -deltaposition.y * 0.1f);
        }
    }
}