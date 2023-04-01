namespace ET.Client
{
    [Event(SceneType.Client)]
    public class AfterCreateClientScene_AddComponent: AEvent<EventType.AfterCreateClientScene>
    {
        protected override async ETTask Run(Scene scene, EventType.AfterCreateClientScene args)
        {
            scene.AddComponent<UIInfoComponent>();
            scene.AddComponent<UIEventComponent>();
            scene.AddComponent<UIComponent>();
           // scene.AddComponent<ResourcesLoaderComponent>();
            Root.Instance.Scene.AddComponent<AddressableComponent>();

            await ETTask.CompletedTask;
        }
    }
}