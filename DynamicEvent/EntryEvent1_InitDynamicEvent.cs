namespace ET
{
    [EnableClass]
    [Event(SceneType.Main)]
    public class EntryEvent1_InitDynamicEvent : AEvent<Scene, EntryEvent1>
    {
        protected override async ETTask Run(Scene scene, EntryEvent1 a)
        {
            scene.AddComponent<DynamicEventComponent>();
            await ETTask.CompletedTask;
        }
    }
}
