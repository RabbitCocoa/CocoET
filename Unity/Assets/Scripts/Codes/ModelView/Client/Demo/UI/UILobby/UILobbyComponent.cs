/*************************
文件:UILobbyComponent.cs
作者:cocoa
创建时间:2023-3-18 17:17:18:12
描述：
************************/

using UnityEngine;
using UnityEngine.UI;

namespace ET.Client
{
    [UIInfo("UILobby", UIWidgeID.UILobby, UIWindowType.Normal)]
    [ComponentOf(typeof (UIWindow))]
    public class UILobbyComponent : Entity,IAwake
    {
        public GameObject GameObject => this.GetParent<UIWindow>().GameObject;
        public Transform Transform => this.GetParent<UIWindow>().Transform;
        
		public Button enterMap;

    }
    
     public class UILobbyComponentAwakeSystem: AwakeSystem<UILobbyComponent>
     {
        protected override void Awake(UILobbyComponent self)
        {
			self.enterMap = self.Transform.Find("Panel/EnterMap").GetComponent<Button>();

        }
     }
        


}