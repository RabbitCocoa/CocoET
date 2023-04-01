/*************************
文件:UIComponent.cs
作者:cocoa
创建时间:2022-12-25 10-57-40
描述： UI控制组件,管控所有的UI
************************/

using System;
using System.Collections.Generic;
using UnityEngine;

namespace ET.Client
{
    [ComponentOf(typeof(Scene))]
 
    public class UIComponent : Entity,IAwake,IDestroy
    {
        #region Root

        public Dictionary<UIWindowType, Transform> typeToRoots = new Dictionary<UIWindowType, Transform>();

        public bool isClicked { get; set; }


        #endregion
        
        
        
        //所有已生成的窗口
        public Dictionary<UIWidgeID, UIWindow> allWindows = new Dictionary<UIWidgeID, UIWindow>();
        //当前正在显示的窗口栈 用List模拟栈
        public Dictionary<UIWindowType, List<UIWidgeID>> showWindowStack = new Dictionary<UIWindowType, List<UIWidgeID>>();

    }
}