using System;

namespace ET.Client
{
    public class UIEventAttribute: BaseAttribute
    {
        public UIWidgeID UIWidgeID { get; }

        public UIEventAttribute(UIWidgeID UIWidgeID)
        {
            this.UIWidgeID = UIWidgeID;
        }
    }
}