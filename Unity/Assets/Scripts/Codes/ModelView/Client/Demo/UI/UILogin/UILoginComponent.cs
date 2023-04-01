/*************************
文件:UILoginComponent.cs
作者:cocoa
创建时间:2023-3-18 16:16:19:04
描述：
************************/

using UnityEngine;
using UnityEngine.UI;

namespace ET.Client
{
    [UIInfo("UILogin", UIWidgeID.UILogin, UIWindowType.Normal)]
    [ComponentOf(typeof (UIWindow))]
    public class UILoginComponent : Entity,IAwake
    {
        public GameObject GameObject => this.GetParent<UIWindow>().GameObject;
        public Transform Transform => this.GetParent<UIWindow>().Transform;
        
		public InputField account;
		public InputField password;
		public Button loginBtn;

    }
    
     public class UILoginComponentAwakeSystem: AwakeSystem<UILoginComponent>
     {
        protected override void Awake(UILoginComponent self)
        {
			self.account = self.Transform.Find("Panel/Account").GetComponent<InputField>();
			self.password = self.Transform.Find("Panel/Password").GetComponent<InputField>();
			self.loginBtn = self.Transform.Find("Panel/LoginBtn").GetComponent<Button>();

        }
     }
        


}