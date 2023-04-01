/*************************
文件:UIPageID.cs
作者:cocoa
创建时间:2023-03-18 16-15-31
描述：
************************/
using UnityEngine;
#if UNITY_EDITOR
namespace ET
{
    
    public class UIPageID : MonoBehaviour
    {
        public enum  PageType
        { 
            Normal,
            Fixed,
            Pop
        }

        public PageType windowtType;


    }
}
#endif