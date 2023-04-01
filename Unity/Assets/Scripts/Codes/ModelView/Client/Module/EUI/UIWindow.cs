/*************************
文件:UIWindow.cs
作者:cocoa
创建时间:2022-12-25 10-52-12
描述： UI窗口 管控该窗口下所有子UI
************************/

using ET;
using UnityEngine;

namespace ET.Client
{
    [ChildOf(typeof (UIComponent))]
    public class UIWindow: Entity, IAwake, IDestroy
    {
        public GameObject GameObject { get; set; }

        public Transform Transform
        {
            get => this.GameObject?.transform;
        }

        public UIWindowType WindowType { get; set; }
        public UIWidgeID UIWidgeID { get; set; }

        public bool IsShow { get; set; } //当前是否已经显示
    }
}