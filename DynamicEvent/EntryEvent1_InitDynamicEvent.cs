namespace ET
{
    [EnableClass]
    [Event(SceneType.Main)]
    public class EntryEvent1_InitDynamicEvent : AEvent<Scene, EntryEvent1>
    {
        protected override async ETTask Run(Scene scene, EntryEvent1 a)
        {
            if (DynamicEventSystem.Instance != null)
                return;
            World.Instance.AddSingleton<DynamicEventSystem>();
            scene.AddComponent<DynamicEventSystemUpdateComponent>();
            await ETTask.CompletedTask;
        }
    }
}
