/*************************
文件:WidgeID.cs
作者:cocoa
创建时间:2023-03-18 16-14-51
描述：
************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ET
{
    public class WidgeID : MonoBehaviour
    {
        public enum Componentype
        {
            Normal,
            CommonComponent //公共组件
        }

        public Componentype componentype;
    }
}
