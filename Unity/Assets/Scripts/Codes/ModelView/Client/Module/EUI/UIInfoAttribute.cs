/*************************
文件:UIInfoAttribute.cs
作者:cocoa
创建时间:2022-12-25 11-24-33
描述：
************************/
namespace ET.Client
{
    public class UIInfoAttribute: BaseAttribute
    {
        public string UIPath;
        public UIWidgeID WidgeID;
        public UIWindowType WindowType;
        public UIInfoAttribute(string uiPath, UIWidgeID WidgeID,UIWindowType windowType)
        {
            this.UIPath = uiPath;
            this.WidgeID = WidgeID;
            this.WindowType = windowType;
        }
    }
}