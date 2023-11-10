using System;
using System.Collections.Generic;

namespace ET
{
    public sealed partial class DynamicEventSystem : Singleton<DynamicEventSystem>, ISingletonAwake
    {
        private readonly HashSet<EntityRef<Entity>> registeredEntities = new HashSet<EntityRef<Entity>>();
        private readonly HashSet<EntityRef<Entity>> needRemoveEntities = new HashSet<EntityRef<Entity>>();

        public void Awake()
        {
            
        }

        public void Update()
        {
            if (this.needRemoveEntities.Count < 1)
            {
                return;
            }
            foreach (var entity in this.needRemoveEntities)
            {
                this.registeredEntities.Remove(entity);
            }
            this.needRemoveEntities.Clear();
        }

        public void RegisterEntity(Entity entity)
        {
            this.registeredEntities.Add(entity);
        }

        public void UnRegisterEntity(Entity entity)
        {
            this.needRemoveEntities.Add(entity);
        }

        public void Publish<A>(Scene scene, A arg) where A : struct
        {
            SceneType domainSceneType = scene.SceneType;
            Type argType = typeof(A);
            if (DynamicEventTypeSystem.Instance.AllEventInfos.TryGetValue(argType, out List<DynamicEventInfo> dynamicEventInfos))
            {
                foreach (DynamicEventInfo dynamicEventInfo in dynamicEventInfos)
                {
                    if (!domainSceneType.HasSameFlag(dynamicEventInfo.SceneType))
                    {
                        continue;
                    }
                    IDynamicEvent<A> dynamicEvent = (IDynamicEvent<A>)dynamicEventInfo.DynamicEvent;
                    foreach (Entity entity in this.registeredEntities)
                    {
                        if (entity is { IsDisposed: false } && dynamicEventInfo.DynamicEvent.EntityType == entity.GetType())
                        {
                            dynamicEvent.Handle(scene, entity, arg).Coroutine();
                        }
                    }
                }
            }
        }

        public async ETTask PublishAsync<A>(Scene scene, A arg) where A : struct
        {
            using ListComponent<ETTask> taskList = ListComponent<ETTask>.Create();
            SceneType domainSceneType = scene.SceneType;
            Type argType = typeof(A);
            if (DynamicEventTypeSystem.Instance.AllEventInfos.TryGetValue(argType, out List<DynamicEventInfo> dynamicEventInfos))
            {
                foreach (DynamicEventInfo dynamicEventInfo in dynamicEventInfos)
                {
                    if (!domainSceneType.HasSameFlag(dynamicEventInfo.SceneType))
                    {
                        continue;
                    }
                    IDynamicEvent<A> dynamicEvent = (IDynamicEvent<A>)dynamicEventInfo.DynamicEvent;
                    foreach (Entity entity in this.registeredEntities)
                    {
                        if (entity is { IsDisposed: false } && dynamicEventInfo.DynamicEvent.EntityType == entity.GetType())
                        {
                            taskList.Add(dynamicEvent.Handle(scene, entity, arg));
                        }
                    }
                }
            }

            try
            {
                await ETTask.WaitAll(taskList);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }
        }
    }
}
