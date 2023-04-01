/*************************
文件:UILobbyComponentSystem.cs
作者:cocoa
创建时间:2023-3-18 17:17:18:12
描述：
************************/

using UnityEngine.UI;

namespace ET.Client
{
    public static class UILobbyComponentSystem
    {
        public static async ETTask EnterMap(this UILobbyComponent self)
        {
            await EnterMapHelper.EnterMapAsync(self.ClientScene());
            self.DomainScene().GetComponent<UIComponent>().UnLoadWindow<UILobbyComponent>();
        }
    }
}