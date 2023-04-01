/*************************
文件:UIHelpEvent.cs
作者:cocoa
创建时间:2023-3-24 21:21:36:55
描述：
************************/

namespace ET.Client
{
    [UIEvent(UIWidgeID.UIHelp)]
    [FriendOf(typeof (ET.Client.UIHelpComponent))]
    public class UIHelpEvent: IEUIEventHandler
    {
        public void OnStart(UIWindow uiWindow)
        {
            UIHelpComponent UIHelp = uiWindow.AddComponent<UIHelpComponent>();
            //----初始化公共组件-----
            UIHelp.btnTest.onClick.AddListener(UIHelp.Test);
            //-----------------------
        }

        public void OnActive(UIWindow uiWindow)
        {
          UIHelpComponent UIHelp = uiWindow.GetComponent<UIHelpComponent>();
        }

        public void OnHide(UIWindow uiWindow)
        {
          UIHelpComponent UIHelp = uiWindow.GetComponent<UIHelpComponent>();
        }

        public void OnDestroy(UIWindow uiWindow)
        {
              AddressableComponent.Instance.ReleaseSublevel<UIHelpComponent>();
        }

        public void OnHideOrDestroy(UIWindow uiWindow)
        {
          UIHelpComponent UIHelp = uiWindow.GetComponent<UIHelpComponent>();
        }
    }
}