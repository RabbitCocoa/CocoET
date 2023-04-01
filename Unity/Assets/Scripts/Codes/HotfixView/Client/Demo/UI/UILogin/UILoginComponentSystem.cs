/*************************
文件:UILoginComponentSystem.cs
作者:cocoa
创建时间:2023-3-18 16:16:19:04
描述：
************************/
using UnityEngine.UI;
namespace ET.Client
{
    [FriendOfAttribute(typeof(ET.Client.UILoginComponent))]
    public static class UILoginComponentSystem
    {
        public static void OnLogin(this UILoginComponent self)
        {
            LoginHelper.Login(
                self.DomainScene(),
                self.account.text,
                self.password.text).Coroutine();
        }
    }
}