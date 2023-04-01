using System;
using System.Collections.Generic;
using UnityEngine;

namespace ET.Client
{
	/// <summary>
	/// 管理所有UI GameObject
	/// </summary>
	[ComponentOf(typeof(Scene))]
	public class UIEventComponent: Entity, IAwake
	{
		public static UIEventComponent Instance { get; set; }
		
		public Dictionary<UIWidgeID, IEUIEventHandler> UIEvents = new Dictionary<UIWidgeID, IEUIEventHandler>();


	}
}