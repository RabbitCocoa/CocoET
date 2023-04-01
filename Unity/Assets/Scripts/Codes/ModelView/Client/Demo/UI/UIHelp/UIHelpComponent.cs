/*************************
文件:UIHelpComponent.cs
作者:cocoa
创建时间:2023-3-24 22:22:48:12
描述：
************************/

using UnityEngine;
using UnityEngine.UI;

namespace ET.Client
{
    [UIInfo("UIHelp", UIWidgeID.UIHelp, UIWindowType.Fixed)]
    [ComponentOf(typeof (UIWindow))]
    public class UIHelpComponent : Entity,IAwake,ILoad
    {
        public GameObject GameObject => this.GetParent<UIWindow>().GameObject;
        public Transform Transform => this.GetParent<UIWindow>().Transform;
        
		public Button btnTest;

    }
    
     public class UIHelpComponentAwakeSystem: AwakeSystem<UIHelpComponent>
     {
        protected override void Awake(UIHelpComponent self)
        {
			self.btnTest = self.Transform.Find("btnTest").GetComponent<Button>();

        }
     }
        


}