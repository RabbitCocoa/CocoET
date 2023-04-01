/*************************
文件:UIWindowSystem.cs
作者:cocoa
创建时间:2022-12-25 16-05-50
描述：
************************/

using UnityEngine;
using UnityEngine.UI;

namespace ET.Client
{
    [FriendOfAttribute(typeof(ET.Client.UIComponent))]
    public static class UIWindowSystem
    {
        public static void SetRoot(this UIWindow self, Transform target)
        {
            if (target == null)
                return;
            self.Transform.SetParent(target, false);
            self.Transform.localScale = Vector3.one;
        }

        public static void SetLabinserting(this UIWindow self)
        {
            var stacks = self.GetParent<UIComponent>().showWindowStack[self.WindowType];
            int index = stacks.LastIndexOf(self.UIWidgeID);
            //逆着数第n个
            index = stacks.Count - index; 
            
            self.Transform.SetSiblingIndex(self.Transform.childCount- index);
        }

 
        
        //没办法环形依赖 无奈只能注掉了
        // public static void CloseSelf(this UIWindow self, bool showPrePage = true)
        // {
        //     self.GetParent<UIComponent>()?.ClosePage(self.UIWidgeID, showPrePage);
        // }
        // public static void ClosePage<T>(this UIWindow self,bool showPrePage = true)where T:Entity
        // {
        //     self.GetParent<UIComponent>()?.ClosePage<T>(showPrePage);
        // }
        //
        // public static void ShowPage<T>(this UIWindow self,bool closePrePage = true) where T:Entity
        // {
        //     self.GetParent<UIComponent>()?.ShowWindow<T>(closePrePage);
        // }
        // public static void UnloadPage<T>(this UIWindow self,bool closePrePage = true) where T:Entity
        // {
        //     self.GetParent<UIComponent>()?.UnLoadWindow<T>();
        // }
    }
}

