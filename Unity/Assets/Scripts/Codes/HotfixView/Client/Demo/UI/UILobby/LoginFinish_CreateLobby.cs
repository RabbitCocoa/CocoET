/*************************
文件:LoginFinish_CreateLobby.cs
作者:cocoa
创建时间:2023-03-18 17-21-57
描述：
************************/


namespace ET.Client
{
    [Event(SceneType.Client)]
    public class LoginFinish_CreateLobby: AEvent<EventType.LoginFinish>
    {
        protected override async ETTask Run(Scene scene, EventType.LoginFinish a)
        {
            await scene.ShowWindowAysnc<UILobbyComponent>();
        }
    }
}