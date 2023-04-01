/*************************
文件:SceneChangeFinish_ShowUIHelp.cs
作者:cocoa
创建时间:2023-03-24 21-39-01
描述：
************************/

using ET.EventType;

namespace ET.Client
{
    [Event(SceneType.Current)]
    public class SceneChangeFinish_ShowUIHelp : AEvent<EventType.SceneChangeFinish>
    {
        protected override async ETTask Run(Scene scene, SceneChangeFinish a)
        {
            await scene.GetComponent<UIComponent>().ShowWindowAsync<UIHelpComponent>();
        }
    }

}

