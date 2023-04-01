/*************************
文件:UIHelpComponentSystem.cs
作者:cocoa
创建时间:2023-3-24 21:21:36:55
描述：
************************/
using UnityEngine.UI;
namespace ET.Client
{
    public class UIHelpLoadComponentSystem: LoadSystem<UIHelpComponent>
    {
        protected override void Load(UIHelpComponent self)
        {
            self.btnTest.onClick.RemoveAllListeners();
            self.btnTest.onClick.AddListener(self.Test);
        }
    }
    public static class UIHelpComponentSystem
    {
        public static void Test(this UIHelpComponent uiHelp)
        {
            Log.Debug("热重载2");
            
        }
    }
}