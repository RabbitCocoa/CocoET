/*************************
文件:AppStartInitFinish_CreateLogin.cs
作者:cocoa
创建时间:2023-03-18 16-29-20
描述：
************************/

using ET.EventType;

namespace ET.Client
{
    [Event(SceneType.Client)]
    public class AppStartInitFinish_CreateLogin: AEvent<EventType.AppStartInitFinish>
    {
        protected override async ETTask Run(Scene scene, AppStartInitFinish a)
        {
            await scene.GetComponent<UIComponent>().ShowWindowAsync<UILoginComponent>();
            await ETTask.CompletedTask;
        }
    }
}