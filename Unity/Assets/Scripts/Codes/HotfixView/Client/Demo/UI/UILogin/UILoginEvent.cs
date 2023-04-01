/*************************
文件:UILoginEvent.cs
作者:cocoa
创建时间:2023-3-18 16:16:19:04
描述：
************************/

namespace ET.Client
{
    [UIEvent(UIWidgeID.UILogin)]
    [FriendOf(typeof (ET.Client.UILoginComponent))]
    public class UILoginEvent: IEUIEventHandler
    {
        public void OnStart(UIWindow uiWindow)
        {
            UILoginComponent UILogin = uiWindow.AddComponent<UILoginComponent>();
            //----初始化公共组件-----
           UILogin.loginBtn.onClick.AddListener(UILogin.OnLogin);
            //-----------------------
        }

        public void OnActive(UIWindow uiWindow)
        {
          UILoginComponent UILogin = uiWindow.GetComponent<UILoginComponent>();
        }

        public void OnHide(UIWindow uiWindow)
        {
          UILoginComponent UILogin = uiWindow.GetComponent<UILoginComponent>();
        }

        public void OnDestroy(UIWindow uiWindow)
        {
              AddressableComponent.Instance.ReleaseSublevel<UILoginComponent>();
        }

        public void OnHideOrDestroy(UIWindow uiWindow)
        {
          UILoginComponent UILogin = uiWindow.GetComponent<UILoginComponent>();
        }
    }
}