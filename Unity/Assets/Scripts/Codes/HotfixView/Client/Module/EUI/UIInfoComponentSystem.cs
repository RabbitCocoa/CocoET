/*************************
文件:UIInfoComponent.cs
作者:cocoa
创建时间:2022-12-25 11-29-06
描述：
************************/

using System;

namespace ET.Client
{
   
    public class UIInfoComponentAwakeSystem: AwakeSystem<UIInfoComponent>
    {
        protected override void Awake(UIInfoComponent self)
        {
            UIInfoComponent.Instance = self;

            var uiEvents = EventSystem.Instance.GetTypes(typeof (UIInfoAttribute));
            foreach (Type type in uiEvents)
            {
                object[] attrs = type.GetCustomAttributes(typeof (UIInfoAttribute), false);
                if (attrs.Length == 0)
                {
                    continue;
                }

                UIInfoAttribute UIInfoAttribute = attrs[0] as UIInfoAttribute;

                self.typeToIds.Add(type, UIInfoAttribute.WidgeID);
                self.idToWindowTypes.Add(UIInfoAttribute.WidgeID,UIInfoAttribute.WindowType);
                self.idToPaths.Add(UIInfoAttribute.WidgeID, UIInfoAttribute.UIPath);
            }
        }
    }

    public class UIInfoComponentDestroySystem: DestroySystem<UIInfoComponent>
    {
        protected override void Destroy(UIInfoComponent self)
        {
            self.typeToIds.Clear();
            self.idToPaths.Clear();
            self.idToWindowTypes.Clear();
            UIInfoComponent.Instance = null;
        }
    }

    [FriendOfAttribute(typeof (UIInfoComponent))]
    public static class UIInfoComponentSystem
    {
   
        //获取WidgeID
        public static UIWidgeID GetWidgeID<T>(this UIInfoComponent self)
        {
            Type t = typeof (T);
            UIWidgeID widgeID = UIWidgeID.None;
            if (self.typeToIds.ContainsKey(t))
            {
                widgeID = self.typeToIds[t];
            }

            return widgeID;
        }

        public static string GetUIPath<T>(this UIInfoComponent self)
        {
            Type t = typeof (T);
            string path = null;
            if (self.typeToIds.ContainsKey(t))
            {
                return self.GetUIPath(self.typeToIds[t]);
            }

            return path;
        }

        //获取UIPath
        public static string GetUIPath(this UIInfoComponent self, UIWidgeID widgeID)
        {
            string path = null;
            if (self.idToPaths.ContainsKey(widgeID))
            {
                return self.idToPaths[widgeID];
            }

            return path;
        }
        
        //获取WindowType
        public static UIWindowType GetWindowType(this UIInfoComponent self, UIWidgeID uiWidgeID)
        {
            if (self.idToWindowTypes.ContainsKey(uiWidgeID))
            {
                return self.idToWindowTypes[uiWidgeID];
            }
            else
            {
                Log.Error($"未定义{uiWidgeID}的Window Type");
                throw new Exception();
            }

        }
    }
}