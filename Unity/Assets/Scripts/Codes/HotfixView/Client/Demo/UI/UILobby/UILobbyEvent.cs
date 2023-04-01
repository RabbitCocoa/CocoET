/*************************
文件:UILobbyEvent.cs
作者:cocoa
创建时间:2023-3-18 17:17:18:12
描述：
************************/

namespace ET.Client
{
    [UIEvent(UIWidgeID.UILobby)]
    [FriendOf(typeof (ET.Client.UILobbyComponent))]
    public class UILobbyEvent: IEUIEventHandler
    {
        public void OnStart(UIWindow uiWindow)
        {
            UILobbyComponent UILobby = uiWindow.AddComponent<UILobbyComponent>();
            //----初始化公共组件-----
            UILobby.enterMap.AddListenerAsyncWithoutRepeated(UILobby.EnterMap);
            //-----------------------
        }

        public void OnActive(UIWindow uiWindow)
        {
            UILobbyComponent UILobby = uiWindow.GetComponent<UILobbyComponent>();
        }

        public void OnHide(UIWindow uiWindow)
        {
            UILobbyComponent UILobby = uiWindow.GetComponent<UILobbyComponent>();
        }

        public void OnDestroy(UIWindow uiWindow)
        {
            AddressableComponent.Instance.ReleaseSublevel<UILobbyComponent>();
        }

        public void OnHideOrDestroy(UIWindow uiWindow)
        {
            UILobbyComponent UILobby = uiWindow.GetComponent<UILobbyComponent>();
        }
    }
}